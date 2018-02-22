using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace ConsoleApp1.SocketClient
{
    /// <summary>
    /// Represents tracking market candle ticker entity.
    /// </summary>
    [DataContract]
    [Table("marketcandles")]
    public class MarketCandle
    {
        /// <summary>
        /// Market candle sample identifier.
        /// </summary>
        [Key, Column("id")]
        public int ID { get; set; }

        /// <summary>
        /// Market name related to the candle sample.
        /// </summary>
        [Column("market")]
        public string Market { get; set; }

        /// <summary>
        /// Date and time span related to the candle sample.
        /// </summary>
        [DataMember]
        [Column("sample")]
        public DateTime Sample { get; set; }

        /// <summary>
        /// Date and time when the actual candle was taken.
        /// </summary>
        [DataMember]
        [Column("actual")]
        public DateTime Actual { get; set; }

        /// <summary>
        /// Market candle sample open value.
        /// </summary>
        [DataMember]
        [Column("open")]
        public decimal Open { get; set; }

        /// <summary>
        /// Market candle sample high value.
        /// </summary>
        [DataMember]
        [Column("high")]
        public decimal High { get; set; }

        /// <summary>
        /// Market candle sample low value.
        /// </summary>
        [DataMember]
        [Column("low")]
        public decimal Low { get; set; }

        /// <summary>
        /// Market candle sample close value.
        /// </summary>
        [DataMember]
        [Column("close")]
        public decimal Close { get; set; }

        /// <summary>
        /// Market candle sample referent volume value.
        /// </summary>
        [DataMember]
        [Column("rvolume")]
        public decimal ReferentVolume { get; set; }

        /// <summary>
        /// Market candle sample currency volume value.
        /// </summary>
        [DataMember]
        [Column("cvolume")]
        public decimal CurrencyVolume { get; set; }

        /// <summary>
        /// Market referent bids total sum.
        /// </summary>
        [DataMember]
        [Column("rbids")]
        public decimal ReferentBids { get; set; }

        /// <summary>
        /// Market currency bids total sum.
        /// </summary>
        [DataMember]
        [Column("cbids")]
        public decimal CurrencyBids { get; set; }

        /// <summary>
        /// Market referent asks total sum.
        /// </summary>
        [DataMember]
        [Column("rasks")]
        public decimal ReferentAsks { get; set; }

        /// <summary>
        /// Market currency asks total sum.
        /// </summary>
        [DataMember]
        [Column("casks")]
        public decimal CurrencyAsks { get; set; }

        /// <summary>
        /// The market state tracking data associated.
        /// </summary>
        [DataMember(Order = 900)]
        public virtual IList<MarketOrderEntry> Orders { get; set; }
    }
}