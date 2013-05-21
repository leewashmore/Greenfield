using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketingCalc.DAL;
using MarketingCalc.DataContract;
using MarketingCalc.ServiceCaller;
namespace MarketingCalc.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            MarketingEntities entity = new MarketingEntities();
            MarketingCalcServiceCaller mcsc = new MarketingCalcServiceCaller();
            List<GetPortfolioList_Result> pf = entity.GetPortfolioList().ToList();
            DateTime dt = new DateTime(2013, 04, 30);
            List<GetPCDataForMarketing_Result> pcdataList = entity.GetPCDataForMarketing(dt).ToList();
            List<GetPCDataForMarketing_Result> portfolioData = null;
            List<PortfolioValuation> pfValuation = new List<PortfolioValuation>();
            foreach (var portfolio in pf)
            {
                mcsc.CallService(portfolio.portfolio_id);

                //portfolioData = pcdataList.Where(p => p.portfolio_id == portfolio.portfolio_id).ToList();
                //List<GetEarningsDataForMarketing_Result> pfEarningsDataList = entity.GetEarningsDataForMarketing(portfolio.portfolio_id, dt, "PRIMARY", "USD", "A", 2013, "CALENDAR").ToList();
                //List<GetMarketCapDataForMarketing_Result > pfMarketCapDataList = entity.GetMarketCapDataForMarketing(portfolio.portfolio_id, dt, "PRIMARY", "USD", "C", 0, "").ToList();
                //Console.WriteLine(portfolio.portfolio_id);
                //foreach (var issuers in portfolioData)
                //{
                //    PortfolioValuation pv = new PortfolioValuation();
                //    pv.portfolio_id = issuers.portfolio_id;
                //    pv.issuer_id = issuers.issuer_id;
                //    pv.Asec_Sec_short_name = issuers.asec_sec_short_name;
                //    pv.dirtvaluepc = issuers.dirty_value_pc;
                //    pv.dirty_price = issuers.dirty_price;
                //    pv.Security_id = pfMarketCapDataList.Where(mktCap => mktCap.issuer_id == issuers.issuer_id && mktCap.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.security_id).FirstOrDefault();
                //    pv.marketcap = pfMarketCapDataList.Where(mktCap => mktCap.issuer_id == issuers.issuer_id && mktCap.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                //    pv.earnings = pfEarningsDataList.Where(mktCap => mktCap.issuer_id == issuers.issuer_id && mktCap.asec_Sec_Short_name == issuers.asec_sec_short_name).Select(v => v.value).FirstOrDefault();
                //    if (pv.dirtvaluepc != null && pv.earnings != null && pv.marketcap != null)
                //    {
                //        pv.percentFactorOwned = (pv.dirtvaluepc / pv.marketcap) * pv.earnings;
                //    }
                //    pfValuation.Add(pv);
                //}

            }
         }
    }
}
