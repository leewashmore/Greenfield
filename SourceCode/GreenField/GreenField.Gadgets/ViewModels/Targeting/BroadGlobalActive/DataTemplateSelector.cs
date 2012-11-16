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
using System.Windows.Data;
using System.Globalization;
using GreenField.ServiceCaller.TargetingDefinitions;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.ViewModels.Targeting.BroadGlobalActive
{
    public class NameCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BasketRegionTemplate { get; set; }
        public DataTemplate BasketCountryTemplate { get; set; }
        public DataTemplate RegionTemplate { get; set; }
        public DataTemplate CountryTemplate { get; set; }
        public DataTemplate OtherTemplate { get; set; }
        public DataTemplate UnsavedBasketCountryTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            var resident = item as IGlobeResident;
            if (resident == null) return this.DefaultTemplate;
            var resolver = new SelectTemplate_IGlobeResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class SelectTemplate_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private NameCellTemplateSelector selector;

            public SelectTemplate_IGlobeResidentResolver(NameCellTemplateSelector selector)
            {
                this.selector = selector;
            }

            public DataTemplate Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.selector.BasketCountryTemplate;
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.selector.BasketRegionTemplate;
            }

            public void Resolve(OtherModel model)
            {
                this.Result = this.selector.OtherTemplate;
            }

            public void Resolve(RegionModel model)
            {
                this.Result = this.selector.RegionTemplate;
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.Result = this.selector.UnsavedBasketCountryTemplate;
            }

            public void Resolve(BgaCountryModel model)
            {
                this.Result = this.selector.CountryTemplate;
            }
        }
    }
}
