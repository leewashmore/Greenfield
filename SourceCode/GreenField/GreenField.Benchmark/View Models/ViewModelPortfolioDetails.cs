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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.Generic;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;

namespace GreenField.Benchmark.ViewModels
{
    [Export(typeof(ViewModelPortfolioDetails))]
    public class ViewModelPortfolioDetails : NotificationObject
    {
        #region PrivateFields

        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbInteractivity">Instance of service caller class</param>
        /// <param name="eventAggregator"></param>
        /// <param name="logger">Instance of LoggerFacade</param>
        [ImportingConstructor]
        public ViewModelPortfolioDetails(IDBInteractivity dbInteractivity, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            this._dbInteractivity = dbInteractivity;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            RetrievePortfolioDetailsData(SelectedPortfolioName);
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// The Portfolio selected from the top menu Portfolio Selector Control
        /// </summary>
        private string _selectedPortfolioName;
        public string SelectedPortfolioName
        {
            get
            {
                return _selectedPortfolioName;
            }
            set
            {
                _selectedPortfolioName = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolioDetailsData);
            }
        }

        /// <summary>
        /// Collection Containing the Data to be shown in the Grid
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> _selectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>();
        public RangeObservableCollection<PortfolioDetailsData> SelectedPortfolioDetailsData
        {
            get
            {
                return _selectedPortfolioDetailsData;
            }
            set
            {
                _selectedPortfolioDetailsData = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolioDetailsData);
            }
        }

        /// <summary>
        /// Collection of all Benchmark Names
        /// </summary>
        private ObservableCollection<string> _benchmarkNamesData;
        public ObservableCollection<string> BenchmarkNamesData
        {
            get
            {
                return _benchmarkNamesData;
            }
            set
            {
                _benchmarkNamesData = value;
                this.RaisePropertyChanged(() => this.BenchmarkNamesData);
            }
        }

        /// <summary>
        /// Property Bind to the BenchmarkSelectionComboBox
        /// </summary>
        private string _selectedBenchmark;
        public string SelectedBenchmark
        {
            get
            {
                return _selectedBenchmark;
            }
            set
            {
                _selectedBenchmark = value;
                this.RaisePropertyChanged(() => this.SelectedBenchmark);
            }
        }

        #endregion

        #region CallbackMethods

        /// <summary>
        /// CallBack Method for Retrieving Portfolio Names
        /// </summary>
        /// <param name="result"></param>
        private void RetrievePortfolioDetailsDataCallbackMethod(List<PortfolioDetailsData> result)
        {
            if (result != null)
            {
                SelectedPortfolioDetailsData.AddRange(result);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Service call to Retrieve the Details for a Portfolio
        /// </summary>
        /// <param name="objPortfolioName">PortfolioName</param>
        private void RetrievePortfolioDetailsData(string objPortfolioName)
        {
            _dbInteractivity.RetrievePortfolioDetailsData(objPortfolioName, RetrievePortfolioDetailsDataCallbackMethod);
        }

        #endregion
    }
}