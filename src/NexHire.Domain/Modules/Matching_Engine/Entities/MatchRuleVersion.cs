using NexHire.Domain.Modules.Matching_Engine.Policies;

namespace NexHire.Domain.Modules.Matching_Engine.Entities;

/// <summary>
/// Stores the scoring rule version used to calculate a match.
/// Keeping the rule version makes old match results reproducible.
/// </summary>
public sealed class MatchRuleVersion
{
    public Guid RuleVersionId { get; set; } = Guid.NewGuid();

    public string Version { get; set; } = Rm10MatchPolicy.RuleVersion;

    public decimal SkillsWeight { get; set; } =
        Rm10MatchPolicy.SkillsWeight;

    public decimal ExperienceWeight { get; set; } =
        Rm10MatchPolicy.ExperienceWeight;

    public decimal EducationWeight { get; set; } =
        Rm10MatchPolicy.EducationWeight;

    public decimal LocationWeight { get; set; } =
        Rm10MatchPolicy.LocationWeight;

    public DateTimeOffset? ApprovedAt { get; set; }

    public DateTimeOffset? ActivatedAt { get; set; }

    /// <summary>
    /// Confirms that all component weights together equal 100%.
    /// </summary>
    public bool HasValidWeights()
    {
        var total =
            SkillsWeight +
            ExperienceWeight +
            EducationWeight +
            LocationWeight;

        return total == 1.00m;
    }
}