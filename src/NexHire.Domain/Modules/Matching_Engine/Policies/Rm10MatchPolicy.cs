using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexHire.Domain.Modules.Matching_Engine.Policies;

/// <summary>
/// Represents the four normalized component scores used by RM-1.0.
/// Every component must be between 0 and 100.
/// </summary>
public sealed record MatchComponentScores(
    decimal Skills,
    decimal Experience,
    decimal Education,
    decimal Location);

/// <summary>
/// Represents the deterministic weighted result produced by RM-1.0.
/// </summary>
public sealed record MatchScoreResult(
    string RuleVersion,
    decimal TotalScore,
    decimal SkillPoints,
    decimal ExperiencePoints,
    decimal EducationPoints,
    decimal LocationPoints);

/// <summary>
/// Implements the RM-1.0 weighted matching contract.
///
/// Important:
/// This policy combines normalized component scores only.
/// It does not decide how experience, education or location
/// are converted into their 0-100 component values.
/// </summary>
public static class Rm10MatchPolicy
{
    public const string RuleVersion = "RM-1.0";

    public const decimal SkillsWeight = 0.55m;
    public const decimal ExperienceWeight = 0.20m;
    public const decimal EducationWeight = 0.15m;
    public const decimal LocationWeight = 0.10m;

    public static MatchScoreResult Calculate(MatchComponentScores scores)
    {
        ValidateScore(scores.Skills, nameof(scores.Skills));
        ValidateScore(scores.Experience, nameof(scores.Experience));
        ValidateScore(scores.Education, nameof(scores.Education));
        ValidateScore(scores.Location, nameof(scores.Location));

        var skillPoints = scores.Skills * SkillsWeight;
        var experiencePoints = scores.Experience * ExperienceWeight;
        var educationPoints = scores.Education * EducationWeight;
        var locationPoints = scores.Location * LocationWeight;

        var totalScore =
            skillPoints +
            experiencePoints +
            educationPoints +
            locationPoints;

        return new MatchScoreResult(
            RuleVersion,
            totalScore,
            skillPoints,
            experiencePoints,
            educationPoints,
            locationPoints);
    }

    private static void ValidateScore(decimal score, string parameterName)
    {
        if (score < 0 || score > 100)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "Match component score must be between 0 and 100.");
        }
    }
}
