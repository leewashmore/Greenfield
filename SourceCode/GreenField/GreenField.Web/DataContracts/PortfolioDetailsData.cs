using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class PortfolioDetailsData
    {
        [DataMember]
        public string EntityTicker { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public double Shares { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public double Value { get; set; }

        [DataMember]
        public double TargetPerc { get; set; }

        [DataMember]
        public double PortfolioPerc { get; set; }

        [DataMember]
        public double BenchmarkPerc { get; set; }

        [DataMember]
        public double BetPerc { get; set; }

        [DataMember]
        public double Upside { get; set; }

        [DataMember]
        public double YTDReturn { get; set; }

        [DataMember]
        public decimal MarketCap { get; set; }

        [DataMember]
        public double PE_FWD { get; set; }

        [DataMember]
        public double PE_Fair { get; set; }

        [DataMember]
        public double PBE_FWD { get; set; }

        [DataMember]
        public double PBE_Fair { get; set; }

        [DataMember]
        public double EVEBITDA_FWD { get; set; }

        [DataMember]
        public double EVEBITDA_Fair { get; set; }

        [DataMember]
        public double SalesGrowthCurrentYear { get; set; }

        [DataMember]
        public double SalesGrowthNextYear { get; set; }

        [DataMember]
        public double NetIncomeGrowthCurrentYear { get; set; }

        [DataMember]
        public double NetIncomeGrowthNextYear { get; set; }

        [DataMember]
        public double ROECurrentYear { get; set; }

        [DataMember]
        public double NetDebtEquityCurrentYear { get; set; }

        [DataMember]
        public double FreeFlowCashMarginCurrentYear { get; set; }
    }
}