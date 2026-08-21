using TradingBot.Models;

namespace TradingBot.Core;

public sealed class PositionManager
{
    private readonly Dictionary<string, Position> _positions = new();
    private readonly List<OrderResult> _orderHistory = [];
    private readonly Lock _lock = new();

    public void RecordOrder(OrderResult order)
    {
        lock (_lock)
        {
            _orderHistory.Add(order);

            if (!_positions.TryGetValue(order.Symbol, out var position))
            {
                position = new Position { Symbol = order.Symbol };
                _positions[order.Symbol] = position;
            }

            if (order.Side == TradeSide.Buy)
            {
                decimal totalCost = position.AverageEntry * position.Quantity + order.Price * order.Quantity;
                position.Quantity += order.Quantity;
                position.AverageEntry = position.Quantity > 0 ? totalCost / position.Quantity : 0;
            }
            else
            {
                position.Quantity -= order.Quantity;
                if (position.Quantity <= 0)
                {
                    position.Quantity = 0;
                    position.AverageEntry = 0;
                }
            }
        }
    }

    public Position? GetPosition(string symbol)
    {
        lock (_lock)
        {
            return _positions.GetValueOrDefault(symbol);
        }
    }

    public IReadOnlyList<Position> GetAllPositions()
    {
        lock (_lock)
        {
            return _positions.Values.Where(p => p.Quantity > 0).ToList();
        }
    }

    public decimal GetTotalPnl(Dictionary<string, decimal> currentPrices)
    {
        lock (_lock)
        {
            decimal pnl = 0;
            foreach (var pos in _positions.Values)
            {
                if (currentPrices.TryGetValue(pos.Symbol, out decimal price))
                {
                    pnl += (price - pos.AverageEntry) * pos.Quantity;
                }
            }
            return pnl;
        }
    }
}
