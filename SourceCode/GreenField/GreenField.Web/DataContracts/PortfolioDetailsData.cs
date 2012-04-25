using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    /// <summary>
    /// Data Contract for Portfolio Details Data
    /// </summary>
    [DataContract]
    public class PortfolioDetailsData
    {
        //#region DataGridColumns

        //[DataMember]
        //public string EntityTicker { get; set; }

        //[DataMember]
        //public string EntityName { get; set; }

        //[DataMember]
        //public string Type { get; set; }

        //[DataMember]
        //public string Country { get; set; }

        //[DataMember]
        //public double Shares { get; set; }

        //[DataMember]
        //public double Price { get; set; }

        //[DataMember]
        //public string Currency { get; set; }

        //[DataMember]
        //public double Value { get; set; }

        //[DataMember]
        //public double TargetPerc { get; set; }

        //[DataMember]
        //public double PortfolioPerc { get; set; }

        //[DataMember]
        //public double BenchmarkPerc { get; set; }

        //[DataMember]
        //public double BetPerc { get; set; }

        //[DataMember]
        //public double Upside { get; set; }

        //[DataMember]
        //public double YTDReturn { get; set; }

        //[DataMember]
        //public decimal MarketCap { get; set; }

        //[DataMember]
        //public double PE_FWD { get; set; }

        //[DataMember]
        //public double PE_Fair { get; set; }

        //[DataMember]
        //public double PBE_FWD { get; set; }

        //[DataMember]
        //public double PBE_Fair { get; set; }

        //[DataMember]
        //public double EVEBITDA_FWD { get; set; }

        //[DataMember]
        //public double EVEBITDA_Fair { get; set; }

        //[DataMember]
        //public double SalesGrowthCurrentYear { get; set; }

        //[DataMember]
        //public double SalesGrowthNextYear { get; set; }

        //[DataMember]
        //public double NetIncomeGrowthCurrentYear { get; set; }

        //[DataMember]
        //public double NetIncomeGrowthNextYear { get; set; }

        //[DataMember]
        //public double ROECurrentYear { get; set; }

        //[DataMember]
        //public double NetDebtEquityCurrentYear { get; set; }

        //[DataMember]
        //public double FreeFlowCashMarginCurrentYear { get; set; }

        //#endregion

        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public string AsecSecShortName { get; set; }

        [DataMember]
        public string IssueName { get; set; }

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

    }
}