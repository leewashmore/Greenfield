using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.FacingServer.Backend.Targeting
{
    public interface IGlobeResident
    {
        Int32 Level { get; }
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
        void Resolve(CashLineModel model);
        void Resolve(TotalLineModel model);
    }
}
