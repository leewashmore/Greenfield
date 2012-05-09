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
using Microsoft.Practices.Prism.ViewModel;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.ComponentModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelHeatMap : ViewModelBase
    {
        protected const string ShapeRelativeUriFormat = "DataSources/Geospatial/{0}.{1}";
        protected const string ShapeExtension = "shp";
        protected const string DbfExtension = "dbf";

        private string _region;
        private Uri _shapefileSourceUri;
        private Uri _shapefileDataSourceUri;
       
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity = new DBInteractivity();

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;
        private BenchmarkSelectionData _benchmarkSelectionData;
        private DateTime? _effectiveDate;

        public ViewModelHeatMap(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            this.PropertyChanged += this.ShapefileViewModelPropertyChanged;
            _dbInteractivity.RetrieveHeatMapData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveHeatMapDataCallbackMethod);
        }
        public event RetrieveHeatMapDataCompleteEventHandler RetrieveHeatMapDataCompletedEvent;

        public string Region
        {
            get
            {
                return this._region;
            }
            set
            {
                if (this._region != value)
                {
                    this._region = value;
                    this.OnPropertyChanged("Region");
                }
            }
        }

        public Uri ShapefileSourceUri
        {
            get
            {
                return this._shapefileSourceUri;
            }
            set
            {
                if (this._shapefileSourceUri != value)
                {
                    this._shapefileSourceUri = value;
                    this.OnPropertyChanged("ShapefileSourceUri");
                }
            }
        }

        public Uri ShapefileDataSourceUri
        {
            get
            {
                return this._shapefileDataSourceUri;
            }
            set
            {
                if (this._shapefileDataSourceUri != value)
                {
                    this._shapefileDataSourceUri = value;
                    this.OnPropertyChanged("ShapefileDataSourceUri");
                }
            }
        }

        private void ShapefileViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Region")
            {
                this.ShapefileSourceUri = new Uri(string.Format(ShapeRelativeUriFormat, this.Region, ShapeExtension), UriKind.Relative);
                this.ShapefileDataSourceUri = new Uri(string.Format(ShapeRelativeUriFormat, this.Region, DbfExtension), UriKind.Relative);
            }
        }
        private List<HeatMapData> _heatMapInfo;
        public List<HeatMapData> HeatMapInfo
        {
            get { return _heatMapInfo; }
            set
            {
                if (_heatMapInfo != value)
                {
                    _heatMapInfo = value;
                    this.OnPropertyChanged("HeatMapInfo");
                }
            }
        }
        void RetrieveHeatMapDataCallbackMethod(List<HeatMapData> result)
        {
            HeatMapInfo = result;
            RetrieveHeatMapDataCompletedEvent(new RetrieveHeatMapDataCompleteEventArgs() { HeatMapInfo = result });
        }
            
    }
}
