using TradingBot.Config;
using TradingBot.Models;

namespace TradingBot.Strategy;

public sealed class DcaStrategy : IStrategy
{
    private readonly StrategyParams _params;
    private readonly Dictionary<string, DateTime> _lastBuyTime = new();
    public string Name => "DCA";

    public DcaStrategy(StrategyParams @params)
    {
        _params = @params;
    }

    public TradeSignal? Analyze(string symbol, List<CandleData> candles)
    {
        if (candles.Count < 2)
            return null;

        if (_lastBuyTime.TryGetValue(symbol, out var lastBuy))
        {
            if (DateTime.UtcNow - lastBuy < TimeSpan.FromHours(_params.DcaIntervalHours))
                return null;
        }

        decimal currentPrice = candles[^1].Close;
        decimal previousPrice = candles[^2].Close;
        decimal change = (currentPrice - previousPrice) / previousPrice * 100;

        if (change <= -_params.DcaDipPercent)
        {
            _lastBuyTime[symbol] = DateTime.UtcNow;

            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Buy,
                Price = currentPrice,
                Quantity = CalculateQuantity(currentPrice),
                OrderType = OrderType.Market,
                Reason = $"DCA: price dropped {change:F2}%"
            };
        }

        if (DateTime.UtcNow - (_lastBuyTime.GetValueOrDefault(symbol, DateTime.MinValue)) >=
            TimeSpan.FromHours(_params.DcaIntervalHours))
        {
            _lastBuyTime[symbol] = DateTime.UtcNow;

            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Buy,
                Price = currentPrice,
                Quantity = CalculateQuantity(currentPrice),
                OrderType = OrderType.Market,
                Reason = "DCA: scheduled interval buy"
            };
        }

        return null;
    }

    private decimal CalculateQuantity(decimal price) =>
        price > 0 ? Math.Round(_params.DcaAmountUsd / price, 6) : 0;
}
