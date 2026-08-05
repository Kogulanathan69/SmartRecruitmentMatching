namespace NexHire.Domain.Entities;

public class MatchingRule
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Skills, Experience, Education, Certification, Location
    public double Weight { get; set; } // percentage weight, should sum to 100 across active rules
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}
