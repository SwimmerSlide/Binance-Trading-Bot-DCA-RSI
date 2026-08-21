using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TradingBot.Models;

namespace TradingBot.Exchange;

public sealed class WebSocketFeed : IDisposable
{
    private ClientWebSocket? _ws;
    private CancellationTokenSource? _cts;
    private readonly List<Action<CandleData>> _subscribers = [];

    public event Action<CandleData>? OnCandle;
    public event Action<string>? OnError;

    public async Task Connect(string url, string[] streams, CancellationToken ct = default)
    {
        _ws = new ClientWebSocket();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

        await _ws.ConnectAsync(new Uri(url), _cts.Token);

        var subscribe = new
        {
            method = "SUBSCRIBE",
            @params = streams,
            id = 1
        };

        string json = JsonSerializer.Serialize(subscribe);
        var bytes = Encoding.UTF8.GetBytes(json);
        await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, _cts.Token);

        _ = Task.Run(() => ReceiveLoop(_cts.Token), _cts.Token);
    }

    private async Task ReceiveLoop(CancellationToken ct)
    {
        var buffer = new byte[4096];

        while (!ct.IsCancellationRequested && _ws?.State == WebSocketState.Open)
        {
            try
            {
                var result = await _ws.ReceiveAsync(buffer, ct);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                ProcessMessage(message);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex.Message);
            }
        }
    }

    private void ProcessMessage(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("k", out var kline))
            {
                var candle = new CandleData
                {
                    Open = decimal.Parse(kline.GetProperty("o").GetString()!),
                    High = decimal.Parse(kline.GetProperty("h").GetString()!),
                    Low = decimal.Parse(kline.GetProperty("l").GetString()!),
                    Close = decimal.Parse(kline.GetProperty("c").GetString()!),
                    Volume = decimal.Parse(kline.GetProperty("v").GetString()!),
                    OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(kline.GetProperty("t").GetInt64()).UtcDateTime
                };

                OnCandle?.Invoke(candle);
            }
        }
        catch
        {
            // Non-kline message
        }
    }

    public async Task Disconnect()
    {
        _cts?.Cancel();
        if (_ws?.State == WebSocketState.Open)
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _ws?.Dispose();
        _cts?.Dispose();
    }
}
