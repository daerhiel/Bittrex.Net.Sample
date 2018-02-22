using Bittrex.Net.Objects;
using System;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the Bittrex exchange type conversion extensions.
    /// </summary>
    public static partial class BittrexTypes
    {
        /// <summary>
        /// Gets the converter for the exchanger market candle data to meta-market representation.
        /// </summary>
        /// <param name="actualTime">Actual time at which the market candle was received.</param>
        /// <returns>The converter delegate instance.</returns>
        public static Func<BittrexCandle, MarketCandle> CreateCandle(DateTime actualTime) => x => new MarketCandle
        {
            Actual = actualTime,
            Sample = x.Timestamp,
            Open = x.Open,
            High = x.High,
            Low = x.Low,
            Close = x.Close,
            CurrencyVolume = x.Volume,
            ReferentVolume = x.BaseVolume
        };

        /// <summary>
        /// Gets the converter for the exchanger market order book buys data to meta-market representation.
        /// </summary>
        /// <param name="type">True if the orderbook entry is a sell order, False, otherwise.</param>
        /// <returns>The converter delegate instance.</returns>
        public static Func<BittrexOrderBookEntry, MarketOrderEntry> CreateEntry(OrderBookEntry type) => x => new MarketOrderEntry
        {
            Type = type,
            Quantity = x.Quantity,
            Rate = x.Rate
        };

        /// <summary>
        /// Converts the order book entry type into order book frame.
        /// </summary>
        /// <param name="type">Order book entry type.</param>
        /// <returns>Order book frame.</returns>
        public static OrderBookFrame ConvertFrame(OrderBookEntryType type)
        {
            switch (type)
            {
                case OrderBookEntryType.NewEntry:
                    return OrderBookFrame.Create;
                case OrderBookEntryType.UpdateEntry:
                    return OrderBookFrame.Update;
                case OrderBookEntryType.RemoveEntry:
                    return OrderBookFrame.Remove;
                default:
                    throw new ArgumentException($"Unsupported order book entry type '{type}'.", nameof(type));
            }
        }

        /// <summary>
        /// Gets the converter for the exchanger market order book buys data to meta-market representation.
        /// </summary>
        /// <param name="type">True if the orderbook entry is a sell order, False, otherwise.</param>
        /// <returns>The converter delegate instance.</returns>
        public static Converter<BittrexOrderBookEntry, MarketOrderEntry> ConvertEntry(OrderBookEntry type) => x => new MarketOrderEntry
        {
            Type = type,
            Frame = ConvertFrame(x.Type),
            Quantity = x.Quantity,
            Rate = x.Rate
        };
    }
}