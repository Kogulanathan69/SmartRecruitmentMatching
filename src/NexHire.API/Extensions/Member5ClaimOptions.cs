namespace NexHire.API.Extensions;

public sealed class Member5ClaimOptions
{
    public string UserIdClaim { get; init; } = "sub";
    public string RoleClaim { get; init; } = "role";
    public string CompanyIdClaim { get; init; } = "company_id";
    public string CandidateProfileIdClaim { get; init; } = "candidate_profile_id";
}
