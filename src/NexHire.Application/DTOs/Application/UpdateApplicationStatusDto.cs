namespace NexHire.Application.DTOs.Application;

public class UpdateApplicationStatusDto
{
    /// <summary>Submitted, UnderReview, Shortlisted, Rejected, Hired</summary>
    public string Status { get; set; } = string.Empty;
}
