using System;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the abstract market summary provider.
    /// </summary>
    public abstract class MarketSummaryProvider
    {
        /// <summary>
        /// Exchanger market name.
        /// </summary>
        public abstract string MarketName { get; }

        /// <summary>
        /// Date/time sample value.
        /// </summary>
        public abstract DateTime Sample { get; }

        /// <summary>
        /// Last market price value.
        /// </summary>
        public abstract decimal? Last { get; }
    }
}