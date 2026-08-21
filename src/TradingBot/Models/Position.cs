namespace TradingBot.Models;

public sealed class Position
{
    public required string Symbol { get; init; }
    public decimal Quantity { get; set; }
    public decimal AverageEntry { get; set; }
    public DateTime OpenedAt { get; init; } = DateTime.UtcNow;

    public decimal UnrealizedPnl(decimal currentPrice) =>
        (currentPrice - AverageEntry) * Quantity;

    public decimal UnrealizedPnlPercent(decimal currentPrice) =>
        AverageEntry > 0 ? (currentPrice - AverageEntry) / AverageEntry * 100 : 0;

    public bool IsOpen => Quantity > 0;
}
