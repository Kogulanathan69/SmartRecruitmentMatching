using NexHire.Application.DTOs.Admin;
using NexHire.Application.DTOs.Company;

namespace NexHire.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync();
    Task UpdateUserStatusAsync(Guid userId, UpdateUserStatusDto dto);
    Task<UpdateMatchingRulesDto> GetMatchingRulesAsync();
    Task UpdateMatchingRulesAsync(UpdateMatchingRulesDto dto);
    Task<IReadOnlyList<CompanyResponseDto>> GetPendingCompaniesAsync();
    Task<CompanyResponseDto> ReviewCompanyAsync(Guid companyId, Guid adminUserId, VerifyCompanyDto dto);
    Task<CompanyVerificationStatusDto> GetCompanyVerificationAsync(Guid companyId, Guid adminUserId);
}
