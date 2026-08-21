using TradingBot.Models;

namespace TradingBot.Indicators;

public static class Atr
{
    public static decimal Calculate(List<CandleData> candles, int period = 14)
    {
        if (candles.Count < period + 1)
            return 0;

        decimal[] trueRanges = new decimal[candles.Count - 1];

        for (int i = 1; i < candles.Count; i++)
        {
            decimal highLow = candles[i].High - candles[i].Low;
            decimal highClose = Math.Abs(candles[i].High - candles[i - 1].Close);
            decimal lowClose = Math.Abs(candles[i].Low - candles[i - 1].Close);
            trueRanges[i - 1] = Math.Max(highLow, Math.Max(highClose, lowClose));
        }

        decimal atr = trueRanges[..period].Average();

        for (int i = period; i < trueRanges.Length; i++)
        {
            atr = (atr * (period - 1) + trueRanges[i]) / period;
        }

        return atr;
    }

    public static decimal[] CalculateSeries(List<CandleData> candles, int period = 14)
    {
        if (candles.Count < period + 1)
            return [];

        decimal[] trueRanges = new decimal[candles.Count - 1];
        for (int i = 1; i < candles.Count; i++)
        {
            decimal highLow = candles[i].High - candles[i].Low;
            decimal highClose = Math.Abs(candles[i].High - candles[i - 1].Close);
            decimal lowClose = Math.Abs(candles[i].Low - candles[i - 1].Close);
            trueRanges[i - 1] = Math.Max(highLow, Math.Max(highClose, lowClose));
        }

        var results = new List<decimal>();
        decimal atr = trueRanges[..period].Average();
        results.Add(atr);

        for (int i = period; i < trueRanges.Length; i++)
        {
            atr = (atr * (period - 1) + trueRanges[i]) / period;
            results.Add(atr);
        }

        return results.ToArray();
    }
}
