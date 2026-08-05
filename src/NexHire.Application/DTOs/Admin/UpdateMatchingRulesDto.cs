namespace NexHire.Application.DTOs.Admin;
using NexHire.Application.DTOs.Matching;

public class UpdateMatchingRulesDto
{
    public List<MatchingRuleDto> Rules { get; set; } = new();
}
