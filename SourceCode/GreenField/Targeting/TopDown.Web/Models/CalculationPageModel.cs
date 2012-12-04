using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using TopDown.Core.ManagingCalculations;

namespace TopDown.Web.Models
{
	public class CalculationPageModel
	{
		[DebuggerStepThrough]
		public CalculationPageModel(IEnumerable<TargetRecord> targets)
		{
			this.Targets = targets.ToList();
		}

		public IEnumerable<TargetRecord> Targets { get; private set; }
	}
}
