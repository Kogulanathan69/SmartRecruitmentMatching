namespace NexHire.Domain.Entities;

public class JobRequiredSkill
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    public int MinProficiencyLevel { get; set; } // 1-5
}
