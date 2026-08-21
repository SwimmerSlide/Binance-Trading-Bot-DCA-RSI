namespace TradingBot.Indicators;

public static class Ema
{
    public static decimal[] Calculate(decimal[] values, int period)
    {
        if (values.Length < period)
            return [];

        decimal multiplier = 2.0m / (period + 1);
        var results = new decimal[values.Length - period + 1];

        decimal sma = 0;
        for (int i = 0; i < period; i++)
            sma += values[i];
        sma /= period;

        results[0] = sma;

        for (int i = 1; i < results.Length; i++)
        {
            results[i] = (values[i + period - 1] - results[i - 1]) * multiplier + results[i - 1];
        }

        return results;
    }

    public static decimal CalculateLatest(decimal[] values, int period)
    {
        var ema = Calculate(values, period);
        return ema.Length > 0 ? ema[^1] : 0;
    }
}
