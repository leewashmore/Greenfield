using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpt
{
    /// <summary>
    /// Something common for every model.
    /// </summary>
    public interface IModel
    {
        void Accept(IModelResolver resolver);
    }

    public interface IModelResolver
    {
        void Resolve(BasketCountryModel model);
		void Resolve(BasketRegionModel model);
		void Resolve(CountryModel model);
		void Resolve(OtherModel model);
		void Resolve(RegionModel model);
		void Resolve(UnsavedBasketCountryModel model);
    }
}
