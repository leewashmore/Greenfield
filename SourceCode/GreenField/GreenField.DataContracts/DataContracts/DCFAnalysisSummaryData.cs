using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace GreenField.DataContracts
{
    /// <summary>
    /// Data-Contract for DCF AnalysisSummary
    /// </summary>
    [DataContract]
    public class DCFAnalysisSummaryData
    {
        [DataMember]
        public string SecurityId { get; set; }

        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public decimal MarketRiskPremium { get; set; }

        [DataMember]
        public decimal Beta { get; set; }

        [DataMember]
        public decimal RiskFreeRate { get; set; }

        [DataMember]
        public decimal StockSpecificDiscount { get; set; }

        [DataMember]
        public decimal MarginalTaxRate { get; set; }

        [DataMember]
        public decimal CostOfEquity { get; set; }

        [DataMember]
        public decimal CostOfDebt { get; set; }

        [DataMember]
        public decimal MarketCap { get; set; }

        [DataMember]
        public decimal GrossDebt { get; set; }

        [DataMember]
        public decimal WeightOfEquity { get; set; }

        [DataMember]
        public decimal WACC { get; set; }

    }
}
