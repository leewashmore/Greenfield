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
using Aims.Data.Client;

namespace TopDown.FacingServer.Backend.Targeting
{
    public partial class CountryModel : ICountry
    {
        public CountryModel()
        {
        }

        public CountryModel(String name, String isoCode)
            : this()
        {
            this.Name = name;
            this.IsoCode = isoCode;
        }
    }
}
