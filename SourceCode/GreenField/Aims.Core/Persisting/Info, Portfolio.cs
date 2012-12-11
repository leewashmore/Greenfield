using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Core.Persisting
{
	public class PortfolioInfo
	{
		[DebuggerStepThrough]
		public PortfolioInfo()
		{
		}


		[DebuggerStepThrough]
		public PortfolioInfo(String id, String name)
		{
			this.Id = id;
			this.Name = name;
		}

		public String Id { get; set; }
		public String Name { get; set; }
	}
}
