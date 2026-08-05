namespace NexHire.Application.DTOs.Matching;

public class MatchingRuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Weight { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
}
