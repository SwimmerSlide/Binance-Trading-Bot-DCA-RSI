namespace TradingBot.Config;

public sealed class StrategyParams
{
    public int RsiPeriod { get; set; } = 14;
    public decimal RsiOversold { get; set; } = 30m;
    public decimal RsiOverbought { get; set; } = 70m;

    public decimal OrderSizeUsd { get; set; } = 50m;

    public decimal DcaAmountUsd { get; set; } = 25m;
    public int DcaIntervalHours { get; set; } = 4;
    public decimal DcaDipPercent { get; set; } = 3m;

    public int GridLevels { get; set; } = 10;
    public decimal GridSpacingPercent { get; set; } = 1.5m;

    public decimal ScalpTakeProfitPercent { get; set; } = 0.5m;
    public decimal ScalpStopLossPercent { get; set; } = 0.3m;
}
