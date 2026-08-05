namespace NexHire.Application.DTOs.Admin;

public class UpdateUserStatusDto
{
    /// <summary>Active, Inactive, Suspended</summary>
    public string Status { get; set; } = string.Empty;
}
