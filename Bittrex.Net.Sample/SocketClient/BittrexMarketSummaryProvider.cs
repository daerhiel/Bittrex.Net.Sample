using Bittrex.Net.Objects;
using System;
using System.Linq;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the market summary provider for Bittrex exchanger.
    /// </summary>
    public class BittrexMarketSummaryProvider : MarketSummaryProvider
    {
        /// <summary>
        /// Bittrex market summary data source.
        /// </summary>
        public BittrexMarketSummary Source { get; }

        /// <summary>
        /// Bittrex market name.
        /// </summary>
        public override string MarketName => Source?.MarketName;

        /// <summary>
        /// Date/time sample value.
        /// </summary>
        public override DateTime Sample => Source?.TimeStamp ?? DateTime.MinValue;

        /// <summary>
        /// Last market price value.
        /// </summary>
        public override decimal? Last => Source?.Last;

        /// <summary>
        /// Initializes the new instance of market summary provider for Bittrex exchanger.
        /// </summary>
        /// <param name="source">Market summary data source.</param>
        public BittrexMarketSummaryProvider(BittrexMarketSummary source)
        {
            Source = source;
        }
    }
}