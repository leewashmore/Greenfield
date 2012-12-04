using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpt
{
    public static class ModelExtender
    {
        public static OtherModel TryAsOther(this IModel model)
        {
            var resolver = new TryAs_IModelResolver();
            model.Accept(resolver);
            return resolver.OtherModelOpt;
        }

        private class TryAs_IModelResolver : IModelResolver
        {
            public void Resolve(BasketCountryModel model)
            {
                
            }

            public void Resolve(BasketRegionModel model)
            {
                
            }

            public void Resolve(CountryModel model)
            {
                
            }

            public OtherModel OtherModelOpt { get; private set; }
            public void Resolve(OtherModel model)
            {
                this.OtherModelOpt = model;
            }

            public void Resolve(RegionModel model)
            {
                
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                
            }
        }

    }
}
