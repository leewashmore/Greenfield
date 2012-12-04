using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.PortfolioPicker
{
	public class TargetingTypeModel
	{
		[DebuggerStepThrough]
		public TargetingTypeModel(Int32 id, String name,  IEnumerable<PortfolioModel> portfolios)
		{
			this.Id = id;
			this.Name = name;
			this.Portfolios = portfolios.ToList();
		}
		public Int32 Id { get; private set; }
		public String Name { get; private set; }
		public IEnumerable<PortfolioModel> Portfolios { get; private set; }
	}
}
