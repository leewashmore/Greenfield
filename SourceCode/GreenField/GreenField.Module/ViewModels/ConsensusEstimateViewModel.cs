using System;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(ConsensusEstimateViewModel))]
    public class ConsensusEstimateViewModel: NotificationObject
    {
        [Import]
        public IDBInteractivity _dbInteractivity { get; set; }

        public ILoggerFacade _logger;

        [ImportingConstructor]
        public ConsensusEstimateViewModel(ILoggerFacade logger)
        {
            _logger = logger;
        }

        private String _message;
        public String Message
        {
            get 
            {
                if (String.IsNullOrEmpty(_message))
                    _dbInteractivity.GetMessage(RetrieveMessageCompleted);
                return _message;
            }
            set
            {
                _message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private List<ConsensusEstimates_Result> _consensusEstimates = new List<ConsensusEstimates_Result>();
        public List<ConsensusEstimates_Result> ConsensusEstimates
        {
            get
            {
                if (_consensusEstimates.Count == 0)
                    RetrieveConsensusEstimates(null,null);
                return _consensusEstimates;
            }
            set
            {
                _consensusEstimates = value;
                this.RaisePropertyChanged("ConsensusEstimates");
            }
        }        

        private List<GetCompanies_Result> _companyList = new List<GetCompanies_Result>();
        public List<GetCompanies_Result> CompanyList
        {
            get
            {
                if (_companyList.Count == 0)
                    _dbInteractivity.RetrieveCompaniesList(RetrieveCompanyListCompleted);
                return _companyList;
            }
            set
            {
                _companyList = value;
                this.RaisePropertyChanged("CompanyList");
            }
        }

        private Dictionary<String, String> _periodType = new Dictionary<String, String>();
        public Dictionary<String, String> PeriodType
        {
            get
            {
                if (_periodType.Count == 0)
                {
                    _periodType.Add("A", "Annual");
                    _periodType.Add("Q1", "First Quarter");
                    _periodType.Add("Q2", "Second Quarter");
                    _periodType.Add("Q3", "Third Quarter");
                    _periodType.Add("Q4", "Fourth Quarter");                    
                }
                return _periodType;
            }
            set
            {
                _periodType = value;
                this.RaisePropertyChanged("PeriodType");
            }
        }

        private List<String> _estimateType = new List<String>();
        public List<String> EstimateType
        {
            get
            {
                if (_estimateType.Count == 0)
                {
                    _estimateType.Add("BVPS");
                    _estimateType.Add("CAPEX");
                    _estimateType.Add("CFPS");
                    _estimateType.Add("DIV");
                    _estimateType.Add("EBIT");
                    _estimateType.Add("EBITDA");
                    _estimateType.Add("EPS");
                    _estimateType.Add("EPSREP");
                    _estimateType.Add("NAV");
                    _estimateType.Add("NDEBT");
                    _estimateType.Add("NTP");
                    _estimateType.Add("NTPREP");
                    _estimateType.Add("OPP");
                    _estimateType.Add("PTP");
                    _estimateType.Add("PTPREP");
                    _estimateType.Add("REVENUE");
                    _estimateType.Add("ROA");
                    _estimateType.Add("ROE");
                }
                return _estimateType;
            }
            set
            {
                _estimateType = value;
                this.RaisePropertyChanged("EstimateType");
            }
        }

        private String _selectedCompany;
        public String SelectedCompany
        {
            get
            {
                return _selectedCompany;
            }
            set
            {
                _selectedCompany = value;
                RetrieveConsensusEstimates(SelectedCompany, SelectedPeriodType);
                this.RaisePropertyChanged("SelectedCompany");
            }
        }

        private String _selectedPeriodType;
        public String SelectedPeriodType
        {
            get
            {
                return _selectedPeriodType;
            }
            set
            {
                _selectedPeriodType = value;
                RetrieveConsensusEstimates(SelectedCompany, SelectedPeriodType);
                this.RaisePropertyChanged("SelectedPeriodType");
            }
        }
       

        public void RetrieveConsensusEstimates(String companyName, String periodType)
        {
            if (!String.IsNullOrEmpty(companyName) && !String.IsNullOrEmpty(periodType))
                _dbInteractivity.RetrieveConsensusEstimates(companyName, periodType,RetrieveConsensusEstimatesCompleted);
        }

        public void RetrieveMessageCompleted(String result)
        {
            Message = result;
        }

        public void RetrieveConsensusEstimatesCompleted(List<ConsensusEstimates_Result> result)
        {
            ConsensusEstimates = result;
        }

        public void RetrieveCompanyListCompleted(List<GetCompanies_Result> result)
        {
            CompanyList = result;
        }

        private DelegateCommand _saveCommand; 
        public DelegateCommand SaveCommand
        { 
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new DelegateCommand(OnCapture, CaptureCanExecute);

                return _saveCommand;
            } 
        }

        private void OnCapture(object parameter)
        {
            Message = "Save Buton Clicked";
        }
        
        private bool CaptureCanExecute(object parameter)
        {    
            return true;
        }      
    }
}
