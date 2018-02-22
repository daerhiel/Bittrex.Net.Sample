using System.Runtime.Serialization;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents the order book entry source type
    /// </summary>
    [DataContract]
    public enum OrderBookFrame : byte
    {
        /// <summary>
        /// Entry is absolute, non-cumulative and serves as a reference frame.
        /// </summary>
        [EnumMember]
        Frame,

        /// <summary>
        /// Entry is created in current differential frame.
        /// </summary>
        [EnumMember]
        Create,

        /// <summary>
        /// Entry is updated in current differential frame.
        /// </summary>
        [EnumMember]
        Update,

        /// <summary>
        /// Entry is removed in current differential frame.
        /// </summary>
        [EnumMember]
        Remove
    }
}