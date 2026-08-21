using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TradingBot.Config;
using TradingBot.Models;

namespace TradingBot.Exchange;

public sealed class BinanceClient
{
    private readonly HttpClient _http;
    private readonly BotConfig _config;
    private const string BaseUrl = "https://api.binance.com";

    public BinanceClient(BotConfig config)
    {
        _config = config;
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        _http.DefaultRequestHeaders.Add("X-MBX-APIKEY", _config.ApiKey);
    }

    public async Task<List<CandleData>> GetCandles(string symbol, string interval, int limit, CancellationToken ct = default)
    {
        string url = $"/api/v3/klines?symbol={symbol}&interval={interval}&limit={limit}";
        var response = await _http.GetAsync(url, ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        var raw = JsonSerializer.Deserialize<JsonElement[][]>(json) ?? [];

        return raw.Select(k => new CandleData
        {
            OpenTime = DateTimeOffset.FromUnixTimeMilliseconds(k[0].GetInt64()).UtcDateTime,
            Open = decimal.Parse(k[1].GetString()!),
            High = decimal.Parse(k[2].GetString()!),
            Low = decimal.Parse(k[3].GetString()!),
            Close = decimal.Parse(k[4].GetString()!),
            Volume = decimal.Parse(k[5].GetString()!)
        }).ToList();
    }

    public async Task<OrderResult> PlaceOrder(string symbol, TradeSide side, OrderType type, decimal quantity, decimal price, CancellationToken ct = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["symbol"] = symbol,
            ["side"] = side.ToString().ToUpperInvariant(),
            ["type"] = type.ToString().ToUpperInvariant(),
            ["quantity"] = quantity.ToString("F8"),
            ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };

        if (type == OrderType.Limit)
        {
            parameters["price"] = price.ToString("F8");
            parameters["timeInForce"] = "GTC";
        }

        string queryString = string.Join("&", parameters.Select(kv => $"{kv.Key}={kv.Value}"));
        string signature = Sign(queryString);
        parameters["signature"] = signature;

        var content = new FormUrlEncodedContent(parameters);
        var response = await _http.PostAsync("/api/v3/order", content, ct);
        var responseJson = await response.Content.ReadAsStringAsync(ct);

        var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

        return new OrderResult
        {
            OrderId = result.GetProperty("orderId").GetInt64().ToString(),
            Symbol = symbol,
            Side = side,
            Price = price,
            Quantity = quantity,
            Status = result.GetProperty("status").GetString() ?? "UNKNOWN",
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<decimal> GetPrice(string symbol, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/api/v3/ticker/price?symbol={symbol}", ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        return decimal.Parse(data.GetProperty("price").GetString()!);
    }

    private string Sign(string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_config.ApiSecret));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexStringLower(hash);
    }
}
