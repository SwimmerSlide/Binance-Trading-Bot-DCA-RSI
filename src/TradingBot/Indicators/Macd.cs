namespace TradingBot.Indicators;

public static class Macd
{
    public static (decimal[] MacdLine, decimal[] SignalLine, decimal[] Histogram) Calculate(
        decimal[] closes, int fastPeriod = 12, int slowPeriod = 26, int signalPeriod = 9)
    {
        if (closes.Length < slowPeriod + signalPeriod)
            return ([], [], []);

        decimal[] fastEma = Ema.Calculate(closes, fastPeriod);
        decimal[] slowEma = Ema.Calculate(closes, slowPeriod);

        int offset = slowPeriod - fastPeriod;
        int macdLength = Math.Min(fastEma.Length - offset, slowEma.Length);

        decimal[] macdLine = new decimal[macdLength];
        for (int i = 0; i < macdLength; i++)
        {
            macdLine[i] = fastEma[i + offset] - slowEma[i];
        }

        decimal[] signalLine = Ema.Calculate(macdLine, signalPeriod);

        int histOffset = macdLine.Length - signalLine.Length;
        decimal[] histogram = new decimal[signalLine.Length];
        for (int i = 0; i < signalLine.Length; i++)
        {
            histogram[i] = macdLine[i + histOffset] - signalLine[i];
        }

        return (macdLine, signalLine, histogram);
    }
}
