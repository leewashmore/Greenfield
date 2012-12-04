using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTaxonomies
{
    public interface INode
    {
        void Accept(INodeResolver resolver);
    }

    public interface INodeResolver
    {
        void Resolve(BasketCountryNode node);
        void Resolve(OtherNode node);
        void Resolve(RegionNode node);
        void Resolve(BasketRegionNode node);
        void Resolve(UnsavedBasketCountryNode node);
    }
}
