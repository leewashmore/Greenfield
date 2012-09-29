using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view model class for SecurityOverview
    /// </summary>
    public class ViewModelSecurityOverview : NotificationObject
    {
        #region Fields
        //MEF singletons
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private EntitySelectionData entitySelectionParam;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (isActive && entitySelectionParam != null)
                    {
                        HandleSecurityReferenceSet(entitySelectionParam);
                    }
                }
            }
        }
        #endregion

        #region Constructor
       /// <summary>
        /// constructor
       /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelSecurityOverview(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            entitySelectionParam = param.DashboardGadgetPayload.EntitySelectionData;

            //subscription to SecurityReferenceSet event
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);

            //EntitySelectionData handling
            if (entitySelectionParam != null)
            {
                HandleSecurityReferenceSet(entitySelectionParam);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// content displayed on the busy indicator
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set 
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }

        /// <summary>
        /// property to contain status value for busy indicator of the gadget
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get { return busyIndicatorStatus; }
            set
            {
                if (busyIndicatorStatus != value)
                {
                    busyIndicatorStatus = value;
                    RaisePropertyChanged(() => BusyIndicatorStatus);
                }
            }
        }

        /// <summary>
        /// issue name property
        /// </summary>
        private string issueName;
        public string IssueName
        {
            get { return issueName; }
            set
            {
                if (issueName != value)
                    issueName = value;
                RaisePropertyChanged(() => this.IssueName);
            }
        }

        /// <summary>
        /// ticker property
        /// </summary>
        private string ticker;
        public string Ticker
        {
            get { return ticker; }
            set
            {
                if (ticker != value)
                    ticker = value;
                RaisePropertyChanged(() => this.Ticker);
            }
        }

        /// <summary>
        /// country property
        /// </summary>
        private string country;
        public string Country
        {
            get { return country; }
            set
            {
                if (country != value)
                    country = value;
                RaisePropertyChanged(() => this.Country);
            }
        }

        /// <summary>
        /// sector property
        /// </summary>
        private string sector;
        public string Sector
        {
            get { return sector; }
            set
            {
                if (sector != value)
                    sector = value;
                RaisePropertyChanged(() => this.Sector);
            }
        }

        /// <summary>
        /// industry property
        /// </summary>
        private string industry;
        public string Industry
        {
            get { return industry; }
            set
            {
                if (industry != value)
                    industry = value;
                RaisePropertyChanged(() => this.Industry);
            }
        }

        /// <summary>
        /// subindustry property
        /// </summary>
        private string subIndustry;
        public string SubIndustry
        {
            get { return subIndustry; }
            set
            {
                if (subIndustry != value)
                    subIndustry = value;
                RaisePropertyChanged(() => this.SubIndustry);
            }
        }

        /// <summary>
        /// primary analyst property
        /// </summary>
        private string primaryAnalyst;
        public string PrimaryAnalyst
        {
            get { return primaryAnalyst; }
            set
            {
                if (primaryAnalyst != value)
                    primaryAnalyst = value;
                RaisePropertyChanged(() => this.PrimaryAnalyst);
            }
        }

        /// <summary>
        /// currency property
        /// </summary>
        private string currency;
        public string Currency
        {
            get { return currency; }
            set
            {
                if (currency != value)
                    currency = value;
                RaisePropertyChanged(() => this.Currency);
            }
        }

        /// <summary>
        /// fiscal year end property
        /// </summary>
        private string fiscalYearEnd;
        public string FiscalYearend
        {
            get { return fiscalYearEnd; }
            set
            {
                if (fiscalYearEnd != value)
                    fiscalYearEnd = value;
                RaisePropertyChanged(() => this.FiscalYearend);
            }
        }

        /// <summary>
        /// website property
        /// </summary>
        private string website;
        public string Website
        {
            get { return website; }
            set
            {
                if (website != value)
                    website = value;
                RaisePropertyChanged(() => this.Website);
            }
        }

        /// <summary>
        /// description property
        /// </summary>
        private string description;
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                    description = value;
                RaisePropertyChanged(() => this.Description);
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// assigns UI field properties based on Entity Selection Data
        /// </summary>
        /// <param name="entitySelectionData">EntitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    entitySelectionParam = entitySelectionData;
                   
                    if (IsActive && entitySelectionParam != null)
                    {
                        dbInteractivity.RetrieveSecurityOverviewData(entitySelectionParam, RetrieveSecurityReferenceDataCallBackMethod);
                        BusyIndicatorContent = "Retrieving security reference data for '" + entitySelectionData.LongName + " (" + entitySelectionData.ShortName + ")'";
                        BusyIndicatorStatus = true;
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);                
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion             

        #region CallBack Method
        /// <summary>
        /// callback method for Security Overview service call - assigns value to UI field properties
        /// </summary>
        /// <param name="securityOverviewData">SecurityOverviewData Collection</param>
        private void RetrieveSecurityReferenceDataCallBackMethod(SecurityOverviewData securityOverviewData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (securityOverviewData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, securityOverviewData, 1);
                    this.IssueName = securityOverviewData.IssueName;
                    this.Ticker = securityOverviewData.Ticker;
                    this.Country = securityOverviewData.Country;
                    this.Sector = securityOverviewData.Sector;
                    this.Industry = securityOverviewData.Industry;
                    this.SubIndustry = securityOverviewData.SubIndustry;
                    this.PrimaryAnalyst = securityOverviewData.PrimaryAnalyst;
                    this.Currency = securityOverviewData.Currency;
                    this.FiscalYearend = securityOverviewData.FiscalYearend;
                    this.Website = securityOverviewData.Website;
                    this.Description = securityOverviewData.Description;
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }                
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                BusyIndicatorStatus = false;
            }
            Logging.LogEndMethod(logger, methodNamespace);            
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
}
