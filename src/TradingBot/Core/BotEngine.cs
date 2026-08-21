using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TradingBot.Config;
using TradingBot.Exchange;
using TradingBot.Models;
using TradingBot.Notifications;
using TradingBot.Strategy;

namespace TradingBot.Core;

public sealed class BotEngine : BackgroundService
{
    private readonly BotConfig _config;
    private readonly BinanceClient _binance;
    private readonly PositionManager _positions;
    private readonly RiskManager _risk;
    private readonly OrderExecutor _executor;
    private readonly TelegramNotify _notify;
    private readonly ILogger<BotEngine> _logger;
    private readonly Dictionary<string, IStrategy> _strategies;

    public BotEngine(
        BotConfig config,
        BinanceClient binance,
        PositionManager positions,
        RiskManager risk,
        OrderExecutor executor,
        TelegramNotify notify,
        RsiStrategy rsi,
        MacdStrategy macd,
        DcaStrategy dca,
        GridStrategy grid,
        ScalpingStrategy scalping,
        ILogger<BotEngine> logger)
    {
        _config = config;
        _binance = binance;
        _positions = positions;
        _risk = risk;
        _executor = executor;
        _notify = notify;
        _logger = logger;

        _strategies = new Dictionary<string, IStrategy>(StringComparer.OrdinalIgnoreCase)
        {
            ["rsi"] = rsi,
            ["macd"] = macd,
            ["dca"] = dca,
            ["grid"] = grid,
            ["scalping"] = scalping
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Trading bot started | Strategy: {Strategy} | Pairs: {Pairs}",
            _config.ActiveStrategy, string.Join(", ", _config.TradingPairs));

        await _notify.Send($"Bot started: {_config.ActiveStrategy} on {string.Join(", ", _config.TradingPairs)}");

        if (!_strategies.TryGetValue(_config.ActiveStrategy, out var strategy))
        {
            _logger.LogError("Unknown strategy: {Strategy}", _config.ActiveStrategy);
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (string pair in _config.TradingPairs)
                {
                    var candles = await _binance.GetCandles(pair, _config.Timeframe, 100, stoppingToken);

                    TradeSignal? signal = strategy.Analyze(pair, candles);

                    if (signal is not null && _risk.IsSignalAllowed(signal))
                    {
                        OrderResult result = await _executor.Execute(signal, stoppingToken);
                        _positions.RecordOrder(result);
                        await _notify.NotifyOrder(result);

                        _logger.LogInformation("{Pair} {Side} @ {Price} | Qty: {Qty}",
                            pair, signal.Side, result.Price, result.Quantity);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Engine cycle error");
            }

            await Task.Delay(TimeSpan.FromSeconds(_config.PollingIntervalSeconds), stoppingToken);
        }
    }
}
