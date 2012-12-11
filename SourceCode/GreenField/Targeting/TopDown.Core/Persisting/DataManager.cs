using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TopDown.Core.ManagingTaxonomies;
using System.Diagnostics;
using System.Data;
using Aims.Core.Persisting;
using Aims.Core.Sql;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Knows how to talk to the database.
    /// </summary>
    public class DataManager : Aims.Core.Persisting.DataManager, IDataManager
    {
        [DebuggerStepThrough]
        public DataManager(SqlConnection connection, SqlTransaction transactionOpt)
            : base(connection, transactionOpt)
        {
        }

        public virtual IEnumerable<BasketInfo> GetAllBaskets()
        {
            using (var builder = this.CreateQueryCommandBuilder<BasketInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [TYPE]", (info, value) => info.Type = value, true)
                .Text(" from [" + TableNames.BASKET + "]")
                .PullAll();
            }
        }
        public IEnumerable<CountryBasketInfo> GetAllCountryBaskets()
        {
            using (var builder = this.CreateQueryCommandBuilder<CountryBasketInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [ISO_COUNTRY_CODE]", (info, value) => info.IsoCountryCode = value, true)
                .Text(" from [" + TableNames.COUNTRY_BASKET + "]")
                .PullAll();
            }
        }
        public IEnumerable<RegionBasketInfo> GetAllRegionBaskets()
        {
            using (var builder = this.CreateQueryCommandBuilder<RegionBasketInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [DEFINITION]", (info, value) => info.DefinitionXml = value, true)
                .Text(" from [" + TableNames.REGION_BASKET + "]")
                .PullAll();
            }
        }

        public virtual DateTime GetLastestDateWhichBenchmarkDataIsAvialableOn()
        {
            DateTime result;
            // used to be Oct 17, 2012
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = "select top 1 PORTFOLIO_DATE from [" + TableNames.GF_BENCHMARK_HOLDINGS + "] order by [PORTFOLIO_DATE] desc";
                var something = command.ExecuteScalar();
                if (something is DBNull || something == null)
                {
                    throw new ApplicationException("There is no data in the \"" + TableNames.GF_BENCHMARK_HOLDINGS + "\" which makes it impossible to use benchmarks.");
                }
                else if (something is DateTime)
                {
                    result = (DateTime)something;
                }
                else
                {
                    throw new ApplicationException("The latest avaliable date for which there are some benchmakr information is expected to be of the " + typeof(DateTime).Name + " type, however it is \"" + something.GetType().FullName + "\".");
                }
            }
            return result;
        }
        public virtual IEnumerable<BenchmarkInfo> GetBenchmarks(DateTime date)
        {
            using (var builder = this.CreateQueryCommandBuilder<BenchmarkInfo>())
            {
                return builder.Text("select")
                    //.Field("  GF_ID", (BenchmarkInfo info, int value) => info.GfId = value)
                    .Field("  PORTFOLIO_DATE", (BenchmarkInfo info, DateTime value) => info.PortfolioDate = value)
                    .Field(",  PORTFOLIO_ID", (info, value) => info.PortfolioId = value, false)
                    //.Field(",  PORTFOLIO_THEME_SUBGROUP_CODE", (info, value) => info.PortfolioThemeSubgroupCode = value, false)
                    //.Field(",  PORTFOLIO_CURRENCY", (info, value) => info.PortfolioCurrency = value, false)
                    .Field(",  BENCHMARK_ID", (info, value) => info.BenchmarkId = value, false)
                    .Field(",  ISSUER_ID", (info, value) => info.IssuerId = value, false)
                    .Field(",  ASEC_SEC_SHORT_NAME", (info, value) => info.AsecSecShortName = value, false)
                    .Field(",  ISSUE_NAME", (info, value) => info.IssueName = value, false)
                    .Field(",  TICKER", (info, value) => info.Ticker = value, false)
                    //.Field(",  SECURITYTHEMECODE", (info, value) => info.SecurityThemeCode = value, false)
                    //.Field(",  A_SEC_INSTR_TYPE", (info, value) => info.ASecInstrType = value, false)
                    //.Field(",  SECURITY_TYPE", (info, value) => info.SecurityType = value, false)
                    //.Field(",  BALANCE_NOMINAL", (BenchmarkInfo info, decimal? value) => info.BalanceNominal = value)
                    //.Field(",  DIRTY_PRICE", (BenchmarkInfo info, decimal? value) => info.DirtyPrice = value)
                    //.Field(",  TRADING_CURRENCY", (info, value) => info.TradingCurrency = value, false)
                    //.Field(",  DIRTY_VALUE_PC", (BenchmarkInfo info, decimal? value) => info.DirtyValuePc = value)
                    .Field(",  BENCHMARK_WEIGHT", (BenchmarkInfo info, decimal? value) => info.BenchmarkWeight = value)
                    //.Field(",  ASH_EMM_MODEL_WEIGHT", (BenchmarkInfo info, decimal? value) => info.AshEmmModelWeight = value)
                    //.Field(",  MARKET_CAP_IN_USD", (BenchmarkInfo info, decimal? value) => info.MarketCapInUsd = value)
                    //.Field(",  ASHEMM_PROP_REGION_CODE", (info, value) => info.AshemmPropRegionCode = value, false)
                    //.Field(",  ASHEMM_PROP_REGION_NAME", (info, value) => info.AshemmPropRegionName = value, false)
                    .Field(",  ISO_COUNTRY_CODE", (info, value) => info.IsoCountryCode = value, false)
                    .Field(",  COUNTRYNAME", (info, value) => info.CountryName = value, false)
                    .Field(",  GICS_SECTOR", (info, value) => info.GicsSector = value, false)
                    .Field(",  GICS_SECTOR_NAME", (info, value) => info.GicsSectorName = value, false)
                    .Field(",  GICS_INDUSTRY", (info, value) => info.GicsIndustry = value, false)
                    .Field(",  GICS_INDUSTRY_NAME", (info, value) => info.GicsIndustryName = value, false)
                    .Field(",  GICS_SUB_INDUSTRY", (info, value) => info.GicsSubIndustry = value, false)
                    .Field(",  GICS_SUB_INDUSTRY_NAME", (info, value) => info.GicsSubIndustryName = value, false)
                    //.Field(",  LOOK_THRU_FUND", (info, value) => info.LookThruFund = value, false)
                    //.Field(",  BARRA_RISK_FACTOR_MOMENTUM", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorMomentum = value)
                    //.Field(",  BARRA_RISK_FACTOR_VOLATILITY", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorVolatility = value)
                    //.Field(",  BARRA_RISK_FACTOR_VALUE", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorValue = value)
                    //.Field(",  BARRA_RISK_FACTOR_SIZE", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorSize = value)
                    //.Field(",  BARRA_RISK_FACTOR_SIZE_NONLIN", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorSizeNonlin = value)
                    //.Field(",  BARRA_RISK_FACTOR_GROWTH", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorGrowth = value)
                    //.Field(",  BARRA_RISK_FACTOR_LIQUIDITY", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorLiquidity = value)
                    //.Field(",  BARRA_RISK_FACTOR_LEVERAGE", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorLeverage = value)
                    //.Field(",  BARRA_RISK_FACTOR_PBETEWLD", (BenchmarkInfo info, decimal? value) => info.BarraRiskFactorPbetewld = value)
                .Text(" from " + TableNames.GF_BENCHMARK_HOLDINGS + " where PORTFOLIO_DATE = ").Parameter(date)
                .PullAll();
            }
        }

        public virtual IEnumerable<BgaPortfolioSecurityFactorInfo> GetBgaPortfolioSecurityFactors(String porfolioId)
        {
            using (var builder = this.CreateQueryCommandBuilder<BgaPortfolioSecurityFactorInfo>())
            {
                return builder
                    .Text("select")
                        .Field("  [PORTFOLIO_ID]", (info, value) => info.BroadGlobalActivePortfolioId = value, true)
                        .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                        .Field(", [FACTOR]", (BgaPortfolioSecurityFactorInfo info, Decimal value) => info.Factor = value)
                        .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [" + TableNames.BGA_PORTFOLIO_SECURITY_FACTOR + "] where PORTFOLIO_ID = ").Parameter(porfolioId)
                    .PullAll();
            }
        }
        public IEnumerator<Int32> ReserveBasketIds(Int32 howMany)
        {
            return this.ReserveIds(TableNames.BASKET, howMany);
        }

        // P-S-T
        public virtual IEnumerable<BuPortfolioSecurityTargetInfo> GetAllPortfolioSecurityTargets()
        {
            using (var builder = this.CreateQueryCommandBuilder<BuPortfolioSecurityTargetInfo>())
            {
                return builder
                    .Text("select")
                        .Field("  [PORTFOLIO_ID]", (info, value) => info.BottomUpPortfolioId = value, true)
                        .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                        .Field(", [TARGET]", (BuPortfolioSecurityTargetInfo info, Decimal value) => info.Target = value)
                        .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET + "]")
                    .PullAll();
            }
        }
        public virtual IEnumerable<BuPortfolioSecurityTargetInfo> GetPortfolioSecurityTargets(String protfolioId)
        {
            using (var builder = this.CreateQueryCommandBuilder<BuPortfolioSecurityTargetInfo>())
            {
                return builder
                    .Text("select")
                        .Field("  [PORTFOLIO_ID]", (info, value) => info.BottomUpPortfolioId = value, true)
                        .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                        .Field(", [TARGET]", (BuPortfolioSecurityTargetInfo info, Decimal value) => info.Target = value)
                        .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET + "]")
                    .Text(" where [PORTFOLIO_ID] = ").Parameter(protfolioId)
                    .PullAll();
            }
        }
        public virtual BuPortfolioSecurityTargetChangesetInfo GetLatestPortfolioSecurityTargetChangeSet()
        {
            using (var builder = this.CreateQueryCommandBuilder<BuPortfolioSecurityTargetChangesetInfo>())
            {
                var foundOpt = builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [TIMESTAMP]", (info, value) => info.Timestamp = value)
                    .Field(", [CALCULATION_ID]", (info, value) => info.CalculationId = value)
                .Text(" from " + TableNames.BU_PORTFOLIO_SECURITY_TARGET_CHANGESET + " order by [TIMESTAMP] desc")
                .PullFirstOrDefault();
                if (foundOpt == null) throw new ApplicationException("There is no latest PST changeset.");
                return foundOpt;
            }
        }
        public virtual IEnumerator<Int32> ReservePortfolioSecurityTargetChangesetIds(Int32 howMany)
        {
            var result = this.ReserveIds(TableNames.BU_PORTFOLIO_SECURITY_TARGET_CHANGESET, howMany);
            return result;
        }
        public virtual Int32 InsertPortfolioSecurityTargetChangeset(BuPortfolioSecurityTargetChangesetInfo changesetInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET_CHANGESET + "] (")
                    .Text("  [ID]")
                    .Text(", [USERNAME]")
                    .Text(", [TIMESTAMP]")
                    .Text(", [CALCULATION_ID]")
                    .Text(") values (")
                        .Parameter(changesetInfo.Id)
                    .Text(", ")
                        .Parameter(changesetInfo.Username)
                    .Text(", ")
                        .Text("GETDATE()")
                    .Text(", ")
                        .Parameter(changesetInfo.CalculationId)
                    .Text(")")
                    .Execute();
            }
        }
        public virtual IEnumerator<Int32> ReservePortfolioSecurityTargetChangeIds(Int32 howMany)
        {
            var result = this.ReserveIds(TableNames.BU_PORTFOLIO_SECURITY_TARGET_CHANGE, howMany);
            return result;
        }
        public virtual Int32 InsertPortfolioSecurityTargetChange(BuPortfolioSecurityTargetChangeInfo changeInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET_CHANGE + "] (")
                    .Text("  [ID]")
                    .Text(", [CHANGESET_ID]")
                    .Text(", [PORTFOLIO_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [TARGET_BEFORE]")
                    .Text(", [TARGET_AFTER]")
                    .Text(", [COMMENT]")
                    .Text(") values (")
                        .Parameter(changeInfo.Id)
                    .Text(", ")
                        .Parameter(changeInfo.ChangesetId)
                    .Text(", ")
                        .Parameter(changeInfo.BottomUpPortfolioId)
                    .Text(", ")
                        .Parameter(changeInfo.SecurityId)
                    .Text(", ")
                        .Parameter(changeInfo.TargetBefore)
                    .Text(", ")
                        .Parameter(changeInfo.TargetAfter)
                    .Text(", ")
                        .Parameter(changeInfo.Comment)
                    .Text(")")
                    .Execute();
            }
        }
        public virtual Int32 InsertPortfolioSecurityTarget(BuPortfolioSecurityTargetInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET + "] (")
                .Text("  [PORTFOLIO_ID]")
                .Text(", [SECURITY_ID]")
                .Text(", [TARGET]")
                .Text(", [CHANGE_ID]")
                .Text(") values (")
                    .Parameter(info.BottomUpPortfolioId)
                .Text(", ")
                    .Parameter(info.SecurityId)
                .Text(", ")
                    .Parameter(info.Target)
                .Text(", ")
                    .Parameter(info.ChangeId)
                .Text(")")
                .Execute();
            }
        }
        public virtual Int32 UpdatePortfolioSecurityTarget(BuPortfolioSecurityTargetInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET + "] set")
                    .Text("  [TARGET] = ").Parameter(info.Target)
                    .Text(", [CHANGE_ID] = ").Parameter(info.ChangeId)
                    .Text(" where [PORTFOLIO_ID] = ").Parameter(info.BottomUpPortfolioId)
                    .Text(" and [SECURITY_ID] = ").Parameter(info.SecurityId)
                    .Execute();
            }
        }
        public virtual Int32 DeletePortfolioSecurityTarget(String portfolioId, String securityId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("delete from [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET + "]")
                        .Text(" where [PORTFOLIO_ID] = ").Parameter(portfolioId)
                        .Text(" and [SECURITY_ID] = ").Parameter(securityId)
                        .Execute();
            }
        }

        // T, TT, TTG, P
        public virtual IEnumerable<TargetingTypeGroupInfo> GetAllTargetingTypeGroups()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeGroupInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [NAME]", (info, value) => info.Name = value, true)
                    .Field(", [BENCHMARK_ID]", (info, value) => info.BenchmarkIdOpt = value, false)
                    .Text(" from [TARGETING_TYPE_GROUP]")
                    .PullAll();
            }
        }
        public virtual IEnumerable<TargetingTypeInfo> GetAllTargetingTypes()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [NAME]", (info, value) => info.Name = value, true)
                    .Field(", [TARGETING_TYPE_GROUP_ID]", (info, value) => info.TargetingTypeGroupId = value)
                    .Field(", [BENCHMARK_ID]", (info, value) => info.BenchmarkIdOpt = value, false)
                    .Field(", [TAXONOMY_ID]", (info, value) => info.TaxonomyId = value)
                    .Text("from [TARGETING_TYPE]")
                    .PullAll();
            }
        }
        public virtual IEnumerable<TargetingTypePortfolioInfo> GetAllTargetingTypePortfolio()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypePortfolioInfo>())
            {
                return builder.Text("select ")
                    .Field("  [TARGETING_TYPE_ID]", (info, value) => info.TargetingTypeId = value)
                    .Field(", [PORTFOLIO_ID]", (info, value) => info.PortfolioId = value, true)
                    .Text(" from [TARGETING_TYPE_PORTFOLIO]")
                    .PullAll();
            }
        }
        public virtual IEnumerable<TaxonomyInfo> GetAllTaxonomies()
        {
            using (var builder = this.CreateQueryCommandBuilder<TaxonomyInfo>())
            {
                return builder.Text("select ")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [DEFINITION]", (info, value) => info.DefinitionXml = value, true)
                    .Text(" from [TAXONOMY]")
                    .PullAll();
            }
        }
        public IEnumerable<UsernameBottomUpPortfolioInfo> GetUsernameBottomUpPortfolios(String username)
        {
            using (var builder = this.CreateQueryCommandBuilder<UsernameBottomUpPortfolioInfo>())
            {
                return builder.Text("select ")
                    .Field("  [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [PORTFOLIO_ID]", (info, value) => info.BottomUpPortfolioId = value, true)
                    .Text(" from [" + TableNames.USERNAME_FUND + "]")
                    .Text(" where [USERNAME] = ").Parameter(username)
                    .PullAll();
            }
        }

        // P-S-TO
        public virtual BgaPortfolioSecurityFactorChangesetInfo GetLatestBgaPortfolioSecurityFactorChangeset()
        {
            using (var builder = this.CreateQueryCommandBuilder<BgaPortfolioSecurityFactorChangesetInfo>())
            {
                return builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [TIMESTAMP]", (info, value) => info.Timestamp = value)
                    .Field(", [CALCULATION_ID]", (info, value) => info.CalculationId = value)
                    .Text(" from [BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET] order by [TIMESTAMP] desc")
                    .PullFirstOrDefault();
            }
        }
        public IEnumerator<Int32> ReserveBgaPortfolioSecurityFactorChangesetIds(Int32 howMany)
        {
            var result = this.ReserveIds("BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET", howMany);
            return result;
        }
        public Int32 InsertBgaPortfolioSecurityFactorChangeset(BgaPortfolioSecurityFactorChangesetInfo changesetInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET] (")
                    .Text("  [ID]")
                    .Text(", [USERNAME]")
                    .Text(", [TIMESTAMP]")
                    .Text(", [CALCULATION_ID]")
                    .Text(") values (")
                        .Parameter(changesetInfo.Id)
                    .Text(", ")
                        .Parameter(changesetInfo.Username)
                    .Text(", ")
                        .Text("GETDATE()")
                    .Text(", ")
                        .Parameter(changesetInfo.CalculationId)
                    .Text(")")
                    .Execute();
            }
        }
        public IEnumerator<Int32> ReserveBgaPortfolioSecurityFactorChangeIds(Int32 howMany)
        {
            var result = this.ReserveIds("BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE", howMany);
            return result;
        }
        public Int32 InsertBgaPortfolioSecurityFactorChange(BgaPortfolioSecurityFactorChangeInfo changeInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE] (")
                    .Text("  [ID]")
                    .Text(", [PORTFOLIO_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [FACTOR_BEFORE]")
                    .Text(", [FACTOR_AFTER]")
                    .Text(", [COMMENT]")
                    .Text(", [CHANGESET_ID]")
                    .Text(") values (")
                        .Parameter(changeInfo.Id)
                    .Text(", ")
                        .Parameter(changeInfo.BroadGlobalActivePortfolioId)
                    .Text(", ")
                        .Parameter(changeInfo.SecurityId)
                    .Text(", ")
                        .Parameter(changeInfo.FactorBefore)
                    .Text(", ")
                        .Parameter(changeInfo.FactorAfter)
                    .Text(", ")
                        .Parameter(changeInfo.Comment)
                    .Text(", ")
                        .Parameter(changeInfo.ChangesetId)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 InsertBgaPortfolioSecurityFactor(BgaPortfolioSecurityFactorInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [BGA_PORTFOLIO_SECURITY_FACTOR] (")
                    .Text("  [PORTFOLIO_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [FACTOR]")
                    .Text(", [CHANGE_ID]")
                    .Text(") values (")
                        .Parameter(info.BroadGlobalActivePortfolioId)
                    .Text(", ")
                        .Parameter(info.SecurityId)
                    .Text(", ")
                        .Parameter(info.Factor)
                    .Text(", ")
                        .Parameter(info.ChangeId)
                    .Text(")")
                    .Execute();
            }

        }
        public Int32 UpdateBgaPortfolioSecurityFactor(BgaPortfolioSecurityFactorInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [BGA_PORTFOLIO_SECURITY_FACTOR] set")
                    .Text("  [FACTOR] = ").Parameter(info.Factor)
                    .Text(", [CHANGE_ID] = ").Parameter(info.ChangeId)
                    .Text(" where [PORTFOLIO_ID] = ").Parameter(info.BroadGlobalActivePortfolioId)
                    .Text(" and   [SECURITY_ID] = ").Parameter(info.SecurityId)
                    .Execute();
            }

        }
        public Int32 DeleteBgaPortfolioSecurityFactor(String portfolioId, String securityId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("delete from [BGA_PORTFOLIO_SECURITY_FACTOR]")
                    .Text(" where [PORTFOLIO_ID] = ").Parameter(portfolioId)
                    .Text(" and   [SECURITY_ID] = ").Parameter(securityId)
                    .Execute();
            }
        }

        // TT-B-Bv
        public virtual IEnumerable<TargetingTypeBasketBaseValueInfo> GetTargetingTypeBasketBaseValues(Int32 targetingId)
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeBasketBaseValueInfo>())
            {
                return builder
                    .Text("select")
                        .Field("  [TARGETING_TYPE_ID]", (info, value) => info.TargetingTypeId = value)
                        .Field(", [BASKET_ID]", (info, value) => info.BasketId = value)
                        .Field(", [BASE_VALUE]", (TargetingTypeBasketBaseValueInfo info, Decimal value) => info.BaseValue = value)
                        .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [" + TableNames.TARGETING_TYPE_BASKET_BASE_VALUE + "]")
                    .Text(" where TARGETING_TYPE_ID = ")
                        .Parameter(targetingId)
                    .PullAll();
            }
        }
        public virtual TargetingTypeBasketBaseValueChangesetInfo GetLatestTargetingTypeBasketBaseValueChangeset()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeBasketBaseValueChangesetInfo>())
            {
                return builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [TIMESTAMP]", (info, value) => info.Timestamp = value)
                    .Field(", [CALCULATION_ID]", (info, value) => info.CalculationId = value)
                    .Text(" from [TARGETING_TYPE_BASKET_BASE_VALUE_CHANGESET] order by [TIMESTAMP] desc")
                    .PullFirstOrDefault();
            }
        }
        public IEnumerator<Int32> ReserveTargetingTypeBasketBaseValueChangesetIds(int howMany)
        {
            var result = this.ReserveIds(TableNames.TARGETING_TYPE_BASKET_BASE_VALUE_CHANGESET, howMany);
            return result;
        }
        public Int32 InsertTargetingTypeBasketBaseValueChangeset(TargetingTypeBasketBaseValueChangesetInfo changesetInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [" + TableNames.TARGETING_TYPE_BASKET_BASE_VALUE_CHANGESET + "] (")
                    .Text("  [ID]")
                    .Text(", [USERNAME]")
                    .Text(", [TIMESTAMP]")
                    .Text(", [CALCULATION_ID]")
                    .Text(") values (")
                        .Parameter(changesetInfo.Id)
                    .Text(", ")
                        .Parameter(changesetInfo.Username)
                    .Text(", ")
                        .Text("GETDATE()")
                    .Text(", ")
                        .Parameter(changesetInfo.CalculationId)
                    .Text(")")
                    .Execute();
            }
        }
        public IEnumerator<Int32> ReserveTargetingTypeBasketBaseValueChangeIds(Int32 howMany)
        {
            var result = this.ReserveIds(TableNames.TARGETING_TYPE_BASKET_BASE_VALUE_CHANGE, howMany);
            return result;
        }
        public Int32 InsertTargetingTypeBasketBaseValueChange(TargetingTypeBasketBaseValueChangeInfo changeInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_BASKET_BASE_VALUE_CHANGE] (")
                    .Text("  [ID]")
                    .Text(", [CHANGESET_ID]")
                    .Text(", [TARGETING_TYPE_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [BASE_VALUE_BEFORE]")
                    .Text(", [BASE_VALUE_AFTER]")
                    .Text(", [COMMENT]")
                    .Text(") values (")
                        .Parameter(changeInfo.Id)
                    .Text(", ")
                        .Parameter(changeInfo.ChangesetId)
                    .Text(", ")
                        .Parameter(changeInfo.TargetingTypeGroupId)
                    .Text(", ")
                        .Parameter(changeInfo.BasketId)
                    .Text(", ")
                        .Parameter(changeInfo.BaseValueBefore)
                    .Text(", ")
                        .Parameter(changeInfo.BaseValueAfter)
                    .Text(", ")
                        .Parameter(changeInfo.Comment)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 InsertTargetingTypeBasketBaseValue(TargetingTypeBasketBaseValueInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_BASKET_BASE_VALUE] (")
                    .Text("  [TARGETING_TYPE_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [BASE_VALUE]")
                    .Text(", [CHANGE_ID]")
                    .Text(") values (")
                        .Parameter(info.TargetingTypeId)
                    .Text(", ")
                        .Parameter(info.BasketId)
                    .Text(", ")
                        .Parameter(info.BaseValue)
                    .Text(", ")
                        .Parameter(info.ChangeId)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 UpdateTargetingTypeBasketBaseValue(TargetingTypeBasketBaseValueInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [TARGETING_TYPE_BASKET_BASE_VALUE] set")
                    .Text("  [BASE_VALUE] = ").Parameter(info.BaseValue)
                    .Text(", [CHANGE_ID] = ").Parameter(info.ChangeId)
                    .Text(" where [TARGETING_TYPE_ID] = ").Parameter(info.TargetingTypeId)
                    .Text(" and   [BASKET_ID] = ").Parameter(info.BasketId)
                    .Execute();
            }
        }
        public Int32 DeleteTargetingTypeBasketBaseValue(Int32 targetingTypeId, Int32 basketId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("delete from [TARGETING_TYPE_BASKET_BASE_VALUE]")
                    .Text(" where [TARGETING_TYPE_ID] = ").Parameter(targetingTypeId)
                    .Text(" and   [BASKET_ID] = ").Parameter(basketId)
                    .Execute();
            }
        }

        // TTG-B-S-V

        public virtual TargetingTypeGroupBasketSecurityBaseValueChangesetInfo GetLatestTargetingTypeGroupBasketSecurityBaseValueChangeset()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeGroupBasketSecurityBaseValueChangesetInfo>())
            {
                return builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [TIMESTAMP]", (info, value) => info.Timestamp = value)
                    .Text(" from [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE_CHANGESET] order by [TIMESTAMP] desc")
                    .PullFirstOrDefault();
            }
        }
        public IEnumerator<Int32> ReserveTargetingTypeGroupBasketSecurityBaseValueChangesetIds(Int32 howMany)
        {
            var result = this.ReserveIds("TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE_CHANGESET", howMany);
            return result;
        }
        public Int32 InsertTargetingTypeGroupBasketSecurityBaseValueChangeset(TargetingTypeGroupBasketSecurityBaseValueChangesetInfo changesetInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE_CHANGESET] (")
                    .Text("  [ID]")
                    .Text(", [USERNAME]")
                    .Text(", [TIMESTAMP]")
                    .Text(", [CALCULATION_ID]")
                    .Text(") values (")
                        .Parameter(changesetInfo.Id)
                    .Text(", ")
                        .Parameter(changesetInfo.Username)
                    .Text(", ")
                        .Text("GETDATE()")
                    .Text(", ")
                        .Parameter(changesetInfo.CalculationId)
                    .Text(")")
                    .Execute();
            }
        }
        public IEnumerator<Int32> ReserveTargetingTypeGroupBasketSecurityBaseValueChangeIds(Int32 howMany)
        {
            var result = this.ReserveIds("TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE_CHANGE", howMany);
            return result;
        }
        public Int32 InsertTargetingTypeGroupBasketSecurityBaseValueChange(TargetingTypeGroupBasketSecurityBaseValueChangeInfo changeInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE_CHANGE] (")
                    .Text("  [ID]")
                    .Text(", [CHANGESET_ID]")
                    .Text(", [TARGETING_TYPE_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [BASE_VALUE_BEFORE]")
                    .Text(", [BASE_VALUE_AFTER]")
                    .Text(", [COMMENT]")
                    .Text(") values (")
                        .Parameter(changeInfo.Id)
                    .Text(", ")
                        .Parameter(changeInfo.ChangesetId)
                    .Text(", ")
                        .Parameter(changeInfo.TargetingTypeGroupId)
                    .Text(", ")
                        .Parameter(changeInfo.BasketId)
                    .Text(", ")
                        .Parameter(changeInfo.SecurityId)
                    .Text(", ")
                        .Parameter(changeInfo.BaseValueBefore)
                    .Text(", ")
                        .Parameter(changeInfo.BaseValueAfter)
                    .Text(", ")
                        .Parameter(changeInfo.Comment)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 InsertTargetingTypeGroupBasketSecurityBaseValue(TargetingTypeGroupBasketSecurityBaseValueInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE] (")
                    .Text("  [TARGETING_TYPE_GROUP_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [BASE_VALUE]")
                    .Text(", [CHANGE_ID]")
                    .Text(") values (")
                        .Parameter(info.TargetingTypeGroupId)
                    .Text(", ")
                        .Parameter(info.BasketId)
                    .Text(", ")
                        .Parameter(info.SecurityId)
                    .Text(", ")
                        .Parameter(info.BaseValue)
                    .Text(", ")
                        .Parameter(info.ChangeId)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 UpdateTargetingTypeGroupBasketSecurityBaseValue(TargetingTypeGroupBasketSecurityBaseValueInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE] set")
                    .Text("  [BASE_VALUE] =").Parameter(info.BaseValue)
                    .Text(", [CHANGE_ID] = ").Parameter(info.ChangeId)
                    .Text(" where [TARGETING_TYPE_GROUP_ID] = ").Parameter(info.TargetingTypeGroupId)
                    .Text(" and   [BASKET_ID] = ").Parameter(info.BasketId)
                    .Text(" and   [SECURITY_ID] = ").Parameter(info.SecurityId)
                    .Execute();
            }
        }
        public Int32 DeleteTargetingTypeGroupBasketSecurityBaseValue(Int32 targetingTypeGroupId, Int32 basketId, String securityId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("delete from [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE]")
                    .Text(" where [TARGETING_TYPE_GROUP_ID] = ").Parameter(targetingTypeGroupId)
                    .Text(" and   [BASKET_ID] = ").Parameter(basketId)
                    .Text(" and   [SECURITY_ID] = ").Parameter(securityId)
                    .Execute();
            }
        }
        public virtual IEnumerable<TargetingTypeGroupBasketSecurityBaseValueInfo> GetTargetingTypeGroupBasketSecurityBaseValues(Int32 targetingTypeGroupId, Int32 basketId)
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeGroupBasketSecurityBaseValueInfo>())
            {
                return builder.Text("select ")
                    .Field("  [TARGETING_TYPE_GROUP_ID]", (info, value) => info.TargetingTypeGroupId = value)
                    .Field(", [BASKET_ID]", (info, value) => info.BasketId = value)
                    .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                    .Field(", [BASE_VALUE]", (TargetingTypeGroupBasketSecurityBaseValueInfo info, Decimal value) => info.BaseValue = value)
                    .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE]")
                    .Text(" where [TARGETING_TYPE_GROUP_ID] = ").Parameter(targetingTypeGroupId)
                    .Text(" and [BASKET_ID] = ").Parameter(basketId)
                    .PullAll();
            }
        }
        public IEnumerable<TargetingTypeGroupBasketSecurityBaseValueInfo> GetTargetingTypeGroupBasketSecurityBaseValues()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeGroupBasketSecurityBaseValueInfo>())
            {
                return builder.Text("select ")
                    .Field("  [TARGETING_TYPE_GROUP_ID]", (info, value) => info.TargetingTypeGroupId = value)
                    .Field(", [BASKET_ID]", (info, value) => info.BasketId = value)
                    .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                    .Field(", [BASE_VALUE]", (TargetingTypeGroupBasketSecurityBaseValueInfo info, Decimal value) => info.BaseValue = value)
                    .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE]")
                    .PullAll();
            }
        }

        // B-P-S-T
        public virtual BasketPortfolioSecurityTargetChangesetInfo GetLatestBasketPortfolioSecurityTargetChangeset()
        {
            using (var builder = this.CreateQueryCommandBuilder<BasketPortfolioSecurityTargetChangesetInfo>())
            {
                return builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [TIMESTAMP]", (info, value) => info.Timestamp = value)
                    .Field(", [CALCULATION_ID]", (info, value) => info.CalculationId = value)
                    .Text(" from [BASKET_PORTFOLIO_SECURITY_TARGET_CHANGESET] order by [TIMESTAMP] desc")
                    .PullFirstOrDefault();
            }
        }
        public IEnumerator<Int32> ReserveBasketPortfolioSecurityTargetChangesetIds(Int32 howMany)
        {
            var result = this.ReserveIds("BASKET_PORTFOLIO_SECURITY_TARGET_CHANGESET", howMany);
            return result;
        }
        public Int32 InsertBasketPortfolioSecurityTargetChangeset(BasketPortfolioSecurityTargetChangesetInfo changesetInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [BASKET_PORTFOLIO_SECURITY_TARGET_CHANGESET] (")
                    .Text("  [ID]")
                    .Text(", [USERNAME]")
                    .Text(", [TIMESTAMP]")
                    .Text(", [CALCULATION_ID]")
                    .Text(") values (")
                        .Parameter(changesetInfo.Id)
                    .Text(", ")
                        .Parameter(changesetInfo.Username)
                    .Text(", ")
                        .Text("GETDATE()")
                    .Text(", ")
                        .Parameter(changesetInfo.CalculationId)
                    .Text(")")
                    .Execute();
            }
        }
        public IEnumerator<Int32> ReserveBasketPortfolioSecurityTargetChangeIds(Int32 howMany)
        {
            var result = this.ReserveIds("BASKET_PORTFOLIO_SECURITY_TARGET_CHANGE", howMany);
            return result;
        }
        public Int32 InsertBasketPortfolioSecurityTargetChange(BasketPortfolioSecurityTargetChangeInfo changeInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [BASKET_PORTFOLIO_SECURITY_TARGET_CHANGE] (")
                    .Text("  [ID]")
                    .Text(", [CHANGESET_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [PORTFOLIO_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [TARGET_BEFORE]")
                    .Text(", [TARGET_AFTER]")
                    .Text(", [COMMENT]")
                    .Text(") values (")
                        .Parameter(changeInfo.Id)
                    .Text(", ")
                        .Parameter(changeInfo.ChangesetId)
                    .Text(", ")
                        .Parameter(changeInfo.BasketId)
                    .Text(", ")
                        .Parameter(changeInfo.PortfolioId)
                    .Text(", ")
                        .Parameter(changeInfo.SecurityId)
                    .Text(", ")
                        .Parameter(changeInfo.TargetBefore)
                    .Text(", ")
                        .Parameter(changeInfo.TargetAfter)
                    .Text(", ")
                        .Parameter(changeInfo.Comment)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 InsertBasketPortfolioSecurityTarget(BasketPortfolioSecurityTargetInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [BASKET_PORTFOLIO_SECURITY_TARGET] (")
                    .Text("  [BASKET_ID]")
                    .Text(", [PORTFOLIO_ID]")
                    .Text(", [SECURITY_ID]")
                    .Text(", [TARGET]")
                    .Text(", [CHANGE_ID]")
                    .Text(") values (")
                        .Parameter(info.BasketId)
                    .Text(", ")
                        .Parameter(info.PortfolioId)
                    .Text(", ")
                        .Parameter(info.SecurityId)
                    .Text(", ")
                        .Parameter(info.Target)
                    .Text(", ")
                        .Parameter(info.ChangeId)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 UpdateBasketPortfolioSecurityTarget(BasketPortfolioSecurityTargetInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [BASKET_PORTFOLIO_SECURITY_TARGET] set")
                    .Text("  [TARGET] = ").Parameter(info.Target)
                    .Text(", [CHANGE_ID] = ").Parameter(info.ChangeId)
                    .Text(" where [BASKET_ID] = ").Parameter(info.BasketId)
                    .Text(" and [PORTFOLIO_ID] = ").Parameter(info.PortfolioId)
                    .Text(" and [SECURITY_ID] = ").Parameter(info.SecurityId)
                    .Execute();
            }
        }
        public Int32 DeleteBasketPortfolioSecurityTarget(Int32 basketId, String portfolioId, String securityId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("delete from [BASKET_PORTFOLIO_SECURITY_TARGET]")
                    .Text(" where [BASKET_ID] = ").Parameter(basketId)
                    .Text(" and [PORTFOLIO_ID] = ").Parameter(portfolioId)
                    .Text(" and [SECURITY_ID] = ").Parameter(securityId)
                    .Execute();
            }
        }
        public virtual IEnumerable<BasketPortfolioSecurityTargetInfo> GetBasketProtfolioSecurityTargets(Int32 basketId, IEnumerable<String> portfolioIds)
        {
#warning Check that there are indexes on the columns that are used in where expressions.

            using (var builder = this.CreateQueryCommandBuilder<BasketPortfolioSecurityTargetInfo>())
            {
                return builder.Text("select ")
                    .Field("  [PORTFOLIO_ID]", (info, value) => info.PortfolioId = value, true)
                    .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                    .Field(", [BASKET_ID]", (info, value) => info.BasketId = value)
                    .Field(", [TARGET]", (BasketPortfolioSecurityTargetInfo info, Decimal value) => info.Target = value)
                    .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [BASKET_PORTFOLIO_SECURITY_TARGET] where [BASKET_ID] = ").Parameter(basketId)
                    .Text(" and [PORTFOLIO_ID] in (").Parameters(portfolioIds).Text(")")
                    .PullAll();
            }
        }
        public virtual IEnumerable<BasketPortfolioSecurityTargetInfo> GetBasketProtfolioSecurityTargets()
        {
            using (var builder = this.CreateQueryCommandBuilder<BasketPortfolioSecurityTargetInfo>())
            {
                return builder.Text("select ")
                    .Field("  [PORTFOLIO_ID]", (info, value) => info.PortfolioId = value, true)
                    .Field(", [SECURITY_ID]", (info, value) => info.SecurityId = value, true)
                    .Field(", [BASKET_ID]", (info, value) => info.BasketId = value)
                    .Field(", [TARGET]", (BasketPortfolioSecurityTargetInfo info, Decimal value) => info.Target = value)
                    .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [BASKET_PORTFOLIO_SECURITY_TARGET]")
                    .PullAll();
            }
        }

        // TT-B-P-T
        public virtual TargetingTypeBasketPortfolioTargetChangesetInfo GetLatestTargetingTypeBasketPortfolioTargetChangeset()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeBasketPortfolioTargetChangesetInfo>())
            {
                return builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [USERNAME]", (info, value) => info.Username = value, true)
                    .Field(", [TIMESTAMP]", (info, value) => info.Timestamp = value)
                    .Field(", [CALCULATION_ID]", (info, value) => info.CalculationId = value)
                    .Text(" from [TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGESET] order by [TIMESTAMP] desc")
                    .PullFirstOrDefault();
            }
        }
        public IEnumerator<Int32> ReserveTargetingTypeBasketPortfolioTargetChangesetIds(Int32 howMany)
        {
            var result = this.ReserveIds("TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGESET", howMany);
            return result;
        }
        public Int32 InsertTargetingTypeBasketPortfolioTargetChangeset(TargetingTypeBasketPortfolioTargetChangesetInfo changesetInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGESET] (")
                    .Text("  [ID]")
                    .Text(", [USERNAME]")
                    .Text(", [TIMESTAMP]")
                    .Text(", [CALCULATION_ID]")
                    .Text(") values (")
                        .Parameter(changesetInfo.Id)
                    .Text(", ")
                        .Parameter(changesetInfo.Username)
                    .Text(", ")
                        .Text("GETDATE()")
                    .Text(", ")
                        .Parameter(changesetInfo.CalculationId)
                    .Text(")")
                    .Execute();
            }
        }
        public IEnumerator<Int32> ReserveTargetingTypeBasketPortfolioTargetChangeIds(Int32 howMany)
        {
            var result = this.ReserveIds("TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGE", howMany);
            return result;
        }
        public Int32 InsertTargetingTypeBasketPortfolioTargetChange(TargetingTypeBasketPortfolioTargetChangeInfo changeInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGE] (")
                    .Text("  [ID]")
                    .Text(", [TARGETING_TYPE_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [PORTFOLIO_ID]")
                    .Text(", [TARGET_BEFORE]")
                    .Text(", [TARGET_AFTER]")
                    .Text(", [COMMENT]")
                    .Text(", [CHANGESET_ID]")
                    .Text(") values (")
                        .Parameter(changeInfo.Id)
                    .Text(", ")
                        .Parameter(changeInfo.TargetingTypeId)
                    .Text(", ")
                        .Parameter(changeInfo.BasketId)
                    .Text(", ")
                        .Parameter(changeInfo.PortfolioId)
                    .Text(", ")
                        .Parameter(changeInfo.Target_Before)
                    .Text(", ")
                        .Parameter(changeInfo.Target_After)
                    .Text(", ")
                        .Parameter(changeInfo.Comment)
                    .Text(", ")
                        .Parameter(changeInfo.ChangesetId)
                    .Text(")")
                    .Execute();
            }
        }
        public Int32 UpdateTargetingTypeBasketPortfolioTarget(TargetingTypeBasketPortfolioTargetInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [TARGETING_TYPE_BASKET_PORTFOLIO_TARGET] set")
                    .Text("  [TARGET] = ").Parameter(info.Target)
                    .Text(", [CHANGE_ID] = ").Parameter(info.ChangeId)
                    .Text(" where [TARGETING_TYPE_ID] = ").Parameter(info.TargetingTypeId)
                    .Text(" and   [BASKET_ID] = ").Parameter(info.BasketId)
                    .Text(" and   [PORTFOLIO_ID] = ").Parameter(info.PortfolioId)
                    .Execute();
            }
        }
        public Int32 InsertTargetingTypeBasketPortfolioTarget(TargetingTypeBasketPortfolioTargetInfo info)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [TARGETING_TYPE_BASKET_PORTFOLIO_TARGET] (")
                    .Text("  [TARGETING_TYPE_ID]")
                    .Text(", [BASKET_ID]")
                    .Text(", [PORTFOLIO_ID]")
                    .Text(", [TARGET]")
                    .Text(", [CHANGE_ID]")
                    .Text(") values (")
                        .Parameter(info.TargetingTypeId)
                    .Text(", ")
                        .Parameter(info.BasketId)
                    .Text(", ")
                        .Parameter(info.PortfolioId)
                    .Text(", ")
                        .Parameter(info.Target)
                    .Text(", ")
                        .Parameter(info.ChangeId)
                    .Text(")")
                    .Execute();
            }

        }
        public Int32 DeleteTargetingTypeBasketPortfolioTarget(Int32 targetingTypeId, Int32 basketId, String portfolioId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("delete from [TARGETING_TYPE_BASKET_PORTFOLIO_TARGET]")
                    .Text(" where [TARGETING_TYPE_ID] = ").Parameter(targetingTypeId)
                    .Text(" and   [BASKET_ID] = ").Parameter(basketId)
                    .Text(" and   [PORTFOLIO_ID] = ").Parameter(portfolioId)
                    .Execute();
            }
        }
        public IEnumerable<TargetingTypeBasketPortfolioTargetInfo> GetTargetingTypeBasketPortfolioTarget(Int32 targetingTypeId, String portfolioId)
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingTypeBasketPortfolioTargetInfo>())
            {
                return builder.Text("select ")
                    .Field("  [TARGETING_TYPE_ID]", (info, value) => info.TargetingTypeId = value)
                    .Field(", [BASKET_ID]", (info, value) => info.BasketId = value)
                    .Field(", [PORTFOLIO_ID]", (info, value) => info.PortfolioId = value, true)
                    .Field(", [TARGET]", (TargetingTypeBasketPortfolioTargetInfo info, Decimal value) => info.Target = value)
                    .Field(", [CHANGE_ID]", (info, value) => info.ChangeId = value)
                    .Text(" from [" + TableNames.TARGETING_TYPE_BASKET_PORTFOLIO_TARGET + "]")
                    .Text(" where [TARGETING_TYPE_ID] = ").Parameter(targetingTypeId)
                    .Text(" and   [PORTFOLIO_ID] = ").Parameter(portfolioId)
                    .PullAll();
            }
        }




        // targeting calculations
        public IEnumerator<Int32> ReserveTargetingComputationIds(Int32 howMany)
        {
            var result = this.ReserveIds(TableNames.TARGETING_CALCULATION, howMany);
            return result;
        }
        public Int32 InsertTargetingComputation(TargetingCalculationInfo computationInfo)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("insert into [" + TableNames.TARGETING_CALCULATION + "] (")
                    .Text("  [ID]")
                    .Text(", [STATUS_CODE]")
                    .Text(", [QUEUED_ON]")
                    .Text(", [STARTED_ON]")
                    .Text(", [FINISHED_ON]")
                    .Text(", [LOG]")
                    .Text(") values (")
                        .Parameter(computationInfo.Id)
                    .Text(", ")
                        .Parameter(computationInfo.StatusCode)
                    .Text(", GETDATE()")
                    .Text(", NULL")
                    .Text(", NULL")
                    .Text(", ")
                        .Parameter(computationInfo.Log)
                    .Text(")")
                    .Execute();
            }
        }
        public IEnumerable<TargetingCalculationInfo> Get2MostRecentTargetingCalculations()
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingCalculationInfo>())
            {
                return builder.Text("select top 2")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [STATUS_CODE]", (info, value) => info.StatusCode = value)
                    .Field(", [QUEUED_ON]", (info, value) => info.QueuedOn = value)
                    .Field(", [STARTED_ON]", (TargetingCalculationInfo info, DateTime? value) => info.StartedOn = value)
                    .Field(", [FINISHED_ON]", (TargetingCalculationInfo info, DateTime? value) => info.FinishedOn = value)
                    .Field(", [LOG]", (info, value) => info.Log = value, false)
                    .Text(" from [" + TableNames.TARGETING_CALCULATION + "] order by [QUEUED_ON] desc")
                    .PullAll();
            }
        }

        public Int32 StartTargetingCalculation(Int32 calculationId)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [" + TableNames.TARGETING_CALCULATION + "] set")
                    .Text("  [STATUS_CODE] = 1")
                    .Text(",  [STARTED_ON] = GETDATE()")
                    .Text(" where  [ID]= ").Parameter(calculationId)
                    .Execute();
            }
        }

        public TargetingCalculationInfo GetTargetingCalculation(Int32 calculationId)
        {
            using (var builder = this.CreateQueryCommandBuilder<TargetingCalculationInfo>())
            {
                var found = builder.Text("select top 1")
                    .Field("  [ID]", (info, value) => info.Id = value)
                    .Field(", [STATUS_CODE]", (info, value) => info.StatusCode = value)
                    .Field(", [QUEUED_ON]", (info, value) => info.QueuedOn = value)
                    .Field(", [STARTED_ON]", (TargetingCalculationInfo info, DateTime? value) => info.StartedOn = value)
                    .Field(", [FINISHED_ON]", (TargetingCalculationInfo info, DateTime? value) => info.FinishedOn = value)
                    .Field(", [LOG]", (info, value) => info.Log = value, false)
                    .Text(" from [" + TableNames.TARGETING_CALCULATION + "] where [ID] = ").Parameter(calculationId)
                    .PullFirstOrDefault();

                if (found == null) throw new ApplicationException("There is no calculation with the \"" + calculationId + "\" ID.");
                if (found.StatusCode > 0 || found.StartedOn.HasValue) throw new ApplicationException("Unable to start a calculation because the existing record (ID: " + calculationId + ") indicates that it was already started on " + found.StartedOn/* .Value <--- can faile like that if null */ + ".");
                return found;
            }
        }

        public Int32 FinishTargetingCalculationUnsafe(Int32 calculationId, Int32 status, String log)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder.Text("update [" + TableNames.TARGETING_CALCULATION + "] set")
                    .Text("  [STATUS_CODE] = ").Parameter(status)
                    .Text(", [FINISHED_ON] = GETDATE()")
                    .Text(", [LOG] = ").Parameter(log)
                    .Text(" where  [ID]= ").Parameter(calculationId)
                    .Execute();
            }
        }

        public Int32 DeleteAllBgaPortfolioSecurityTargets()
        {
            using (var builder = this.CreateCommandBuilder())
            {
                return builder
                    .Text("delete from [" + TableNames.BGA_PORTFOLIO_SECURITY_FACTOR + "]")
                    .Execute();
            }
        }

        public void InsertBgaPortfolioSecurityTargets(IEnumerable<BgaPortfolioSecurityTargetInfo> targetInfos)
        {

            var table = this.CreateDataTableForInsertingBgaPortfolioSecurityTargets(targetInfos);

            var pusher = this.TransactionOpt != null ? new SqlBulkCopy(this.Connection, SqlBulkCopyOptions.CheckConstraints, this.TransactionOpt) : new SqlBulkCopy(this.Connection);
            pusher.BulkCopyTimeout = 0;	// no timeout
            pusher.DestinationTableName = TableNames.BGA_PORTFOLIO_SECURITY_TARGET;
            pusher.WriteToServer(table);
        }

        public Int32 DeleteBgaPortfolioSecurityTargets(IEnumerable<BgaPortfolioSecurityTargetInfo> targetInfos)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                var b = builder.Text("delete from [" + TableNames.BGA_PORTFOLIO_SECURITY_TARGET + "] where 1=0 ");
                foreach (var i in targetInfos)
                    b = b.Text(" or ([BGA_PORTFOLIO_ID]='" + i.BroadGlobalActivePortfolioId + "' and [SECURITY_ID]='" + i.SecurityId + "')");
                return b.Execute();
            }

        }

        protected DataTable CreateDataTableForInsertingBgaPortfolioSecurityTargets(IEnumerable<BgaPortfolioSecurityTargetInfo> items)
        {
            var result = new DataTable(TableNames.BGA_PORTFOLIO_SECURITY_TARGET);
            var portfolioIdColumn = result.Columns.Add("BGA_PORTFOLIO_ID", typeof(String));
            var securityIdColumn = result.Columns.Add("SECURITY_ID", typeof(String));
            var targetColumn = result.Columns.Add("TARGET", typeof(Decimal));

            foreach (var item in items)
            {
                var row = result.NewRow();
                row[portfolioIdColumn] = item.BroadGlobalActivePortfolioId;
                row[securityIdColumn] = item.SecurityId;
                row[targetColumn] = item.Target;
                result.Rows.Add(row);
            }

            return result;
        }

        public IEnumerable<CalculationWithChangesets> GetAllCalculationWithChangesets(Int32 howMany)
        {
            using (var builder = this.CreateQueryCommandBuilder<CalculationWithChangesets>())
            {
                return builder.Text("select top " + howMany)
                    .Field("  t0.[ID]", (info, value) => info.CalculationInfo.Id = value)
                    .Field(", t0.[STATUS_CODE]", (info, value) => info.CalculationInfo.StatusCode = value)
                    .Field(", t0.[QUEUED_ON]", (info, value) => info.CalculationInfo.QueuedOn = value)
                    .Field(", t0.[STARTED_ON]", (CalculationWithChangesets info, DateTime? value) => info.CalculationInfo.StartedOn = value)
                    .Field(", t0.[FINISHED_ON]", (CalculationWithChangesets info, DateTime? value) => info.CalculationInfo.FinishedOn = value)
                    .Field(", t0.[LOG]", (info, value) => info.CalculationInfo.Log = value, false)

                    // B-P-S-T
                    .Outer(", t1.[ID]",
                        info => info.BpstChangesetOpt = info.BpstChangesetOpt ?? new BasketPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.Id = value)
                    .Outer(", t1.[USERNAME]",
                        info => info.BpstChangesetOpt = info.BpstChangesetOpt ?? new BasketPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.Username = value)
                    .Outer(", t1.[TIMESTAMP]",
                        info => info.BpstChangesetOpt = info.BpstChangesetOpt ?? new BasketPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.Timestamp = value)
                    .Outer(", t1.[CALCULATION_ID]",
                        info => info.BpstChangesetOpt = info.BpstChangesetOpt ?? new BasketPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.CalculationId = value)

                    // P-S-F
                    .Outer(", t2.[ID]",
                        info => info.PsfChangesetOpt = info.PsfChangesetOpt ?? new BgaPortfolioSecurityFactorChangesetInfo(),
                        (info, value) => info.Id = value)
                    .Outer(", t2.[USERNAME]",
                        info => info.PsfChangesetOpt = info.PsfChangesetOpt ?? new BgaPortfolioSecurityFactorChangesetInfo(),
                        (info, value) => info.Username = value)
                    .Outer(", t2.[TIMESTAMP]",
                        info => info.PsfChangesetOpt = info.PsfChangesetOpt ?? new BgaPortfolioSecurityFactorChangesetInfo(),
                        (info, value) => info.Timestamp = value)
                    .Outer(", t2.[CALCULATION_ID]",
                        info => info.PsfChangesetOpt = info.PsfChangesetOpt ?? new BgaPortfolioSecurityFactorChangesetInfo(),
                        (info, value) => info.CalculationId = value)

                    // P-S-T
                    .Outer(", t3.[ID]",
                        info => info.PstChangesetOpt = info.PstChangesetOpt ?? new BuPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.Id = value)
                    .Outer(", t3.[USERNAME]",
                        info => info.PstChangesetOpt = info.PstChangesetOpt ?? new BuPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.Username = value)
                    .Outer(", t3.[TIMESTAMP]",
                        info => info.PstChangesetOpt = info.PstChangesetOpt ?? new BuPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.Timestamp = value)
                    .Outer(", t3.[CALCULATION_ID]",
                        info => info.PstChangesetOpt = info.PstChangesetOpt ?? new BuPortfolioSecurityTargetChangesetInfo(),
                        (info, value) => info.CalculationId = value)

                    // TT-B-Bv
                    .Outer(", t4.[ID]",
                        info => info.TtbbvChangesetOpt = info.TtbbvChangesetOpt ?? new TargetingTypeBasketBaseValueChangesetInfo(),
                        (info, value) => info.Id = value)
                    .Outer(", t4.[USERNAME]",
                        info => info.TtbbvChangesetOpt = info.TtbbvChangesetOpt ?? new TargetingTypeBasketBaseValueChangesetInfo(),
                        (info, value) => info.Username = value)
                    .Outer(", t4.[TIMESTAMP]",
                        info => info.TtbbvChangesetOpt = info.TtbbvChangesetOpt ?? new TargetingTypeBasketBaseValueChangesetInfo(),
                        (info, value) => info.Timestamp = value)
                    .Outer(", t4.[CALCULATION_ID]",
                        info => info.TtbbvChangesetOpt = info.TtbbvChangesetOpt ?? new TargetingTypeBasketBaseValueChangesetInfo(),
                        (info, value) => info.CalculationId = value)

                    // TT-B-P-T
                    .Outer(", t5.[ID]",
                        info => info.TtbptChangesetOpt = info.TtbptChangesetOpt ?? new TargetingTypeBasketPortfolioTargetChangesetInfo(),
                        (info, value) => info.Id = value)
                    .Outer(", t5.[USERNAME]",
                        info => info.TtbptChangesetOpt = info.TtbptChangesetOpt ?? new TargetingTypeBasketPortfolioTargetChangesetInfo(),
                        (info, value) => info.Username = value)
                    .Outer(", t5.[TIMESTAMP]",
                        info => info.TtbptChangesetOpt = info.TtbptChangesetOpt ?? new TargetingTypeBasketPortfolioTargetChangesetInfo(),
                        (info, value) => info.Timestamp = value)
                    .Outer(", t5.[CALCULATION_ID]",
                        info => info.TtbptChangesetOpt = info.TtbptChangesetOpt ?? new TargetingTypeBasketPortfolioTargetChangesetInfo(),
                        (info, value) => info.CalculationId = value)

                    // TTG-B-S-Bv 
                    .Outer(", t5.[ID]",
                        info => info.TtgbsbvChangesetOpt = info.TtgbsbvChangesetOpt ?? new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(),
                        (info, value) => info.Id = value)
                    .Outer(", t5.[USERNAME]",
                        info => info.TtgbsbvChangesetOpt = info.TtgbsbvChangesetOpt ?? new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(),
                        (info, value) => info.Username = value)
                    .Outer(", t5.[TIMESTAMP]",
                        info => info.TtgbsbvChangesetOpt = info.TtgbsbvChangesetOpt ?? new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(),
                        (info, value) => info.Timestamp = value)
                    .Outer(", t5.[CALCULATION_ID]",
                        info => info.TtgbsbvChangesetOpt = info.TtgbsbvChangesetOpt ?? new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(),
                        (info, value) => info.CalculationId = value)

                    .Text(" from [" + TableNames.TARGETING_CALCULATION + "] t0")
                    .Text(" left outer join [" + TableNames.BASKET_PORTFOLIO_SECURITY_TARGET_CHANGESET + "] t1 on t0.[ID] = t1.[CALCULATION_ID]")
                    .Text(" left outer join [" + TableNames.BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET + "] t2 on t0.[ID] = t2.[CALCULATION_ID]")
                    .Text(" left outer join [" + TableNames.BU_PORTFOLIO_SECURITY_TARGET_CHANGESET + "] t3 on t0.[ID] = t3.[CALCULATION_ID]")
                    .Text(" left outer join [" + TableNames.TARGETING_TYPE_BASKET_BASE_VALUE_CHANGESET + "] t4 on t0.[ID] = t4.[CALCULATION_ID]")
                    .Text(" left outer join [" + TableNames.TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGESET + "] t5 on t0.[ID] = t5.[CALCULATION_ID]")
                    .Text(" left outer join [" + TableNames.TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE_CHANGESET + "] t6 on t0.[ID] = t6.[CALCULATION_ID]")
                    .Text(" order by t0.[QUEUED_ON] desc")
                    .PullAll();
            }
        }


        public Int32 CreateBasketCountry(Int32 id, String isoCode)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                var b = builder.Text("insert into  [" + TableNames.COUNTRY_BASKET + "](ID, ISO_COUNTRY_CODE) values(")
                    .Parameter(id)
                    .Text(",")
                    .Parameter(isoCode)
                    .Text(")");
                return b.Execute();
            }

        }

        public Int32 CreateBasketAsCountry(Int32 id)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                var b = builder.Text("insert into  [" + TableNames.BASKET + "](ID, [TYPE]) values(")
                    .Parameter(id)
                    .Text(",'country')");
                return b.Execute();
            }

        }


        public int UpdateTaxonomy(string taxonomy, int id)
        {
            using (var builder = this.CreateCommandBuilder())
            {
                var b = builder.Text("update  [TAXONOMY] set DEFINITION=")
                    .Parameter(taxonomy)
                    .Text(" where id=")
                    .Parameter(id);
                return b.Execute();
            }
        }
    }
}
