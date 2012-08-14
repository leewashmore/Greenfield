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
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingServiceReference;
using Microsoft.Practices.Prism.ViewModel;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;

namespace GreenField.Gadgets.Models
{
    public class ICPPresentationInfo : NotificationObject
    {
        public long PresentationID { get; set; }

        private string _presenter = string.Empty;
        public string Presenter 
        {
            get { return _presenter; }
            set
            {
                if (_presenter != value)
                {
                    _presenter = value;
                    RaisePropertyChanged(() => this.Presenter);
                }
            }

        }

        private DateTime? _presentationDate;
        public DateTime? PresentationDate
        {
            get { return _presentationDate; }
            set
            {
                if (_presentationDate != value)
                {
                    _presentationDate = value;
                    RaisePropertyChanged(() => this.PresentationDate);
                }
            }
        }

        private string _status = string.Empty;
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    RaisePropertyChanged(() => this.Status);
                }
            }

        }

        private long _statusTypeID = StatusTypes.InProgress;
        public long StatusTypeID
        {
            get { return _statusTypeID; }
            set
            {
                if (_statusTypeID != value)
                {
                    _statusTypeID = value;
                    RaisePropertyChanged(() => this.StatusTypeID);
                }
            }
        }
        
        private string _securityTicker = string.Empty;
        public string SecurityTicker
        {
            get { return _securityTicker; }
            set
            {
                if (_securityTicker != value)
                {
                    _securityTicker = value;
                    RaisePropertyChanged(() => this.SecurityTicker);
                }
            }

        }

        private string _securityName = string.Empty;
        public string SecurityName
        {
            get { return _securityName; }
            set
            {
                if (_securityName != value)
                {
                    _securityName = value;
                    RaisePropertyChanged(() => this.SecurityName);
                }
            }

        }

        private string _securityCountry = string.Empty;
        public string SecurityCountry
        {
            get { return _securityCountry; }
            set
            {
                if (_securityCountry != value)
                {
                    _securityCountry = value;
                    RaisePropertyChanged(() => this.SecurityCountry);
                }
            }

        }

        private string _securityCountryCode = string.Empty;
        public string SecurityCountryCode
        {
            get { return _securityCountryCode; }
            set
            {
                if (_securityCountryCode != value)
                {
                    _securityCountryCode = value;
                    RaisePropertyChanged(() => this.SecurityCountryCode);
                }
            }

        }

        private string _securityIndustry = string.Empty;
        public string SecurityIndustry
        {
            get { return _securityIndustry; }
            set
            {
                if (_securityIndustry != value)
                {
                    _securityIndustry = value;
                    RaisePropertyChanged(() => this.SecurityIndustry);
                }
            }

        }

        private string _securityCashPosition = string.Empty;
        public string SecurityCashPosition
        {
            get { return _securityCashPosition; }
            set
            {
                if (_securityCashPosition != value)
                {
                    _securityCashPosition = value;
                    RaisePropertyChanged(() => this.SecurityCashPosition);
                }
            }

        }

        private string _securityPosition = string.Empty;
        public string SecurityPosition
        {
            get { return _securityPosition; }
            set
            {
                if (_securityPosition != value)
                {
                    _securityPosition = value;
                    RaisePropertyChanged(() => this.SecurityPosition);
                }
            }

        }

        private string _securityMSCIStdWeight = string.Empty;
        public string SecurityMSCIStdWeight
        {
            get { return _securityMSCIStdWeight; }
            set
            {
                if (_securityMSCIStdWeight != value)
                {
                    _securityMSCIStdWeight = value;
                    RaisePropertyChanged(() => this.SecurityMSCIStdWeight);
                }
            }

        }

        private string _securityMSCIIMIWeight = string.Empty;
        public string SecurityMSCIIMIWeight
        {
            get { return _securityMSCIIMIWeight; }
            set
            {
                if (_securityMSCIIMIWeight != value)
                {
                    _securityMSCIIMIWeight = value;
                    RaisePropertyChanged(() => this.SecurityMSCIIMIWeight);
                }
            }

        }

        private string _securityGlobalActiveWeight = string.Empty;
        public string SecurityGlobalActiveWeight
        {
            get { return _securityGlobalActiveWeight; }
            set
            {
                if (_securityGlobalActiveWeight != value)
                {
                    _securityGlobalActiveWeight = value;
                    RaisePropertyChanged(() => this.SecurityGlobalActiveWeight);
                }
            }

        }

        private string _securityLastClosingPrice = string.Empty;
        public string SecurityLastClosingPrice
        {
            get { return _securityLastClosingPrice; }
            set
            {
                if (_securityLastClosingPrice != value)
                {
                    _securityLastClosingPrice = value;
                    RaisePropertyChanged(() => this.SecurityLastClosingPrice);
                }
            }

        }

        private string _securityMarketCapitalization = string.Empty;
        public string SecurityMarketCapitalization
        {
            get { return _securityMarketCapitalization; }
            set
            {
                if (_securityMarketCapitalization != value)
                {
                    _securityMarketCapitalization = value;
                    RaisePropertyChanged(() => this.SecurityMarketCapitalization);
                }
            }

        }

        private string _securityPFVMeasure = string.Empty;
        public string SecurityPFVMeasure
        {
            get { return _securityPFVMeasure; }
            set
            {
                if (_securityPFVMeasure != value)
                {
                    _securityPFVMeasure = value;
                    RaisePropertyChanged(() => this.SecurityPFVMeasure);
                }
            }

        }

        private string _securityBuyRange = string.Empty;
        public string SecurityBuyRange
        {
            get { return _securityBuyRange; }
            set
            {
                if (_securityBuyRange != value)
                {
                    _securityBuyRange = value;
                    RaisePropertyChanged(() => this.SecurityBuyRange);
                    RaisePropertyChanged(() => this.SecurityBuySellRange);
                    RaisePropertyChanged(() => this.SecurityRecommendation);
                }
            }

        }

        private string _securitySellRange = string.Empty;
        public string SecuritySellRange
        {
            get { return _securitySellRange; }
            set
            {
                if (_securitySellRange != value)
                {
                    _securitySellRange = value;
                    RaisePropertyChanged(() => this.SecuritySellRange);
                    RaisePropertyChanged(() => this.SecurityBuySellRange);
                    RaisePropertyChanged(() => this.SecurityRecommendation);
                }
            }

        }

        private string _securityBuySellRange = string.Empty;
        public string SecurityBuySellRange
        {
            get
            {
                if ( SecurityBuyRange != string.Empty && SecuritySellRange != string.Empty)
                    _securityBuySellRange = SecurityBuyRange + " - " + SecuritySellRange;
                return _securityBuySellRange;
            }
            set
            {
                if (_securityBuySellRange != value)
                {
                    _securityBuySellRange = value;
                    RaisePropertyChanged(() => this.SecurityBuySellRange);
                    RaisePropertyChanged(() => this.SecurityRecommendation);
                }
            }
        }

        private string _securityRecommendation = string.Empty;
        public string SecurityRecommendation
        {
            get
            {
                if (SecurityBuyRange != string.Empty && SecuritySellRange != string.Empty && SecurityPosition != string.Empty)
                {
                    _securityRecommendation = "Buy";
                    //if (Double.Parse(SecurityPosition) != 0)
                    //{
                    //    if (Double.Parse(SecurityLastClosingPrice) <= Double.Parse(SecuritySellRange))
                    //        _securityRecommendation =  "Hold";
                    //    else if (Double.Parse(SecurityLastClosingPrice) > Double.Parse(SecuritySellRange))
                    //        _securityRecommendation = "Sell";
                    //}
                    //else
                    //{
                    //    if (Double.Parse(SecurityLastClosingPrice) > Double.Parse(SecurityBuyRange))
                    //        _securityRecommendation = "Watch";
                    //    else if (Double.Parse(SecurityLastClosingPrice) <= Double.Parse(SecurityBuyRange))
                    //        _securityRecommendation = "Buy";
                    //}
                }
                return _securityRecommendation;
            }
            set
            {
                if (_securityRecommendation != value)
                {
                    _securityRecommendation = value;
                    RaisePropertyChanged(() => this.SecurityRecommendation);
                }
            }
        }

        private string _investmentThesis = string.Empty; 
        public string InvestmentThesis
        {
            get { return _investmentThesis; }
            set
            {
                if (_investmentThesis != value)
                {
                    _investmentThesis = value;
                    RaisePropertyChanged(() => this.InvestmentThesis);
                }
            }
        }

        private string _background = string.Empty;
        public string Background
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    RaisePropertyChanged(() => this.Background);
                }
            }
        }

        private string _valuations = string.Empty;
        public string Valuations
        {
            get { return _valuations; }
            set
            {
                if (_valuations != value)
                {
                    _valuations = value;
                    RaisePropertyChanged(() => this.Valuations);
                }
            }
        }

        private string _earningsOutlook = string.Empty;
        public string EarningsOutlook
        {
            get { return _earningsOutlook; }
            set
            {
                if (_earningsOutlook != value)
                {
                    _earningsOutlook = value;
                    RaisePropertyChanged(() => this.EarningsOutlook);
                }
            }
        }

        private string _competitiveAdvantage = string.Empty;
        public string CompetitiveAdvantage
        {
            get { return _competitiveAdvantage; }
            set
            {
                if (_competitiveAdvantage != value)
                {
                    _competitiveAdvantage = value;
                    RaisePropertyChanged(() => this.CompetitiveAdvantage);
                }
            }
        }

        private string _competitiveDisadvantage = string.Empty;
        public string CompetitiveDisadvantage
        {
            get { return _competitiveDisadvantage; }
            set
            {
                if (_competitiveDisadvantage != value)
                {
                    _competitiveDisadvantage = value;
                    RaisePropertyChanged(() => this.CompetitiveDisadvantage);
                }
            }
        }

        
        
        
        public ICPPresentationInfo()
        {

        }

        public ICPPresentationInfo(PresentationInfoResult result)
        {
            this.PresentationID = result.PresentationID;
            this.Presenter = result.Presenter.ToString();
            this.PresentationDate = result.PresentationDate;
            this.Status = result.StatusType.ToString();
            this.StatusTypeID = result.StatusTypeID;
            this.SecurityTicker = result.SecurityTicker.ToString();
            this.SecurityName = result.SecurityName.ToString();
            this.SecurityCountry = result.SecurityCountry.ToString();
            this.SecurityCountryCode = result.SecurityCountryCode.ToString();
            this.SecurityIndustry = result.SecurityIndustry.ToString();
            this.SecurityCashPosition = result.SecurityCashPosition.ToString();
            this.SecurityPosition = result.SecurityPosition.ToString();
            this.SecurityMSCIStdWeight = result.SecurityMSCIStdWeight.ToString();
            this.SecurityMSCIIMIWeight = result.SecurityMSCIIMIWeight.ToString();
            this.SecurityGlobalActiveWeight = result.SecurityGlobalActiveWeight.ToString();
            this.SecurityLastClosingPrice = result.SecurityLastClosingPrice.ToString();
            this.SecurityMarketCapitalization = result.SecurityMarketCapitalization.ToString();
            this.SecurityPFVMeasure = result.SecurityPFVMeasure.ToString();
            this.SecurityBuyRange = result.SecurityBuyRange.ToString();
            this.SecuritySellRange = result.SecuritySellRange.ToString();
            if (result.InvestmentThesis != null) this.InvestmentThesis = result.InvestmentThesis.ToString();
            if (result.Background != null) this.Background = result.Background.ToString();
            if (result.Valuations != null) this.Valuations = result.Valuations.ToString();
            if (result.EarningsOutlook != null) this.EarningsOutlook = result.EarningsOutlook.ToString();
            if (result.CompetitiveAdvantage != null) this.CompetitiveAdvantage = result.CompetitiveAdvantage.ToString();
            if (result.CompetitiveDisadvantage != null) this.CompetitiveDisadvantage = result.CompetitiveDisadvantage.ToString();

        }

        public PresentationInfo ConvertToDB()
        {
            PresentationInfo pinfo = new PresentationInfo();
            pinfo.PresentationID = this.PresentationID;
            pinfo.Presenter = this.Presenter;
            pinfo.StatusTypeID = this.StatusTypeID;
            pinfo.SecurityTicker = this.SecurityTicker;
            pinfo.SecurityName = this.SecurityName;
            pinfo.SecurityCountry = this.SecurityCountry;
            pinfo.SecurityCountryCode = this.SecurityCountryCode;
            pinfo.SecurityIndustry = this.SecurityIndustry;
            try { pinfo.SecurityCashPosition = float.Parse(this.SecurityCashPosition); } catch { pinfo.SecurityCashPosition = 0; }
            try { pinfo.SecurityPosition = long.Parse(this.SecurityPosition); } catch { pinfo.SecurityPosition = 0; }
            try { pinfo.SecurityMSCIStdWeight = float.Parse(this.SecurityMSCIStdWeight); } catch { pinfo.SecurityMSCIStdWeight = 0; }
            try { pinfo.SecurityMSCIIMIWeight = float.Parse(this.SecurityMSCIIMIWeight); } catch { pinfo.SecurityMSCIIMIWeight = 0; }
            try { pinfo.SecurityGlobalActiveWeight = float.Parse(this.SecurityGlobalActiveWeight); } catch { pinfo.SecurityGlobalActiveWeight = 0; }
            try { pinfo.SecurityLastClosingPrice = float.Parse(this.SecurityLastClosingPrice); } catch { pinfo.SecurityLastClosingPrice = 0; }
            try { pinfo.SecurityMarketCapitalization = float.Parse(this.SecurityMarketCapitalization); } catch { pinfo.SecurityMarketCapitalization = 0; }
            pinfo.SecurityPFVMeasure = this.SecurityPFVMeasure;
            try { pinfo.SecurityBuyRange = float.Parse(this.SecurityBuyRange); } catch { pinfo.SecurityBuyRange = 0; }
            try { pinfo.SecuritySellRange = float.Parse(this.SecuritySellRange); } catch { pinfo.SecuritySellRange = 0; }
            pinfo.SecurityRecommendation = this.SecurityRecommendation;
            pinfo.InvestmentThesis = this.InvestmentThesis;
            pinfo.Background = this.Background;
            pinfo.Valuations = this.Valuations;
            pinfo.EarningsOutlook = this.EarningsOutlook;
            pinfo.CompetitiveAdvantage = this.CompetitiveAdvantage;
            pinfo.CompetitiveDisadvantage = this.CompetitiveDisadvantage;
            pinfo.CreatedBy = "UserLoggedIn";
            pinfo.CreatedOn = DateTime.Now;
            pinfo.ModifiedBy = "UserLoggedIn";
            pinfo.ModifiedOn = DateTime.Now;
            return pinfo;
        }


    }
}
