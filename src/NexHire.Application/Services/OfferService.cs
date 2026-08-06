using AutoMapper;
using NexHire.Application.Common.Exceptions;
using NexHire.Application.DTOs.Offer;
using NexHire.Application.Interfaces.Repositories;
using NexHire.Application.Interfaces.Services;
using NexHire.Domain.Entities;
using NexHire.Domain.Enums;

namespace NexHire.Application.Services;

public class OfferService : IOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OfferService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OfferResponseDto> CreateOfferAsync(CreateOfferDto dto)
    {
        var application = await _unitOfWork.Applications.GetByIdAsync(dto.JobApplicationId)
            ?? throw new NotFoundException("Job application not found.");

        var existingOffer = await _unitOfWork.Offers.GetByApplicationIdAsync(dto.JobApplicationId);

        if (existingOffer != null)
            throw new BusinessRuleException("An offer has already been issued for this application.");

        var offer = new Offer
        {
            Id = Guid.NewGuid(),
            JobApplicationId = dto.JobApplicationId,
            SalaryOffered = dto.SalaryOffered,
            Currency = dto.Currency,
            JoiningDate = dto.JoiningDate,
            Remarks = dto.Remarks,
            Status = OfferStatus.Pending,
            IssuedAt = DateTime.UtcNow
        };

        await _unitOfWork.Offers.AddAsync(offer);

        application.Status = ApplicationStatus.Hired;
        application.StatusUpdatedAt = DateTime.UtcNow;
        _unitOfWork.Applications.Update(application);

        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Offers.GetByIdAsync(offer.Id);
        return _mapper.Map<OfferResponseDto>(created);
    }

    public async Task<OfferResponseDto> UpdateStatusAsync(Guid offerId, UpdateOfferStatusDto dto)
    {
        var offer = await _unitOfWork.Offers.GetByIdAsync(offerId)
            ?? throw new NotFoundException("Offer not found.");

        if (!Enum.TryParse<OfferStatus>(dto.Status, out var status))
            throw new ValidationException("Status must be Accepted, Declined, or Withdrawn.");

        offer.Status = status;
        offer.RespondedAt = DateTime.UtcNow;

        _unitOfWork.Offers.Update(offer);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<OfferResponseDto>(offer);
    }

    public async Task<OfferResponseDto?> GetByApplicationAsync(Guid applicationId)
    {
        var offer = await _unitOfWork.Offers.GetByApplicationIdAsync(applicationId);
        return offer == null ? null : _mapper.Map<OfferResponseDto>(offer);
    }
}
