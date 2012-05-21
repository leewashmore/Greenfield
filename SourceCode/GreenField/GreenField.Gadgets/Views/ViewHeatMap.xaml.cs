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
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Class for the Heat Map View
    /// </summary>
    public partial class ViewHeatMap : ViewBaseUserControl

    {
      /// <summary>
      /// Constant String for country performance
      /// </summary>
       private const string COUNTRY_PERFORMANCE_FIELD = "CountryPerformance";
        /// <summary>
       ///  Constant String for country YTD
        /// </summary>
       private const string COUNTRY_YTD_FIELD = "CountryYTD";
        /// <summary>
        /// Private Collection of type Heat Map Data
        /// </summary>
       private List<HeatMapData> _heatMapInfo;
        /// <summary>
        /// Private Collection of type Map Shape
        /// </summary>
       private List<MapShape> _shapes = new List<MapShape>();
       #region Constructor
       /// <summary>
       /// Constructor
        /// </summary>
       /// <param name="dataContextSource">ViewModelHeatMap as Data context for this View</param>
       public ViewHeatMap(ViewModelHeatMap dataContextSource)
       {
           InitializeComponent();
           this.DataContext = dataContextSource;
           this.DataContextHeatMap = dataContextSource;
           dataContextSource.RetrieveHeatMapDataCompletedEvent += new RetrieveHeatMapDataCompleteEventHandler(dataContextSource_RetrieveHeatMapDataCompletedEvent);
           dataContextSource.heatMapDataLoadedEvent +=
           new DataRetrievalProgressIndicatorEventHandler(dataContextSource_heatMapDataLoadedEvent);
       }
       #endregion

       /// <summary>
       /// Property of the type of View Model for this view
       /// </summary>
       private ViewModelHeatMap _dataContextHeatMap;
       public ViewModelHeatMap DataContextHeatMap
       {
           get { return _dataContextHeatMap; }
           set { _dataContextHeatMap = value; }
       }
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
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
                        _shape.ExtendedData.SetValue(COUNTRY_YTD_FIELD, countryRecord.CountryYTD);
                         AddColorizerToInformationLayer(_shape, countryRecord);
                    }
                  
                }
            }
          
        }
        /// <summary>
        /// Adding Colour to Each Shape
        /// </summary>
        /// <param name="_shape">Shape</param>
        /// <param name="countryRecord">Country record of type heat map data</param>
         private void AddColorizerToInformationLayer(MapShape _shape, HeatMapData countryRecord)
         {
             if ((int)(countryRecord.CountryPerformance) == 3)
                 _shape.Fill = new SolidColorBrush(Colors.Green);
             else
                 if ((int)(countryRecord.CountryPerformance) == 1)
                     _shape.Fill = new SolidColorBrush(Colors.Red);
                 else
                     if ((int)(countryRecord.CountryPerformance) == 2)
                         _shape.Fill = new SolidColorBrush(Colors.Gray);
                     else
                         if ((int)(countryRecord.CountryPerformance) == 0)
                             _shape.Fill = new SolidColorBrush(Colors.White);                     
         }

        /// <summary>
        /// Completed event for Map Preview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
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
        /// <summary>
        /// Registers Properties for Heat Map
        /// </summary>
        /// <param name="shape">Map Shape</param>
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
