namespace TradingBot.Models;

public sealed class OrderResult
{
    public required string OrderId { get; init; }
    public required string Symbol { get; init; }
    public required TradeSide Side { get; init; }
    public required decimal Price { get; init; }
    public required decimal Quantity { get; init; }
    public string Status { get; init; } = "UNKNOWN";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public decimal Fee { get; init; }

    public decimal TotalValue => Price * Quantity;
    public bool IsFilled => Status is "FILLED" or "NEW";
}
