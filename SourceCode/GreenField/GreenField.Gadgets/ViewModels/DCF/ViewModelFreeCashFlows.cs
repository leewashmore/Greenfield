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
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFreeCashFlows: NotificationObject
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
        private RangeObservableCollection<FreeCashFlowsData> _freeCashFlowsDataInfo;       

        /// <summary>
        /// Private member to store basic data gadget visibilty
        /// </summary>
        private Visibility _freeCashFlowGadgetVisibility = Visibility.Collapsed;
        /// <summary>
        /// Private member to store Selected Security ID
        /// </summary>
        private EntitySelectionData _securitySelectionData = null;

        #endregion
        
        #region PROPERTIES

        /// <summary>
        /// Stores data for fcf
        /// </summary>
        public RangeObservableCollection<FreeCashFlowsData> FreeCashFlowsDataInfo
        {
            get { return _freeCashFlowsDataInfo; }
            set
            {
                if (_freeCashFlowsDataInfo == null)
                {
                    _freeCashFlowsDataInfo = new RangeObservableCollection<FreeCashFlowsData>();                   
                }
               RaisePropertyChanged(() => this.FreeCashFlowsDataInfo);
            }
        }
        
        /// <summary>
        /// Stores fcf arranged data
        /// </summary>
        private List<FreeCashFlowsData> _freeCashFlowsOutputData = null;
        public List<FreeCashFlowsData> FreeCashFlowsOutputData
        {
            get
            {
                return _freeCashFlowsOutputData;
            }
            set
            {
                _freeCashFlowsOutputData = value;
                RaisePropertyChanged(() => this.FreeCashFlowsOutputData);
            }
        }
       
        public Visibility FreeCashFlowGadgetVisibility
        {
            get { return _freeCashFlowGadgetVisibility; }
            set
            {
                _freeCashFlowGadgetVisibility = value;
                RaisePropertyChanged(() => this.FreeCashFlowGadgetVisibility);
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
                if (_securitySelectionData != null && IsActive)
                {
                    if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                    }
                }
            }
        }
        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return _busyIndicatorStatus;
            }
            set
            {
                _busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }
        #endregion

        #region CONSTRUCTOR
        public ViewModelFreeCashFlows(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
            if (_securitySelectionData != null && IsActive)
            {
                if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                {
                    CallingWebMethod();
                }
            }
            
        }

        #endregion       

        #region EVENTHANDLERS
        /// <summary>
        /// event to handle data
        /// </summary>
        public event RetrieveFreeCashFlowsDataCompletedEventHandler RetrieveFreeCashFlowsDataCompleteEvent;
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

                    if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                        
                    }
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
        private void RetrieveFreeCashFlowsDataCallbackMethod(List<FreeCashFlowsData> freeCashFlowsData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                FreeCashFlowsDataInfo = null;
                FreeCashFlowGadgetVisibility = Visibility.Collapsed;
                if (freeCashFlowsData != null && freeCashFlowsData.Count > 0)
                {
                    FreeCashFlowGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(_logger, methodNamespace, freeCashFlowsData, 1);
                    FreeCashFlowsDataInfo.Clear();
                    FreeCashFlowsDataInfo.AddRange(freeCashFlowsData);
                    FreeCashFlowsOutputData = ReArrangingData(FreeCashFlowsDataInfo);
                    RetrieveFreeCashFlowsDataCompleteEvent(new RetrieveFreeCashFlowsDataCompleteEventArs() { FreeCashFlowsInfo = FreeCashFlowsOutputData });
                    
                    
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
            finally { BusyIndicatorStatus = false; }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region Helper Methods
        public List<FreeCashFlowsData> ReArrangingData(RangeObservableCollection<FreeCashFlowsData> FreeCashFlowsData)
        {
            List<FreeCashFlowsData> result = new List<FreeCashFlowsData>();
            #region IF
            if (FreeCashFlowsData != null && FreeCashFlowsData.Count > 0)
            {
                List<FreeCashFlowsData> data = new List<FreeCashFlowsData>(FreeCashFlowsData);
                //Revenue Growth
                data = FreeCashFlowsData.Where(a => a.FieldName == "Revenue Growth").ToList();
                result.Add(new FreeCashFlowsData()
                {
                    FieldName = "Revenue Growth"                   
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                });

                //EBITDA Margins
                data = FreeCashFlowsData.Where(a => a.FieldName == "EBITDA Margins").ToList();
                    result.Add(new FreeCashFlowsData()
                    {
                     FieldName = "EBITDA Margins"                                          
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                    });
               
                //EBITDA 
                data = FreeCashFlowsData.Where(a => a.FieldName == "EBITDA").ToList();
                
                    result.Add(new FreeCashFlowsData()
                    {
                        FieldName = "EBITDA"
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                    });
                
                //(-) Taxes
                data = FreeCashFlowsData.Where(a => a.FieldName == "Taxes").ToList();
                result.Add(new FreeCashFlowsData()
                    {
                        FieldName = "(-) Taxes"               
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                    });
                
                //(+/-) Change in WC

                data = FreeCashFlowsData.Where(a => a.FieldName == "Change in WC").ToList();
                 result.Add(new FreeCashFlowsData()
                    {
                        FieldName = "(+/-) Change in WC"
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                    });

                //(-) Capex
                data = FreeCashFlowsData.Where(a => a.FieldName == "Capex").ToList();
                
                    result.Add(new FreeCashFlowsData()
                    {
                        FieldName = "(-) Capex"                  
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                    });

                //Blank Row
                result.Add(new FreeCashFlowsData() { });
               
                //(=) Free Cash Flow
                data = FreeCashFlowsData.Where(a => a.FieldName == "Free Cash Flow").ToList();
                result.Add(new FreeCashFlowsData()
                    {
                        FieldName = "(=) Free Cash Flow"             
                    ,ValueY0 = data.Where(a => a.PeriodYear == System.DateTime.Now.Year).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY1 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 1)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY2 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 2)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY3 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 3)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY4 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 4)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY5 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 5)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY6 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 6)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY7 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 7)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY8 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 8)).Select(a => a.Amount).FirstOrDefault().ToString()
                    ,ValueY9 = data.Where(a => a.PeriodYear == (System.DateTime.Now.Year + 9)).Select(a => a.Amount).FirstOrDefault().ToString()
                    });
               

            }
            #endregion
            foreach (FreeCashFlowsData item in result)
            {
                if (item.FieldName == "Revenue Growth" || item.FieldName == "EBITDA Margins")
                {
                    item.ValueY0 = (!String.IsNullOrEmpty(item.ValueY0)) ? Convert.ToDecimal(item.ValueY0).ToString("N1") + "%" : string.Empty;
                    item.ValueY1 = (!String.IsNullOrEmpty(item.ValueY1)) ? Convert.ToDecimal(item.ValueY1).ToString("N1") + "%" : string.Empty;
                    item.ValueY2 = (!String.IsNullOrEmpty(item.ValueY2)) ? Convert.ToDecimal(item.ValueY2).ToString("N1") + "%" : string.Empty;
                    item.ValueY3 = (!String.IsNullOrEmpty(item.ValueY3)) ? Convert.ToDecimal(item.ValueY3).ToString("N1") + "%" : string.Empty;
                    item.ValueY4 = (!String.IsNullOrEmpty(item.ValueY4)) ? Convert.ToDecimal(item.ValueY4).ToString("N1") + "%" : string.Empty;
                    item.ValueY5 = (!String.IsNullOrEmpty(item.ValueY5)) ? Convert.ToDecimal(item.ValueY5).ToString("N1") + "%" : string.Empty;
                    item.ValueY6 = (!String.IsNullOrEmpty(item.ValueY6)) ? Convert.ToDecimal(item.ValueY6).ToString("N1") + "%" : string.Empty;
                    item.ValueY7 = (!String.IsNullOrEmpty(item.ValueY7)) ? Convert.ToDecimal(item.ValueY7).ToString("N1") + "%" : string.Empty;
                    item.ValueY8 = (!String.IsNullOrEmpty(item.ValueY8)) ? Convert.ToDecimal(item.ValueY8).ToString("N1") + "%" : string.Empty;
                    item.ValueY9 = (!String.IsNullOrEmpty(item.ValueY9)) ? Convert.ToDecimal(item.ValueY9).ToString("N1") + "%" : string.Empty;
                }
                #region else
                else
                {
                    if (!String.IsNullOrEmpty(item.ValueY0))
                    {
                        if (Convert.ToDecimal(item.ValueY0) < 0)
                        {
                            item.ValueY0 = "(" + Convert.ToDecimal(item.ValueY0).ToString("N0") + ")";
                            item.ValueY0 = item.ValueY0.Replace("-", "");
                        }
                        else
                            item.ValueY0 = Convert.ToDecimal(item.ValueY0).ToString("N0");
                    }
                    if (!String.IsNullOrEmpty(item.ValueY1))
                    {
                        if (Convert.ToDecimal(item.ValueY1) < 0)
                        {
                            item.ValueY1 = "(" + Convert.ToDecimal(item.ValueY1).ToString("N0") + ")";
                            item.ValueY1 = item.ValueY1.Replace("-", "");
                         }
                        else
                            item.ValueY1 = Convert.ToDecimal(item.ValueY1).ToString("N0");

                        if (!String.IsNullOrEmpty(item.ValueY2))
                        {
                            if (Convert.ToDecimal(item.ValueY2) < 0)
                            {
                                item.ValueY2 = "(" + Convert.ToDecimal(item.ValueY2).ToString("N0") + ")";
                                item.ValueY2 = item.ValueY2.Replace("-", "");
                            }
                            else
                                item.ValueY2 = Convert.ToDecimal(item.ValueY2).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY3))
                        {
                            if (Convert.ToDecimal(item.ValueY3) < 0)
                            {
                                item.ValueY3 = "(" + Convert.ToDecimal(item.ValueY3).ToString("N0") + ")";
                                item.ValueY3 = item.ValueY3.Replace("-", "");
                            }
                            else
                                item.ValueY3 = Convert.ToDecimal(item.ValueY3).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY4))
                        {
                            if (Convert.ToDecimal(item.ValueY4) < 0)
                            {
                                item.ValueY4 = "(" + Convert.ToDecimal(item.ValueY4).ToString("N0") + ")";
                                item.ValueY4 = item.ValueY4.Replace("-", "");
                            }
                            else
                                item.ValueY4 = Convert.ToDecimal(item.ValueY4).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY5))
                        {
                            if (Convert.ToDecimal(item.ValueY5) < 0)
                            {
                                item.ValueY5 = "(" + Convert.ToDecimal(item.ValueY5).ToString("N0") + ")";
                                item.ValueY5 = item.ValueY5.Replace("-", "");
                            }
                            else
                                item.ValueY5 = Convert.ToDecimal(item.ValueY5).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY6))
                        {
                            if (Convert.ToDecimal(item.ValueY6) < 0)
                            {
                                item.ValueY6 = "(" + Convert.ToDecimal(item.ValueY6).ToString("N0") + ")";
                                item.ValueY6 = item.ValueY6.Replace("-", "");
                            }
                            else
                                item.ValueY6 = Convert.ToDecimal(item.ValueY6).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY7))
                        {
                            if (Convert.ToDecimal(item.ValueY7) < 0)
                            {
                                item.ValueY7 = "(" + Convert.ToDecimal(item.ValueY7).ToString("N0") + ")";
                                item.ValueY7 = item.ValueY7.Replace("-", "");
                            }
                            else
                                item.ValueY7 = Convert.ToDecimal(item.ValueY7).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY8))
                        {
                            if (Convert.ToDecimal(item.ValueY8) < 0)
                            {
                                item.ValueY8 = "(" + Convert.ToDecimal(item.ValueY8).ToString("N0") + ")";
                                item.ValueY8 = item.ValueY8.Replace("-", "");
                        }
                            else
                                item.ValueY8 = Convert.ToDecimal(item.ValueY8).ToString("N0");
                        }
                        if (!String.IsNullOrEmpty(item.ValueY9))
                        {
                            if (Convert.ToDecimal(item.ValueY9) < 0)
                            {
                                item.ValueY9 = "(" + Convert.ToDecimal(item.ValueY9).ToString("N0") + ")";
                                item.ValueY9 = item.ValueY9.Replace("-", "");
                            }
                            else
                                item.ValueY9 = Convert.ToDecimal(item.ValueY9).ToString("N0");
                        }
                    }
                }
                    #endregion

                    
            }
            return result;
        }
        #endregion

        #region SERVICE CALL METOHD
        /// <summary>
        /// Calls web service method
        /// </summary>
        private void CallingWebMethod()
        {
            if (_securitySelectionData != null && IsActive)
            {
                _dbInteractivity.RetrieveDCFFreeCashFlowsData(_securitySelectionData, RetrieveFreeCashFlowsDataCallbackMethod);
                BusyIndicatorStatus = true;
            }            

        }
        #endregion  


        #region EventUnSubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
}


   
