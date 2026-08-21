namespace TradingBot.Utils;

public static class TimeframeHelper
{
    public static TimeSpan ToTimeSpan(string timeframe) => timeframe switch
    {
        "1m" => TimeSpan.FromMinutes(1),
        "3m" => TimeSpan.FromMinutes(3),
        "5m" => TimeSpan.FromMinutes(5),
        "15m" => TimeSpan.FromMinutes(15),
        "30m" => TimeSpan.FromMinutes(30),
        "1h" => TimeSpan.FromHours(1),
        "2h" => TimeSpan.FromHours(2),
        "4h" => TimeSpan.FromHours(4),
        "6h" => TimeSpan.FromHours(6),
        "8h" => TimeSpan.FromHours(8),
        "12h" => TimeSpan.FromHours(12),
        "1d" => TimeSpan.FromDays(1),
        "3d" => TimeSpan.FromDays(3),
        "1w" => TimeSpan.FromDays(7),
        _ => TimeSpan.FromHours(1)
    };

    public static string ToBinanceInterval(string timeframe) => timeframe;

    public static string ToBybitInterval(string timeframe) => timeframe switch
    {
        "1m" => "1",
        "3m" => "3",
        "5m" => "5",
        "15m" => "15",
        "30m" => "30",
        "1h" => "60",
        "2h" => "120",
        "4h" => "240",
        "1d" => "D",
        "1w" => "W",
        _ => "60"
    };

    public static int CandlesNeeded(string strategy) => strategy switch
    {
        "rsi" => 50,
        "macd" => 60,
        "dca" => 10,
        "grid" => 20,
        "scalping" => 40,
        _ => 100
    };
}
