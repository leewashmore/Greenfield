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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Windows.Markup;
using System.Text;
using Telerik.Windows.Data;
using Telerik.Windows.Controls.GridView;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Controls;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRelativePerformance : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformance _dataContextRelativePerformance;
        public ViewModelRelativePerformance DataContextRelativePerformance
        {
            get { return _dataContextRelativePerformance; }
            set { _dataContextRelativePerformance = value; }
        }

        #endregion

        private List<RelativePerformanceSectorData> _relativePerformanceSectorInfo;
        PortfolioSelectionData _PortfolioSelectionData;
        private DateTime? _effectiveDate;
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        public ViewRelativePerformance(ViewModelRelativePerformance DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
            DataContextSource.RelativePerformanceDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceLoadEvent);
            DataContextSource.RelativePerformanceGridBuildEvent += new RelativePerformanceGridBuildEventHandler(DataContextSource_RelativePerformanceGridBuildEvent);
            this.DataContextRelativePerformance = DataContextSource;
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
                //CellTemp.Append("<TextBlock ");
                //CellTemp.Append("Text = '{Binding RelativePerformanceCountrySpecificInfo[" + cIndex + "].ActivePosition, StringFormat= (\\{0:n2\\}%)}'/>");
                CellTemp.Append("</StackPanel>");
                CellTemp.Append("</DataTemplate>");

                dataColumn.CellTemplate = XamlReader.Load(CellTemp.ToString()) as DataTemplate;
                decimal? aggregateSectorAlphaValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex)).Sum(t => t.Alpha == null ? 0 : t.Alpha);
                string aggregateSectorAlpha = aggregateSectorAlphaValue == null ? String.Empty : Math.Round(Decimal.Parse(aggregateSectorAlphaValue.ToString()), 2).ToString();
                decimal? aggregateSectorActiviePositionValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex)).Sum(t => t.ActivePosition == null ? 0 : t.ActivePosition);
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

            _PortfolioSelectionData = (this.DataContext as ViewModelRelativePerformance)._PortfolioSelectionData;
            _effectiveDate = (this.DataContext as ViewModelRelativePerformance).EffectiveDate;
            _dbInteractivity = (this.DataContext as ViewModelRelativePerformance)._dbInteractivity;
            _eventAggregator = (this.DataContext as ViewModelRelativePerformance)._eventAggregator;
        }

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceRelativePerformanceLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }
        #endregion

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
                
                //string cellSectorID = (cell.Value as RelativePerformanceCountrySpecificData).SectorID;
                //string cellCountryID = (cell.ParentRow.DataContext as RelativePerformanceData).CountryID;
                decimal? activePosition = (cell.ParentRow.DataContext as RelativePerformanceData).AggregateCountryActivePosition;

                ToolTip toolTip = new ToolTip()
                {
                    //Content = new RelativePerformanceTooltip(_dbInteractivity, _PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), cellCountryID, cellSectorID)
                    Content = activePosition,
                };

                ToolTipService.SetToolTip(cell, toolTip);
            }
        }

        private void dgRelativePerformance_SelectedCellsChanged(object sender, GridViewSelectedCellsChangedEventArgs e)
        {
            //Ignore involuntary selection event
            if (e.AddedCells.Count == 0)
                return;

            int selectedColumnIndex = e.AddedCells[0].Column.DisplayIndex;

            //Ignore cells on Column ID column
            if (selectedColumnIndex == 0)
                return;

            //Ignore null cells
            if (selectedColumnIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                if (((e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[selectedColumnIndex - 1] as RelativePerformanceCountrySpecificData).Alpha == null)
                    return;
            }

            string countryID = (e.AddedCells[0].Item as RelativePerformanceData).CountryID;
            string sectorID = null;
            if (selectedColumnIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                sectorID = ((e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[e.AddedCells[0].Column.DisplayIndex - 1] as RelativePerformanceCountrySpecificData).SectorID;
            }

            _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
            {
                CountryID = countryID,
                SectorID = sectorID,                
            });            
 
        }

        private void FooterCellBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            

            if (e.OriginalSource is TextBlock)
            {
                string sectorID = ((e.OriginalSource as TextBlock).DataContext as AggregateResult).FunctionName.ToString();
                _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
                {
                    SectorID = sectorID
                });
            }
        }

        #region Export To Excel Methods
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ChildExportOptions childExportOptions = new ChildExportOptions
                (
                new List<RadExportOptions>
                {
                    new RadExportOptions() 
                    {
                        Element = this.dgRelativePerformance,
                        ElementName = "Relative Performace Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE);
            childExportOptions.Show();
        }

        private void dgRelativePerformance_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, () =>
            {
                if (e.Value is RelativePerformanceData)
                {
                    RelativePerformanceData value = e.Value as RelativePerformanceData;
                    int columnIndex = (e.Context as GridViewDataColumn).DisplayIndex;
                    if (columnIndex == 0)
                    {
                        return value.CountryID;
                    }
                    else if (columnIndex == this.dgRelativePerformance.Columns.Count - 1)
                    {
                        string result = value.AggregateCountryAlpha.ToString()
                            + "(" + Math.Round((decimal)value.AggregateCountryActivePosition, 2).ToString() + "%)";
                        return result;
                    }
                }

                if (e.Value is RelativePerformanceCountrySpecificData)
                {
                    RelativePerformanceCountrySpecificData value = e.Value as RelativePerformanceCountrySpecificData;

                    string result = String.Empty;
                    if (value.Alpha != null)
                    {
                        result = value.Alpha.ToString() + "(" + Math.Round((decimal)value.ActivePosition, 2).ToString() + "%)";
                    }
                    return result;
                }
                return e.Value;
            });
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformance.Dispose();
            this.DataContextRelativePerformance.RelativePerformanceDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceLoadEvent);
            this.DataContextRelativePerformance.RelativePerformanceGridBuildEvent -= new RelativePerformanceGridBuildEventHandler(DataContextSource_RelativePerformanceGridBuildEvent);
            this.DataContextRelativePerformance= null;
            this.DataContext = null;
        }
        #endregion
    }
}
