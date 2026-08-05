using AutoMapper;
using NexHire.Application.DTOs.Application;
using NexHire.Application.DTOs.Company;
using NexHire.Application.DTOs.Interview;
using NexHire.Application.DTOs.Job;
using NexHire.Application.DTOs.JobSeeker;
using NexHire.Application.DTOs.Matching;
using NexHire.Application.DTOs.Offer;
using NexHire.Domain.Entities;

namespace NexHire.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------- Job ----------
        CreateMap<Job, JobResponseDto>()
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company.Name))
            .ForMember(d => d.CompanyVerified, o => o.MapFrom(s => s.Company.Verification != null && s.Company.Verification.Status == NexHire.Domain.Enums.VerificationStatus.Verified))
            .ForMember(d => d.ApplicationCount, o => o.MapFrom(s => s.Applications.Count))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.RequiredSkills, o => o.MapFrom(s => s.RequiredSkills.Select(rs => rs.Skill.Name)))
            .ForMember(d => d.PreferredSkills, o => o.MapFrom(s => s.PreferredSkills.Select(ps => ps.Skill.Name)));

        CreateMap<CreateJobDto, Job>()
            .ForMember(d => d.RequiredSkills, o => o.Ignore())
            .ForMember(d => d.PreferredSkills, o => o.Ignore());

        // ---------- Company ----------
        CreateMap<CreateCompanyDto, Company>();
        CreateMap<UpdateCompanyDto, Company>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));

        // ---------- JobSeeker ----------
        CreateMap<CreateJobSeekerProfileDto, JobSeekerProfile>();
        CreateMap<UpdateJobSeekerProfileDto, JobSeekerProfile>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<AddEducationDto, Education>();
        CreateMap<AddExperienceDto, Experience>();
        CreateMap<AddProjectDto, Project>();
        CreateMap<AddCertificationDto, Certification>();

        // ---------- Application ----------
        CreateMap<JobApplication, ApplicationResponseDto>()
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.Job.Title))
            .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Job.Company.Name))
            .ForMember(d => d.CandidateName, o => o.MapFrom(s => s.JobSeekerProfile.User.FirstName + " " + s.JobSeekerProfile.User.LastName))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        // ---------- Matching ----------
        CreateMap<MatchResult, MatchScoreResponseDto>()
            .ForMember(d => d.Band, o => o.MapFrom(s => ScoreBand(s.OverallScore)))
            .ForMember(d => d.Breakdown, o => o.MapFrom(s => s.ScoreDetails))
            .ForMember(d => d.IsEligible, o => o.Ignore())
            .ForMember(d => d.MandatoryRuleFailures, o => o.Ignore())
            .ForMember(d => d.MatchedSkills, o => o.Ignore())
            .ForMember(d => d.MissingSkills, o => o.Ignore());

        CreateMap<MatchScoreDetail, MatchScoreDetailDto>();

        CreateMap<MatchingRule, MatchingRuleDto>();

        // ---------- Interview ----------
        CreateMap<Interview, InterviewResponseDto>()
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.JobApplication.Job.Title))
            .ForMember(d => d.CandidateName, o => o.MapFrom(s =>
                s.JobApplication.JobSeekerProfile.User.FirstName + " " + s.JobApplication.JobSeekerProfile.User.LastName))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateInterviewDto, Interview>();

        // ---------- Offer ----------
        CreateMap<Offer, OfferResponseDto>()
            .ForMember(d => d.JobTitle, o => o.MapFrom(s => s.JobApplication.Job.Title))
            .ForMember(d => d.CandidateName, o => o.MapFrom(s =>
                s.JobApplication.JobSeekerProfile.User.FirstName + " " + s.JobApplication.JobSeekerProfile.User.LastName))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateOfferDto, Offer>();
    }

    private static string ScoreBand(double score) => score switch
    {
        >= 85 => "Excellent",
        >= 70 => "Strong",
        >= 50 => "Moderate",
        _ => "Weak"
    };
}
