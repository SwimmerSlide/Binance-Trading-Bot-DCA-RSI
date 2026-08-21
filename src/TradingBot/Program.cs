using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingBot.Config;
using TradingBot.Core;
using TradingBot.Exchange;
using TradingBot.Notifications;
using TradingBot.Strategy;

namespace TradingBot;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(BotConfig.Load());
                services.AddSingleton<StrategyParams>();
                services.AddSingleton<BinanceClient>();
                services.AddSingleton<BybitClient>();
                services.AddSingleton<WebSocketFeed>();
                services.AddSingleton<OrderBookAggregator>();
                services.AddSingleton<PositionManager>();
                services.AddSingleton<RiskManager>();
                services.AddSingleton<OrderExecutor>();
                services.AddSingleton<TelegramNotify>();
                services.AddSingleton<RsiStrategy>();
                services.AddSingleton<MacdStrategy>();
                services.AddSingleton<DcaStrategy>();
                services.AddSingleton<GridStrategy>();
                services.AddSingleton<ScalpingStrategy>();
                services.AddSingleton<BotEngine>();
                services.AddHostedService<BotEngine>(sp => sp.GetRequiredService<BotEngine>());
            })
            .Build();

        await host.RunAsync();
    }
}
