using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTaxonomies
{
    public interface IOtherNodeResident
    {
        void Accept(IOtherNodeResidentResolver resolver);
    }
    public interface IOtherNodeResidentResolver
    {
        void Resolve(BasketCountryNode basketCountry);
        void Resolve(UnsavedBasketCountryNode unsavedBasketCountry);
    }
}
