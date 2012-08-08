using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// DataContract for Risk Index Exposures
    /// </summary>
    [DataContract]
    public class RiskIndexExposuresData
    {
        [DataMember]
        public string EntityType { get; set; }

        [DataMember]
        public Decimal? Momentum { get; set; }

        [DataMember]
        public Decimal? Volatility { get; set; }

        [DataMember]
        public Decimal? Value { get; set; }

        [DataMember]
        public Decimal? Size{ get; set; }

        [DataMember]
        public Decimal? SizeNonLinear{ get; set; }

        [DataMember]
        public Decimal? Growth { get; set; }

        [DataMember]
        public Decimal? Liquidity { get; set; }

        [DataMember]
        public Decimal? Leverage { get; set; }
    }
}
