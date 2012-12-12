using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using TopDown.Core.ManagingSecurities;
using Aims.Core.Persisting;
using Aims.Core;

namespace TopDown.Core.ManagingPortfolios
{
    public class PortfolioManager : Aims.Core.PortfolioManager
    {
        private PortfolioToJsonSerializer portfolioSerializer;

        [DebuggerStepThrough]
        public PortfolioManager(
            IStorage<PortfolioRepository> portfolioRepositoryStorage,
            ManagingPortfolios.PortfolioToJsonSerializer portfolioSerializer
        )
            : base(portfolioRepositoryStorage)
        {
            this.portfolioSerializer = portfolioSerializer;
        }

        public String SerializeToJson(IEnumerable<BottomUpPortfolio> portfolios)
        {
            var builder = new StringBuilder();
            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.WriteArray(portfolios, portfolio =>
                {
                    writer.Write(delegate
                    {
                        this.portfolioSerializer.SerializeBottomUpPortfolio(portfolio, writer);
                    });
                });
            }
            return builder.ToString();
        }
    }
}
