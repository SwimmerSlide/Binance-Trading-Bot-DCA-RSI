namespace TradingBot.Utils;

public static class DecimalMath
{
    public static decimal Sqrt(decimal value)
    {
        if (value < 0) throw new ArgumentException("Cannot calculate sqrt of negative number");
        if (value == 0) return 0;

        decimal guess = value / 2;
        for (int i = 0; i < 100; i++)
        {
            decimal next = (guess + value / guess) / 2;
            if (Math.Abs(next - guess) < 0.0000000001m)
                break;
            guess = next;
        }

        return guess;
    }

    public static decimal StandardDeviation(decimal[] values)
    {
        if (values.Length == 0) return 0;
        decimal mean = values.Average();
        decimal variance = values.Sum(v => (v - mean) * (v - mean)) / values.Length;
        return Sqrt(variance);
    }

    public static decimal PercentChange(decimal from, decimal to) =>
        from != 0 ? (to - from) / from * 100 : 0;

    public static decimal Clamp(decimal value, decimal min, decimal max) =>
        Math.Max(min, Math.Min(max, value));

    public static decimal RoundToTickSize(decimal value, decimal tickSize) =>
        tickSize > 0 ? Math.Round(value / tickSize) * tickSize : value;
}
