namespace NexHire.Domain.Entities;

public class Skill
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }

    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    public ICollection<JobRequiredSkill> JobRequiredSkills { get; set; } = new List<JobRequiredSkill>();
    public ICollection<JobPreferredSkill> JobPreferredSkills { get; set; } = new List<JobPreferredSkill>();
}
