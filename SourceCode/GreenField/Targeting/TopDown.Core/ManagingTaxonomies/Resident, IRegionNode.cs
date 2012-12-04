using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTaxonomies
{
    public interface IRegionNodeResident
    {
        void Accept(IRegionNodeResidentResolver resolver);
    }

    public interface IRegionNodeResidentResolver
    {
        void Resolve(RegionNode regionNode);
        void Resolve(BasketRegionNode basketRegionNode);
        void Resolve(BasketCountryNode basketCountryNode);
    }
}
