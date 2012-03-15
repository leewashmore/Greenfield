using System;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(AggregatedDataViewModel))]
    public class AggregatedDataViewModel : NotificationObject
    {
        [Import]
        public IDBInteractivity _dbInteractivity { get; set; }

        public ILoggerFacade _logger;

        [ImportingConstructor]
        public AggregatedDataViewModel(ILoggerFacade logger)
        {
            _logger = logger;
        }

        private String _message;
        public String Message
        {
            get
            {
                if (String.IsNullOrEmpty(_message))
                    _message = "In Aggregated Data View";
                return _message;
            }
            set
            {
                _message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private List<string> aggregateViewList = new List<String>();
        public List<String> AggregateViewList
        {
            get
            {
                if (aggregateViewList.Count == 0)
                    _dbInteractivity.RetrieveAggregateDataListView(RetrieveAggregateDataListViewCompleted);
                return aggregateViewList;
            }
            set
            {
                aggregateViewList = value;
                this.RaisePropertyChanged("AggregateViewList");
            }
        }

        private List<string> _portfolioNames;// = new List<String>();        
        public List<String> PortfolioNames
        {
            get
            {
                if (_portfolioNames == null)
                {
                    _portfolioNames = new List<string>();
                    _dbInteractivity.RetrievePortfolioNames("U_POS_EXP_BASEVIEW", SetPortFolioCombo);
                }
                return _portfolioNames;
            }
            set
            {
                _portfolioNames = value;
                this.RaisePropertyChanged("PortfolioNames");
            }
        }

        private void SetPortFolioCombo(List<string> list)
        {
            PortfolioNames = list;
            //_logger.Log("Portfolio Names received", Category.Debug, Priority.None);
            _logger.Log("Portfolio Names received", Category.Info, Priority.None);
            //_logger.Log("Portfolio Names received", Category.Exception, Priority.Low);
            //_logger.Log("Portfolio Names received", Category.Exception, Priority.Medium);
            //_logger.Log("Portfolio Names received", Category.Exception, Priority.High);

        }

        private String _selectedView;
        public String SelectedView
        {
            get
            {
                return _selectedView;
            }
            set
            {
                _selectedView = value;
                this.RaisePropertyChanged("SelectedView");



                FillAggregateDataGrid();

            }
        }

        private List<AggregatedData> __aggregateDataList = new List<AggregatedData>();
        public List<AggregatedData> AggregateDataList
        {
            get
            {
                return __aggregateDataList;
            }
            set
            {
                __aggregateDataList = value;
                this.RaisePropertyChanged("AggregateDataList");
            }
        }

        public void RetrieveAggregateDataListViewCompleted(List<String> result)
        {
            if (result.Count > 0)
                AggregateViewList = result;
        }

        public void FillAggregateDataGrid()
        {
            if (!String.IsNullOrEmpty(SelectedView))
                _dbInteractivity.RetrieveAggregateDataForSelectedView(SelectedView, RetrieveAggregateDataForSelectedViewCompleted);
        }

        public void RetrieveAggregateDataForSelectedViewCompleted(List<AggregatedData> result)
        {
            AggregateDataList = result;
        }

    }
}
