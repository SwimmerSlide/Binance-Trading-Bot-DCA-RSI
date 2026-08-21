# Binance Trading Bot — DCA + RSI + MACD + Grid

[![Build](https://img.shields.io/github/actions/workflow/status/tradingtools/binance-bot/build.yml?branch=main&style=flat-square)](https://github.com/tradingtools/binance-bot/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![Stars](https://img.shields.io/github/stars/tradingtools/binance-bot?style=flat-square)](https://github.com/tradingtools/binance-bot/stargazers)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple?style=flat-square)](https://dotnet.microsoft.com/)

**Binance + Bybit | DCA + RSI + MACD + Grid + Scalping | Risk Management | Telegram Alerts**

Automated cryptocurrency trading bot supporting multiple strategies and exchanges. Built with .NET 9, featuring real-time WebSocket feeds, position management, and comprehensive risk controls.

---

## Screenshots

![Dashboard](docs/screenshots/dashboard.png)
![Trades](docs/screenshots/trade-history.png)

---

## Features

- **5 Strategies** — RSI, MACD, DCA, Grid Trading, Scalping
- **Multi-Exchange** — Binance and Bybit support
- **Real-Time Data** — WebSocket price feeds and order book
- **Risk Management** — Daily loss limits, position sizing, max open positions
- **Technical Indicators** — RSI, MACD, EMA, Bollinger Bands, ATR
- **Position Tracking** — Average entry, PnL, unrealized gains
- **Telegram Alerts** — Order fills, signals, and error notifications
- **Configurable** — JSON-based configuration for all parameters

---

## Architecture

```
src/TradingBot/
├── Core/           → Engine, positions, risk management, order execution
├── Exchange/       → Binance client, Bybit client, WebSocket, order book
├── Strategy/       → RSI, MACD, DCA, Grid, Scalping implementations
├── Indicators/     → RSI, MACD, EMA, Bollinger Bands, ATR
├── Models/         → Signals, positions, candles, orders
├── Config/         → Bot config, strategy parameters
├── Utils/          → Decimal math, timeframe helpers
└── Notifications/  → Telegram alerts
```

---

## Build

### Requirements

- .NET 9 SDK
- Binance or Bybit API key

### Compile

```bash
dotnet build src/TradingBot/TradingBot.csproj -c Release
```

### Run

```bash
dotnet run --project src/TradingBot/TradingBot.csproj
```

---

## Configuration

Create `bot-config.json`:

```json
{
  "Exchange": "binance",
  "ApiKey": "YOUR_API_KEY",
  "ApiSecret": "YOUR_API_SECRET",
  "TradingPairs": ["BTCUSDT", "ETHUSDT"],
  "ActiveStrategy": "rsi",
  "Timeframe": "1h",
  "MaxDailyLossUsd": 100,
  "MaxPositionSizeUsd": 500,
  "MaxOpenPositions": 5,
  "TelegramBotToken": "YOUR_BOT_TOKEN",
  "TelegramChatId": "YOUR_CHAT_ID"
}
```

---

## Strategies

| Strategy | Description | Best For |
|----------|-------------|----------|
| **RSI** | Buy oversold (< 30), sell overbought (> 70) | Ranging markets |
| **MACD** | Trade crossovers between MACD and signal line | Trending markets |
| **DCA** | Periodic buys + extra buys on dips | Long-term accumulation |
| **Grid** | Place orders at fixed price intervals | Sideways markets |
| **Scalping** | BB + EMA for quick entries with tight stops | High volatility |

---

## Disclaimer

This software is provided for **educational purposes only**. Cryptocurrency trading involves significant financial risk. The developers are not responsible for any trading losses. Always test with small amounts first. Past performance does not guarantee future results. Use at your own risk.

---

## License

MIT License — See [LICENSE](LICENSE) for details.
