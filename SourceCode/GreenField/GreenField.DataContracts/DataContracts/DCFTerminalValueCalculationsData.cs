using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data-Contract for DCF-TerminalValueCalculations
    /// </summary>
    public class DCFTerminalValueCalculationsData
    {
        [DataMember]
        public string SecurityId { get; set; }

        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public decimal CashFlow2020 { get; set; }

        [DataMember]
        public decimal? SustainableROIC { get; set; }

        [DataMember]
        public decimal SustainableDividendPayoutRatio { get; set; }

        [DataMember]
        public decimal LongTermNominalGDPGrowth { get; set; }

        [DataMember]
        public decimal TerminalGrowthRate { get; set; }

        [DataMember]
        public decimal TerminalValueNominal { get; set; }

        [DataMember]
        public decimal TerminalValuePresent { get; set; }

    }
}
