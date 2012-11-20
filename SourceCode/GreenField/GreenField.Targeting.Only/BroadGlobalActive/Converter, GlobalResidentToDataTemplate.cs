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
using GreenField.Targeting.Only.Backend.Targeting;

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    public class GlobalResidentToDataTemplateConverter : IValueConverter
    {
        public DataTemplate BasketRegionTemplate { get; set; }
        public DataTemplate BasketCountryTemplate { get; set; }
        public DataTemplate RegionTemplate { get; set; }
        public DataTemplate CountryTemplate { get; set; }
        public DataTemplate OtherTemplate { get; set; }
        public DataTemplate UnsavedBasketCountryTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var residentOpt = value as IGlobeResident;
            if (residentOpt == null) return this.DefaultTemplate;

            var resolver = new Convert_IGlobeResident(this);
            residentOpt.Accept(resolver);
            return resolver.Result ?? this.DefaultTemplate;
        }

        private class Convert_IGlobeResident : IGlobeResidentResolver
        {
            private GlobalResidentToDataTemplateConverter parent;

            public Convert_IGlobeResident(GlobalResidentToDataTemplateConverter parent)
            {
                this.parent = parent;
            }

            public DataTemplate Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.parent.BasketCountryTemplate;
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.parent.BasketRegionTemplate;
            }

            public void Resolve(OtherModel model)
            {
                this.Result = this.parent.OtherTemplate;
            }

            public void Resolve(RegionModel model)
            {
                this.Result = this.parent.RegionTemplate;
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.Result = this.parent.UnsavedBasketCountryTemplate;
            }

            public void Resolve(BgaCountryModel model)
            {
                this.Result = this.parent.CountryTemplate;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
