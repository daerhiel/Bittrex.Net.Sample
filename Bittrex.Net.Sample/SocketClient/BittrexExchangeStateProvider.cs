using Bittrex.Net.Objects;
using System.Collections.Generic;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the exchange state provider for Bittrex exchanger.
    /// </summary>
    public class BittrexExchangeStateProvider : ExchangeStateProvider
    {
        /// <summary>
        /// Bittrex exchange state data source.
        /// </summary>
        public BittrexExchangeState Source { get; }

        /// <summary>
        /// Bittrex market name.
        /// </summary>
        public override string MarketName => Source?.MarketName;

        /// <summary>
        /// The list of bids associated with exchange state.
        /// </summary>
        public override IList<MarketOrderEntry> Bids => Source?.Buys.ConvertAll(BittrexTypes.ConvertEntry(OrderBookEntry.Bid));

        /// <summary>
        /// The list of asks associated with exchange state.
        /// </summary>
        public override IList<MarketOrderEntry> Asks => Source?.Sells.ConvertAll(BittrexTypes.ConvertEntry(OrderBookEntry.Ask));

        /// <summary>
        /// Initializes the new exchange state provider for Bittrex exchanger.
        /// </summary>
        /// <param name="source">Exchange state data source.</param>
        public BittrexExchangeStateProvider(BittrexExchangeState source)
        {
            Source = source;
        }
    }
}