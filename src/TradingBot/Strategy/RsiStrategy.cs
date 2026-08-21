using TradingBot.Config;
using TradingBot.Indicators;
using TradingBot.Models;

namespace TradingBot.Strategy;

public sealed class RsiStrategy : IStrategy
{
    private readonly StrategyParams _params;
    public string Name => "RSI";

    public RsiStrategy(StrategyParams @params)
    {
        _params = @params;
    }

    public TradeSignal? Analyze(string symbol, List<CandleData> candles)
    {
        if (candles.Count < _params.RsiPeriod + 1)
            return null;

        decimal[] closes = candles.Select(c => c.Close).ToArray();
        decimal currentRsi = Rsi.Calculate(closes, _params.RsiPeriod);

        if (currentRsi <= _params.RsiOversold)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Buy,
                Price = candles[^1].Close,
                Quantity = CalculateQuantity(candles[^1].Close),
                OrderType = OrderType.Market,
                Reason = $"RSI oversold: {currentRsi:F1}"
            };
        }

        if (currentRsi >= _params.RsiOverbought)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Sell,
                Price = candles[^1].Close,
                Quantity = CalculateQuantity(candles[^1].Close),
                OrderType = OrderType.Market,
                Reason = $"RSI overbought: {currentRsi:F1}"
            };
        }

        return null;
    }

    private decimal CalculateQuantity(decimal price) =>
        price > 0 ? Math.Round(_params.OrderSizeUsd / price, 6) : 0;
}

public interface IStrategy
{
    string Name { get; }
    TradeSignal? Analyze(string symbol, List<CandleData> candles);
}
