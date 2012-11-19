using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using GreenField.Targeting.Only.Backend.Targeting;

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    public class NameCellStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style BasketRegionStyle { get; set; }
        public Style BasketCountryStyle { get; set; }
        public Style OtherStyle { get; set; }
        public Style RegionStyle { get; set; }
        public Style UnsavedBasketCountryStyle { get; set; }
        public Style CountryStyle { get; set; }

        public override Style SelectStyle(Object item, DependencyObject container)
        {
            var resident = item as IGlobeResident;
            if (resident == null) return this.DefaultStyle;
            var resolver = new SelectStyle_IGlobeResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class SelectStyle_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private NameCellStyleSelector selector;

            public SelectStyle_IGlobeResidentResolver(NameCellStyleSelector selector)
            {
                this.selector = selector;
            }

            public Style Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.selector.BasketCountryStyle;
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.selector.BasketRegionStyle;
            }

            public void Resolve(OtherModel model)
            {
                this.Result = this.selector.OtherStyle;
            }

            public void Resolve(RegionModel model)
            {
                this.Result = this.selector.RegionStyle;
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.Result = this.selector.UnsavedBasketCountryStyle;
            }

            public void Resolve(BgaCountryModel model)
            {
                this.Result = this.selector.CountryStyle;
            }

            
        }


        
    }
}
