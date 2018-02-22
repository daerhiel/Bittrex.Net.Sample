using Bittrex.Net;
using Bittrex.Net.Objects;
using ConsoleApp1.SocketClient;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        private static readonly BittrexSocketClient _onlineClient = new BittrexSocketClient();
        private static readonly Dictionary<string, MarketStream> _streams = new Dictionary<string, MarketStream>(StringComparer.OrdinalIgnoreCase);
        private static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        /// <summary>
        /// Called on exchanger service connection lost.
        /// </summary>
        private static void OnConnectionLost()
        {
            foreach (var association in _streams)
            {
            }
        }

        /// <summary>
        /// Called on exchanger service connection restored.
        /// </summary>
        private static void OnConnectionRestored()
        {
            foreach (var association in _streams)
            {
            }
        }

        /// <summary>
        /// Responds to the changes in exchange data.
        /// </summary>
        /// <param name="exchangeDelta">Exchange delta stream data.</param>
        protected static void OnExchangeDeltaUpdate(BittrexExchangeState exchangeDelta)
        {
            if (_streams.TryGetValue(exchangeDelta.MarketName, out var stream))
                stream.UpdateExchangeDelta(new BittrexExchangeStateProvider(exchangeDelta));
        }

        /// <summary>
        /// Responds to the changes in market data.
        /// </summary>
        /// <param name="marketDelta">Market delta stream data.</param>
        protected static void OnMarketDeltaUpdate(BittrexMarketSummary marketDelta)
        {
            if (_streams.TryGetValue(marketDelta.MarketName, out var stream))
                stream.UpdateMarketDelta(new BittrexMarketSummaryProvider(marketDelta));
        }

        /// <summary>
        /// Starts monitoring the market name stream.
        /// </summary>
        /// <param name="marketName">Market name.</param>
        /// <returns>Asynchronous task result.</returns>
        public static void StartMonitoring(string marketName)
        {
            if (!_streams.TryGetValue(marketName, out var stream))
            {
                var exchangeState = _onlineClient.QueryExchangeState(marketName);
                var exchangeDelta = _onlineClient.SubscribeToExchangeDeltas(marketName, OnExchangeDeltaUpdate);
                var marketDelta = _onlineClient.SubscribeToMarketDeltaStream(marketName, OnMarketDeltaUpdate);
                _streams.Add(marketName, new MarketStream(marketName, new BittrexExchangeStateProvider(exchangeState.Result), exchangeDelta.Result, marketDelta.Result));
            }
        }

        /// <summary>
        /// Stops monitoring the market name stream.
        /// </summary>
        /// <param name="marketName">Market name.</param>
        /// <returns>Asynchronous task result.</returns>
        public static void StopMonitoring(string marketName)
        {
            if (_streams.TryGetValue(marketName, out var stream))
            {
                _onlineClient.UnsubscribeFromStream(stream.ExchangeDeltaStream);
                _onlineClient.UnsubscribeFromStream(stream.MarketDeltaStream);
            }
        }

        static void Main(string[] args)
        {
            BittrexSocketClient.ConnectionLost += OnConnectionLost;
            BittrexSocketClient.ConnectionRestored += OnConnectionRestored;
            StartMonitoring("BTC-ZCL");
            Task.Factory.StartNew(() => { while (true) Thread.Sleep(100); });
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            _closing.WaitOne();
            StopMonitoring("BTC-ZCL");
        }

        protected static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            Console.WriteLine("Exit");
            _closing.Set();
        }
    }
}
