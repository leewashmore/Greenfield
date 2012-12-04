using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    /// <summary>
    /// WCF doesn't know how to handle interfaces. It turns them into Object.
    /// So the best we can do is to inherit from the same base object.
    /// </summary>
    [DataContract]
    [KnownType(typeof(RegionModel))]
    [KnownType(typeof(OtherModel))]
    [KnownType(typeof(BasketRegionModel))]
    [KnownType(typeof(BasketCountryModel))]
    [KnownType(typeof(CountryModel))]
    [KnownType(typeof(UnsavedBasketCountryModel))]
    public abstract class GlobeResident
    {
        public abstract void Accept(IGlobeResidentResolver resolver);
    }

    public interface IGlobeResidentResolver
    {
        void Resolve(RegionModel model);
        void Resolve(BasketCountryModel model);
        void Resolve(BasketRegionModel model);
        void Resolve(CountryModel model);
        void Resolve(UnsavedBasketCountryModel model);
        void Resolve(OtherModel model);
    }
}
