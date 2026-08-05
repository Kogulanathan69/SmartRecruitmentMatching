namespace NexHire.Domain.ValueObjects;

public record SalaryRange
{
    public decimal Min { get; }
    public decimal Max { get; }
    public string Currency { get; }

    public SalaryRange(decimal min, decimal max, string currency = "LKR")
    {
        if (min < 0 || max < 0)
            throw new ArgumentException("Salary values cannot be negative");
        if (min > max)
            throw new ArgumentException("Min salary cannot exceed max salary");

        Min = min;
        Max = max;
        Currency = currency;
    }

    public bool Overlaps(SalaryRange other) =>
        Currency == other.Currency && Min <= other.Max && Max >= other.Min;
}
