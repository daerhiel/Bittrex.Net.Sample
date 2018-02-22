using System.Runtime.Serialization;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the market order book entry type enumeration.
    /// </summary>
    [DataContract]
    public enum OrderBookEntry
    {
        /// <summary>
        /// The entry is a buy order accumulator.
        /// </summary>
        [EnumMember]
        Bid,

        /// <summary>
        /// The entry is a sell order accumulator.
        /// </summary>
        [EnumMember]
        Ask
    }
}