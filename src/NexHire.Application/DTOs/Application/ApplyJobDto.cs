namespace NexHire.Application.DTOs.Application;

public class ApplyJobDto
{
    public Guid JobId { get; set; }
    public Guid? ResumeId { get; set; }
    public string? CoverLetter { get; set; }
}
