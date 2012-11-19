using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.Targeting.Only.Backend.Targeting
{
    public interface IGlobeResident
    {
        IExpandableModel Parent { get; }
        Boolean IsVisible { get; }
        void Accept(IGlobeResidentResolver resolver);
    }

    public interface IGlobeResidentResolver
    {
        void Resolve(BasketCountryModel model);
        void Resolve(BasketRegionModel model);
        void Resolve(OtherModel model);
        void Resolve(RegionModel model);
        void Resolve(UnsavedBasketCountryModel model);
        void Resolve(BgaCountryModel model);
    }
}
