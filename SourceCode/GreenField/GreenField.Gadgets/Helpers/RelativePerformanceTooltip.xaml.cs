using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.ServiceCaller;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.Helpers
{
    public partial class RelativePerformanceTooltip : UserControl
    {
        public RelativePerformanceTooltip(IDBInteractivity dbInteractivity, FundSelectionData fundSelectionData, BenchmarkSelectionData benchmarkSelectionData, DateTime effectiveDate, string countryID, int? sectorID)
        {
            InitializeComponent();
            dbInteractivity.RetrieveRelativePerformanceSecurityData(fundSelectionData, benchmarkSelectionData, effectiveDate, RetrieveRelativePerformanceSecurityDataCallBackMethod, countryID, sectorID);
            this.CountryID.Text = countryID.ToString();
            this.SectorID.Text = sectorID.ToString();
            
        }

        private ObservableCollection<RelativePerformanceSecurityData> _relativePerformanceToolTipInfo;
        public ObservableCollection<RelativePerformanceSecurityData> RelativePerformanceToolTipInfo
        {
            get { return _relativePerformanceToolTipInfo; }
            set
            {
                _relativePerformanceToolTipInfo = value;
                if (value.Count > 0)
                {
                    this.FirstSecurityName.Text = value[0].SecurityName;
                    this.FirstSecurityAlpha.Text = value[0].SecurityAlpha.ToString();
                    this.FirstSecurityActivePosition.Text = value[0].SecurityActivePosition.ToString();
                }
                if (value.Count > 1)
                {
                    this.SecondSecurityName.Text = value[1].SecurityName;
                    this.SecondSecurityAlpha.Text = value[1].SecurityAlpha.ToString();
                    this.SecondSecurityActivePosition.Text = value[1].SecurityActivePosition.ToString();
                }
                if (value.Count > 2)
                {
                    this.ThirdSecurityName.Text = value[2].SecurityName;
                    this.ThirdSecurityAlpha.Text = value[2].SecurityAlpha.ToString();
                    this.ThirdSecurityActivePosition.Text = value[2].SecurityActivePosition.ToString();
                }
                if (value.Count > 3)
                {
                    this.FourthSecurityName.Text = value[3].SecurityName;
                    this.FourthSecurityAlpha.Text = value[3].SecurityAlpha.ToString();
                    this.FourthSecurityActivePosition.Text = value[3].SecurityActivePosition.ToString();
                }
                if (value.Count > 4)
                {
                    this.FifthSecurityName.Text = value[4].SecurityName;
                    this.FifthSecurityAlpha.Text = value[4].SecurityAlpha.ToString();
                    this.FifthSecurityActivePosition.Text = value[4].SecurityActivePosition.ToString();
                }

            }
        }

        private void RetrieveRelativePerformanceSecurityDataCallBackMethod(List<RelativePerformanceSecurityData> result)
        {
            RelativePerformanceToolTipInfo = new ObservableCollection<RelativePerformanceSecurityData>(result);
        }
    }
}
