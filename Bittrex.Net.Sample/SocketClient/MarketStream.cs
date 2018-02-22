using System;
using System.Collections.Generic;
using System.Threading;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represent internal market stream references.
    /// </summary>
    public class MarketStream : IDisposable
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private List<MarketOrderEntry> _bids = new List<MarketOrderEntry>();
        private List<MarketOrderEntry> _asks = new List<MarketOrderEntry>();
        private MarketStreamSeries _candles1Mins = new MarketStreamSeries(1, (int)_seriesSpan.TotalMinutes);
        private MarketStreamSeries _candles5Mins = new MarketStreamSeries(5, (int)_seriesSpan.TotalMinutes);
        private MarketStreamSeries _candles15Mins = new MarketStreamSeries(15, (int)_seriesSpan.TotalMinutes);
        private MarketStreamSeries _candles30Mins = new MarketStreamSeries(30, (int)_seriesSpan.TotalMinutes);
        private decimal _bidsReferentTotal;
        private decimal _bidsCurrencyTotal;
        private decimal _asksReferentTotal;
        private decimal _asksCurrencyTotal;
        private bool _isDisposed;

        private static TimeSpan _seriesSpan = TimeSpan.FromDays(28);

        /// <summary>
        /// Market Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Exchange delta stream identifier.
        /// </summary>
        public int ExchangeDeltaStream { get; }

        /// <summary>
        /// Market delta stream identifier.
        /// </summary>
        public int MarketDeltaStream { get; }

        /// <summary>
        /// Candle stream with 1-minute time series interval.
        /// </summary>
        public MarketStreamSeries Candles1Mins => _candles1Mins;

        /// <summary>
        /// Candle stream with 5-minute time series interval.
        /// </summary>
        public MarketStreamSeries Candles5Mins => _candles5Mins;

        /// <summary>
        /// Candle stream with 15-minute time series interval.
        /// </summary>
        public MarketStreamSeries Candles15Mins => _candles15Mins;

        /// <summary>
        /// Candle stream with 30-minute time series interval.
        /// </summary>
        public MarketStreamSeries Candles30Mins => _candles30Mins;

        /// <summary>
        /// Initializes the new instance internal market stream reference.
        /// </summary>
        /// <param name="marketName">Market name.</param>
        /// <param name="exchangeState">Exchange initial states.</param>
        /// <param name="exchangeDelta">Exchange delta stream identifier.</param>
        /// <param name="marketDelta">Market delta stream identifier.</param>
        public MarketStream(string marketName, ExchangeStateProvider exchangeState, int exchangeDelta, int marketDelta)
        {
            Name = marketName;
            UpdateExchangeDelta(exchangeState);
            ExchangeDeltaStream = exchangeDelta;
            MarketDeltaStream = marketDelta;
        }

        /// <summary>
        /// Updates the received changes in exchange data.
        /// </summary>
        /// <param name="exchangeDelta">Exchange delta stream data.</param>
        public void UpdateExchangeDelta(ExchangeStateProvider exchangeDelta)
        {
            if (exchangeDelta != null && Name == exchangeDelta.MarketName)
            {
                _lock.EnterWriteLock();
                try
                {
                    if (exchangeDelta.Bids is IList<MarketOrderEntry> bids)
                        foreach (var bid in bids)
                            switch (bid.Frame)
                            {
                                case OrderBookFrame.Create:
                                    _bidsReferentTotal += bid.Quantity * bid.Rate;
                                    _bidsCurrencyTotal += bid.Quantity;
                                    _bids.Add(bid);
                                    break;
                                case OrderBookFrame.Update:
                                    if (_bids.FindIndex(x => x.Rate == bid.Rate) is var update && update >= 0)
                                    {
                                        var delta = bid.Quantity - _bids[update].Quantity;
                                        _bidsReferentTotal += delta * bid.Rate;
                                        _bidsCurrencyTotal += delta;
                                        _bids[update] = bid;
                                    }
                                    break;
                                case OrderBookFrame.Remove:
                                    if (_bids.FindIndex(x => x.Rate == bid.Rate) is var remove && remove >= 0)
                                    {
                                        var delta = bid.Quantity - _bids[remove].Quantity;
                                        _bidsReferentTotal += delta * bid.Rate;
                                        _bidsCurrencyTotal += delta;
                                        _bids.RemoveAt(remove);
                                    }
                                    break;
                            }
                    if (exchangeDelta.Asks is IList<MarketOrderEntry> asks)
                        foreach (var ask in asks)
                            switch (ask.Frame)
                            {
                                case OrderBookFrame.Create:
                                    _asksReferentTotal += ask.Quantity * ask.Rate;
                                    _asksCurrencyTotal += ask.Quantity;
                                    _asks.Add(ask);
                                    break;
                                case OrderBookFrame.Update:
                                    if (_asks.FindIndex(x => x.Rate == ask.Rate) is var update && update >= 0)
                                    {
                                        var delta = ask.Quantity - _asks[update].Quantity;
                                        _asksReferentTotal += delta * ask.Rate;
                                        _asksCurrencyTotal += delta;
                                        _asks[update] = ask;
                                    }
                                    break;
                                case OrderBookFrame.Remove:
                                    if (_asks.FindIndex(x => x.Rate == ask.Rate) is var remove && remove >= 0)
                                    {
                                        var delta = ask.Quantity - _asks[remove].Quantity;
                                        _asksReferentTotal += delta * ask.Rate;
                                        _asksCurrencyTotal += delta;
                                        _asks.RemoveAt(remove);
                                    }
                                    break;
                            }
                    Console.WriteLine($"Market Depth {Name}: BID={_bidsCurrencyTotal} / ASK={_asksCurrencyTotal} - [Base: BID={_bidsReferentTotal} / ASK={_asksReferentTotal}]");
                }
                finally
                { _lock.ExitWriteLock(); }
            }
        }

        /// <summary>
        /// Updates the received changes in market data.
        /// </summary>
        /// <param name="marketDelta">Market delta stream data.</param>
        public void UpdateMarketDelta(MarketSummaryProvider marketDelta)
        {
            if (marketDelta != null && Name == marketDelta.MarketName)
            {
                _candles1Mins.MergeDelta(marketDelta);
                _candles5Mins.MergeDelta(marketDelta);
                _candles15Mins.MergeDelta(marketDelta);
                _candles30Mins.MergeDelta(marketDelta);
                var candle1min = _candles1Mins.Last;
                Console.WriteLine($"[{candle1min.Sample}] Candle: 1 Min {candle1min.Open} - [{candle1min.Low} - {candle1min.High}] - {candle1min.Close}");
                var candle5min = _candles5Mins.Last;
                Console.WriteLine($"[{candle5min.Sample}] Candle: 5 Min {candle5min.Open} - [{candle5min.Low} - {candle5min.High}] - {candle5min.Close}");
                var candle15min = _candles15Mins.Last;
                Console.WriteLine($"[{candle15min.Sample}] Candle: 15 Min {candle15min.Open} - [{candle15min.Low} - {candle15min.High}] - {candle15min.Close}");
                var candle30min = _candles30Mins.Last;
                Console.WriteLine($"[{candle30min.Sample}] Candle: 30 Min {candle30min.Open} - [{candle30min.Low} - {candle30min.High}] - {candle30min.Close}");
            }
        }

        /// <summary>
        /// Performs tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if managed resources should be released; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _candles1Mins.Dispose();
                    _candles5Mins.Dispose();
                    _candles15Mins.Dispose();
                    _candles30Mins.Dispose();
                    _lock.Dispose();
                }
                _isDisposed = true;
            }
        }

        /// <summary>
        /// Performs tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}