using TradingBot.Config;
using TradingBot.Models;

namespace TradingBot.Strategy;

public sealed class GridStrategy : IStrategy
{
    private readonly StrategyParams _params;
    private readonly Dictionary<string, List<decimal>> _gridLevels = new();
    private readonly Dictionary<string, HashSet<decimal>> _filledLevels = new();
    public string Name => "Grid";

    public GridStrategy(StrategyParams @params)
    {
        _params = @params;
    }

    public TradeSignal? Analyze(string symbol, List<CandleData> candles)
    {
        if (candles.Count < 10)
            return null;

        decimal currentPrice = candles[^1].Close;

        if (!_gridLevels.ContainsKey(symbol))
        {
            InitializeGrid(symbol, currentPrice);
        }

        var levels = _gridLevels[symbol];
        var filled = _filledLevels[symbol];

        decimal? nearestBuyLevel = levels
            .Where(l => l < currentPrice && !filled.Contains(l))
            .OrderByDescending(l => l)
            .FirstOrDefault();

        if (nearestBuyLevel.HasValue && currentPrice <= nearestBuyLevel.Value * 1.001m)
        {
            filled.Add(nearestBuyLevel.Value);

            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Buy,
                Price = nearestBuyLevel.Value,
                Quantity = CalculateQuantity(nearestBuyLevel.Value),
                OrderType = OrderType.Limit,
                Reason = $"Grid buy at {nearestBuyLevel.Value:F2}"
            };
        }

        decimal? nearestSellLevel = levels
            .Where(l => l > currentPrice && filled.Contains(l - _params.GridSpacingPercent / 100 * currentPrice))
            .OrderBy(l => l)
            .FirstOrDefault();

        if (nearestSellLevel.HasValue && currentPrice >= nearestSellLevel.Value * 0.999m)
        {
            return new TradeSignal
            {
                Symbol = symbol,
                Side = TradeSide.Sell,
                Price = nearestSellLevel.Value,
                Quantity = CalculateQuantity(nearestSellLevel.Value),
                OrderType = OrderType.Limit,
                Reason = $"Grid sell at {nearestSellLevel.Value:F2}"
            };
        }

        return null;
    }

    private void InitializeGrid(string symbol, decimal centerPrice)
    {
        var levels = new List<decimal>();
        decimal spacing = centerPrice * _params.GridSpacingPercent / 100;

        for (int i = -_params.GridLevels / 2; i <= _params.GridLevels / 2; i++)
        {
            levels.Add(centerPrice + i * spacing);
        }

        _gridLevels[symbol] = levels;
        _filledLevels[symbol] = [];
    }

    private decimal CalculateQuantity(decimal price) =>
        price > 0 ? Math.Round(_params.OrderSizeUsd / price, 6) : 0;
}
