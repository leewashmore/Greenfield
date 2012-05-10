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
       private const string COUNTRY_PERFORMANCE_FIELD = "CountryPerformance";
       private const string COUNTRY_YTD_FIELD = "CountryYTD";
       private List<HeatMapData> _heatMapInfo;
       private List<MapShape> _shapes = new List<MapShape>(); 

       public ViewHeatMap(ViewModelHeatMap dataContextSource)
       {
           InitializeComponent();
           this.DataContext = dataContextSource;
           this.DataContextHeatMap = dataContextSource;
           dataContextSource.RetrieveHeatMapDataCompletedEvent += new RetrieveHeatMapDataCompleteEventHandler(dataContextSource_RetrieveHeatMapDataCompletedEvent);
           dataContextSource.heatMapDataLoadedEvent +=
           new DataRetrievalProgressIndicatorEventHandler(dataContextSource_heatMapDataLoadedEvent);
       }

       /// <summary>
       /// Property of the type of View Model for this view
       /// </summary>
       private ViewModelHeatMap _dataContextHeatMap;
       public ViewModelHeatMap DataContextHeatMap
       {
           get { return _dataContextHeatMap; }
           set { _dataContextHeatMap = value; }
       }

         private void dataContextSource_RetrieveHeatMapDataCompletedEvent(Common.RetrieveHeatMapDataCompleteEventArgs e)
        {
            _heatMapInfo = e.HeatMapInfo;

            if (_heatMapInfo != null)
            {
                foreach (MapShape _shape in _shapes)
                {
                    string countryID = (string)_shape.ExtendedData.GetValue("ISO_2DIGIT");

                    HeatMapData countryRecord = _heatMapInfo.Where(r => r.CountryID == countryID).FirstOrDefault();
                    if (countryRecord != null)
                    {
                        _shape.ExtendedData.SetValue(COUNTRY_PERFORMANCE_FIELD, (int)(countryRecord.CountryPerformance));
                        _shape.ExtendedData.SetValue(COUNTRY_YTD_FIELD,countryRecord.CountryYTD);
                    }
                }
            }
            //AddColorizerToInformationLayer();
        }

          //private void AddColorizerToInformationLayer()
         // {
         //     this.informationLayer.Colorizer.ExtendedPropertyName = "COUNTRY_PERFORMANCE_FIELD";             
         //     ColorMeasureScale scale = new ColorMeasureScale();
         //     //MapShapeFillCollection msc = new MapShapeFillCollection();
            
         //     //scale.RangeCollection[0].MaxValue = 10;
         //     //scale.sha
         //     //scale.RangeCollection[scale.RangeCollection.Count - 2].MaxValue = scale.RangeCollection.Last().MaxValue;
         //     //scale.RangeCollection.Remove(scale.RangeCollection.Last()); 
              

         //}


        private void MapPreviewReadCompleted(object sender, PreviewReadShapesCompletedEventArgs eventArgs)
        {
            if (eventArgs.Error == null)
            {
                foreach (MapShape shape in eventArgs.Items)
                {
                    this.SetAdditionalData(shape);
                }
            }

            _heatMapInfo = ((ViewModelHeatMap)this.DataContext).HeatMapInfo;
        }

        private void SetAdditionalData(MapShape shape)
        {
            ExtendedData extendedData = shape.ExtendedData;
            if (extendedData != null)
            {
                _shapes.Add(shape);               
               
                if (!extendedData.PropertySet.ContainsKey(COUNTRY_PERFORMANCE_FIELD))
                {
                    extendedData.PropertySet.RegisterProperty(COUNTRY_PERFORMANCE_FIELD, "CountryPerformance", typeof(int), 0);
                }
                if (!extendedData.PropertySet.ContainsKey(COUNTRY_YTD_FIELD))
                {
                    extendedData.PropertySet.RegisterProperty(COUNTRY_YTD_FIELD, "CountryYTD", typeof(Decimal), Convert.ToDecimal(0));
                }              
            }
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_heatMapDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorMap.IsBusy = true;
            }
            else
            {
                this.busyIndicatorMap.IsBusy = false;
            }
        }

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextHeatMap.heatMapDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_heatMapDataLoadedEvent);
            this.DataContextHeatMap.Dispose();
            this.DataContextHeatMap = null;
            this.DataContext = null;
        }
        #endregion

    }
}
