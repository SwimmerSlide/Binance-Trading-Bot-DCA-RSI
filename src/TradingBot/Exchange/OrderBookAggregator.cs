namespace TradingBot.Exchange;

public sealed class OrderBookAggregator
{
    private readonly SortedDictionary<decimal, decimal> _bids = new(Comparer<decimal>.Create((a, b) => b.CompareTo(a)));
    private readonly SortedDictionary<decimal, decimal> _asks = [];
    private readonly Lock _lock = new();

    public void UpdateBid(decimal price, decimal quantity)
    {
        lock (_lock)
        {
            if (quantity <= 0)
                _bids.Remove(price);
            else
                _bids[price] = quantity;
        }
    }

    public void UpdateAsk(decimal price, decimal quantity)
    {
        lock (_lock)
        {
            if (quantity <= 0)
                _asks.Remove(price);
            else
                _asks[price] = quantity;
        }
    }

    public decimal GetBestBid()
    {
        lock (_lock)
        {
            return _bids.Count > 0 ? _bids.First().Key : 0;
        }
    }

    public decimal GetBestAsk()
    {
        lock (_lock)
        {
            return _asks.Count > 0 ? _asks.First().Key : 0;
        }
    }

    public decimal GetSpread()
    {
        decimal bid = GetBestBid();
        decimal ask = GetBestAsk();
        return bid > 0 && ask > 0 ? ask - bid : 0;
    }

    public decimal GetMidPrice() => (GetBestBid() + GetBestAsk()) / 2;

    public decimal GetBidDepth(int levels = 10)
    {
        lock (_lock)
        {
            return _bids.Take(levels).Sum(x => x.Value * x.Key);
        }
    }

    public decimal GetAskDepth(int levels = 10)
    {
        lock (_lock)
        {
            return _asks.Take(levels).Sum(x => x.Value * x.Key);
        }
    }
}
