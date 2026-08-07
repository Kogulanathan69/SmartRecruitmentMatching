
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using NexHire.Application.Modules.Matching_Engine.DTOs;
using NexHire.Application.Modules.Matching_Engine.Interfaces;
using NexHire.Domain.Modules.Matching_Engine.Policies;

namespace NexHire.Application.Modules.Matching_Engine.Services;

/// <summary>
/// Application service that executes the deterministic RM-1.0
/// matching policy and returns a traceable result.
/// </summary>
public sealed class MatchingService : IMatchingService
{
    public CalculateMatchResponse Calculate(CalculateMatchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CandidateProfileId == Guid.Empty)
        {
            throw new ArgumentException(
                "Candidate profile ID is required.",
                nameof(request));
        }

        if (request.VacancyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Vacancy ID is required.",
                nameof(request));
        }

        var componentScores = new MatchComponentScores(
            request.SkillsScore,
            request.ExperienceScore,
            request.EducationScore,
            request.LocationScore);

        var result = Rm10MatchPolicy.Calculate(componentScores);

        var replayReceipt = CreateReplayReceipt(
            request,
            result.RuleVersion);

        return new CalculateMatchResponse
        {
            CandidateProfileId = request.CandidateProfileId,
            VacancyId = request.VacancyId,

            RuleVersion = result.RuleVersion,

            SkillsScore = request.SkillsScore,
            ExperienceScore = request.ExperienceScore,
            EducationScore = request.EducationScore,
            LocationScore = request.LocationScore,

            TotalScore = result.TotalScore,

            ReplayReceipt = replayReceipt,

            CalculatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates the same receipt whenever the same candidate,
    /// vacancy, scores and rule version are used.
    /// </summary>
    private static string CreateReplayReceipt(
        CalculateMatchRequest request,
        string ruleVersion)
    {
        var receiptInput = string.Join(
            "|",
            ruleVersion,
            request.CandidateProfileId.ToString("N"),
            request.VacancyId.ToString("N"),
            request.SkillsScore.ToString(CultureInfo.InvariantCulture),
            request.ExperienceScore.ToString(CultureInfo.InvariantCulture),
            request.EducationScore.ToString(CultureInfo.InvariantCulture),
            request.LocationScore.ToString(CultureInfo.InvariantCulture));

        var bytes = Encoding.UTF8.GetBytes(receiptInput);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}