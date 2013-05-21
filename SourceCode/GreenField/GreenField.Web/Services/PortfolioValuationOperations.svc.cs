using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.DAL;
using GreenField.DataContracts;
using System.IO;
using GreenField.Web.Helpers;
namespace GreenField.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PortfolioValuationOperations" in code, svc and config file together.
    [ServiceContract]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PortfolioValuationOperations
    {
         [OperationContract]
        public List<PortfolioValuation>  PortfolioLevelValuationForMarketing(string portfolio_id)
        {
            MarketingEntities entity = new MarketingEntities();
            DateTime dt = new DateTime(2013, 04, 30);
            List<GetPCDataForMarketing_Result> portfolioData = entity.GetPCDataForMarketing(dt, portfolio_id).ToList();

            List<GetIssuerLevelPFDataForMarketing_Result> pfEarningsDataList = entity.GetIssuerLevelPFDataForMarketing(portfolio_id, dt,290, "PRIMARY", "USD", "A", DateTime.Today.Year, "CALENDAR").ToList();
            List<GetIssuerLevelPFDataForMarketing_Result> pfNextYearEarningsDataList = entity.GetIssuerLevelPFDataForMarketing(portfolio_id, dt, 290, "PRIMARY", "USD", "A", DateTime.Today.Year+1, "CALENDAR").ToList();
            List<GetIssuerLevelPFDataForMarketing_Result> pfFwdEarningsDataList = entity.GetIssuerLevelPFDataForMarketing(portfolio_id, dt, 279, "PRIMARY", "USD", "C", 0, "FISCAL").ToList();
            
             List<GetIssuerLevelPFDataForMarketing_Result> pfEquityList = entity.GetIssuerLevelPFDataForMarketing(portfolio_id, dt,104, "PRIMARY", "USD", "A", DateTime.Today.Year, "CALENDAR").ToList();
            List<GetIssuerLevelPFDataForMarketing_Result> pfNextYearEquityList = entity.GetIssuerLevelPFDataForMarketing(portfolio_id, dt, 104, "PRIMARY", "USD", "A", DateTime.Today.Year+1, "CALENDAR").ToList();
            List<GetIssuerLevelPFDataForMarketing_Result> pfFwdEquityList = entity.GetIssuerLevelPFDataForMarketing(portfolio_id, dt, 280, "PRIMARY", "USD", "C", 0, "FISCAL").ToList();

             List<GetSecurityLevelPFDataForMarketing_Result> pfMarketCapDataList =entity.GetSecurityLevelPFDataForMarketing(portfolio_id, dt,185, "PRIMARY", "USD", "C", 0, "").ToList();
            List<PortfolioValuation> pfValuation = new List<PortfolioValuation>();
            decimal? totalMarketValue = portfolioData.Sum(g => g.dirty_value_pc);
            
            foreach (var issuers in portfolioData)
            {
                PortfolioValuation pv = new PortfolioValuation();
                pv.portfolio_id = issuers.portfolio_id;
                pv.issuer_id = issuers.issuer_id;
                pv.Asec_Sec_short_name = issuers.asec_sec_short_name;
                pv.dirtvaluepc = issuers.dirty_value_pc;
                pv.dirty_price = issuers.dirty_price;
                pv.Security_id = pfMarketCapDataList.Where(mktCap => mktCap.issuer_id == issuers.issuer_id && mktCap.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.security_id).FirstOrDefault();
                pv.marketcap = pfMarketCapDataList.Where(mktCap => mktCap.issuer_id == issuers.issuer_id && mktCap.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                pv.earnings = pfEarningsDataList.Where(earnings => earnings.issuer_id == issuers.issuer_id && earnings.asec_Sec_Short_name == issuers.asec_sec_short_name ).Select(v => v.value).FirstOrDefault();
                pv.fwdearnings = pfFwdEarningsDataList.Where(fwdearnings => fwdearnings.issuer_id == issuers.issuer_id && fwdearnings.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                pv.nextYearEarnings = pfNextYearEarningsDataList.Where(nxtYearEarnings => nxtYearEarnings.issuer_id == nxtYearEarnings.issuer_id && nxtYearEarnings.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                pv.equity = pfEquityList.Where(equity => equity.issuer_id == issuers.issuer_id && equity.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                pv.fwdEquity = pfFwdEquityList.Where(fwdequity => fwdequity.issuer_id == issuers.issuer_id && fwdequity.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                pv.nextYearEquity = pfNextYearEquityList.Where(nextyearequity => nextyearequity.issuer_id == issuers.issuer_id && nextyearequity.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();

                if (pv.dirtvaluepc != null && pv.earnings != null && pv.marketcap != null)
                {
                    pv.percentFactorOwned = (pv.dirtvaluepc / pv.marketcap) * pv.earnings;

                }

                if (pv.dirtvaluepc != null && pv.fwdearnings != null && pv.marketcap != null)
                {
                    pv.fwdpercentFactorOwned = (pv.dirtvaluepc / pv.marketcap) * pv.fwdearnings;
                }

                if (pv.dirtvaluepc != null && pv.nextYearEarnings != null && pv.marketcap != null)
                {
                    pv.nextYearPercentFactorOwned= (pv.dirtvaluepc / pv.marketcap) * pv.nextYearEarnings;
                }

                if (pv.dirtvaluepc != null && pv.equity != null && pv.marketcap != null)
                {
                    pv.equityFactorOwned = (pv.dirtvaluepc / pv.marketcap) * pv.equity;
                }

                if (pv.dirtvaluepc != null && pv.fwdEquity != null && pv.marketcap != null)
                {
                    pv.fwdEquityFactorOwned = (pv.dirtvaluepc / pv.marketcap) * pv.fwdEquity;
                }
                if (pv.dirtvaluepc != null && pv.nextYearEquity != null && pv.marketcap != null)
                {
                    pv.nextYearEquityFactorOwned = (pv.dirtvaluepc / pv.marketcap) * pv.nextYearEquity;
                }


                pfValuation.Add(pv);
            }


             PercentOwnerShipMethodology(pfValuation, portfolio_id, entity, dt);
             WeightedMethodology(pfValuation, portfolio_id, entity,dt);
             MedianMethodology(pfValuation, portfolio_id, entity, dt);
             SimpleAverageMethodology(pfValuation, portfolio_id, entity, dt);
             //String xmlText = SerializeObjectToXML(pfValuation);

             return pfValuation;
        }

        private void PercentOwnerShipMethodology(List<PortfolioValuation> pfValuation, String portfolio_id, MarketingEntities entity, DateTime effDate)
        {
            //persist in the database current P/E
            List<decimal?> mktValue = pfValuation.Where(g => g.portfolio_id == portfolio_id && g.percentFactorOwned.HasValue).Select(data => data.dirtvaluepc).ToList();
            List<decimal?> percentFactorOwned = pfValuation.Where(g => g.portfolio_id == portfolio_id).Select(data => data.percentFactorOwned).ToList();

            decimal? percentageOwned = GroupCalculations.PercentageOwned(mktValue, percentFactorOwned);

            int status = entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "PCT_OWNED", 0, 166, percentageOwned);

            //persist in the database FWD P/E
            List<decimal?> fwdpercentFactorOwned = pfValuation.Where(g => g.portfolio_id == portfolio_id).Select(data => data.fwdpercentFactorOwned).ToList();
            mktValue = pfValuation.Where(g => g.portfolio_id == portfolio_id && g.fwdpercentFactorOwned.HasValue).Select(data => data.dirtvaluepc).ToList();

            decimal? fwdpercentageOwned = GroupCalculations.PercentageOwned(mktValue, fwdpercentFactorOwned);
            status = entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "PCT_OWNED",0, 187, fwdpercentageOwned);

            //persist in the database Next Year P/E
            List<decimal?> nextYearPercentFactorOwned = pfValuation.Where(g => g.portfolio_id == portfolio_id).Select(data => data.nextYearPercentFactorOwned).ToList();
            mktValue = pfValuation.Where(g => g.portfolio_id == portfolio_id && g.nextYearPercentFactorOwned.HasValue).Select(data => data.dirtvaluepc).ToList();

            decimal? nextyearpercentageOwnership = GroupCalculations.PercentageOwned(mktValue, nextYearPercentFactorOwned);
            status = entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "PCT_OWNED",1, 166, nextyearpercentageOwnership);
            
            //persist current year P/B
            mktValue = pfValuation.Where(g => g.portfolio_id == portfolio_id && g.equityFactorOwned.HasValue).Select(data => data.dirtvaluepc).ToList();
            List<decimal?> equityPercentFactorOwned = pfValuation.Where(g => g.portfolio_id == portfolio_id).Select(data => data.equityFactorOwned).ToList();
            decimal? pbPercentOwnership = GroupCalculations.PercentageOwned(mktValue, equityPercentFactorOwned);
            status = entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "PCT_OWNED", 0, 164, pbPercentOwnership);

            //persist Forward P/B
            mktValue = pfValuation.Where(g => g.portfolio_id == portfolio_id && g.fwdEquityFactorOwned.HasValue).Select(data => data.dirtvaluepc).ToList();
            List<decimal?> fwdEquityPercentFactorOwned = pfValuation.Where(g => g.portfolio_id == portfolio_id).Select(data => data.fwdEquityFactorOwned).ToList();
            decimal? fwdPBPercentOwnership = GroupCalculations.PercentageOwned(mktValue, fwdEquityPercentFactorOwned);
            status = entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "PCT_OWNED", 0, 188, fwdPBPercentOwnership);

            //persist Next Year P/B
            mktValue = pfValuation.Where(g => g.portfolio_id == portfolio_id && g.nextYearEquityFactorOwned.HasValue).Select(data => data.dirtvaluepc).ToList();
            List<decimal?> nxtYearEquityPercentFactorOwned = pfValuation.Where(g => g.portfolio_id == portfolio_id).Select(data => data.nextYearEquityFactorOwned).ToList();
            decimal? nxtYearPBPercentOwnership = GroupCalculations.PercentageOwned(mktValue, nxtYearEquityPercentFactorOwned);
            status = entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "PCT_OWNED",  1, 164, nxtYearPBPercentOwnership);


        }

         private void WeightedMethodology(List<PortfolioValuation> pfValuation, String portfolio_id, MarketingEntities entity, DateTime effDate)
         {

             decimal? totalMarketValue = pfValuation.Where(g => g.percentFactorOwned.HasValue).Sum(g => g.dirtvaluepc);
             decimal? totalMarketValueFwd = pfValuation.Where(g => g.fwdpercentFactorOwned.HasValue).Sum(g => g.dirtvaluepc);
             decimal? nextYearTotalMarketValue = pfValuation.Where(g => g.nextYearPercentFactorOwned.HasValue).Sum(g => g.dirtvaluepc);
             decimal? totalMarketValuePB = pfValuation.Where(g => g.equityFactorOwned.HasValue).Sum(g => g.dirtvaluepc);
             decimal? fwdTotalMarketValuePB = pfValuation.Where(g => g.fwdEquityFactorOwned.HasValue).Sum(g => g.dirtvaluepc);
             decimal? nextYearTotalMarketValuePB = pfValuation.Where(g => g.nextYearEquityFactorOwned.HasValue).Sum(g => g.dirtvaluepc);
             decimal? totalMarketValueMktCap = pfValuation.Where(g => g.marketcap.HasValue).Sum(g => g.dirtvaluepc);

             DataScrubber d = new DataScrubber();
             foreach (var pfData in pfValuation)
             {
                 //For current weighted PE
                 if (pfData.dirtvaluepc != null && pfData.percentFactorOwned != null)
                 {
                     pfData.weight = pfData.dirtvaluepc / totalMarketValue;
                 }
                 if (pfData.marketcap != null && pfData.earnings != null && pfData.percentFactorOwned != null)
                 {
                     pfData.currentYearPE = pfData.marketcap / pfData.earnings;
                     pfData.currentYearPE = d.doRangeScrubbing(pfData.currentYearPE, 166);
                 }
                 if (pfData.weight != null && pfData.currentYearPE != null)
                 {
                     pfData.currYearPEContr = pfData.weight * pfData.currentYearPE;
                 }

                 //For forward weighted PE

                 if (pfData.dirtvaluepc != null && pfData.fwdpercentFactorOwned != null)
                 {
                     pfData.fwdWeight = pfData.dirtvaluepc / totalMarketValueFwd;
                 }
                 if (pfData.marketcap != null && pfData.fwdearnings != null && pfData.fwdpercentFactorOwned != null)
                 {
                     pfData.fwdPE = pfData.marketcap / pfData.fwdearnings;
                     pfData.fwdPE = d.doRangeScrubbing(pfData.fwdPE, 187);
                 }
                 if (pfData.fwdWeight != null && pfData.fwdPE != null)
                 {
                     pfData.fwdPEContr = pfData.fwdWeight * pfData.fwdPE;
                 }

                 //For Next Year weighted PE

                 if (pfData.dirtvaluepc != null && pfData.nextYearPercentFactorOwned!= null)
                 {
                     pfData.nextYearWeight = pfData.dirtvaluepc /nextYearTotalMarketValue;
                 }
                 if (pfData.marketcap != null && pfData.nextYearEarnings != null && pfData.nextYearPercentFactorOwned != null)
                 {
                     pfData.nextYearPE = pfData.marketcap / pfData.nextYearEarnings;
                     pfData.nextYearPE = d.doRangeScrubbing(pfData.nextYearPE, 166);
                 }
                 if (pfData.nextYearWeight != null && pfData.nextYearPE != null)
                 {
                     pfData.nextYearPEContr = pfData.nextYearWeight * pfData.nextYearPE;
                 }

                 //For current weighted PB
                 if (pfData.dirtvaluepc != null && pfData.equityFactorOwned != null)
                 {
                     pfData.weightPB = pfData.dirtvaluepc / totalMarketValue;
                 }
                 if (pfData.marketcap != null && pfData.equity != null && pfData.equityFactorOwned != null)
                 {
                     pfData.currentYearPB = pfData.marketcap / pfData.equity;
                     pfData.currentYearPB = d.doRangeScrubbing(pfData.currentYearPB, 164);
                 }
                 if (pfData.weightPB != null && pfData.currentYearPB != null)
                 {
                     pfData.currYearPBContr = pfData.weightPB * pfData.currentYearPB;
                 }

                 //For forward weighted PB
                 if (pfData.dirtvaluepc != null && pfData.fwdEquityFactorOwned != null)
                 {
                     pfData.fwdWeightPB = pfData.dirtvaluepc / fwdTotalMarketValuePB;
                 }
                 if (pfData.marketcap != null && pfData.fwdEquity != null && pfData.fwdEquityFactorOwned != null)
                 {
                     pfData.fwdPB = pfData.marketcap / pfData.fwdEquity;
                     pfData.fwdPB = d.doRangeScrubbing(pfData.fwdPB, 188);
                 }
                 if (pfData.weightPB != null && pfData.currentYearPB != null)
                 {
                     pfData.fwdPBContr = pfData.fwdWeightPB * pfData.fwdPB;
                 }

                 //For next year weighted PB
                 if (pfData.dirtvaluepc != null && pfData.nextYearEquityFactorOwned != null)
                 {
                     pfData.nextYearWeightPB = pfData.dirtvaluepc / nextYearTotalMarketValuePB;
                 }
                 if (pfData.marketcap != null && pfData.nextYearEquity != null && pfData.nextYearEquityFactorOwned != null)
                 {
                     pfData.nextYearPB = pfData.marketcap / pfData.nextYearEquity;
                     pfData.nextYearPB = d.doRangeScrubbing(pfData.nextYearPB, 164);
                 }
                 if (pfData.weightPB != null && pfData.currentYearPB != null)
                 {
                     pfData.nextYearPBContr = pfData.nextYearWeightPB * pfData.nextYearPB;
                 }

                 //For Weighted MarketCap
                 if (pfData.dirtvaluepc != null && pfData.marketcap != null)
                 {
                     pfData.weightMktCap = pfData.dirtvaluepc / totalMarketValueMktCap;
                 }

                 //For Weighted MarketCap
                 if (pfData.weightMktCap != null && pfData.marketcap != null)
                 {
                     pfData.mktCapContr = pfData.weightMktCap * pfData.marketcap;
                 }
             }
          
             decimal? pfWeightPE = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.currYearPEContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED", 0, 166, pfWeightPE);

             decimal? pFwdWeightPE = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.fwdPEContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED",0, 187, pFwdWeightPE);

             decimal? nextYearWeightPE = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.nextYearPEContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED", 0, 166, nextYearWeightPE);

             decimal? pfWeightPB = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.currYearPBContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED", 0, 164, pfWeightPB);

             decimal? pFwdWeightPB = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.fwdPBContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED",0, 188, pFwdWeightPB);

             decimal? nextYearWeightPB = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.nextYearPBContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED", 1 , 164, nextYearWeightPB);

             decimal? weightedMktCap = pfValuation.Where(g => g.portfolio_id == portfolio_id).Sum(data => data.mktCapContr);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "WEIGHTED", 0, 185, weightedMktCap);
         }

         private void MedianMethodology(List<PortfolioValuation> pfValuation, String portfolio_id, MarketingEntities entity, DateTime effDate)
         {
             List<decimal?> list = pfValuation.Where(g => g.currentYearPE.HasValue).Select(g=>g.currentYearPE).ToList();
            decimal? medianCurrPE =  GroupCalculations.Median(list);
            list = pfValuation.Where(g => g.fwdPE.HasValue).Select(g => g.fwdPE).ToList();
            decimal? medianfwdPE = GroupCalculations.Median(list);
            list = pfValuation.Where(g => g.nextYearPE.HasValue).Select(g => g.nextYearPE).ToList();
            decimal? nextyearPE = GroupCalculations.Median(list);
            list = pfValuation.Where(g => g.currentYearPB.HasValue).Select(g => g.currentYearPB).ToList();
            decimal? averageCurrPB = GroupCalculations.Median(list);
            list = pfValuation.Where(g => g.fwdPB.HasValue).Select(g => g.fwdPB).ToList();
            decimal? medianfwdPB = GroupCalculations.Median(list);
            list = pfValuation.Where(g => g.nextYearPB.HasValue).Select(g => g.nextYearPB).ToList();
            decimal? nextyearPB = GroupCalculations.Median(list);

            list = pfValuation.Where(g => g.marketcap.HasValue).Select(g => g.marketcap).ToList();
            decimal? medianmktCap = GroupCalculations.Median(list);

            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 0, 166, medianCurrPE);
            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 0, 187, medianfwdPE);
            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 1, 166, nextyearPE);
            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 0, 164, averageCurrPB);
            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 0, 188, medianfwdPB);
            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 1, 164, nextyearPB);
            entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "MEDIAN", 0, 185, medianmktCap);
         }

         private void SimpleAverageMethodology(List<PortfolioValuation> pfValuation, String portfolio_id, MarketingEntities entity, DateTime effDate)
         {
             List<decimal?> list = pfValuation.Where(g => g.currentYearPE.HasValue).Select(g => g.currentYearPE).ToList();
             decimal? averageCurrYearPE = GroupCalculations.SimpleAverage(list);

             list = pfValuation.Where(g => g.fwdPE.HasValue).Select(g => g.fwdPE).ToList();
             decimal? averageFwdPE = GroupCalculations.SimpleAverage(list);

             list = pfValuation.Where(g => g.nextYearPE.HasValue).Select(g => g.nextYearPE).ToList();
             decimal? averagenextyearPE = GroupCalculations.SimpleAverage(list);

             list = pfValuation.Where(g => g.currentYearPB.HasValue).Select(g => g.currentYearPB).ToList();
             decimal? averageCurrYearPB = GroupCalculations.SimpleAverage(list);

             list = pfValuation.Where(g => g.fwdPB.HasValue).Select(g => g.fwdPB).ToList();
             decimal? averageFwdPB = GroupCalculations.SimpleAverage(list);

             list = pfValuation.Where(g => g.nextYearPB.HasValue).Select(g => g.nextYearPB).ToList();
             decimal? averagenextyearPB = GroupCalculations.SimpleAverage(list);

             list = pfValuation.Where(g => g.marketcap.HasValue).Select(g => g.marketcap).ToList();
             decimal? averagemktCap = GroupCalculations.SimpleAverage(list);


             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 0, 166, averageCurrYearPE);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 0, 187, averageFwdPE);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 1, 166, averagenextyearPE);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 0, 164, averageCurrYearPB);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 0, 188, averageFwdPB);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 1, 164, averagenextyearPB);
             entity.SaveUpdatedPortfolioValuation(effDate, portfolio_id, "AVERAGE", 0, 185, averagemktCap);

         }

        

    }
}
