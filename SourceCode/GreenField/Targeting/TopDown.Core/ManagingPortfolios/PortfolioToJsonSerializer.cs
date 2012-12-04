using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingPortfolios
{
	public class PortfolioToJsonSerializer
	{
		private ManagingSecurities.SecurityToJsonSerializer securitySerializer;
		
		[DebuggerStepThrough]
		public PortfolioToJsonSerializer(ManagingSecurities.SecurityToJsonSerializer securitySerializer)
		{
			this.securitySerializer = securitySerializer;
		}
		
		public void SerializeBottomUpPortfolio(BottomUpPortfolio portfolio, IJsonWriter writer)
		{
			writer.Write(portfolio.Id, JsonNames.Name);
			writer.Write(portfolio.Id, JsonNames.Id);
			writer.Write(JsonNames.Security, delegate
			{
				this.securitySerializer.SerializeFund(portfolio.Fund, writer);
			});
		}
	}
}
