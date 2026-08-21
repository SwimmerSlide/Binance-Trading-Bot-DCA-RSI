using TradingBot.Config;
using TradingBot.Exchange;
using TradingBot.Models;

namespace TradingBot.Core;

public sealed class OrderExecutor
{
    private readonly BinanceClient _binance;
    private readonly BybitClient _bybit;
    private readonly BotConfig _config;

    public OrderExecutor(BinanceClient binance, BybitClient bybit, BotConfig config)
    {
        _binance = binance;
        _bybit = bybit;
        _config = config;
    }

    public async Task<OrderResult> Execute(TradeSignal signal, CancellationToken ct = default)
    {
        return _config.Exchange.ToLowerInvariant() switch
        {
            "binance" => await ExecuteOnBinance(signal, ct),
            "bybit" => await ExecuteOnBybit(signal, ct),
            _ => throw new InvalidOperationException($"Unknown exchange: {_config.Exchange}")
        };
    }

    private async Task<OrderResult> ExecuteOnBinance(TradeSignal signal, CancellationToken ct)
    {
        var result = await _binance.PlaceOrder(
            signal.Symbol,
            signal.Side,
            signal.OrderType,
            signal.Quantity,
            signal.Price,
            ct);

        return result;
    }

    private async Task<OrderResult> ExecuteOnBybit(TradeSignal signal, CancellationToken ct)
    {
        var result = await _bybit.PlaceOrder(
            signal.Symbol,
            signal.Side,
            signal.OrderType,
            signal.Quantity,
            signal.Price,
            ct);

        return result;
    }
}
