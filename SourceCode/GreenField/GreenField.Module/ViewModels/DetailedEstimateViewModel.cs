using System;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(DetailedEstimateViewModel))]
    public class DetailedEstimateViewModel: NotificationObject
    {
        [Import]
        public IDBInteractivity _dbInteractivity { get; set; }
       
        public DetailedEstimateViewModel()
        {
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

        private List<DetailedEstimates_Result> _detailedEstimates = new List<DetailedEstimates_Result>();
        public List<DetailedEstimates_Result> DetailedEstimates
        {
            get
            {
                if (_detailedEstimates.Count == 0)
                    RetrieveDetailedEstimates(null,null,null);
                return _detailedEstimates;
            }
            set
            {
                _detailedEstimates = value;
                this.RaisePropertyChanged("DetailedEstimates");
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
                RetrieveDetailedEstimates(SelectedCompany, SelectedPeriodType, SelectedEstimateType);
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
                RetrieveDetailedEstimates(SelectedCompany, SelectedPeriodType, SelectedEstimateType);
                this.RaisePropertyChanged("SelectedPeriodType");
            }
        }

        private String _selectedEstimateType;
        public String SelectedEstimateType
        {
            get
            {
                return _selectedEstimateType;
            }
            set
            {
                _selectedEstimateType = value;
                RetrieveDetailedEstimates(SelectedCompany, SelectedPeriodType, SelectedEstimateType);
                this.RaisePropertyChanged("SelectedEstimateType");
            }
        }


        public void RetrieveDetailedEstimates(String companyName, String periodType, String estimateType)
        {
            if (!String.IsNullOrEmpty(companyName) && !String.IsNullOrEmpty(periodType)
                && !String.IsNullOrEmpty(estimateType))
                _dbInteractivity.RetrieveDetailedEstimates(companyName, periodType, estimateType,
                    RetrieveDetailedEstimatesCompleted);
        }

        public void RetrieveMessageCompleted(String result)
        {
            Message = result;
        }

        public void RetrieveDetailedEstimatesCompleted(List<DetailedEstimates_Result> result)
        {
            DetailedEstimates = result;
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
