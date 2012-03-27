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
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Windows.Markup;
using System.Text;
using Telerik.Windows.Data;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRelativePerformance : UserControl
    {
        public ViewRelativePerformance(ViewModelRelativePerformance DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
            DataContextSource.RelativePerformanceGridBuildEvent += new RelativePerformanceGridBuild(DataContextSource_RelativePerformanceGridBuildEvent);
        }

        private List<RelativePerformanceData> _relativePerformanceInfo;
        public List<RelativePerformanceData> RelativePerformanceInfo
        {
            get { return _relativePerformanceInfo; }
            set
            {
                _relativePerformanceInfo = value;
                this.dgRelativePerformance.ItemsSource = value;

            }
        }

        void DataContextSource_RelativePerformanceGridBuildEvent(RelativePerformanceGridBuildEventArgs e)
        {
            
            //Clear grid of previous sector info
            for (int columnIndex = 1; columnIndex < dgRelativePerformance.Columns.Count - 1; columnIndex++)
            {
                dgRelativePerformance.Columns.RemoveAt(columnIndex);
            }

            int cIndex = 0;

            foreach (RelativePerformanceSectorData sectorData in e.RelativePerformanceSectorInfo)
            {
                Telerik.Windows.Controls.GridViewDataColumn dataColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                dataColumn.Header = sectorData.SectorName;
                dataColumn.DataMemberBinding = new System.Windows.Data.Binding("RelativePerformanceCountrySpecificInfo[" + cIndex + "]");

                StringBuilder CellTemp = new StringBuilder();
                CellTemp.Append("<DataTemplate ");
                CellTemp.Append("xmlns='http://schemas.microsoft.com/winfx/");
                CellTemp.Append("2006/xaml/presentation' ");
                CellTemp.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");

                //Be sure to replace "YourNamespace" and "YourAssembly" with your app's 
                //actual namespace and assembly here
                CellTemp.Append("xmlns:local = 'clr-namespace:GreenField.Gadgets.Views");
                CellTemp.Append(";assembly=GreenField.Gadgets'>");

                CellTemp.Append("<StackPanel Orientation='Horizontal'>");
                CellTemp.Append("<TextBlock ");
                CellTemp.Append("Text = '{Binding RelativePerformanceCountrySpecificInfo[" + cIndex + "].Alpha}'/>");
                CellTemp.Append("<TextBlock ");
                CellTemp.Append("Text = '{Binding RelativePerformanceCountrySpecificInfo[" + cIndex + "].ActivePosition, StringFormat=(\\{0:n2\\}%)}'/>");
                CellTemp.Append("</StackPanel>");
                CellTemp.Append("</DataTemplate>");

                dataColumn.CellTemplate = XamlReader.Load(CellTemp.ToString()) as DataTemplate;
                double? aggregateSectorAlphaValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex)).Sum(t => t.Alpha == null ? 0 : t.Alpha);
                string aggregateSectorAlpha = aggregateSectorAlphaValue == null ? String.Empty : Math.Round(Decimal.Parse(aggregateSectorAlphaValue.ToString()), 2).ToString();
                double? aggregateSectorActiviePositionValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex)).Sum(t => t.ActivePosition == null ? 0 : t.ActivePosition);
                string aggregateSectorActiviePosition = aggregateSectorActiviePositionValue == null ? String.Empty : Math.Round(Decimal.Parse(aggregateSectorActiviePositionValue.ToString()), 2).ToString();
                
                

                var aggregateAlphaSumFunction = new AggregateFunction<RelativePerformanceData, string>
                {
                    AggregationExpression = Models => string.Format("{0} ({1}%)", aggregateSectorAlpha, aggregateSectorActiviePosition)
                };
                dataColumn.AggregateFunctions.Add(aggregateAlphaSumFunction);
                
                dgRelativePerformance.Columns.Insert(++cIndex, dataColumn);
            }

            RelativePerformanceInfo = e.RelativePerformanceInfo;
        }

        private void dgRelativePerformance_MouseMove(object sender, MouseEventArgs e)
        {
            
        }


        
    }
}
