using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Map;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;

namespace GreenField.Gadgets.Views
{
    public partial class ViewHeatMap : ViewBaseUserControl
    {
        private const string NonDbfDataField = "HugsPerCapita";

        public ViewHeatMap(ViewModelHeatMap dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
        }

        private void MapPreviewReadCompleted(object sender, PreviewReadShapesCompletedEventArgs eventArgs)
        {
            if (eventArgs.Error == null)
            {
                foreach (MapShape shape in eventArgs.Items)
                {
                    this.SetAdditionalData(shape);
                }
            }
        }

        private void SetAdditionalData(MapShape shape)
        {
            ExtendedData extendedData = shape.ExtendedData;
            if (extendedData != null)
            {
                // add new property to ExtendedData   
                if (!extendedData.PropertySet.ContainsKey(NonDbfDataField))
                {
                    extendedData.PropertySet.RegisterProperty(NonDbfDataField, "HugsPerCapita", typeof(int), 0);
                }

                string country = (string)shape.ExtendedData.GetValue("ISO_2DIGIT");
                int additionalFieldValue = this.GetHugsByCountry(country);

                // assign value to new property   
                shape.ExtendedData.SetValue(NonDbfDataField, additionalFieldValue);

            }
        }

        private int GetHugsByCountry(string stateName)
        {
            switch (stateName)
            {
                case "RU":
                    return 43;
                case "IN":
                    return 50;
                default:
                    return 0;
            }
        }
    }
}
