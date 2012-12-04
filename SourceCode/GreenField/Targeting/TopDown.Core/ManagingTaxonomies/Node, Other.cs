using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingTaxonomies
{
    public class OtherNode : Repository<BasketCountryNode>, ITaxonomyResident, INode
    {
        [DebuggerStepThrough]
        public void RegisterResident(BasketCountryNode basketCountry)
        {
            base.RegisterValue(basketCountry);
        }
        
        [DebuggerStepThrough]
        public IEnumerable<BasketCountryNode> GetBasketCountries()
        {
            return base.GetValues();
        }

        [DebuggerStepThrough]
        public void Accept(ITaxonomyResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(INodeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
