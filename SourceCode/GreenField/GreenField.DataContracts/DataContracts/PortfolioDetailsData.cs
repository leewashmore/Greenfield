using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract for Portfolio Details Data
    /// </summary>
    [DataContract]
    public class PortfolioDetailsData
    {
        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public string AsecSecShortName { get; set; }

        [DataMember]
        public string IssueName { get; set; }

        [DataMember]
        public string Issuer_Proxy { get; set; }

        [DataMember]
        public string PortfolioPath { get; set; }

        [DataMember]
        public string PfcHoldingPortfolio { get; set; }

        [DataMember]
        public string PortfolioId { get; set; }

        [DataMember]
        public Boolean IsExpanded { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string SecurityThemeCode { get; set; }

        [DataMember]
        public string A_Sec_Instr_Type { get; set; }

        [DataMember]
        public string SecurityType { get; set; }

        [DataMember]
        public string ProprietaryRegionCode { get; set; }

        [DataMember]
        public string IsoCountryCode { get; set; }

        [DataMember]
        public string SectorName { get; set; }

        [DataMember]
        public string IndustryName { get; set; }

        [DataMember]
        public string SubIndustryName { get; set; }

        [DataMember]
        public decimal? BalanceNominal { get; set; }

        [DataMember]
        public decimal? DirtyValuePC { get; set; }

        [DataMember]
        public decimal? PortfolioDirtyValuePC { get; set; }

        [DataMember]
        public string TradingCurrency { get; set; }

        [DataMember]
        public decimal? AshEmmModelWeight { get; set; }

        [DataMember]
        public decimal? PortfolioWeight { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight { get; set; }

        [DataMember]
        public decimal? MarketCapUSD { get; set; }

        [DataMember]
        public decimal? ActivePosition { get; set; }

        [DataMember]
        public decimal? ReAshEmmModelWeight { get; set; }

        [DataMember]
        public decimal? RePortfolioWeight { get; set; }

        [DataMember]
        public decimal? ReBenchmarkWeight { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string SecurityId { get; set; }

        [DataMember]
        public decimal? MarketCap { get; set; }

        [DataMember]
        public decimal? Upside { get; set; }

        [DataMember]
        public decimal? ForwardPE { get; set; }

        [DataMember]
        public decimal? ForwardPBV { get; set; }

        [DataMember]
        public decimal? ForwardEB_EBITDA { get; set; }

        [DataMember]
        public decimal? RevenueGrowthCurrentYear { get; set; }

        [DataMember]
        public decimal? RevenueGrowthNextYear { get; set; }

        [DataMember]
        public decimal? NetIncomeGrowthCurrentYear { get; set; }

        [DataMember]
        public decimal? NetIncomeGrowthNextYear { get; set; }

        [DataMember]
        public decimal? ROE { get; set; }

        [DataMember]
        public decimal? NetDebtEquity { get; set; }

        [DataMember]
        public decimal? FreecashFlowMargin { get; set; }

        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public string IssuerName { get; set; }

        [DataMember]
        public decimal? FairValue { get; set; }

        public List<PortfolioDetailsData> Children { get; set; }
    }
}