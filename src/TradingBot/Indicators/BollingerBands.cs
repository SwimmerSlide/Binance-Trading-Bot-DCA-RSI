namespace TradingBot.Indicators;

public static class BollingerBands
{
    public static BollingerResult Calculate(decimal[] closes, int period = 20, decimal multiplier = 2.0m)
    {
        if (closes.Length < period)
            return new BollingerResult([], [], []);

        var middle = new List<decimal>();
        var upper = new List<decimal>();
        var lower = new List<decimal>();

        for (int i = period - 1; i < closes.Length; i++)
        {
            var window = closes[(i - period + 1)..(i + 1)];
            decimal sma = window.Average();
            decimal variance = window.Sum(x => (x - sma) * (x - sma)) / period;
            decimal stdDev = (decimal)Math.Sqrt((double)variance);

            middle.Add(sma);
            upper.Add(sma + multiplier * stdDev);
            lower.Add(sma - multiplier * stdDev);
        }

        return new BollingerResult(middle.ToArray(), upper.ToArray(), lower.ToArray());
    }
}

public sealed record BollingerResult(decimal[] Middle, decimal[] Upper, decimal[] Lower);
