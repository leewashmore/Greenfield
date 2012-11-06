using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.Common.Helper;
using GreenField.DataContracts;
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
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        public ILoggerFacade logger;
        
        /// <summary>
        /// Private member to store basic data
        /// </summary>
        private RangeObservableCollection<FreeCashFlowsData> freeCashFlowsDataInfo;       

        /// <summary>
        /// Private member to store basic data gadget visibilty
        /// </summary>
        private Visibility freeCashFlowGadgetVisibility = Visibility.Collapsed;
        /// <summary>
        /// Private member to store Selected Security ID
        /// </summary>
        private EntitySelectionData securitySelectionData = null;

        #endregion
        
        #region PROPERTIES

        /// <summary>
        /// Stores data for fcf
        /// </summary>
        public RangeObservableCollection<FreeCashFlowsData> FreeCashFlowsDataInfo
        {
            get { return freeCashFlowsDataInfo; }
            set
            {
                if (freeCashFlowsDataInfo == null)
                {
                    freeCashFlowsDataInfo = new RangeObservableCollection<FreeCashFlowsData>();                   
                }
               RaisePropertyChanged(() => this.FreeCashFlowsDataInfo);
            }
        }
        
        /// <summary>
        /// Stores fcf arranged data
        /// </summary>
        private List<FreeCashFlowsData> freeCashFlowsOutputData = null;
        public List<FreeCashFlowsData> FreeCashFlowsOutputData
        {
            get
            {
                return freeCashFlowsOutputData;
            }
            set
            {
                freeCashFlowsOutputData = value;
                RaisePropertyChanged(() => this.FreeCashFlowsOutputData);
            }
        }
       
        public Visibility FreeCashFlowGadgetVisibility
        {
            get { return freeCashFlowGadgetVisibility; }
            set
            {
                freeCashFlowGadgetVisibility = value;
                RaisePropertyChanged(() => this.FreeCashFlowGadgetVisibility);
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (securitySelectionData != null && IsActive)
                {
                    if (securitySelectionData.InstrumentID != null && securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                    }
                }
            }
        }
        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return busyIndicatorStatus;
            }
            set
            {
                busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }
        #endregion

        #region CONSTRUCTOR
        public ViewModelFreeCashFlows(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
            if (securitySelectionData != null && IsActive)
            {
                if (securitySelectionData.InstrumentID != null && securitySelectionData.InstrumentID != string.Empty)
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    securitySelectionData = entitySelectionData;

                    if (securitySelectionData.InstrumentID != null && securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                        
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                FreeCashFlowsDataInfo = null;
                FreeCashFlowGadgetVisibility = Visibility.Collapsed;
                if (freeCashFlowsData != null && freeCashFlowsData.Count > 0)
                {
                    FreeCashFlowGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(logger, methodNamespace, freeCashFlowsData, 1);
                    FreeCashFlowsDataInfo.Clear();
                    FreeCashFlowsDataInfo.AddRange(freeCashFlowsData);
                    FreeCashFlowsOutputData = ReArrangingData(FreeCashFlowsDataInfo);
                    RetrieveFreeCashFlowsDataCompleteEvent(new RetrieveFreeCashFlowsDataCompleteEventArs() { FreeCashFlowsInfo = FreeCashFlowsOutputData });
                    
                    
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
            finally { BusyIndicatorStatus = false; }
            Logging.LogEndMethod(logger, methodNamespace);
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

            //Method to apply Formatting
            List<FreeCashFlowsData> resultAfterFormatting = ApplyingFormatting(result);            
            return result;
        }
        /// <summary>
        /// Method to format data
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Formatted data</returns>
        private List<FreeCashFlowsData> ApplyingFormatting(List<FreeCashFlowsData> result)
        {
            foreach (FreeCashFlowsData item in result)
            {
                decimal valueY0 = 0;
                decimal valueY1;
                decimal valueY2;
                decimal valueY3;
                decimal valueY4;
                decimal valueY5 = 0;
                decimal valueY6;
                decimal valueY7;
                decimal valueY8;
                decimal valueY9;                

                valueY0 = decimal.TryParse(item.ValueY0, out valueY0) ? valueY0 : 0;
                valueY1 = decimal.TryParse(item.ValueY1, out valueY1) ? valueY1 : 0;
                valueY2 = decimal.TryParse(item.ValueY2, out valueY2) ? valueY2 : 0;
                valueY3 = decimal.TryParse(item.ValueY3, out valueY3) ? valueY3 : 0;
                valueY4 = decimal.TryParse(item.ValueY4, out valueY4) ? valueY4 : 0;

                valueY5 = ((valueY0 + valueY1 + valueY2 + valueY3 + valueY4) / 5) * 0.99M;               
               
                valueY6 = valueY5 * 0.99M;
                valueY7 = valueY6 * 0.99M;
                valueY8 = valueY7 * 0.99M;
                valueY9 = valueY8 * 0.99M;

                if (item.FieldName == "Revenue Growth" || item.FieldName == "EBITDA Margins")
                {
                    item.ValueY0 = (!(String.IsNullOrEmpty(item.ValueY0))) ? Convert.ToDecimal(item.ValueY0).ToString("N1") + "%" : string.Empty;
                    item.ValueY1 = (!(String.IsNullOrEmpty(item.ValueY1))) ? Convert.ToDecimal(item.ValueY1).ToString("N1") + "%" : string.Empty;
                    item.ValueY2 = (!(String.IsNullOrEmpty(item.ValueY2))) ? Convert.ToDecimal(item.ValueY2).ToString("N1") + "%" : string.Empty;
                    item.ValueY3 = (!(String.IsNullOrEmpty(item.ValueY3))) ? Convert.ToDecimal(item.ValueY3).ToString("N1") + "%" : string.Empty;
                    item.ValueY4 = (!(String.IsNullOrEmpty(item.ValueY4))) ? Convert.ToDecimal(item.ValueY4).ToString("N1") + "%" : string.Empty;
                    item.ValueY5 = (!(String.IsNullOrEmpty(valueY5.ToString()))) ? valueY5.ToString("N1") + "%" : string.Empty;
                    item.ValueY6 = (!(String.IsNullOrEmpty(valueY6.ToString()))) ? valueY6.ToString("N1") + "%" : string.Empty;
                    item.ValueY7 = (!(String.IsNullOrEmpty(valueY7.ToString()))) ? valueY7.ToString("N1") + "%" : string.Empty;
                    item.ValueY8 = (!(String.IsNullOrEmpty(valueY8.ToString()))) ? valueY8.ToString("N1") + "%" : string.Empty;
                    item.ValueY9 = (!(String.IsNullOrEmpty(valueY9.ToString()))) ? valueY9.ToString("N1") + "%" : string.Empty;                    
                }
                #region else
                else
                {
                    
                    
                    if (!(String.IsNullOrEmpty(item.ValueY0)))
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
                        if (valueY5 < 0)
                        {
                            item.ValueY5 = "(" + valueY5.ToString("N0") + ")" ;
                            //item.ValueY6 = "(" + Convert.ToDecimal(item.ValueY6).ToString("N0") + ")";
                            item.ValueY5 = item.ValueY5.Replace("-", "");
                        }
                        else
                        {
                            item.ValueY5 = valueY5.ToString("N0");
                        }
                        if (valueY6 < 0)
                        {
                            item.ValueY6 = "(" + valueY6.ToString("N0") + ")";
                            //item.ValueY6 = "(" + Convert.ToDecimal(item.ValueY6).ToString("N0") + ")";
                            item.ValueY6 = item.ValueY6.Replace("-", "");
                        }
                        else
                        {
                            item.ValueY6 = valueY6.ToString("N0");
                        }

                        if (valueY7 < 0)
                        {
                            item.ValueY7 = "(" + valueY7.ToString("N0") + ")";
                            //item.ValueY6 = "(" + Convert.ToDecimal(item.ValueY6).ToString("N0") + ")";
                            item.ValueY7 = item.ValueY7.Replace("-", "");
                        }
                        else
                        {
                            item.ValueY7 = valueY7.ToString("N0");
                        } 
                        if (valueY8 < 0)
                        {
                            item.ValueY8 = "(" + valueY8.ToString("N0") + ")";
                            //item.ValueY6 = "(" + Convert.ToDecimal(item.ValueY6).ToString("N0") + ")";
                            item.ValueY8 = item.ValueY8.Replace("-", "");
                        }
                        else
                        {
                            item.ValueY8 = valueY8.ToString("N0");
                        }
                        if (valueY9 < 0)
                        {
                            item.ValueY9 = "(" + valueY9.ToString("N0") + ")";
                            //item.ValueY6 = "(" + Convert.ToDecimal(item.ValueY6).ToString("N0") + ")";
                            item.ValueY9 = item.ValueY9.Replace("-", "");
                        }
                        else
                        {
                            item.ValueY9 = valueY9.ToString("N0");
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
            if (securitySelectionData != null && IsActive)
            {
                dbInteractivity.RetrieveDCFFreeCashFlowsData(securitySelectionData, RetrieveFreeCashFlowsDataCallbackMethod);
                BusyIndicatorStatus = true;
            }            

        }
        #endregion  


        #region EventUnSubscribe

        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
}


   
