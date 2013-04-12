using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using AIMS.Composites.DAL;
using AIMS.Composites.Service.DimensionWebService;

namespace AIMS.Composites.Service
{
    public class CompositesOperations : ICompositesOperations
    {
        #region PropertyDeclaration

        private Entities dimensionEntity;

        public Entities DimensionEntity
        {
            get
            {
                return dimensionEntity ??
                       (dimensionEntity =
                        new Entities(new Uri(ConfigurationSettings.AppSettings["DimensionWebService"])));
            }
        }

        #endregion

        #region FaultResourceManager

        /*
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }
         */

        #endregion

        #region CompositesServices

        public List<GetComposites_Result> GetComposites()
        {
            try
            {
                return new AIMS_MainEntities().GetComposites().ToList();
            }
            catch (Exception)
            {
                //ExceptionTrace.LogException(ex);
                //string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                //throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));

                return null;
            }
        }

        public List<GetCompositePortfolios_Result> GetCompositePortfolios(string compositeId)
        {
            try
            {
                return new AIMS_MainEntities().GetCompositePortfolios(compositeId).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void PopulateCompositeLTHoldings()
        {
            try
            {
                List<GetComposites_Result> composites = GetComposites();
                foreach (GetComposites_Result composite in composites)
                {
                    // Step 1   Retrieve the list of portfolios in the composite that are active (using the new COMPOSITE_MATRIX table).
                    List<GetCompositePortfolios_Result> portfolios = GetCompositePortfolios(composite.COMPOSITE_ID);

                    // Step 2   For portfolios returned in Step 1, retrieve all records from GF_PORTFOLIO_LTHOLDINGS.
                    foreach (GetCompositePortfolios_Result portfolio in portfolios)
                    {
                        // Step 4   Aggregate remaining records together by the ASEC_SEC_SHORT_NAME, and PORTFOLIO_DATE.
                        var aggregateSecurities =
                            DimensionEntity.GF_PORTFOLIO_LTHOLDINGS.Where(
                                record => record.PORTFOLIO_ID == portfolio.PORTFOLIO)
                                           .ToList()
                                           .GroupBy(x => new {x.PORTFOLIO_DATE, x.ASEC_SEC_SHORT_NAME},
                                                    (key, group) =>
                                                    new
                                                        {
                                                            group.First().GF_ID,
                                                            key.PORTFOLIO_DATE,
                                                            group.First().PORPATH,
                                                            group.First().PORTFOLIO_THEME_SUBGROUP_CODE,
                                                            group.First().PORTFOLIO_CURRENCY,
                                                            group.First().ISSUER_ID,
                                                            key.ASEC_SEC_SHORT_NAME,
                                                            group.First().ISSUE_NAME,
                                                            group.First().TICKER,
                                                            group.First().SECURITYTHEMECODE,
                                                            group.First().A_SEC_INSTR_TYPE,
                                                            group.First().SECURITY_TYPE,
                                                            BALANCE_NOMINAL = group.Sum(x => x.BALANCE_NOMINAL),
                                                            group.First().DIRTY_PRICE,
                                                            group.First().TRADING_CURRENCY,
                                                            DIRTY_VALUE_PC = group.Sum(x => x.DIRTY_VALUE_PC),
                                                            BENCHMARK_WEIGHT = group.Sum(x => x.BENCHMARK_WEIGHT),
                                                            ASH_EMM_MODEL_WEIGHT =
                                                        group.Sum(x => x.ASH_EMM_MODEL_WEIGHT),
                                                            group.First().MARKET_CAP_IN_USD,
                                                            group.First().ASHEMM_PROP_REGION_CODE,
                                                            group.First().ASHEMM_PROP_REGION_NAME,
                                                            group.First().ISO_COUNTRY_CODE,
                                                            group.First().COUNTRYNAME,
                                                            group.First().GICS_SECTOR,
                                                            group.First().GICS_SECTOR_NAME,
                                                            group.First().GICS_INDUSTRY,
                                                            group.First().GICS_INDUSTRY_NAME,
                                                            group.First().GICS_SUB_INDUSTRY,
                                                            group.First().GICS_SUB_INDUSTRY_NAME,
                                                            group.First().LOOK_THRU_FUND
                                                        }).ToList();

                        var compositeLtholdings = new List<GF_COMPOSITE_LTHOLDINGS>();

                        foreach (var aggregateSecurity in aggregateSecurities)
                        {
                            var compositeLth = new GF_COMPOSITE_LTHOLDINGS
                                {
                                    GF_ID = aggregateSecurity.GF_ID,
                                    PORTFOLIO_DATE = aggregateSecurity.PORTFOLIO_DATE,
                                    PORTFOLIO_ID = composite.COMPOSITE_ID,
                                    A_PFCHOLDINGS_PORLT = composite.COMPOSITE_ID,
                                    PORPATH = Encoding.UTF8.GetBytes(aggregateSecurity.PORPATH),
                                    PORTFOLIO_THEME_SUBGROUP_CODE = aggregateSecurity.PORTFOLIO_THEME_SUBGROUP_CODE,
                                    PORTFOLIO_CURRENCY = Encoding.UTF8.GetBytes(aggregateSecurity.PORTFOLIO_CURRENCY),
                                    ISSUER_ID = aggregateSecurity.ISSUER_ID,
                                    ASEC_SEC_SHORT_NAME = aggregateSecurity.ASEC_SEC_SHORT_NAME,
                                    ISSUE_NAME = aggregateSecurity.ISSUE_NAME,
                                    TICKER = aggregateSecurity.TICKER,
                                    SECURITYTHEMECODE = aggregateSecurity.SECURITYTHEMECODE,
                                    A_SEC_INSTR_TYPE = Encoding.UTF8.GetBytes(aggregateSecurity.A_SEC_INSTR_TYPE),
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
                                };

                            compositeLtholdings.Add(compositeLth);
                        }

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
                                    context = AddToContext(context, entityToInsert, count, 1000, true);
                                }

                                context.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                            }
                            finally
                            {
                                if (context != null)
                                    context.Dispose();
                            }

                            //scope.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private AIMS_MainEntities AddToContext(AIMS_MainEntities context, GF_COMPOSITE_LTHOLDINGS entity, int count,
                                               int commitCount, bool recreateContext)
        {
            context.Set<GF_COMPOSITE_LTHOLDINGS>().Add(entity);

            if (count%commitCount == 0)
            {
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

        /* *

         * For each composite,
            Step 1   Retrieve the list of portfolios in the composite that are active (using the new COMPOSITE_MATRIX table).
            Step 2   For portfolios returned in Step 1, retrieve all records from GF_PORTFOLIO_LTHOLDINGS.
            Step 3   Delete records when appropriate based on Look_Thru setting in COMPOSITE_MATRIX view.  When Look_Thru <> 'Y', delete records returned from view where PORTFOLIO_ID <> A_PFCHOLDINGS_PORLT
            Step 4   Aggregate remaining records together by the ASEC_SEC_SHORT_NAME, and PORTFOLIO_DATE.
          
         */
    }
}