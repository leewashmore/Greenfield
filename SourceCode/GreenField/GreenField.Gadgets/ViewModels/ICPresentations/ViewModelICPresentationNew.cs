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
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;



namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelICPresentationNew : NotificationObject
    {

        #region Fields

        public IRegionManager _regionManager { private get; set; }

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        private EntitySelectionData _entitySelectionData;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        #endregion

        #region Constructor
        public ViewModelICPresentationNew(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
           // _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            //Subscription to SecurityReferenceSet event
            //_eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);

            //EntitySelectionData handling
            if (_entitySelectionData != null)
            {
                //HandleSecurityReferenceSet(_entitySelectionData);
            }
        }
        #endregion       

        
//        #region Properties

//        /// <summary>
//        /// Content displayed on the busy indicator
//        /// </summary>
//        private string _busyIndicatorContent;
//        public string BusyIndicatorContent
//        {
//            get { return _busyIndicatorContent; }
//            set
//            {
//                _busyIndicatorContent = value;
//                RaisePropertyChanged(() => this.BusyIndicatorContent);
//            }
//        }

//        /// <summary>
//        /// property to contain status value for busy indicator of the gadget
//        /// </summary>
//        private bool _busyIndicatorStatus;
//        public bool BusyIndicatorStatus
//        {
//            get { return _busyIndicatorStatus; }
//            set
//            {
//                if (_busyIndicatorStatus != value)
//                {
//                    _busyIndicatorStatus = value;
//                    RaisePropertyChanged(() => BusyIndicatorStatus);
//                }
//            }
//        }


//            /// <summary>
//        /// IssueName Property
//        /// </summary>
//        private string _securityName;
//        public string SecurityName
//        {
//            get { return _securityName; }
//            set
//            {
//                if (_securityName != value)
//                    _securityName = value;
//                RaisePropertyChanged(() => this.SecurityName);
//            }
//        }

//        /// <summary>
//        /// Ticker Property
//        /// </summary>
//        private string _securityTicker;
//        public string SecurityTicker
//        {
//            get { return _securityTicker; }
//            set
//            {
//                if (_securityTicker != value)
//                    _securityTicker = value;
//                RaisePropertyChanged(() => this.SecurityTicker);
//            }
//        }

//        /// <summary>
//        /// Country Property
//        /// </summary>
//        private string _country;
//        public string SecurityCountry
//        {
//            get { return _country; }
//            set
//            {
//                if (_country != value)
//                    _country = value;
//                RaisePropertyChanged(() => this.SecurityCountry);
//            }
//        }

        
//        /// <summary>
//        /// Industry Property
//        /// </summary>
//        private string _industry;
//        public string SecurityIndustry
//        {
//            get { return _industry; }
//            set
//            {
//                if (_industry != value)
//                    _industry = value;
//                RaisePropertyChanged(() => this.SecurityIndustry);
//            }
//        }


//        /// <summary>
//        /// Analyst Property
//        /// </summary>
//        private string _analyst;
//        public string Analyst
//        {
//            get { return _analyst; }
//            set
//            {
//                if (_analyst != value)
//                    _analyst = value;
//                RaisePropertyChanged(() => this.Analyst);
//            }
//        }

//        /// <summary>
//        /// Price
//        /// </summary>
//        private string _price;
//        public string Price
//        {
//            get { return _price; }
//            set
//            {
//                if (_price != value)
//                    _price = value;
//                RaisePropertyChanged(() => this.Price);
//            }
//        }

//        /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _marketCap;
//        public string MarketCap
//        {
//            get { return _marketCap; }
//            set
//            {
//                if (_marketCap != value)
//                    _marketCap = value;
//                RaisePropertyChanged(() => this.MarketCap);
//            }
//        }

//        /// <summary>
//        /// FVCalc Property
//        /// </summary>
//        private string _fvCalc;
//        public string FVCalc
//        {
//            get { return _fvCalc; }
//            set
//            {
//                if (_fvCalc != value)
//                    _fvCalc = value;
//                RaisePropertyChanged(() => this.FVCalc);
//            }
//        }

//        /// <summary>
//        /// SecurityBuySellvsCrnt Property
//        /// </summary>
//        private string _securityBuySellvsCrnt;
//        public string SecurityBuySellvsCrnt
//        {
//            get { return _securityBuySellvsCrnt; }
//            set
//            {
//                if (_securityBuySellvsCrnt != value)
//                    _securityBuySellvsCrnt = value;
//                RaisePropertyChanged(() => this.SecurityBuySellvsCrnt);
//            }
//        }
    
//         /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _totalCurrentHoldings;
//        public string TotalCurrentHoldings
//        {
//            get { return _totalCurrentHoldings; }
//            set
//            {
//                if (_totalCurrentHoldings != value)
//                    _totalCurrentHoldings = value;
//                RaisePropertyChanged(() => this.TotalCurrentHoldings);
//            }
//        }

//         /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _percentEMIF;
//        public string PercentEMIF
//        {
//            get { return _percentEMIF; }
//            set
//            {
//                if (_percentEMIF != value)
//                    _percentEMIF = value;
//                RaisePropertyChanged(() => this.PercentEMIF);
//            }
//        }

//         /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _bmWeight;
//        public string SecurityBMWeight
//        {
//            get { return _bmWeight; }
//            set
//            {
//                if (_bmWeight != value)
//                    _bmWeight = value;
//                RaisePropertyChanged(() => this.SecurityBMWeight);
//            }
//        }

//         /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _activeWeight;
//        public string SecurityActiveWeight
//        {
//            get { return _activeWeight; }
//            set
//            {
//                if (_activeWeight != value)
//                    _activeWeight = value;
//                RaisePropertyChanged(() => this.SecurityActiveWeight);
//            }
//        }

//                 /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _ytdAbsolute;
//        public string YTDRet_Absolute
//        {
//            get { return _ytdAbsolute; }
//            set
//            {
//                if (_ytdAbsolute != value)
//                    _ytdAbsolute = value;
//                RaisePropertyChanged(() => this.YTDRet_Absolute);
//            }
//        }

//                 /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _ytdRelToLoc;
//        public string YTDRet_RELtoLOC
//        {
//            get { return _ytdRelToLoc; }
//            set
//            {
//                if (_ytdRelToLoc != value)
//                    _ytdRelToLoc = value;
//                RaisePropertyChanged(() => this.YTDRet_RELtoLOC);
//            }
//        }

//                 /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _ytdRelToEm;
//        public string YTDRet_RELtoEM
//        {
//            get { return _ytdRelToEm; }
//            set
//            {
//                if (_ytdRelToEm != value)
//                    _ytdRelToEm = value;
//                RaisePropertyChanged(() => this.YTDRet_RELtoEM);
//            }
//        }

//                /// <summary>
//        /// Market Cap Property
//        /// </summary>
//        private string _recommendation;
//        public string SecurityRecommendation
//        {
//            get { return _recommendation; }
//            set
//            {
//                if (_recommendation != value)
//                    _recommendation = value;
//                RaisePropertyChanged(() => this.SecurityRecommendation);
//            }
//        }

//        //private ObservableCollection<ICPAttachmentInfo> _attachmentInfo;
//        //public ObservableCollection<ICPAttachmentInfo> AttachmentInfo
//        //{
//        //    get 
//        //    {
//        //        if (_attachmentInfo == null)
//        //            _attachmentInfo = new ObservableCollection<ICPAttachmentInfo>();
//        //        return _attachmentInfo; 
//        //    }
//        //    set
//        //    {
//        //        if (_attachmentInfo != value)
//        //        {
//        //            _attachmentInfo = value;
//        //            RaisePropertyChanged(() => this.AttachmentInfo);
//        //        }
//        //    }
//        //}

//        private bool _securitySelected = false;
//        public bool SecuritySelected
//        {
//            get 
//            {
//                if (ViewPluginFlag == ViewPluginFlagEnumeration.Update)
//                    _securitySelected = true;
//                return _securitySelected; 
//            }
//            set
//            {
//                if (_securitySelected != value)
//                {
//                    _securitySelected = value;
//                    RaisePropertyChanged(() => this.SecuritySelected);
//                    RaisePropertyChanged(() => this.SaveCommand);
//                  //  RaisePropertyChanged(() => this.BrowseCommand);
//                }
//            }
//        }

//        //private string _uploadedFileName;
//        //public string UploadedFileName
//        //{
//        //    get { return _uploadedFileName; }
//        //    set
//        //    {
//        //        if (_uploadedFileName != value)
//        //        {
//        //            _uploadedFileName = value;
//        //            RaisePropertyChanged(() => this.UploadedFileName);
//        //        }
//        //    }
//        //}

//        private ViewPluginFlagEnumeration _viewPluginFlag;
//        public ViewPluginFlagEnumeration ViewPluginFlag
//        {
//            get
//            {
//                return _viewPluginFlag;
//            }
//            set
//            {
//                _viewPluginFlag = value;
//                RaisePropertyChanged(() => this.ViewPluginFlag);
//                RaisePropertyChanged(() => this.SecuritySelectionVisibility);
//                RaisePropertyChanged(() => this.PresenterSelectionVisibility);
//            }
//        }

//        private ObservableCollection<ICPSecurityInfo> _securityInfo;
//        public ObservableCollection<ICPSecurityInfo> SecurityInfo
//        {
//            get
//            {
//                if (ViewPluginFlag == ViewPluginFlagEnumeration.Create)
//                {
//                    _securityInfo = new ObservableCollection<ICPSecurityInfo>();
//                    ICPSecurityInfo s1 = new ICPSecurityInfo
//                    {
//                        SecurityBuyRange = "20",
//                        SecurityCashPosition = "0",
//                        SecurityCountry = "India",
//                        SecurityCountryCode = "IND",
//                        SecurityGlobalActiveWeight = "0",
//                        SecurityIndustry = "Chemical",
//                        SecurityLastClosingPrice = "25.6",
//                        SecurityMarketCapitalization = "6459800",
//                        SecurityMSCIIMIWeight = "0",
//                        SecurityMSCIStdWeight = "0",
//                        SecurityName = "Reliance Capital Ltd.",
//                        SecurityPFVMeasure = "PE2012",
//                        SecurityPosition = "0",
//                        SecuritySellRange = "30",
//                        SecurityTicker = "REL"                        
//                    };
//                    ICPSecurityInfo s2 = new ICPSecurityInfo
//                    {
//                        SecurityBuyRange = "28",
//                        SecurityCashPosition = "0",
//                        SecurityCountry = "United States",
//                        SecurityCountryCode = "US",
//                        SecurityGlobalActiveWeight = "0",
//                        SecurityIndustry = "Internet",
//                        SecurityLastClosingPrice = "25.6",
//                        SecurityMarketCapitalization = "56,894,000",
//                        SecurityMSCIIMIWeight = "0",
//                        SecurityMSCIStdWeight = "0",
//                        SecurityName = "Facebook",
//                        SecurityPFVMeasure = "PE2012",
//                        SecurityPosition = "0",
//                        SecuritySellRange = "33",
//                        SecurityTicker = "FCB"
//                    };
//                    _securityInfo.Add(s1);
//                    _securityInfo.Add(s2);
//                }
                    
//                return _securityInfo;
//            }
//            set
//            {
//                _securityInfo = value;
//               //RaisePropertyChanged(() => this.PresentationInfo);
//            }
//        }

//        //private ICPSecurityInfo _selectedSecurityInfo;
//        //public ICPSecurityInfo SelectedSecurityInfo
//        //{
//        //    get { return _selectedSecurityInfo; }
//        //    set
//        //    {
//        //        _selectedSecurityInfo = value;
//        //        RaisePropertyChanged(() => this.SelectedSecurityInfo);
//        //        SecuritySelected = true;
//        //        PresentationInfo = new ICPPresentationInfo
//        //        {
//        //            Presenter = "LoggedInUser",
//        //            SecurityBuyRange = value.SecurityBuyRange,
//        //            SecurityBuySellRange = value.SecurityBuySellRange,
//        //            SecurityCashPosition = value.SecurityCashPosition,
//        //            SecurityCountry = value.SecurityCountry,
//        //            SecurityCountryCode = value.SecurityCountryCode,
//        //            SecurityGlobalActiveWeight = value.SecurityGlobalActiveWeight,
//        //            SecurityIndustry = value.SecurityIndustry,
//        //            SecurityLastClosingPrice = value.SecurityLastClosingPrice,
//        //            SecurityMarketCapitalization = value.SecurityMarketCapitalization,
//        //            SecurityMSCIIMIWeight = value.SecurityMSCIIMIWeight,
//        //            SecurityMSCIStdWeight = value.SecurityMSCIStdWeight,
//        //            SecurityName = value.SecurityName,
//        //            SecurityPFVMeasure =value.SecurityPFVMeasure,
//        //            SecurityPosition = value.SecurityPosition,
//        //            SecurityRecommendation  =value.SecurityRecommendation,
//        //            SecuritySellRange = value.SecuritySellRange,
//        //            SecurityTicker = value.SecurityTicker,
//        //            Status = "In Progress"                    
//        //        };
//        //    }
//        //}

//        private SecurityInformation _selectedSecurityInfo;
//        public SecurityInformation SelectedSecurityInfo
//        {
//            get { return _selectedSecurityInfo; }
//            set
//            {
//                _selectedSecurityInfo = value;
//                RaisePropertyChanged(() => this.SelectedSecurityInfo);
//                SecuritySelected = true;
//                //PresentationInfo = new ICPPresentationData
//                //{
//                //    Presenter = "LoggedInUser",
//                //    SecurityBuyRange = value.SecurityBuySellvsCrnt,
//                //    SecurityBuySellRange = value.SecurityBuySellvsCrnt,
//                //    SecurityCashPosition = value.TotalCurrentHoldings,
//                //    SecurityCountry = value.SecurityCountry,
//                //    SecurityCountryCode = "Country Code",
//                //    SecurityGlobalActiveWeight = value.SecurityActiveWeight,
//                //    SecurityIndustry = value.SecurityIndustry,
//                //    SecurityLastClosingPrice = "2800",
//                //    SecurityMarketCapitalization = value.SecurityMarketCapitalization,
//                //    SecurityMSCIIMIWeight = value.SecurityBMWeight,
//                //    SecurityMSCIStdWeight = value.SecurityBMWeight,
//                //    SecurityName = value.SecurityName,
//                //    SecurityPFVMeasure = value.FVCalc,
//                //    SecurityPosition = value.Price,
//                //    SecurityRecommendation = value.SecurityRecommendation,
//                //    SecuritySellRange = value.SecurityBuySellvsCrnt,
//                //    SecurityTicker = value.SecurityTicker,
//                //    Status = "In Progress"
//                //};
//            }
//        }


        
//        //private ICPPresentationData _presentationInfo;
//        //public ICPPresentationData PresentationInfo
//        //{
//        //    get
//        //    {
//        //        if (_presentationInfo == null)
//        //            return new ICPPresentationData();
//        //        return _presentationInfo;
//        //    }
//        //    set
//        //    {
//        //        _presentationInfo = value;
//        //        RaisePropertyChanged(() => this.PresentationInfo);
//        //    }
//        //}



//        private Visibility _securitySelectionVisibility = Visibility.Visible;
//        public Visibility SecuritySelectionVisibility
//        {
//            get 
//            {
//                if (ViewPluginFlag == ViewPluginFlagEnumeration.Create)
//                    _securitySelectionVisibility = Visibility.Visible;
//                //switch (ViewPluginFlag)
//                //{
//                //    case ViewPluginFlagEnumeration.Create:
//                //        _securitySelectionVisibility = Visibility.Visible;
//                //        break;
//                //    case ViewPluginFlagEnumeration.Update:
//                //        _securitySelectionVisibility = Visibility.Collapsed;
//                //        break;
//                //}
//                return _securitySelectionVisibility; }
//        }

//        private Visibility _presenterSelectionVisibility = Visibility.Collapsed;
//        public Visibility PresenterSelectionVisibility
//        {
//            get
//            {
//                if (ViewPluginFlag == ViewPluginFlagEnumeration.Create)
//                        _presenterSelectionVisibility = Visibility.Collapsed;
//                //switch (ViewPluginFlag)
//                //{
//                //    case ViewPluginFlagEnumeration.Create:
//                //        _presenterSelectionVisibility = Visibility.Collapsed;
//                //        break;
//                //    case ViewPluginFlagEnumeration.Update:
//                //        _presenterSelectionVisibility = Visibility.Visible;
//                //        break;

//                return _presenterSelectionVisibility;}            
//        }

//        public ICommand SaveCommand
//        {
//            get { return new DelegateCommand<object>(ICPPresentationsSaveItem, SecuritySelectionValidation);  }
//        }

//        //public ICommand BrowseCommand
//        //{
//        //    get
//        //    {
//        //        return new DelegateCommand<object>(ICPPresentationsBrowse, SecuritySelectionValidation);
//        //    }
//        //}

//        #endregion

//        #region ICommand Methods

//        private void ICPPresentationsSaveItem(object param)
//        {
//            //ObservableCollection<AttachedFileInfo> UpdatedAttachmentInfo = new ObservableCollection<AttachedFileInfo>();

            
//            if (ViewPluginFlag == ViewPluginFlagEnumeration.Create)
//            {
//                //_dbInteractivity.CreatePresentation(PresentationInfo.ConvertToDB()
//                //       , (presentationID) =>
//                //       {
//                //           if (presentationID != null)
//                //           {
//                //               this.PresentationInfo.PresentationID = long.Parse(presentationID.ToString());
//                //           }
//                //       });
//            }
//            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_PRESENTATIONS);
//            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteePresentations", UriKind.Relative));
//        }

//        //public void ICPPresentationsBrowse(object param)
//        //{
//        //    OpenFileDialog openFile = new OpenFileDialog
//        //    {
//        //        Multiselect = false,
//        //        Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*"
//        //    };
            
//        //    if (openFile.ShowDialog() == true)
//        //    {
//        //        UploadedFileName = openFile.File.Name;
//        //        string serializedData = GetFileSerializedData(openFile.File.OpenRead());

//        //        AttachmentInfo.Add(new ICPAttachmentInfo ()
//        //        {
//        //            AttachmentID = AttachmentInfo.Count,
//        //            AttachmentName = UploadedFileName,
//        //            AttachmentSerializedData = serializedData
//        //        });
//        //    }
//        //}

//        private bool SecuritySelectionValidation(object param)
//        {
//            return SecuritySelected;
//            //return true;
//        }

//        #endregion

//        #region Helper Methods

//        //private string GetFileSerializedData(FileStream fileStream)
//        //{
//        //    byte[] bytes = new byte[fileStream.Length];

//        //    int numBytesToRead = (int)fileStream.Length;
//        //    int numBytesRead = 0;

//        //    while (numBytesToRead > 0)
//        //    {
//        //        int n = fileStream.Read(bytes, numBytesRead, numBytesToRead);
//        //        if (n == 0) break;

//        //        numBytesRead += n;
//        //        numBytesToRead -= n;
//        //    }
//        //    numBytesToRead = bytes.Length;

//        //    StringBuilder sb = new StringBuilder();
//        //    StringWriter wr = new StringWriter(sb);

//        //    XmlSerializer serializer = new XmlSerializer(typeof(byte[]));
            
//        //    serializer.Serialize(wr, bytes);
//        //    return sb.ToString();
//        //}

     

//        #endregion

//        #region Callback

//        //private void GetPresentationsCallBackMethod(List<PresentationInfoResult> val)
//        //{
//        //    ObservableCollection<PresentationInfoResult> PresentationInfoCollObj = new ObservableCollection<PresentationInfoResult>(val);
//        //    ObservableCollection<ICPPresentationInfo> ICPPresentationInfoCollObj = new ObservableCollection<ICPPresentationInfo>();
//        //    foreach (PresentationInfoResult pinfo in PresentationInfoCollObj)
//        //    {
//        //        ICPPresentationInfo ICPPresentationInfoObj = new ICPPresentationInfo(pinfo);
//        //        ICPPresentationInfoCollObj.Add(ICPPresentationInfoObj);
//        //    }

//        //    PresentationInfo = ICPPresentationInfoCollObj;

//        //}

//         /// <summary>
//        /// Callback method for Security Overview Service call - assigns value to UI Field Properties
//        /// </summary>
//        /// <param name="securityOverviewData">SecurityOverviewData Collection</param>
//        private void RetrieveSecurityDetailsCallBackMethod(SecurityInformation securityDetails)
//        {
//            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
//            Logging.LogBeginMethod(_logger, methodNamespace);
//            try
//            {
//                if (securityDetails != null)
//                {
//                    Logging.LogMethodParameter(_logger, methodNamespace, securityDetails, 1);
//                    this.SecurityName = securityDetails.SecurityName;
//                    this.SecurityTicker = securityDetails.SecurityTicker;
//                    this.SecurityCountry = securityDetails.SecurityCountry;
//                    this.SecurityIndustry= securityDetails.SecurityIndustry;                    
//                    this.Analyst = securityDetails.Analyst;
//                    this.SecurityRecommendation = securityDetails.SecurityRecommendation;
//                    this.Price = securityDetails.Price;
//                    this.PercentEMIF = securityDetails.PercentEMIF;
//                    this.MarketCap = securityDetails.SecurityMarketCapitalization;
//                    this.FVCalc = securityDetails.FVCalc;
//                    this.SecurityActiveWeight = securityDetails.SecurityActiveWeight;
//                    this.SecurityBMWeight = securityDetails.SecurityBMWeight;
//                    this.SecurityBuySellvsCrnt = securityDetails.SecurityBuySellvsCrnt;
//                    this.TotalCurrentHoldings = securityDetails.TotalCurrentHoldings;
//                    this.YTDRet_Absolute = securityDetails.YTDRet_Absolute;
//                    this.YTDRet_RELtoEM = securityDetails.YTDRet_RELtoEM;
//                    this.YTDRet_RELtoLOC = securityDetails.YTDRet_RELtoLOC;

//                    SelectedSecurityInfo = securityDetails;
//                }
//                else
//                {
//                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
//                }                
//            }
//            catch (Exception ex)
//            {
//                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
//                Logging.LogException(_logger, ex);
//            }
//            finally
//            {
//                BusyIndicatorStatus = false;
//            }
//            Logging.LogEndMethod(_logger, methodNamespace);            
//        }

//        #endregion
        
//        #region Event Handler

//           /// <summary>
//        /// Assigns UI Field Properties based on Entity Selection Data
//        /// </summary>
//        /// <param name="entitySelectionData">EntitySelectionData</param>
//        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
//        {
//            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
//            Logging.LogBeginMethod(_logger, methodNamespace);
//            try
//            {
//                if (entitySelectionData != null)
//                {
//                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
//                    _entitySelectionData = entitySelectionData;
                   
//                    if (IsActive && _entitySelectionData != null)
//                    {
//                        //_dbInteractivity.RetrieveSecurityDetails(entitySelectionData,RetrieveSecurityDetailsCallBackMethod);
//                        BusyIndicatorContent = "Retrieving security reference data for '" + entitySelectionData.LongName + " (" + entitySelectionData.ShortName + ")'";
//                        BusyIndicatorStatus = true;
//                    }
//                }
//                else
//                {
//                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
//                }
//            }
//            catch (Exception ex)
//            {
//                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
//                Logging.LogException(_logger, ex);                
//            }
//            Logging.LogEndMethod(_logger, methodNamespace);
//        }
//#endregion

//        #region INavigationAware methods

//        //public void OnNavigatedFrom(NavigationContext navigationContext)
//        //{

//        //}

//        //public void OnNavigatedTo(NavigationContext navigationContext)
//        //{
//        //    ViewPluginFlag = (navigationContext.NavigationService.Region.Context as ICPNavigationInfo).ViewPluginFlagEnumerationObject;
//        //    PresentationInfo = (navigationContext.NavigationService.Region.Context as ICPNavigationInfo).PresentationInfoObject;
//        //}

//        //public bool IsNavigationTarget(NavigationContext navigationContext)
//        //{
//        //    return true;
//        //}

//        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

    }
}
