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
using System.ComponentModel;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Controls.Map;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;
using GreenField.Common.Helper;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Class for the Heat Map View
    /// </summary>
    public partial class ViewHeatMap : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Constant String for country performance
        /// </summary>
        private const string COUNTRY_PERFORMANCE_FIELD = "CountryPerformance";
        /// <summary>
        ///  Constant String for country YTD
        /// </summary>
        private const string COUNTRY_YTD_FIELD = "CountryYTD";
        /// <summary>
        ///  Constant String for Benchmark YTD
        /// </summary>
        private const string BENCHMARK_YTD_FIELD = "BenchmarkYTD";
        /// <summary>
        /// Private Collection of type Heat Map Data
        /// </summary>
        private List<HeatMapData> heatMapInfo;
        /// <summary>
        /// Private Collection of type Map Shape
        /// </summary>
        private List<MapShape> shapes = new List<MapShape>();
        /// <summary>
        /// Map shape
        /// </summary>
        public MapShape mapShape;
        /// <summary>
        /// Event Aggregator 
        /// </summary>
        private IEventAggregator eventAggregator;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelHeatMap as Data context for this View</param>
        public ViewHeatMap(ViewModelHeatMap dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            eventAggregator = (this.DataContext as ViewModelHeatMap).eventAggregator;
            this.DataContextHeatMap = dataContextSource;
            dataContextSource.RetrieveHeatMapDataCompletedEvent += new RetrieveHeatMapDataCompleteEventHandler(dataContextSource_RetrieveHeatMapDataCompletedEvent);
            dataContextSource.HeatMapDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_heatMapDataLoadedEvent);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelHeatMap dataContextHeatMap;
        public ViewModelHeatMap DataContextHeatMap
        {
            get { return dataContextHeatMap; }
            set { dataContextHeatMap = value; }
        }
        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextHeatMap != null)
                    DataContextHeatMap.IsActive = isActive;
            }
        }
        /// <summary>
        /// Selector Payload from Dashboard
        /// </summary>
        private DashboardGadgetPayload selectorPayload;
        public DashboardGadgetPayload SelectorPayload
        {
            get
            {
                if (selectorPayload == null)
                    selectorPayload = new DashboardGadgetPayload();
                return selectorPayload;
            }
            set { selectorPayload = value; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        private void dataContextSource_RetrieveHeatMapDataCompletedEvent(Common.RetrieveHeatMapDataCompleteEventArgs e)
        {
            heatMapInfo = e.HeatMapInfo;

            if (heatMapInfo != null)
            {
                foreach (MapShape shape in shapes)
                {
                    SetAdditionalData(shape);
                }
            }
        }
        /// <summary>
        /// Adding Colour to Each Shape
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="countryRecord">Country record of type heat map data</param>
        private void AddColorizerToInformationLayer(MapShape shape, HeatMapData countryRecord)
        {
            if ((int)(countryRecord.CountryPerformance) == 3)
            {
                shape.Fill = new SolidColorBrush(Colors.Green);
            }
            else
                if ((int)(countryRecord.CountryPerformance) == 1)
                {
                    shape.Fill = new SolidColorBrush(Colors.Red);
                }
                else
                    if ((int)(countryRecord.CountryPerformance) == 2)
                    {
                        shape.Fill = new SolidColorBrush(Colors.Gray);
                    }
                    else
                        if ((int)(countryRecord.CountryPerformance) == 0)
                        {
                            shape.Fill = new SolidColorBrush(Colors.White);
                        }
        }

        /// <summary>
        /// Adding default Colour to  Shape with no values
        /// </summary>
        /// <param name="shape">Shape</param>         
        private void AddTransparentColorizerToInformationLayer(MapShape shape)
        {
            shape.Fill = new SolidColorBrush(Colors.Transparent);
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
                    ToolTip toolTip = new ToolTip();
                    toolTip.Content = new ExtendedDataWraper() { Data = shape.ExtendedData };
                    toolTip.ContentTemplate = this.Resources["CustomToolTipDataTemplate"] as DataTemplate;
                    ToolTipService.SetToolTip(shape, toolTip);
                    shapes.Add(shape);
                    shape.MouseLeftButtonUp += new MouseButtonEventHandler(shape_MouseLeftButtonUp);
                    this.SetAdditionalData(shape);
                }
            }
            heatMapInfo = ((ViewModelHeatMap)this.DataContext).HeatMapInfo;
        }

        /// <summary>
        /// Registers Properties for Heat Map
        /// </summary>
        /// <param name="shape">Map Shape</param>
        private void SetAdditionalData(MapShape shape)
        {            
            ExtendedData data = shape.ExtendedData;
            if (data != null)
            {
                if (!data.PropertySet.ContainsKey(COUNTRY_PERFORMANCE_FIELD))
                {
                    data.PropertySet.RegisterProperty(COUNTRY_PERFORMANCE_FIELD, "CountryPerformance", typeof(int), 0);
                }
                if (!data.PropertySet.ContainsKey(COUNTRY_YTD_FIELD))
                {
                    data.PropertySet.RegisterProperty(COUNTRY_YTD_FIELD, "CountryYTD", typeof(Decimal), Convert.ToDecimal(0));
                }
                if (!data.PropertySet.ContainsKey(BENCHMARK_YTD_FIELD))
                {
                    data.PropertySet.RegisterProperty(BENCHMARK_YTD_FIELD, "BenchmarkYTD", typeof(Decimal), Convert.ToDecimal(0));
                }

                if (heatMapInfo != null)
                {
                    string countryID = (string)shape.ExtendedData.GetValue("ISO_2DIGIT");
                    HeatMapData countryRecord = heatMapInfo.Where(r => r.CountryID == countryID).FirstOrDefault();

                    if (countryRecord != null)
                    {
                        shape.ExtendedData.SetValue(COUNTRY_PERFORMANCE_FIELD, (int)(countryRecord.CountryPerformance));
                        shape.ExtendedData.SetValue(COUNTRY_YTD_FIELD, countryRecord.CountryYTD);
                        shape.ExtendedData.SetValue(BENCHMARK_YTD_FIELD, countryRecord.BenchmarkYTD);
                        AddColorizerToInformationLayer(shape, countryRecord);
                    }
                    else
                    {
                        shape.ExtendedData.SetValue(COUNTRY_PERFORMANCE_FIELD, null);
                        shape.ExtendedData.SetValue(COUNTRY_YTD_FIELD, null);
                        shape.ExtendedData.SetValue(BENCHMARK_YTD_FIELD, null);
                        AddTransparentColorizerToInformationLayer(shape);
                    }
                }
            }
        }
        #endregion

        #region Event Handlers

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
        /// <summary>
        /// Mouse Button up event binded with the view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapShape shape = sender as MapShape;
            if (shape == null)
            {
                return;
            }
            string country = (string)shape.ExtendedData.GetValue("ISO_2DIGIT");
            SelectorPayload.HeatMapCountryData = country;
            eventAggregator.GetEvent<HeatMapClickEvent>().Publish(SelectorPayload.HeatMapCountryData);
        }
        #endregion

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextHeatMap.HeatMapDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_heatMapDataLoadedEvent);
            this.DataContextHeatMap.Dispose();
            this.DataContextHeatMap = null;
            this.DataContext = null;
        }
        #endregion
    }
}
