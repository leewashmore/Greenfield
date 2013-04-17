using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AIMS.Composites.DAL;
using AIMS.Composites.Service.DimensionWebService;

namespace AIMS.Composites.Service
{
    public class CompositesOperations : ICompositesOperations
    {
        #region PropertyDeclaration

        //private const int InsertRecordsBatchSize = 1000;
        private readonly IDumper _dumper;
        private Entities _dimensionEntity;

        public CompositesOperations(IDumper dumper)
        {
            _dumper = dumper;
        }

        public Entities DimensionEntity
        {
            get
            {
                return _dimensionEntity ??
                       (_dimensionEntity =
                        new Entities(new Uri(ConfigurationSettings.AppSettings["DimensionWebService"])));
            }
        }

        #endregion

        #region CompositesServices

        public List<GetComposites_Result> GetComposites()
        {
            try
            {
                return new AIMS_MainEntities().GetComposites().ToList();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        public List<GetCompositePortfolios_Result> GetCompositePortfolios(string compositeId)
        {
            try
            {
                return new AIMS_MainEntities().GetCompositePortfolios(compositeId).ToList();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }

        public void PopulateCompositeLTHoldings()
        {
            try
            {
                var stopwatchPopulateCompositeLTHoldings = new Stopwatch();
                stopwatchPopulateCompositeLTHoldings.Start();

                _dumper.WriteLine("Started PopulateCompositeLTHoldings operation.");
                _dumper.Write("GetComposites ... ");
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                List<GetComposites_Result> composites = GetComposites();
                _dumper.WriteLine(string.Format("{0} Composites returned.", composites.Count()), stopwatch);

                int COMPOSITE_LTHOLDINGS_ID = 1;
                int indedx = 1;
                foreach (GetComposites_Result composite in composites)
                {
                    // Step 1   Retrieve the list of portfolios in the composite that are active (using the new COMPOSITE_MATRIX table).
                    _dumper.WriteLine("");
                    _dumper.WriteLine(string.Format("{0}. WORKING ON ON CompositeId:'{1}'", (indedx++),
                                                    composite.COMPOSITE_ID));
                    _dumper.Indent();
                    _dumper.Write("GetCompositePortfolios ... ");
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    List<GetCompositePortfolios_Result> portfolios = GetCompositePortfolios(composite.COMPOSITE_ID);
                    _dumper.WriteLine(string.Format("{0} CompositePortfolios returned.", portfolios.Count()), stopwatch);

                    // Create a list of portfolio.PORTFOLIO_IDS
                    int portfolioIndex = 1;
                    _dumper.Write("Composite Portfolios: ");
                    var CompositePortfoliosIds = new List<string>();
                    foreach (GetCompositePortfolios_Result portfolio in portfolios)
                    {
                        CompositePortfoliosIds.Add(portfolio.PORTFOLIO);
                        _dumper.Write(string.Format("{0}.{1} ", portfolioIndex++, portfolio.PORTFOLIO));
                    }
                    _dumper.WriteLine("");

                    // Step 2   For portfolios returned in Step 1, retrieve all records from GF_PORTFOLIO_LTHOLDINGS.
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    _dumper.Write(
                        "For portfolios returned, retrieve all records from GF_PORTFOLIO_LTHOLDINGS: count= ");
                    List<GF_PORTFOLIO_LTHOLDINGS> gfPortfolioLtholdings =
                        DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.ToList().Where(
                            record => CompositePortfoliosIds.Contains(record.PORTFOLIO_ID)).ToList();
                    _dumper.WriteLine(gfPortfolioLtholdings.Count().ToString(CultureInfo.InvariantCulture),
                                      stopwatch);

                    // Step 3   Delete records when appropriate based on Look_Thru setting in COMPOSITE_MATRIX view.  When Look_Thru <> 'Y', delete records returned from view where PORTFOLIO_ID <> A_PFCHOLDINGS_PORLT
                    var gfPortfolioLthondings_New = new List<GF_PORTFOLIO_LTHOLDINGS>();
                    if (composite.LOOK_THRU != true)
                        gfPortfolioLthondings_New.AddRange(
                            gfPortfolioLtholdings.Where(
                                Ltholdings =>
                                Ltholdings.PORTFOLIO_ID == Ltholdings.A_PFCHOLDINGS_PORLT));
                    else
                        gfPortfolioLthondings_New = gfPortfolioLtholdings;

                    _dumper.WriteLine(
                        string.Format(
                            "Removed where COMPOSITE_MATRIX.Look_Thru <> 'Y' and PORTFOLIO_ID <> A_PFCHOLDINGS_PORLT: new count= {0}",
                            gfPortfolioLthondings_New.Count().ToString(CultureInfo.InvariantCulture)));

                    // Step 4   Aggregate remaining records together by the ASEC_SEC_SHORT_NAME, and PORTFOLIO_DATE.
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var aggregateSecurities =
                        gfPortfolioLthondings_New.GroupBy(x => new {x.PORTFOLIO_DATE, x.ASEC_SEC_SHORT_NAME},
                                                          (key, group) =>
                                                          new
                                                              {
                                                                  @group.First().GF_ID,
                                                                  key.PORTFOLIO_DATE,
                                                                  @group.First().PORPATH,
                                                                  @group.First().PORTFOLIO_THEME_SUBGROUP_CODE,
                                                                  @group.First().PORTFOLIO_CURRENCY,
                                                                  @group.First().ISSUER_ID,
                                                                  key.ASEC_SEC_SHORT_NAME,
                                                                  @group.First().ISSUE_NAME,
                                                                  @group.First().TICKER,
                                                                  @group.First().SECURITYTHEMECODE,
                                                                  @group.First().A_SEC_INSTR_TYPE,
                                                                  @group.First().SECURITY_TYPE,
                                                                  BALANCE_NOMINAL =
                                                              @group.Sum(x => x.BALANCE_NOMINAL),
                                                                  @group.First().DIRTY_PRICE,
                                                                  @group.First().TRADING_CURRENCY,
                                                                  DIRTY_VALUE_PC = @group.Sum(x => x.DIRTY_VALUE_PC),
                                                                  BENCHMARK_WEIGHT =
                                                              @group.Sum(x => x.BENCHMARK_WEIGHT),
                                                                  ASH_EMM_MODEL_WEIGHT =
                                                              @group.Sum(x => x.ASH_EMM_MODEL_WEIGHT),
                                                                  @group.First().MARKET_CAP_IN_USD,
                                                                  @group.First().ASHEMM_PROP_REGION_CODE,
                                                                  @group.First().ASHEMM_PROP_REGION_NAME,
                                                                  @group.First().ISO_COUNTRY_CODE,
                                                                  @group.First().COUNTRYNAME,
                                                                  @group.First().GICS_SECTOR,
                                                                  @group.First().GICS_SECTOR_NAME,
                                                                  @group.First().GICS_INDUSTRY,
                                                                  @group.First().GICS_INDUSTRY_NAME,
                                                                  @group.First().GICS_SUB_INDUSTRY,
                                                                  @group.First().GICS_SUB_INDUSTRY_NAME,
                                                                  @group.First().LOOK_THRU_FUND
                                                              }).ToList();

                    _dumper.WriteLine(
                        string.Format("GF_PORTFOLIO_LTHOLDINGS: aggregated records = {0}", aggregateSecurities.Count()),
                        stopwatch);

                    List<GF_COMPOSITE_LTHOLDINGS> compositeLtholdings =
                        aggregateSecurities.Select(aggregateSecurity => new GF_COMPOSITE_LTHOLDINGS
                            {
                                GF_ID = COMPOSITE_LTHOLDINGS_ID++,
                                PORTFOLIO_DATE = aggregateSecurity.PORTFOLIO_DATE,
                                PORTFOLIO_ID = composite.COMPOSITE_ID,
                                A_PFCHOLDINGS_PORLT = composite.COMPOSITE_ID,
                                PORPATH = aggregateSecurity.PORPATH,
                                PORTFOLIO_THEME_SUBGROUP_CODE = aggregateSecurity.PORTFOLIO_THEME_SUBGROUP_CODE,
                                PORTFOLIO_CURRENCY = aggregateSecurity.PORTFOLIO_CURRENCY,
                                BENCHMARK_ID = composite.BENCHMARK_ID,
                                ISSUER_ID = aggregateSecurity.ISSUER_ID,
                                ASEC_SEC_SHORT_NAME = aggregateSecurity.ASEC_SEC_SHORT_NAME,
                                ISSUE_NAME = aggregateSecurity.ISSUE_NAME,
                                TICKER = aggregateSecurity.TICKER,
                                SECURITYTHEMECODE = aggregateSecurity.SECURITYTHEMECODE,
                                A_SEC_INSTR_TYPE = aggregateSecurity.A_SEC_INSTR_TYPE,
                                SECURITY_TYPE = aggregateSecurity.SECURITY_TYPE,
                                BALANCE_NOMINAL = aggregateSecurity.BALANCE_NOMINAL,
                                DIRTY_PRICE = aggregateSecurity.DIRTY_PRICE,
                                TRADING_CURRENCY = aggregateSecurity.TRADING_CURRENCY,
                                DIRTY_VALUE_PC = aggregateSecurity.DIRTY_VALUE_PC,
                                BENCHMARK_WEIGHT = aggregateSecurity.BENCHMARK_WEIGHT,
                                ASH_EMM_MODEL_WEIGHT = aggregateSecurity.ASH_EMM_MODEL_WEIGHT,
                                MARKET_CAP_IN_USD = aggregateSecurity.MARKET_CAP_IN_USD,
                                ASHEMM_PROP_REGION_CODE = aggregateSecurity.ASHEMM_PROP_REGION_CODE,
                                ASHEMM_PROP_REGION_NAME = aggregateSecurity.ASHEMM_PROP_REGION_NAME,
                                ISO_COUNTRY_CODE = aggregateSecurity.ISO_COUNTRY_CODE,
                                COUNTRYNAME = aggregateSecurity.COUNTRYNAME,
                                GICS_SECTOR = aggregateSecurity.GICS_SECTOR,
                                GICS_SECTOR_NAME = aggregateSecurity.GICS_SECTOR_NAME,
                                GICS_INDUSTRY = aggregateSecurity.GICS_INDUSTRY,
                                GICS_INDUSTRY_NAME = aggregateSecurity.GICS_INDUSTRY_NAME,
                                GICS_SUB_INDUSTRY = aggregateSecurity.GICS_SUB_INDUSTRY,
                                GICS_SUB_INDUSTRY_NAME = aggregateSecurity.GICS_SUB_INDUSTRY_NAME,
                                LOOK_THRU_FUND = aggregateSecurity.LOOK_THRU_FUND
                            }).ToList();

                    new CompositeLtHondings().Save(compositeLtholdings, _dumper);
                    //SaveCompositeLtHondings(compositeLtholdings);

                    _dumper.Unindent();
                }

                _dumper.WriteLine(string.Format("PopulateCompositeLTHoldings sucessfully completed."),
                                  stopwatchPopulateCompositeLTHoldings);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        /*
        private void SaveCompositeLtHondings(List<GF_COMPOSITE_LTHOLDINGS> compositeLtholdings)
        {
            _dumper.Write("saving ");
            // question loop and insert of do in one shot
            //using (TransactionScope scope = new TransactionScope())
            {
                AIMS_MainEntities context = null;
                try
                {
                    context = new AIMS_MainEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;

                    int count = 0;
                    foreach (GF_COMPOSITE_LTHOLDINGS entityToInsert in compositeLtholdings)
                    {
                        ++count;
                        context = AddToContext(context, entityToInsert, count, InsertRecordsBatchSize,
                                               true);
                    }

                    context.SaveChanges();
                    _dumper.Write(string.Format("{0} DONE.", compositeLtholdings.Count));
                    _dumper.WriteLine("");
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
                finally
                {
                    if (context != null)
                        context.Dispose();
                }

                //scope.Complete();
            }
        }
         * */

        private static byte[] GetBytes(string str)
        {
            if (str != null)
            {
                var bytes = new byte[str.Length*sizeof (char)];
                Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
                return bytes;
            }

            return null;
        }

        private static string GetString(byte[] bytes)
        {
            var chars = new char[bytes.Length/sizeof (char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private void LogException(Exception ex)
        {
            _dumper.WriteLine(
                string.Format("Error:{0}  InnerException 1.{1}  InnerException 2.{2}", ex.Message,
                              ex.InnerException == null ? "" : ex.InnerException.Message,
                              ex.InnerException == null || ex.InnerException.InnerException == null
                                  ? ""
                                  : ex.InnerException.InnerException.Message), true);
        }

        private AIMS_MainEntities AddToContext(AIMS_MainEntities context, GF_COMPOSITE_LTHOLDINGS entity, int count,
                                               int commitCount, bool recreateContext)
        {
            context.Set<GF_COMPOSITE_LTHOLDINGS>().Add(entity);

            if (count%commitCount == 0)
            {
                _dumper.Write(string.Format("{0},", count.ToString(CultureInfo.InvariantCulture)));
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new AIMS_MainEntities();
                    context.Configuration.AutoDetectChangesEnabled = false;
                }
            }

            return context;
        }

        #endregion

        public void TruncateCompositeLTHoldings()
        {
            try
            {
                _dumper.Write("TRUNCATE TABLE [GF_COMPOSITE_LTHOLDINGS] ... ");

                var db = new AIMS_MainEntities();
                ObjectContext objCtx = ((IObjectContextAdapter) db).ObjectContext;
                objCtx.ExecuteStoreCommand("TRUNCATE TABLE [GF_COMPOSITE_LTHOLDINGS]");

                _dumper.WriteLine(" truncated.", true);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }
    }
}