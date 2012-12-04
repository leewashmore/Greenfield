using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingTaxonomies;

namespace TopDown.Core.ManagingBpt.ChangingBt
{
    public class ModelToChangesetTransformer
    {
		private GlobeTraverser traverser;
		
		[DebuggerStepThrough]
		public ModelToChangesetTransformer(GlobeTraverser traverser)
		{
			this.traverser = traverser;
		}

		public Changeset TryTransformToChangeset(RootModel model, Taxonomy taxonomy)
        {
			var models = this.traverser.TraverseGlobe(model.Globe);
			var otherOpt = models.Select(x => x.TryAsOther()).Where(x => x != null).SingleOrDefault();
			if (otherOpt == null) return null; // no other model, no unsaved baskets, no changeset

			var changes = otherOpt.UnsavedBasketCountries.Select(
				x => new CountryBasketChange(x, otherOpt)
			);

			if (!changes.Any()) return null; // no changes, no changeset

			var change = new TaxonomyChange(taxonomy, model);

			var result = new Changeset(change, changes);
			return result;
        }
    }
}
