using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents tracking market orderbook entry object.
    /// </summary>
    [DataContract]
    [Table("marketorderbooks")]
    public class MarketOrderEntry
    {
        /// <summary>
        /// Market orderbook sample identifier.
        /// </summary>
        [Key, Column("id")]
        public int ID { get; set; }

        /// <summary>
        /// Market candle sample identifier.
        /// </summary>
        [Column("candle")]
        public int Candle { get; set; }

        /// <summary>
        /// Market orderbook entry quantity value.
        /// </summary>
        [DataMember]
        [Column("type")]
        public OrderBookEntry Type { get; set; }

        /// <summary>
        /// Market orderbook entry frame value source.
        /// </summary>
        [Column("frame")]
        public OrderBookFrame Frame { get; set; }

        /// <summary>
        /// Market orderbook entry quantity value.
        /// </summary>
        [DataMember]
        [Column("quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Market orderbook entry rate value.
        /// </summary>
        [DataMember]
        [Column("rate")]
        public decimal Rate { get; set; }
    }
}