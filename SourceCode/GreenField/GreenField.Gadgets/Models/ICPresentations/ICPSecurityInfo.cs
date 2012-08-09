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

namespace GreenField.Gadgets.Models
{

    public class ICPSecurityInfo : NotificationObject
    {
        
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
                if (SecurityBuyRange != string.Empty && SecuritySellRange != string.Empty)
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
                    if (Double.Parse(SecurityPosition) != 0)
                    {
                        if (Double.Parse(SecurityLastClosingPrice) <= Double.Parse(SecuritySellRange))
                            _securityRecommendation = "Hold";
                        else if (Double.Parse(SecurityLastClosingPrice) > Double.Parse(SecuritySellRange))
                            _securityRecommendation = "Sell";
                    }
                    else
                    {
                        if (Double.Parse(SecurityLastClosingPrice) > Double.Parse(SecurityBuyRange))
                            _securityRecommendation = "Watch";
                        else if (Double.Parse(SecurityLastClosingPrice) <= Double.Parse(SecurityBuyRange))
                            _securityRecommendation = "Buy";
                    }
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
    }
}
