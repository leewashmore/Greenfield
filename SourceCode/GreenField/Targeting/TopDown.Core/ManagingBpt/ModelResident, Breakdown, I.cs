using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpt
{
    public interface IGlobeResident : IModel
    {
        void Accept(IGlobeResidentResolver resolver);
    }

    public interface IGlobeResidentResolver
    {
        void Resolve(RegionModel model);
        void Resolve(BasketRegionModel model);
        void Resolve(OtherModel model);
    }
}
