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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{

    /// <summary>
    /// view modelclass for SecurityOverview
    /// </summary>
    public class ViewModelSecurityOverview : NotificationObject
    {
        #region Fields
        //MEF Singletons
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private EntitySelectionData _entitySelectionData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">MEF Eventaggregator instance</param>
        public ViewModelSecurityOverview(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

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
        /// Content displayed on the busy indicator
        /// </summary>
        private string _busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return _busyIndicatorContent; }
            set 
            {
                _busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }        

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
        /// PrimaryAnalyst Property
        /// </summary>
        private string _primaryAnalyst;
        public string PrimaryAnalyst
        {
            get { return _primaryAnalyst; }
            set
            {
                if (_primaryAnalyst != value)
                    _primaryAnalyst = value;
                RaisePropertyChanged(() => this.PrimaryAnalyst);
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
        /// <param name="entitySelectionData">EntitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _dbInteractivity.RetrieveSecurityOverviewData(entitySelectionData, RetrieveSecurityReferenceDataCallBackMethod);
                    BusyIndicatorContent = "Retrieving security reference data for '" + entitySelectionData.LongName + " (" + entitySelectionData.ShortName + ")'";
                    if (SecurityOverviewDataLoadEvent != null)
                        SecurityOverviewDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
                if (SecurityOverviewDataLoadEvent != null)
                    SecurityOverviewDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Event
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler SecurityOverviewDataLoadEvent;
        #endregion

        #region CallBack Method
        /// <summary>
        /// Callback method for Security Overview Service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="securityOverviewData">SecurityOverviewData Collection</param>
        private void RetrieveSecurityReferenceDataCallBackMethod(SecurityOverviewData securityOverviewData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (securityOverviewData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, securityOverviewData, 1);
                    this.IssueName = securityOverviewData.IssueName;
                    this.Ticker = securityOverviewData.Ticker;
                    this.Country = securityOverviewData.Country;
                    this.Sector = securityOverviewData.Sector;
                    this.Industry = securityOverviewData.Industry;
                    this.SubIndustry = securityOverviewData.SubIndustry;
                    this.PrimaryAnalyst = securityOverviewData.PrimaryAnalyst;
                    this.Currency = securityOverviewData.Currency;
                    this.FiscalYearEnd = securityOverviewData.FiscalYearEnd;
                    this.Website = securityOverviewData.Website;
                    this.Description = securityOverviewData.Description;
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
            if (SecurityOverviewDataLoadEvent != null)
                SecurityOverviewDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
        }

        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }

        #endregion
    }
}
