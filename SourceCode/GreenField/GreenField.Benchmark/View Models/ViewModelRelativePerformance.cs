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
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.Common.Helper;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using System.Linq;
using GreenField.Gadgets.Helpers;


namespace GreenField.Benchmark.ViewModels
{
    [Export]
    public class ViewModelRelativePerformance : NotificationObject
    {
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        private RelativePerformanceData _relativePerformanceData;
        private EntitySelectionData _entitySelectionData;

        [ImportingConstructor]
        public ViewModelRelativePerformance(IDBInteractivity dbInteractivity, ILoggerFacade logger,IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dbInteractivity = dbInteractivity;
            _logger = logger;
            //_dbInteractivity.RetrieveRelativePerformanceData("Dummy Security Name", RetrieveRelativePerformanceDataCallBackMethod);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
            if (_entitySelectionData != null)
                HandleSecurityReferenceSet(_entitySelectionData);
        }

        #region PropertyDeclaration

        private RangeObservableCollection<RelativePerformanceData> _entityRelativePerformanceData =
            new RangeObservableCollection<RelativePerformanceData>();

        public RangeObservableCollection<RelativePerformanceData> EntityRelativePerformanceData
        {
            get
            {
                return _entityRelativePerformanceData;
            }
            set
            {
                _entityRelativePerformanceData = value;
                this.RaisePropertyChanged(() => this.EntityRelativePerformanceData);
            }
        }

        private string _portfolioIdentifier="";

        public string PortfolioIdentifier
        {
            get
            { 
                return _portfolioIdentifier;
            }
            set
            { 
                _portfolioIdentifier = value;
                this.RaisePropertyChanged(() => this.PortfolioIdentifier);
            }
        }
        

        #endregion

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            //ArgumentNullException
            if (entitySelectionData == null)
                return;
            //Fetch the Data for selected Security
            if (EntityRelativePerformanceData.Count == 0)
            {
                //_dbInteractivity.RetrieveRelativePerformanceData(PortfolioIdentifier,entitySelectionData.ShortName, RetrieveRelativePerformanceDataCallBackMethod);
            }
        }

        void RetrieveRelativePerformanceDataCallBackMethod(List<RelativePerformanceData> result)
        {
            EntityRelativePerformanceData.AddRange(result);
        }
    }
}