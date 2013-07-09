using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using TopDown.Core.Persisting;
using Aims.Core.Sql;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace TopDown.Core.ManagingCalculations
{
    public class OutputTargetingFileException : ApplicationException
    {
        public OutputTargetingFileException(String message, Exception e) : base(message, e) { }
    }

    public class TradingTargetsFileManager
    {

        public TradingTargetsFileManager()
        {

        }

        private class GroupKey
        {
            public String PortfolioId { get; set; }
            public String CountryCode { get; set; }

            public override bool Equals(object obj)
            {
                var o = (GroupKey)obj;
                if (o != null)
                {
                    return o.PortfolioId == this.PortfolioId && o.CountryCode == this.CountryCode;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return PortfolioId.GetHashCode()^this.CountryCode.GetHashCode();
            }
        }

        private List<TradingTargetRecord> GetLines(SecurityRepository securityRepository, IDataManager dataManager)
        {
            //SecurityRepository securities = repositoryMananer.ClaimSecurityRepository(dataManager);
            var targets = dataManager.GetAllTargets();
            var result = targets.Select(x => new TradingTargetRecord(x.PortfolioId, securityRepository.FindSecurity(x.SecurityId), x.Target)).ToList();

            var groups = result.GroupBy(x => new GroupKey { PortfolioId = x.PortfolioId, CountryCode = x.Security.IsoCountryCode })
                .Select(x => new { Key = x.Key, Total = x.Sum(y => y.Target) })
                .ToDictionary(x => x.Key);
            foreach (var r in result)
            {
                var key = new GroupKey { CountryCode = r.Security.IsoCountryCode, PortfolioId = r.PortfolioId };
                if (groups.ContainsKey(key))
                {
                    r.SumByCountry = groups[key].Total;
                    if (r.SumByCountry != 0)
                        r.PercentByContry = r.Target / r.SumByCountry;
                }
            }

            var groupsByPortfolio = result.GroupBy(x => x.PortfolioId).Select(x => new { Key = x.Key, Total = x.Sum(y => y.Target) });
            foreach (var r in groupsByPortfolio)
            {
                // cash account line
                var record = new TradingTargetRecord(r.Key, new CompanySecurity("MODOFFSET", "", "MODOFFSET", "", new Country("US", "USA"), "", "", "USD", "", "US"), 1 - r.Total);
                record.SumByCountry = record.Target;
                record.PercentByContry = 1;
                result.Add(record);
            }
            var proxyportfolios = dataManager.GetAllProxyPortfolios();
            foreach (var proxy in proxyportfolios)
            {

                var lists = result.Where(x => x.PortfolioId == proxy.ProxyPortfolioId).ToList(); // get the list of targets for proxy portfolio
                var cloneList = CloneList(lists); //clone the list
                cloneList.ForEach(s => s.PortfolioId = proxy.PortfolioId);
                //below to lines replace the targets of actual portfolio with the proxy portfolio
                result.RemoveAll(x => x.PortfolioId == proxy.PortfolioId);
                // replace the proxy portfolio with the actual portfolio)
                result.AddRange(cloneList);
            }

            return result;
        }
        // This method does the deep cloning of the list

        public static List<T> CloneList<T>(List<T> oldList)
        {

            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, oldList);
            stream.Position = 0;
            return (List<T>)formatter.Deserialize(stream);



        } 


        public string GetFileContent(SecurityRepository securityRepository, IDataManager dataManager)
        {
            var lines = GetLines(securityRepository, dataManager);
            var dt = DateTime.Today;
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.Target != 0)
                    sb.AppendLine(String.Format("{0:yyyyMMdd},M_{1},{2},{3},{4},{5},{6},{7},{8},{9}", dt, line.PortfolioId, line.Security.ShortName, line.Security.IsoCountryCode, line.Security.Currency, line.PercentByContry, line.SumByCountry, line.Target, line.Security.Ticker, line.Security.Isin));
            }

            return sb.ToString();
        }
    }
}
