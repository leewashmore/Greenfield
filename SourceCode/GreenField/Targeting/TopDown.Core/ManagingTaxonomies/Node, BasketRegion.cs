using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingBaskets;

namespace TopDown.Core.ManagingTaxonomies
{
    public class BasketRegionNode : IRegionNodeResident, ITaxonomyResident, INode
    {
        [DebuggerStepThrough]
        public BasketRegionNode(RegionBasket basket)
        {
            this.Basket = basket;
        }

        public RegionBasket Basket { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IRegionNodeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(INodeResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(ITaxonomyResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
