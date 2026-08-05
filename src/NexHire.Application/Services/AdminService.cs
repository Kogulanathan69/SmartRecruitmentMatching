using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Admin;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMatchingService _matchingService;
    private readonly ICompanyService _companyService;

    public AdminService(IUnitOfWork unitOfWork, IMatchingService matchingService, ICompanyService companyService)
    {
        _unitOfWork = unitOfWork;
        _matchingService = matchingService;
        _companyService = companyService;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        var pendingCompanies = await _unitOfWork.Companies.GetByStatusAsync(CompanyStatus.Pending);
        var openComplaints = await _unitOfWork.Complaints.GetByStatusAsync(ComplaintStatus.Open);
        var allCompanies = await _unitOfWork.Companies.GetByStatusAsync(CompanyStatus.Active);

        return new AdminDashboardDto
        {
            TotalUsers = await _unitOfWork.Users.CountTotalAsync(),
            TotalJobSeekers = await _unitOfWork.Users.CountByRoleAsync(UserRole.JobSeeker),
            TotalEmployers = await _unitOfWork.Users.CountByRoleAsync(UserRole.Employer),
            TotalCompanies = allCompanies.Count + pendingCompanies.Count,
            PendingCompanyVerifications = pendingCompanies.Count,
            TotalJobsPosted = await _unitOfWork.Jobs.CountAllAsync(),
            TotalApplications = await _unitOfWork.Applications.CountAllAsync(),
            OpenComplaints = openComplaints.Count
        };
    }

    public async Task UpdateUserStatusAsync(Guid userId, UpdateUserStatusDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (!Enum.TryParse<UserStatus>(dto.Status, out var status))
            throw new ValidationException("Status must be Active, Inactive, or Suspended.");

        user.Status = status;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<UpdateMatchingRulesDto> GetMatchingRulesAsync()
    {
        var rules = await _matchingService.GetRulesAsync();
        return new UpdateMatchingRulesDto { Rules = rules.ToList() };
    }

    public async Task UpdateMatchingRulesAsync(UpdateMatchingRulesDto dto)
    {
        var totalWeight = dto.Rules.Where(r => r.IsActive).Sum(r => r.Weight);
        if (Math.Abs(totalWeight - 100) > 0.01)
            throw new ValidationException("Active matching rule weights must sum to 100.");

        await _matchingService.UpdateRulesAsync(dto.Rules);
    }
    public async Task<IReadOnlyList<NexHire.Application.DTOs.Company.CompanyResponseDto>> GetPendingCompaniesAsync()
    {
        var companies = await _unitOfWork.Companies.GetByStatusAsync(CompanyStatus.Pending);
        var results = new List<NexHire.Application.DTOs.Company.CompanyResponseDto>();
        foreach (var company in companies)
        {
            var response = await _companyService.GetByIdAsync(company.Id);
            if (response is not null) results.Add(response);
        }
        return results;
    }

    public Task<NexHire.Application.DTOs.Company.CompanyResponseDto> ReviewCompanyAsync(
        Guid companyId, Guid adminUserId, NexHire.Application.DTOs.Company.VerifyCompanyDto dto) =>
        _companyService.VerifyCompanyAsync(companyId, adminUserId, dto);

    public Task<NexHire.Application.DTOs.Company.CompanyVerificationStatusDto> GetCompanyVerificationAsync(
        Guid companyId, Guid adminUserId) =>
        _companyService.GetVerificationStatusAsync(companyId, adminUserId, true);

}
