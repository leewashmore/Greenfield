using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingCalculations
{
    public class TradingTargetsFileManager
    {
        private IDataManager dataManager;
        private PortfolioRepository portfolios;
        private SecurityRepository securities;
    
        public TradingTargetsFileManager(SecurityRepository securities, PortfolioRepository portfolios, IDataManager dataManager)
        {
            this.dataManager = dataManager;
            this.portfolios = portfolios;
            this.securities = securities;

        }

        //public IEnumerable<TradingTargetRecord> GetLines()
        //{

        //    var targets = this.dataManager.GetAllTargets();
        //    var result = targets.Select(x => new TradingTargetRecord(x.PortfolioId, securities.FindSecurity(x.SecurityId), x.Target)).ToList();
        //    var groups = result.GroupBy()


        //}
    }
}
