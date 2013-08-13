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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelBasicData : NotificationObject
    {
        #region PRIVATE FIELDS
        //MEF Singletons

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        public ILoggerFacade _logger;
        
        /// <summary>
        /// Private member to store basic data
        /// </summary>
        private BasicData _basicDataInfo;       

        /// <summary>
        /// Private member to store basic data gadget visibilty
        /// </summary>
        private Visibility _basicDataGadgetVisibility = Visibility.Collapsed;
        /// <summary>
        /// Private member to store Selected Security ID
        /// </summary>
        private EntitySelectionData _securitySelectionData = null;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Stores data for Basic data grid
        /// </summary>
        public BasicData BasicDataInfo
        {
            get { return _basicDataInfo; }
            set
            {
                if (_basicDataInfo != value)
                {
                    _basicDataInfo = value;
                    PopulateBasicDataDictionary();
                    RaisePropertyChanged(() => this.BasicDataInfo);
                }
            }
        }
       
        /// <summary>
        /// Sets the visibility for Basic Data Gadget
        /// </summary>
        public Visibility BasicDataGadgetVisibility
        {
            get { return _basicDataGadgetVisibility; }
            set
            {
                _basicDataGadgetVisibility = value;
                RaisePropertyChanged(() => this.BasicDataGadgetVisibility);
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                CallingWebMethod();
            }
        }

        /// <summary>
        /// Holds the Basic Data points and corresponding values
        /// </summary>
        private List<KeyValuePair<String, String>> basicDataDataList;
        public List<KeyValuePair<String, String>> BasicDataDataList
        {
            get
            {
                if (basicDataDataList == null)
                {
                    basicDataDataList = new List<KeyValuePair<string,string>>();
                }                
                return basicDataDataList;
            }
            set
            {
                basicDataDataList = value;
                RaisePropertyChanged(() => this.BasicDataDataList);
            }
        }
        #endregion

        #region CONSTRUCTOR
        public ViewModelBasicData(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
            CallingWebMethod();
            
        }

        #endregion

        #region EVENTS
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler BasicDataLoadEvent;

        #endregion

        #region EVENTHANDLERS
        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _securitySelectionData = entitySelectionData;

                    CallingWebMethod();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }
        #endregion

        #region CALLBACK METHOD
        /// <summary>
        /// Callback method that assigns value to the BAsicDataInfo property
        /// </summary>
        /// <param name="result">basic data </param>
        private void RetrieveBasicDataCallbackMethod(List<BasicData> basicData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                BasicDataInfo = null;
                BasicDataGadgetVisibility = Visibility.Collapsed;
                if (basicData != null && basicData.Count > 0)
                {
                    BasicDataGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(_logger, methodNamespace, basicData, 1);
                    BasicDataInfo = basicData.FirstOrDefault();
                    this.RaisePropertyChanged(() => this.BasicDataInfo);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (BasicDataLoadEvent != null)
                    BasicDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                BasicDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region Web Method Call
        private void CallingWebMethod()
        {
            if (_securitySelectionData != null && IsActive)
            {
                if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                {
                    if (BasicDataLoadEvent != null)
                        BasicDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveBasicData(_securitySelectionData, RetrieveBasicDataCallbackMethod);
                }
            }
        }
        #endregion

        #region EventUnSubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion

        #region private methods
        private void PopulateBasicDataDictionary()
        {
            if (BasicDataInfo != null)
            {
                Dictionary<String, String> basicDataValues = new Dictionary<string, string>();

                string weekRange52weekLow = BasicDataInfo.WeekRange52Low.HasValue
                    ? ((decimal)(BasicDataInfo.WeekRange52Low)).ToString("N2") : String.Empty;
                string weekRange52weekHigh = BasicDataInfo.WeekRange52High.HasValue
                    ? ((decimal)(BasicDataInfo.WeekRange52High)).ToString("N2") : String.Empty;
                basicDataValues.Add("52 Week Range", String.Format("{0}-{1}",
                    weekRange52weekLow, weekRange52weekHigh));

                string averageVolumne = BasicDataInfo.AverageVolume.HasValue
                    ? ((decimal)(BasicDataInfo.AverageVolume)).ToString("N0") : String.Empty;
                basicDataValues.Add("Average Volume – 6 Month", averageVolumne);

                string marketCapitalization = BasicDataInfo.MarketCapitalization.HasValue
                    ? ((decimal)(BasicDataInfo.MarketCapitalization)).ToString("N0") : String.Empty;
                basicDataValues.Add("Market Capitalization", marketCapitalization);

                string enterpriseValue = BasicDataInfo.EnterpriseValue.HasValue
                    ? ((decimal)(BasicDataInfo.EnterpriseValue)).ToString("N0") : String.Empty;
                basicDataValues.Add("Enterprise Value", enterpriseValue);

                string sharesOutstanding = BasicDataInfo.SharesOutstanding.HasValue
                   ? ((decimal)(BasicDataInfo.SharesOutstanding)).ToString("N0") : String.Empty;
                basicDataValues.Add("Shares Outstanding", sharesOutstanding);

                string betaValue = BasicDataInfo.Beta.HasValue
                   ? ((decimal)(BasicDataInfo.Beta)).ToString("N2") : String.Empty;
                basicDataValues.Add(String.Format("Beta({0})", BasicDataInfo.BetaSource), betaValue);

                BasicDataDataList.Clear();  //values used to be added to bottom of the list making it larger and larger, Added this to clear before adding. 
                BasicDataDataList.AddRange(basicDataValues);
            }
        }
        #endregion

    }
}
