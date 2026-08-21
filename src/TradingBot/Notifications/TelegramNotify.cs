using System.Text;
using System.Text.Json;
using TradingBot.Config;
using TradingBot.Models;

namespace TradingBot.Notifications;

public sealed class TelegramNotify
{
    private readonly HttpClient _http;
    private readonly BotConfig _config;

    public TelegramNotify(BotConfig config)
    {
        _config = config;
        _http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
    }

    public async Task NotifyOrder(OrderResult order)
    {
        string emoji = order.Side == TradeSide.Buy ? "🟢" : "🔴";

        var msg = new StringBuilder();
        msg.AppendLine($"{emoji} <b>{order.Side}</b> {order.Symbol}");
        msg.AppendLine($"Price: <code>{order.Price:F8}</code>");
        msg.AppendLine($"Qty: <code>{order.Quantity:F6}</code>");
        msg.AppendLine($"Total: <code>${order.TotalValue:F2}</code>");
        msg.AppendLine($"Status: {order.Status}");

        await Send(msg.ToString());
    }

    public async Task Send(string message)
    {
        if (string.IsNullOrEmpty(_config.TelegramBotToken) || string.IsNullOrEmpty(_config.TelegramChatId))
            return;

        string url = $"https://api.telegram.org/bot{_config.TelegramBotToken}/sendMessage";

        var payload = new
        {
            chat_id = _config.TelegramChatId,
            text = message,
            parse_mode = "HTML"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            await _http.PostAsync(url, content);
        }
        catch
        {
            // Non-critical notification failure
        }
    }
}
