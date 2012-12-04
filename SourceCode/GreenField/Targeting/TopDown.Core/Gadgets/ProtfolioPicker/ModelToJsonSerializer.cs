using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingPortfolios;

namespace TopDown.Core.Gadgets.PortfolioPicker
{
    public class ModelToJsonSerializer
    {
        public void SerializeTargetingType(TargetingTypeModel targeting, IJsonWriter writer)
        {
            writer.Write(targeting.Id, "id");
            writer.Write(targeting.Name, "name");
            writer.WriteArray(targeting.Portfolios, "portfolios", portfolio =>
            {
                writer.Write(delegate
                {
					this.SerializePortfolio(portfolio, writer);
                });
            });
        }

		public void SerializePortfolio(PortfolioModel portfolio, IJsonWriter writer)
        {
            writer.Write(portfolio.Id, "id");
            writer.Write(portfolio.Id, "name");
        }
    }
}
