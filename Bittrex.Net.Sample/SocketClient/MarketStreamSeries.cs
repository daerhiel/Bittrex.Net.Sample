using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the streamed candle data container.
    /// </summary>
    public class MarketStreamSeries : IDisposable
    {
        private ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private List<MarketCandle> _candles = new List<MarketCandle>(10000);
        private int _seriesLimit;
        private int _spanMinutes;
        private bool _isDisposed;

        public MarketCandle First => _candles.FirstOrDefault();

        public MarketCandle Last => _candles.LastOrDefault();

        public IList<MarketCandle> Candles
        {
            get
            {
                _lock.EnterReadLock();
                try
                { return _candles.ToArray(); }
                finally
                { _lock.ExitReadLock(); }
            }
        }

        /// <summary>
        /// Initializes the new instance of streamed candle data container.
        /// </summary>
        /// <param name="spanMinutes">The minite span interval to aggregate the data into.</param>
        public MarketStreamSeries(int spanMinutes, int seriesLimit)
        {
            if ((TimeSpan.TicksPerHour / TimeSpan.TicksPerMinute) % spanMinutes == 0)
                if (seriesLimit % spanMinutes == 0)
                {
                    _spanMinutes = spanMinutes;
                    _seriesLimit = seriesLimit;
                }
                else
                    throw new ArgumentException($"Series limit {seriesLimit} is not supported, as it doesn't cover span minutes N times.", nameof(seriesLimit));
            else
                throw new ArgumentException($"Minutes span {spanMinutes} is not supported, as it doesn't fit into an hour N times.", nameof(spanMinutes));
        }

        /// <summary>
        /// Merges the marked data into candle stream.
        /// </summary>
        /// <param name="marketDelta">Market delta stream data.</param>
        public void MergeDelta(MarketSummaryProvider marketDelta)
        {
            if (marketDelta != null)
            {
                _lock.EnterWriteLock();
                try
                {
                    var reference = TimeCalculator.GetReference(marketDelta.Sample, _spanMinutes);
                    var candle = _candles.LastOrDefault(x => x.Sample == reference);
                    if (candle == null)
                    {
                        if ((candle = _candles.LastOrDefault()) != null)
                            do
                                _candles.Add(candle = new MarketCandle { Sample = candle.Sample.AddMinutes(_spanMinutes) });
                            while (candle.Sample < reference);
                        else
                            _candles.Add(candle = new MarketCandle { Sample = reference });
                        candle.Open = marketDelta.Last.Value;
                        candle.Low = marketDelta.Last.Value;
                        candle.High = marketDelta.Last.Value;
                        candle.Close = marketDelta.Last.Value;
                    }
                    if (marketDelta.Last != null)
                    {
                        if (candle.Low > marketDelta.Last)
                            candle.Low = marketDelta.Last.Value;
                        if (candle.High < marketDelta.Last)
                            candle.High = marketDelta.Last.Value;
                        candle.Close = marketDelta.Last.Value;
                    }
                    while (_candles.Count > _seriesLimit / _spanMinutes)
                        _candles.RemoveAt(0);
                }
                finally
                { _lock.ExitWriteLock(); }
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
                    _lock.Dispose();
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