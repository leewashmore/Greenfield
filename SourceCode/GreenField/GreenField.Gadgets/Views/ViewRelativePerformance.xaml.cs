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
using Telerik.Windows.Controls.GridView;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRelativePerformance : UserControl
    {
        private List<RelativePerformanceSectorData> _relativePerformanceSectorInfo;
        private FundSelectionData _fundSelectionData;
        private BenchmarkSelectionData _benchmarkSelectionData;
        private DateTime _effectiveDate;
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

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
            _relativePerformanceSectorInfo = e.RelativePerformanceSectorInfo;

            //Clear grid of previous sector info
            for (int columnIndex = 1; columnIndex < this.dgRelativePerformance.Columns.Count - 1; columnIndex++)
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
                    AggregationExpression = Models => string.Format("{0} ({1}%)", aggregateSectorAlpha, aggregateSectorActiviePosition),
                    FunctionName = sectorData.SectorID.ToString()
                };

                dataColumn.AggregateFunctions.Add(aggregateAlphaSumFunction);

                dataColumn.FooterCellStyle = this.Resources["GridViewCustomFooterCellStyle"] as Style;
                
                dgRelativePerformance.Columns.Insert(++cIndex, dataColumn);
            }

            RelativePerformanceInfo = e.RelativePerformanceInfo;

            _fundSelectionData = (this.DataContext as ViewModelRelativePerformance)._fundSelectionData;
            _benchmarkSelectionData = (this.DataContext as ViewModelRelativePerformance)._benchmarkSelectionData;
            _effectiveDate = (this.DataContext as ViewModelRelativePerformance)._effectiveDate;
            _dbInteractivity = (this.DataContext as ViewModelRelativePerformance)._dbInteractivity;
            _eventAggregator = (this.DataContext as ViewModelRelativePerformance)._eventAggregator;
        }        

        private void dgRelativePerformance_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row is GridViewHeaderRow)
                return;
            if (e.Row.Cells[0] is GridViewFooterCell)
                return;
            foreach (GridViewCell cell in e.Row.Cells)
            {
                //Null Check
                if (cell.Value == null)
                    continue;

                //No toolTip service for Blank cells
                if ((cell.Value as RelativePerformanceCountrySpecificData).Alpha == null)
                    continue;

                //No toolTip service for CountryId Column
                if (cell.Column.DisplayIndex == 0)
                    continue;
                
                //No toolTip service for Totals Column
                if (cell.Column.DisplayIndex == this.dgRelativePerformance.Columns.Count - 1)
                    continue;
                
                int cellSectorID = (cell.Value as RelativePerformanceCountrySpecificData).SectorID;
                string cellCountryID = (cell.ParentRow.DataContext as RelativePerformanceData).CountryID;

                ToolTip toolTip = new ToolTip()
                {
                    Content = new RelativePerformanceTooltip(_dbInteractivity, _fundSelectionData, _benchmarkSelectionData, _effectiveDate, cellCountryID, cellSectorID)
                };

                ToolTipService.SetToolTip(cell, toolTip);
            }
        }

        private void dgRelativePerformance_SelectedCellsChanged(object sender, GridViewSelectedCellsChangedEventArgs e)
        {
            //Ignore involuntary selection event
            if (e.AddedCells.Count == 0)
                return;

            //Ignore cells on Column ID column
            if (e.AddedCells[0].Column.DisplayIndex == 0)
                return;

            string countryID = (e.AddedCells[0].Item as RelativePerformanceData).CountryID;
            int? sectorID = null;
            if (e.AddedCells[0].Column.DisplayIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                sectorID = (e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[e.AddedCells[0].Column.DisplayIndex - 1].SectorID;
            }

            _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
            {
                countryID = countryID,
                sectorID = sectorID
            });            
 
        }

        private void FooterCellBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int sectorID;
            bool sectorIDNullValidation = int.TryParse(((e.OriginalSource as TextBlock).DataContext as AggregateResult).FunctionName, out sectorID);
            if (sectorIDNullValidation)
            {
                _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
                {
                    sectorID = sectorID
                });
            }
        }        
    }
}
