namespace NexHire.Domain.ValueObjects;

public record MatchScore
{
    public double Value { get; }

    public MatchScore(double value)
    {
        Value = Math.Clamp(value, 0, 100);
    }

    public string Band => Value switch
    {
        >= 85 => "Excellent",
        >= 70 => "Strong",
        >= 50 => "Moderate",
        _ => "Weak"
    };

    public static MatchScore FromWeightedComponents(IEnumerable<(double score, double weight)> components)
    {
        var list = components.ToList();
        var totalWeight = list.Sum(c => c.weight);
        if (totalWeight == 0) return new MatchScore(0);

        var weighted = list.Sum(c => c.score * c.weight) / totalWeight;
        return new MatchScore(weighted);
    }
}
