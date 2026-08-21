using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TradingBot.Config;
using TradingBot.Models;

namespace TradingBot.Exchange;

public sealed class BybitClient
{
    private readonly HttpClient _http;
    private readonly BotConfig _config;
    private const string BaseUrl = "https://api.bybit.com";

    public BybitClient(BotConfig config)
    {
        _config = config;
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public async Task<OrderResult> PlaceOrder(string symbol, TradeSide side, OrderType type, decimal quantity, decimal price, CancellationToken ct = default)
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var body = new
        {
            category = "spot",
            symbol,
            side = side.ToString(),
            orderType = type.ToString(),
            qty = quantity.ToString("F8"),
            price = type == OrderType.Limit ? price.ToString("F8") : null
        };

        string jsonBody = JsonSerializer.Serialize(body);
        string signPayload = $"{timestamp}{_config.ApiKey}{jsonBody}";
        string signature = Sign(signPayload);

        var request = new HttpRequestMessage(HttpMethod.Post, "/v5/order/create")
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };

        request.Headers.Add("X-BAPI-API-KEY", _config.ApiKey);
        request.Headers.Add("X-BAPI-SIGN", signature);
        request.Headers.Add("X-BAPI-TIMESTAMP", timestamp.ToString());

        var response = await _http.SendAsync(request, ct);
        var responseJson = await response.Content.ReadAsStringAsync(ct);
        var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

        return new OrderResult
        {
            OrderId = result.GetProperty("result").GetProperty("orderId").GetString() ?? "",
            Symbol = symbol,
            Side = side,
            Price = price,
            Quantity = quantity,
            Status = "NEW",
            Timestamp = DateTime.UtcNow
        };
    }

    public async Task<decimal> GetPrice(string symbol, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"/v5/market/tickers?category=spot&symbol={symbol}", ct);
        var json = await response.Content.ReadAsStringAsync(ct);
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        string priceStr = data.GetProperty("result").GetProperty("list")[0].GetProperty("lastPrice").GetString()!;
        return decimal.Parse(priceStr);
    }

    private string Sign(string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_config.ApiSecret));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexStringLower(hash);
    }
}
