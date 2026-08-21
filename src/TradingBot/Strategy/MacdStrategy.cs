using TradingBot.Config;
using TradingBot.Indicators;
using TradingBot.Models;

namespace TradingBot.Strategy;

public sealed class MacdStrategy : IStrategy
{
    private readonly StrategyParams _params;
    public string Name => "MACD";

    public MacdStrategy(StrategyParams @params)
    {
        _params = @params;
    }

    public TradeSignal? Analyze(string symbol, List<CandleData> candles)
    {
        if (candles.Count < 35)
            return null;

        decimal[] closes = candles.Select(c => c.Close).ToArray();
        var (macdLine, signalLine, histogram) = Macd.Calculate(closes, 12, 26, 9);

        if (macdLine.Length < 2 || signalLine.Length < 2)
            return null;

        bool bullishCross = macdLine[^2] < signalLine[^2] && macdLine[^1] > signalLine[^1];
        bool bearishCross = macdLine[^2] > signalLine[^2] && macdLine[^1] < signalLine[^1];

        if (bullishCross)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Buy,
                Price = candles[^1].Close,
                Quantity = CalculateQuantity(candles[^1].Close),
                OrderType = OrderType.Market,
                Reason = "MACD bullish crossover"
            };
        }

        if (bearishCross)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Sell,
                Price = candles[^1].Close,
                Quantity = CalculateQuantity(candles[^1].Close),
                OrderType = OrderType.Market,
                Reason = "MACD bearish crossover"
            };
        }

        return null;
    }

    private decimal CalculateQuantity(decimal price) =>
        price > 0 ? Math.Round(_params.OrderSizeUsd / price, 6) : 0;
}
