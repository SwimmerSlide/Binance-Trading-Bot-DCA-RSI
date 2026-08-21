namespace TradingBot.Models;

public sealed class TradeSignal
{
    public required string Symbol { get; init; }
    public required TradeSide Side { get; init; }
    public required decimal Price { get; init; }
    public required decimal Quantity { get; init; }
    public OrderType OrderType { get; init; } = OrderType.Market;
    public decimal? StopLoss { get; init; }
    public decimal? TakeProfit { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public enum TradeSide { Buy, Sell }
public enum OrderType { Market, Limit }
