using System.Text.Json;

namespace TradingBot.Config;

public sealed class BotConfig
{
    public string Exchange { get; set; } = "binance";
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string[] TradingPairs { get; set; } = ["BTCUSDT", "ETHUSDT"];
    public string ActiveStrategy { get; set; } = "rsi";
    public string Timeframe { get; set; } = "1h";
    public int PollingIntervalSeconds { get; set; } = 60;
    public decimal MaxDailyLossUsd { get; set; } = 100m;
    public decimal MaxPositionSizeUsd { get; set; } = 500m;
    public int MaxOpenPositions { get; set; } = 5;
    public string? TelegramBotToken { get; set; }
    public string? TelegramChatId { get; set; }

    public static BotConfig Load()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "bot-config.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<BotConfig>(json) ?? new BotConfig();
        }
        return new BotConfig();
    }

    public void Save()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "bot-config.json");
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }
}
