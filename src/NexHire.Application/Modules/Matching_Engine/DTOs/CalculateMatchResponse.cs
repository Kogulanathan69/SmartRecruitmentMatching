using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexHire.Application.Modules.Matching_Engine.DTOs;

/// <summary>
/// Result returned after an RM-1.0 match calculation.
/// </summary>
public sealed class CalculateMatchResponse
{
    public Guid CandidateProfileId { get; set; }

    public Guid VacancyId { get; set; }

    public string RuleVersion { get; set; } = string.Empty;

    public decimal SkillsScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }

    public decimal TotalScore { get; set; }

    public string ReplayReceipt { get; set; } = string.Empty;

    public DateTimeOffset CalculatedAt { get; set; }
}