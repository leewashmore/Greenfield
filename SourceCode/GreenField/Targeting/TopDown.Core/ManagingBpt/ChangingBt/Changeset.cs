using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingBt
{
    public class Changeset
    {
		[DebuggerStepThrough]
		public Changeset(TaxonomyChange taxonomyChange, IEnumerable<CountryBasketChange> changes)
		{
			this.TaxonomyChange = taxonomyChange;
			this.CountryBasketChanges = changes.ToList();
		}

		public TaxonomyChange TaxonomyChange { get; private set; }
		public IEnumerable<CountryBasketChange> CountryBasketChanges { get; private set; }
	}
}
