using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingBt
{
	public class TaxonomyChange
	{
		[DebuggerStepThrough]
		public TaxonomyChange(Taxonomy taxonomy, RootModel model)
		{
			this.Taxonomy = taxonomy;
			this.Model = model;
		}

		public Taxonomy Taxonomy { get; set; }
		public RootModel Model { get; private set; }
	}
}