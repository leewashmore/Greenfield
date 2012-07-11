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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.Collections.Generic;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.DataContracts;

namespace GreenField.Common.Helper
{
    public class DashboardGadgetPayload
    {
        public EntitySelectionData EntitySelectionData { get; set; }

        public PortfolioSelectionData PortfolioSelectionData { get; set; }

        public BenchmarkSelectionData BenchmarkSelectionData { get; set; }

        private DateTime? _effectiveDate = DateTime.SpecifyKind(DateTime.Now.AddDays(-1).Date, DateTimeKind.Unspecified);
        public DateTime? EffectiveDate
        {
            get
            {
                return _effectiveDate;
            }
            set
            {
                _effectiveDate = value;
            }
        }
       
        public MarketSnapshotSelectionData MarketSnapshotSelectionData { get; set; }

        public FilterSelectionData FilterSelectionData { get; set; }

        public String CountrySelectionData { get; set; }

        private String periodSelectionData = "YTD";

        public String PeriodSelectionData
        {
            get
            {
                return periodSelectionData; 
            }

            set
            { 
                periodSelectionData = value; 
            }
        }

        public bool IsExCashSecurityData { get; set; }

        public string CommoditySelectedVal { get; set; }

        public String HeatMapCountryData { get; set; }

        public List<String> RegionFXData { get; set; }

        public bool IsLookThruEnabled { get; set; }

        private String nodeNameSelectionData = "Country";

        public String NodeNameSelectionData
        {
            get
            {
                return nodeNameSelectionData;
            }
            set
            {
                nodeNameSelectionData = value;
            }
        }

    }
}
