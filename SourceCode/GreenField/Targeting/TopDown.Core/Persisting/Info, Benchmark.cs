using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class BenchmarkInfo
    {
		[DebuggerStepThrough]
		public BenchmarkInfo()
		{
		}
	
        [DebuggerStepThrough]
		public BenchmarkInfo(DateTime portfolioDate, String portfolioId, String benchmarkId, String issuerId, String asecSecShortName, String issueName, String ticker, Decimal? benchmarkWeight, String isoCountryCode, String countryName, String gicsSector, String gicsSectorName, String gicsIndustry, String gicsIndustryName, String gicsSubIndustry, String gicsSubIndustryName)
        {
			this.PortfolioDate = portfolioDate;
			this.PortfolioId = portfolioId;
			this.BenchmarkId = benchmarkId;
			this.IssuerId = issuerId;
			this.AsecSecShortName = asecSecShortName;
			this.IssueName = issueName;
			this.Ticker = ticker;
			this.BenchmarkWeight = benchmarkWeight;
			this.IsoCountryCode = isoCountryCode;
			this.CountryName = countryName;
			this.GicsSector = gicsSector;
			this.GicsSectorName = gicsSectorName;
			this.GicsIndustry = gicsIndustry;
			this.GicsIndustryName = gicsIndustryName;
			this.GicsSubIndustry = gicsSubIndustry;
			this.GicsSubIndustryName = gicsSubIndustryName;
        }

		//[DebuggerStepThrough]
		//public BenchmarkInfo(Decimal gfId, DateTime portfolioDate, String portfolioId, String portfolioThemeSubgroupCode, String portfolioCurrency, String benchmarkId, String issuerId, String asecSecShortName, String issueName, String ticker, String securityThemeCode, String asecInstrType, String securityType, Decimal? balanceNominal, Decimal? dirtyPrice, String tradingCurrency, Decimal? dirtyValuePc, Decimal? benchmarkWeight, Decimal? ashEmmModelWeight, Decimal? marketCapInUsd, String ashemmPropRegionCode, String ashemmPropRegionName, String isoCountryCode, String countryName, String gicsSector, String gicsSectorName, String gicsIndustry, String gicsIndustryName, String gicsSubIndustry, String gicsSubIndustryName, String lookThruFund, Decimal? barraRiskFactorMomentum, Decimal? barraRiskFactorVolatility, Decimal? barraRiskFactorValue, Decimal? barraRiskFactorSize, Decimal? barraRiskFactorSizeNonlin, Decimal? barraRiskFactorGrowth, Decimal? barraRiskFactorLiquidity, Decimal? barraRiskFactorLeverage, Decimal? barraRiskFactorPbetewld)
		//{ 
		//    this.GfId = GfId;
		//    this.PortfolioDate = PortfolioDate;
		//    this.PortfolioId = PortfolioId;
		//    this.PortfolioThemeSubgroupCode = PortfolioThemeSubgroupCode;
		//    this.PortfolioCurrency = PortfolioCurrency;
		//    this.BenchmarkId = BenchmarkId;
		//    this.IssuerId = IssuerId;
		//    this.AsecSecShortName = AsecSecShortName;
		//    this.IssueName = IssueName;
		//    this.Ticker = Ticker;
		//    this.SecurityThemeCode = SecurityThemeCode;
		//    this.ASecInstrType = ASecInstrType;
		//    this.SecurityType = SecurityType;
		//    this.BalanceNominal = BalanceNominal;
		//    this.DirtyPrice = DirtyPrice;
		//    this.TradingCurrency = TradingCurrency;
		//    this.DirtyValuePc = DirtyValuePc;
		//    this.BenchmarkWeight = BenchmarkWeight;
		//    this.AshEmmModelWeight = AshEmmModelWeight;
		//    this.MarketCapInUsd = MarketCapInUsd;
		//    this.AshemmPropRegionCode = AshemmPropRegionCode;
		//    this.AshemmPropRegionName = AshemmPropRegionName;
		//    this.IsoCountryCode = IsoCountryCode;
		//    this.CountryName = CountryName;
		//    this.GicsSector = GicsSector;
		//    this.GicsSectorName = GicsSectorName;
		//    this.GicsIndustry = GicsIndustry;
		//    this.GicsIndustryName = GicsIndustryName;
		//    this.GicsSubIndustry = GicsSubIndustry;
		//    this.GicsSubIndustryName = GicsSubIndustryName;
		//    this.LookThruFund = LookThruFund;
		//    this.BarraRiskFactorMomentum = BarraRiskFactorMomentum;
		//    this.BarraRiskFactorVolatility = BarraRiskFactorVolatility;
		//    this.BarraRiskFactorValue = BarraRiskFactorValue;
		//    this.BarraRiskFactorSize = BarraRiskFactorSize;
		//    this.BarraRiskFactorSizeNonlin = BarraRiskFactorSizeNonlin;
		//    this.BarraRiskFactorGrowth = BarraRiskFactorGrowth;
		//    this.BarraRiskFactorLiquidity = BarraRiskFactorLiquidity;
		//    this.BarraRiskFactorLeverage = BarraRiskFactorLeverage;
		//    this.BarraRiskFactorPbetewld = BarraRiskFactorPbetewld;
		//}



        //public Decimal GfId { get; set; }
        public DateTime PortfolioDate { get; set; }
        public String PortfolioId { get; set; }
        //public String PortfolioThemeSubgroupCode { get; set; }
        //public String PortfolioCurrency { get; set; }
        public String BenchmarkId { get; set; }
        public String IssuerId { get; set; }
        public String AsecSecShortName { get; set; }
        public String IssueName { get; set; }
        public String Ticker { get; set; }
        //public String SecurityThemeCode { get; set; }
        //public String ASecInstrType { get; set; }
        //public String SecurityType { get; set; }
        //public Decimal? BalanceNominal { get; set; }
        //public Decimal? DirtyPrice { get; set; }
        //public String TradingCurrency { get; set; }
        //public Decimal? DirtyValuePc { get; set; }
        public Decimal? BenchmarkWeight { get; set; }
        //public Decimal? AshEmmModelWeight { get; set; }
        //public Decimal? MarketCapInUsd { get; set; }
        //public String AshemmPropRegionCode { get; set; }
        //public String AshemmPropRegionName { get; set; }
        public String IsoCountryCode { get; set; }
        public String CountryName { get; set; }
        public String GicsSector { get; set; }
        public String GicsSectorName { get; set; }
        public String GicsIndustry { get; set; }
        public String GicsIndustryName { get; set; }
        public String GicsSubIndustry { get; set; }
        public String GicsSubIndustryName { get; set; }
        //public String LookThruFund { get; set; }
        //public Decimal? BarraRiskFactorMomentum { get; set; }
        //public Decimal? BarraRiskFactorVolatility { get; set; }
        //public Decimal? BarraRiskFactorValue { get; set; }
        //public Decimal? BarraRiskFactorSize { get; set; }
        //public Decimal? BarraRiskFactorSizeNonlin { get; set; }
        //public Decimal? BarraRiskFactorGrowth { get; set; }
        //public Decimal? BarraRiskFactorLiquidity { get; set; }
        //public Decimal? BarraRiskFactorLeverage { get; set; }
        //public Decimal? BarraRiskFactorPbetewld { get; set; }
    }
}
