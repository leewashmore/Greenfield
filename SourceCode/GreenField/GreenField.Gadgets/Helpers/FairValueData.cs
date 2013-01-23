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

namespace GreenField.Gadgets.Helpers
{
    public class FairValueData : NotificationObject
    {
        /// <summary>
        /// Source of FairValueData
        /// </summary>
        private String source;
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    source = value;
                    SourceLabel = value;
                    if (source == "Primary" || source == "Industry")
                        SourceLabel += " Analyst";
                    RaisePropertyChanged(() => this.Source);
                }
            }
        }

        /// <summary>
        /// Source of FairValueDataLabel
        /// </summary>
        private String sourceLabel;
        public string SourceLabel
        {
            get
            {
                return sourceLabel;
            }
            set
            {
                if (sourceLabel != value)
                {
                    sourceLabel = value;
                    RaisePropertyChanged(() => this.SourceLabel);
                }
            }
        }

        /// <summary>
        /// Measure of FairValueData
        /// </summary>
        private String measure;
        public string Measure
        {
            get
            {
                return measure;
            }
            set
            {
                if (measure != value)
                {
                    measure = value;
                    RaisePropertyChanged(() => this.Measure);
                }
            }
        }

        /// <summary>
        /// Buy Value of FairValueData
        /// </summary>
        private decimal? buy;
        public decimal? Buy
        {
            get
            {
                return buy;
            }
            set
            {
                if (buy != value)
                {
                    buy = value;
                    RaisePropertyChanged(() => this.Buy);
                }
            }
        }

        /// <summary>
        /// Sell Value of FairValueData
        /// </summary>
        private decimal? sell;
        public decimal? Sell
        {
            get
            {
                return sell;
            }
            set
            {
                if (sell != value)
                {
                    sell = value;
                    RaisePropertyChanged(() => this.Sell);
                }
            }
        }

        /// <summary>
        /// Upside Value of FairValueData
        /// </summary>
        private decimal? upside;
        public decimal? Upside
        {
            get
            {
                return upside;
            }
            set
            {
                if (upside != value)
                {
                    upside = value;
                    RaisePropertyChanged(() => this.Upside);
                }
            }
        }

        /// <summary>
        /// Updated Date Value of FairValueData
        /// </summary>
        private DateTime? date;
        public DateTime? Date
        {
            get
            {
                return date;
            }
            set
            {
                if (date != value)
                {
                    date = value;
                    RaisePropertyChanged(() => this.Date);
                }
            }
        }

        /// <summary>
        /// DataId of FairValueData
        /// </summary>
        private Int32? dataId;
        public Int32? DataId
        {
            get
            {
                return dataId;
            }
            set
            {
                if (dataId != value)
                {
                    dataId = value;
                    RaisePropertyChanged(() => this.DataId);
                }
            }
        }

        /// <summary>
        /// Verifies whether original item has been updated or not
        /// </summary>
        private bool isUpdated = false;
        public bool IsUpdated
        {
            get
            {
                return isUpdated;
            }
            set
            {
                if (isUpdated != value)
                {
                    isUpdated = value;
                    RaisePropertyChanged(() => this.IsUpdated);
                }
            }
        }

        /// <summary>
        /// Indicates whether the row is ReadOnly or not
        /// </summary>
        private bool isReadOnly = true;
        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    RaisePropertyChanged(() => this.IsReadOnly);
                }
            }
        }
    }
}
