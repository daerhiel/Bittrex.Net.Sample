using System.Collections.Generic;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the abstract exchange state provider.
    /// </summary>
    public abstract class ExchangeStateProvider
    {
        /// <summary>
        /// Exchanger market name.
        /// </summary>
        public abstract string MarketName { get; }

        /// <summary>
        /// The list of bids associated with exchange state.
        /// </summary>
        public abstract IList<MarketOrderEntry> Bids { get; }

        /// <summary>
        /// The list of asks associated with exchange state.
        /// </summary>
        public abstract IList<MarketOrderEntry> Asks { get; }
    }
}