using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexHire.Domain.Modules.Matching_Engine.Entities;

/// <summary>
/// Stores one deterministic RM-1.0 matching result
/// for a candidate and a vacancy.
/// </summary>
public sealed class MatchResult
{
    public Guid MatchResultId { get; set; } = Guid.NewGuid();

    // References the candidate profile owned by another module.
    public Guid CandidateProfileId { get; set; }

    // References the vacancy owned by another module.
    public Guid VacancyId { get; set; }

    // Identifies exactly which matching rule version was used.
    public Guid RuleVersionId { get; set; }

    // Normalized component scores: each value is between 0 and 100.
    public decimal SkillsScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }

    // Final weighted RM-1.0 score: 0 to 100.
    public decimal TotalScore { get; set; }

    /// <summary>
    /// Stores a deterministic receipt/snapshot identifier
    /// so that the calculation can later be traced or replayed.
    /// </summary>
    public string ReplayReceipt { get; set; } = string.Empty;

    public DateTimeOffset CalculatedAt { get; set; } =
        DateTimeOffset.UtcNow;
}