using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTaxonomies
{
    public class TaxonomyRepository : KeyedRepository<Int32, Taxonomy>
    {
        public TaxonomyRepository(IEnumerable<Taxonomy> taxonomies)
        {
            taxonomies.ForEach(x => base.RegisterValue(x, x.Id));
        }

        public Taxonomy GetTaxonomy(Int32 id)
        {
            var found = base.FindValue(id);
            if (found == null) throw new ApplicationException("There is no taxonomy with the \"" + id + "\" ID.");
            return found;
        }
    }
}
