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
		public PortfolioInfo(String id, String name, Boolean isBottomUp)
		{
			this.Id = id;
			this.Name = name;
            this.IsBottomUp = isBottomUp;
		}

		public String Id { get; set; }
		public String Name { get; set; }
        public Boolean IsBottomUp { get; set; }
	}
}
