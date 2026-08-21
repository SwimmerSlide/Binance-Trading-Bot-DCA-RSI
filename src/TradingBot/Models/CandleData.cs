namespace TradingBot.Models;

public sealed record CandleData
{
    public DateTime OpenTime { get; init; }
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public decimal Volume { get; init; }

    public bool IsBullish => Close > Open;
    public bool IsBearish => Close < Open;
    public decimal Body => Math.Abs(Close - Open);
    public decimal Range => High - Low;
    public decimal UpperWick => High - Math.Max(Open, Close);
    public decimal LowerWick => Math.Min(Open, Close) - Low;
}
