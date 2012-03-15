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
using System.Windows.Data;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Gadgets.ViewModels
{

    /// <summary>
    /// view modelclass for SecurityOverview
    /// </summary>
    public class ViewModelSecurityOverview : NotificationObject
    {
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private EntitySelectionData _entitySelectionData;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">MEF Eventaggrigator instance</param>
        public ViewModelSecurityOverview(DashBoardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _entitySelectionData = param.DashboardGadgetPayLoad.EntitySelectionData;

            //Subscription to SecurityReferenceSet event
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            
            //EntitySelectionData handling
            if (_entitySelectionData != null)
            {
                HandleSecurityReferenceSet(_entitySelectionData);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// IssueName Property
        /// </summary>
        private string _issueName;
        public string IssueName
        {
            get { return _issueName; }
            set
            {
                if (_issueName != value)
                    _issueName = value;
                RaisePropertyChanged(() => this.IssueName);
            }
        }

        /// <summary>
        /// Ticker Property
        /// </summary>
        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set
            {
                if (_ticker != value)
                    _ticker = value;
                RaisePropertyChanged(() => this.Ticker);
            }
        }

        /// <summary>
        /// Country Property
        /// </summary>
        private string _country;
        public string Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                    _country = value;
                RaisePropertyChanged(() => this.Country);
            }
        }

        /// <summary>
        /// Sector Property
        /// </summary>
        private string _sector;
        public string Sector
        {
            get { return _sector; }
            set
            {
                if (_sector != value)
                    _sector = value;
                RaisePropertyChanged(() => this.Sector);
            }
        }

        /// <summary>
        /// Industry Property
        /// </summary>
        private string _industry;
        public string Industry
        {
            get { return _industry; }
            set
            {
                if (_industry != value)
                    _industry = value;
                RaisePropertyChanged(() => this.Industry);
            }
        }

        /// <summary>
        /// SubIndustry Property
        /// </summary>
        private string _subIndustry;
        public string SubIndustry
        {
            get { return _subIndustry; }
            set
            {
                if (_subIndustry != value)
                    _subIndustry = value;
                RaisePropertyChanged(() => this.SubIndustry);
            }
        }

        /// <summary>
        /// Exchange Property
        /// </summary>
        private string _exchange;
        public string Exchange
        {
            get { return _exchange; }
            set
            {
                if (_exchange != value)
                    _exchange = value;
                RaisePropertyChanged(() => this.Exchange);
            }
        }

        /// <summary>
        /// Currency Property
        /// </summary>
        private string _currency;
        public string Currency
        {
            get { return _currency; }
            set
            {
                if (_currency != value)
                    _currency = value;
                RaisePropertyChanged(() => this.Currency);
            }
        }

        /// <summary>
        /// FiscalYearEnd Property
        /// </summary>
        private string _fiscalYearEnd;
        public string FiscalYearEnd
        {
            get { return _fiscalYearEnd; }
            set
            {
                if (_fiscalYearEnd != value)
                    _fiscalYearEnd = value;
                RaisePropertyChanged(() => this.FiscalYearEnd);
            }
        }

        /// <summary>
        /// Website Property
        /// </summary>
        private string _website;
        public string Website
        {
            get { return _website; }
            set
            {
                if (_website != value)
                    _website = value;
                RaisePropertyChanged(() => this.Website);
            }
        }

        /// <summary>
        /// Description Property
        /// </summary>
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                    _description = value;
                RaisePropertyChanged(() => this.Description);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Entity Selection Data
        /// </summary>
        /// <param name="securityReferenceData">EntitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _dbInteractivity.RetrieveSecurityReferenceDataByTicker(entitySelectionData.ShortName, RetrieveSecurityReferenceDataCallBackMethod);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion


        private void RetrieveSecurityReferenceDataCallBackMethod(SecurityReferenceData securityReferenceData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (securityReferenceData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, securityReferenceData, 1);
                    this.IssueName = securityReferenceData.IssueName;
                    this.Ticker = securityReferenceData.Ticker;
                    this.Country = securityReferenceData.Country;
                    this.Sector = securityReferenceData.Sector;
                    this.Industry = securityReferenceData.Industry;
                    this.SubIndustry = securityReferenceData.SubIndustry;
                    this.Exchange = securityReferenceData.Exchange;
                    this.Currency = securityReferenceData.Currency;
                    this.FiscalYearEnd = securityReferenceData.FiscalYearEnd;
                    this.Website = securityReferenceData.Website;
                    this.Description = securityReferenceData.Description; 
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
    }
}
