using TradingBot.Config;
using TradingBot.Indicators;
using TradingBot.Models;

namespace TradingBot.Strategy;

public sealed class ScalpingStrategy : IStrategy
{
    private readonly StrategyParams _params;
    public string Name => "Scalping";

    public ScalpingStrategy(StrategyParams @params)
    {
        _params = @params;
    }

    public TradeSignal? Analyze(string symbol, List<CandleData> candles)
    {
        if (candles.Count < 30)
            return null;

        decimal[] closes = candles.Select(c => c.Close).ToArray();

        var bb = BollingerBands.Calculate(closes, 20, 2.0m);
        decimal[] ema9 = Ema.Calculate(closes, 9);
        decimal[] ema21 = Ema.Calculate(closes, 21);
        decimal atr = Atr.Calculate(candles, 14);

        decimal currentClose = closes[^1];
        bool nearLowerBand = currentClose <= bb.Lower[^1] * 1.005m;
        bool nearUpperBand = currentClose >= bb.Upper[^1] * 0.995m;
        bool emaBullish = ema9[^1] > ema21[^1];
        bool emaBearish = ema9[^1] < ema21[^1];

        if (nearLowerBand && emaBullish)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Buy,
                Price = currentClose,
                Quantity = CalculateQuantity(currentClose),
                OrderType = OrderType.Market,
                StopLoss = currentClose - atr * 1.5m,
                TakeProfit = currentClose + atr * 2.0m,
                Reason = "Scalp: BB lower + EMA bullish"
            };
        }

        if (nearUpperBand && emaBearish)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Sell,
                Price = currentClose,
                Quantity = CalculateQuantity(currentClose),
                OrderType = OrderType.Market,
                StopLoss = currentClose + atr * 1.5m,
                TakeProfit = currentClose - atr * 2.0m,
                Reason = "Scalp: BB upper + EMA bearish"
            };
        }

        return null;
    }

    private decimal CalculateQuantity(decimal price) =>
        price > 0 ? Math.Round(_params.OrderSizeUsd / price, 6) : 0;
}
