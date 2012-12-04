using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTaxonomies
{
    public interface ITaxonomyResident
    {
        void Accept(ITaxonomyResidentResolver resolver);
    }

    public interface ITaxonomyResidentResolver
    {
        void Resolve(RegionNode node);
        void Resolve(BasketRegionNode node);
        void Resolve(OtherNode node);
    }
}
