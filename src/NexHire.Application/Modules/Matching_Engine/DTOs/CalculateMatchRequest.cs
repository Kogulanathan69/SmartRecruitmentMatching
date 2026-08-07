using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexHire.Application.Modules.Matching_Engine.DTOs;

/// <summary>
/// Input required by the matching application service.
/// Component scores must already be normalized from 0 to 100.
/// </summary>
public sealed class CalculateMatchRequest
{
    public Guid CandidateProfileId { get; set; }

    public Guid VacancyId { get; set; }

    public decimal SkillsScore { get; set; }

    public decimal ExperienceScore { get; set; }

    public decimal EducationScore { get; set; }

    public decimal LocationScore { get; set; }
}