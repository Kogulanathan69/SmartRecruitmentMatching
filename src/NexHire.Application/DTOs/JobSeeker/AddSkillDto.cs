namespace NexHire.Application.DTOs.JobSeeker;

public class AddSkillDto
{
    public string SkillName { get; set; } = string.Empty;
    public int ProficiencyLevel { get; set; } // 1-5
    public int YearsOfExperience { get; set; }
}
