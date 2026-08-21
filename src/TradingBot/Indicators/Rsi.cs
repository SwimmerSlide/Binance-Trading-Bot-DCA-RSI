namespace TradingBot.Indicators;

public static class Rsi
{
    public static decimal Calculate(decimal[] closes, int period = 14)
    {
        if (closes.Length < period + 1)
            return 50m;

        decimal gainSum = 0, lossSum = 0;

        for (int i = 1; i <= period; i++)
        {
            decimal change = closes[i] - closes[i - 1];
            if (change > 0) gainSum += change;
            else lossSum += Math.Abs(change);
        }

        decimal avgGain = gainSum / period;
        decimal avgLoss = lossSum / period;

        for (int i = period + 1; i < closes.Length; i++)
        {
            decimal change = closes[i] - closes[i - 1];
            decimal gain = change > 0 ? change : 0;
            decimal loss = change < 0 ? Math.Abs(change) : 0;

            avgGain = (avgGain * (period - 1) + gain) / period;
            avgLoss = (avgLoss * (period - 1) + loss) / period;
        }

        if (avgLoss == 0) return 100m;
        decimal rs = avgGain / avgLoss;
        return 100m - 100m / (1m + rs);
    }

    public static decimal[] CalculateSeries(decimal[] closes, int period = 14)
    {
        if (closes.Length < period + 1)
            return [];

        var results = new List<decimal>();
        for (int i = period; i < closes.Length; i++)
        {
            var slice = closes[..(i + 1)];
            results.Add(Calculate(slice, period));
        }

        return results.ToArray();
    }
}
