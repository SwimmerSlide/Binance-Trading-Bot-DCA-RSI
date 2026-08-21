using TradingBot.Config;
using TradingBot.Models;

namespace TradingBot.Core;

public sealed class RiskManager
{
    private readonly BotConfig _config;
    private readonly PositionManager _positions;
    private decimal _dailyPnl;
    private DateTime _lastResetDate = DateTime.UtcNow.Date;

    public RiskManager(BotConfig config, PositionManager positions)
    {
        _config = config;
        _positions = positions;
    }

    public bool IsSignalAllowed(TradeSignal signal)
    {
        ResetDailyIfNeeded();

        if (_dailyPnl <= -_config.MaxDailyLossUsd)
            return false;

        decimal positionValue = signal.Price * signal.Quantity;
        if (positionValue > _config.MaxPositionSizeUsd)
            return false;

        var existing = _positions.GetPosition(signal.Symbol);
        if (existing is not null && existing.Quantity > 0 && signal.Side == TradeSide.Buy)
        {
            decimal totalExposure = existing.Quantity * signal.Price + positionValue;
            if (totalExposure > _config.MaxPositionSizeUsd * 3)
                return false;
        }

        int openPositions = _positions.GetAllPositions().Count;
        if (openPositions >= _config.MaxOpenPositions && signal.Side == TradeSide.Buy)
            return false;

        return true;
    }

    public void RecordPnl(decimal amount)
    {
        _dailyPnl += amount;
    }

    public decimal GetDailyPnl() => _dailyPnl;

    public decimal GetRemainingBudget() =>
        Math.Max(0, _config.MaxDailyLossUsd + _dailyPnl);

    private void ResetDailyIfNeeded()
    {
        var today = DateTime.UtcNow.Date;
        if (today > _lastResetDate)
        {
            _dailyPnl = 0;
            _lastResetDate = today;
        }
    }
}
