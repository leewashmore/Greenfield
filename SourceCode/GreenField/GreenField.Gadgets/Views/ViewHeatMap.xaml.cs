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
        private const string COUNTRY_PERFORMANCE_FIELD = "CountryPerformance";
        private const string COUNTRY_YTD_FIELD = "CountryYTD";
        private List<HeatMapData> _heatMapInfo;

        #region Constructor
        public ViewHeatMap(ViewModelHeatMap dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            
            dataContextSource.RetrieveHeatMapDataCompletedEvent += new RetrieveHeatMapDataCompleteEventHandler(dataContextSource_RetrieveHeatMapDataCompletedEvent);
        } 
        #endregion

        #region Event Handler
        private void dataContextSource_RetrieveHeatMapDataCompletedEvent(Common.RetrieveHeatMapDataCompleteEventArgs e)
        {
            _heatMapInfo = e.HeatMapInfo;
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
        #endregion

        #region Helper Methods
        private void SetAdditionalData(MapShape shape)
        {
            ExtendedData extendedData = shape.ExtendedData;
            if (extendedData != null)
            {
                string countryID = (string)shape.ExtendedData.GetValue("ISO_2DIGIT");

                if (!extendedData.PropertySet.ContainsKey(COUNTRY_PERFORMANCE_FIELD))
                {
                    extendedData.PropertySet.RegisterProperty(COUNTRY_PERFORMANCE_FIELD, COUNTRY_PERFORMANCE_FIELD, typeof(int), (int)PerformanceType.NO_RELATION);
                }

                if (!extendedData.PropertySet.ContainsKey(COUNTRY_YTD_FIELD))
                {
                    extendedData.PropertySet.RegisterProperty(COUNTRY_YTD_FIELD, COUNTRY_YTD_FIELD, typeof(Double?), null);
                }

                if (_heatMapInfo != null)
                {
                    HeatMapData countryRecord = _heatMapInfo.Where(r => r.CountryID == countryID).FirstOrDefault();
                    if (countryRecord != null)
                    {
                        shape.ExtendedData.SetValue(COUNTRY_PERFORMANCE_FIELD, _heatMapInfo.Where(r => r.CountryID == countryID).FirstOrDefault().CountryPerformance);
                        shape.ExtendedData.SetValue(COUNTRY_YTD_FIELD, _heatMapInfo.Where(r => r.CountryID == countryID).FirstOrDefault().CountryYTD);
                    }
                }               

            }
        }         
        #endregion

        private void MapShapeReader_ReadCompleted(object sender, ReadShapesCompletedEventArgs eventArgs)
        {

        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
