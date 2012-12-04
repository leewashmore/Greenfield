using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpt
{
    public interface IRegionModelResident : IModel
    {
        void Accept(IRegionModelResidentResolver resolver);
    }
    public interface IRegionModelResidentResolver
    {
        void Resolve(BasketCountryModel model);
        void Resolve(BasketRegionModel model);
        void Resolve(RegionModel model);
    }
}
