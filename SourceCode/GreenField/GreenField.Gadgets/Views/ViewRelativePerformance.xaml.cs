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
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using System.IO;
using System.Collections;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRelativePerformance : ViewBaseUserControl
    {
        #region Fields
        private List<RelativePerformanceSectorData> _relativePerformanceSectorInfo;
        PortfolioSelectionData _PortfolioSelectionData;
        private DateTime? _effectiveDate;
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger; 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DataContextSource"></param>
        public ViewRelativePerformance(ViewModelRelativePerformance DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
            DataContextSource.RelativePerformanceDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceLoadEvent);
            DataContextSource.RelativePerformanceGridBuildEvent += new RelativePerformanceGridBuildEventHandler(DataContextSource_RelativePerformanceGridBuildEvent);
            DataContextSource.RelativePerformanceToggledSectorGridBuildEvent += new RelativePerformanceToggledSectorGridBuildEventHandler(RelativePerformanceToggledSectorGridBuildEvent);
            this.DataContextRelativePerformance = DataContextSource;
        } 
        #endregion

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

        private List<RelativePerformanceSecurityData> _relativePerformanceSecurityInfo;
        public List<RelativePerformanceSecurityData> RelativePerformanceSecurityInfo
        {
            get { return _relativePerformanceSecurityInfo; }
            set
            {
                _relativePerformanceSecurityInfo = value;
               // this.dgSectorSecurityDetails.ItemsSource = value;
            }
        }
       
        #endregion

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

        #region Event Handlers
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
                dataColumn.HeaderCellStyle = this.Resources["GridViewHeaderCellClickable"] as Style;
                dataColumn.FooterCellStyle = this.Resources["GridViewCustomFooterCellStyle"] as Style;

                dgRelativePerformance.Columns.Insert(++cIndex, dataColumn);
            }

            RelativePerformanceInfo = e.RelativePerformanceInfo;

            //_PortfolioSelectionData = (this.DataContext as ViewModelRelativePerformance)._PortfolioSelectionData;
            //_effectiveDate = (this.DataContext as ViewModelRelativePerformance).EffectiveDate;
            //_dbInteractivity = (this.DataContext as ViewModelRelativePerformance)._dbInteractivity;
            _eventAggregator = (this.DataContext as ViewModelRelativePerformance)._eventAggregator;
        }

        void RelativePerformanceToggledSectorGridBuildEvent(RelativePerformanceToggledSectorGridBuildEventArgs e)
        {
            //Clear grid of previous sector info
            for (int columnIndex = 1; columnIndex < this.dgSectorSecurityDetails.Columns.Count - 1; columnIndex++)
            {
                dgSectorSecurityDetails.Columns.RemoveAt(columnIndex);
            }

            int cIndex = 0;

            foreach (string countryName in e.RelativePerformanceCountryNameInfo)
            {
                List<string> s1 = e.RelativePerformanceSecurityInfo.Where(t => t.SecurityCountryID == countryName).Select(t => t.SecurityName).ToList();

                Telerik.Windows.Controls.GridViewDataColumn dataColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                dataColumn.Header = countryName;
                dataColumn.DataMemberBinding = new System.Windows.Data.Binding("RelativePerformanceSecurityInfo[" + countryName + "]");
                //dataColumn.DataMemberBinding = new System.Windows.Data.Binding("RelativePerformanceSecurityData[" + cIndex + "]");

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
                CellTemp.Append("Text = '{Binding RelativePerformanceSecurityInfo[" + countryName + "]}'/>");
                //CellTemp.Append("Text = '{Binding RelativePerformanceSecurityData[" + cIndex + "].SecurityName}'/>");
                CellTemp.Append("</StackPanel>");
                CellTemp.Append("</DataTemplate>");

                dataColumn.CellTemplate = XamlReader.Load(CellTemp.ToString()) as DataTemplate;
                dataColumn.HeaderCellStyle = this.Resources["GridViewHeaderCell"] as Style;
                dgSectorSecurityDetails.Columns.Insert(cIndex++, dataColumn);
            }

            //RelativePerformanceSecurityInfo = e.RelativePerformanceSecurityInfo;

            //_PortfolioSelectionData = (this.DataContext as ViewModelRelativePerformance)._PortfolioSelectionData;
            //_effectiveDate = (this.DataContext as ViewModelRelativePerformance).EffectiveDate;
            //_dbInteractivity = (this.DataContext as ViewModelRelativePerformance)._dbInteractivity;
            _eventAggregator = (this.DataContext as ViewModelRelativePerformance)._eventAggregator;
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
            {
                btnExportExcel.Visibility = Visibility.Collapsed;
                btnExportPDF.Visibility = Visibility.Collapsed;
                btnPrint.Visibility = Visibility.Collapsed;
                dgRelativePerformance.Visibility = Visibility.Collapsed;
                dgSectorSecurityDetails.Visibility = Visibility.Collapsed;
                btnToggle.Visibility = Visibility.Visible;
                dgCountrySecurityDetails.Visibility = Visibility.Visible;

                _eventAggregator.GetEvent<RelativePerformanceGridCountrySectorClickEvent>().Publish(new RelativePerformanceGridCellData()
                {
                    CountryID = (e.AddedCells[0].Item as RelativePerformanceData).CountryID,
                    SectorID = null,
                });
                return;
            }

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

        #region Helper Methods

        private void btn_ToggleClick(object sender, RoutedEventArgs e)
        {
            btnExportExcel.Visibility = Visibility.Visible;
            btnExportPDF.Visibility = Visibility.Visible;
            btnPrint.Visibility = Visibility.Visible;
            dgRelativePerformance.Visibility = Visibility.Visible;
            btnToggle.Visibility = Visibility.Collapsed;
            dgSectorSecurityDetails.Visibility = Visibility.Collapsed;
            dgCountrySecurityDetails.Visibility = Visibility.Collapsed;
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

        private void CustomHeaderCellBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                string sectorID = (e.OriginalSource as TextBlock).Text.ToString();
                _eventAggregator.GetEvent<RelativePerformanceGridCountrySectorClickEvent>().Publish(new RelativePerformanceGridCellData()
                {
                    CountryID = null,
                    SectorID = sectorID
                });

                btnExportExcel.Visibility = Visibility.Collapsed;
                btnExportPDF.Visibility = Visibility.Collapsed;
                btnPrint.Visibility = Visibility.Collapsed;
                dgRelativePerformance.Visibility = Visibility.Collapsed;
                btnToggle.Visibility = Visibility.Visible;
                dgCountrySecurityDetails.Visibility = Visibility.Collapsed;
                dgSectorSecurityDetails.Visibility = Visibility.Visible;
            }
        }

        #endregion

        //#region Export To Pdf Methods

        ///// <summary>
        ///// Event handler when user wants to Export the Grid to PDF
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnExportToPdf_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog dialog = new SaveFileDialog();
        //    dialog.DefaultExt = "*.pdf";
        //    dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

        //    if (dialog.ShowDialog() == true)
        //    {
        //        RadDocument document = CreateDocument(dgRelativePerformance);
        //        document.LayoutMode = DocumentLayoutMode.Paged;
        //        document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
        //        document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
        //        PdfFormatProvider provider = new PdfFormatProvider();
        //        using (Stream output = dialog.OpenFile())
        //        {
        //            provider.Export(document, output);
        //        }
        //    }
        //}       

        //private RadDocument CreateDocument(RadGridView grid)
        //{
        //    List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
        //                                             orderby c.DisplayIndex
        //                                             select c).ToList();
        //    Table table = new Table();
        //    RadDocument document = new RadDocument();
        //    Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
        //    section.Blocks.Add(table);
        //    document.Sections.Add(section);

        //    if (grid.ShowColumnHeaders)
        //    {
        //        TableRow headerRow = new TableRow();
        //        if (grid.GroupDescriptors.Count() > 0)
        //        {
        //            TableCell indentCell = new TableCell();
        //            indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
        //            indentCell.Background = Colors.Gray;
        //            headerRow.Cells.Add(indentCell);
        //        }

        //        for (int i = 0; i < columns.Count(); i++)
        //        {
        //            TableCell cell = new TableCell();
        //            cell.Background = Colors.White;
        //            AddCellValue(cell, columns[i].UniqueName);
        //            cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
        //            headerRow.Cells.Add(cell);
        //        }

        //        table.Rows.Add(headerRow);
        //    }

        //    if (grid.Items.Groups != null)
        //    {
        //        for (int i = 0; i < grid.Items.Groups.Count(); i++)
        //        {
        //            AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid);
        //        }
        //    }
        //    else
        //    {
        //        AddDataRows(table, grid.Items, columns, grid);
        //    }

        //    return document;
        //}

        //private void AddDataRows(Table table, IList items, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        //{
        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        TableRow row = new TableRow();

        //        if (grid.GroupDescriptors.Count() > 0)
        //        {
        //            TableCell indentCell = new TableCell();
        //            indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
        //            indentCell.Background = Colors.White;
        //            row.Cells.Add(indentCell);
        //        }

        //        for (int j = 0; j < columns.Count(); j++)
        //        {
        //            TableCell cell = new TableCell();

        //            object value = columns[j].GetValueForItem(items[i]);

        //            AddCellValue(cell, value != null ? value.ToString() : string.Empty);

        //            cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
        //            cell.Background = Colors.White;

        //            row.Cells.Add(cell);
        //        }

        //        table.Rows.Add(row);
        //    }
        //}

        //private void AddGroupRow(Table table, QueryableCollectionViewGroup group, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        //{
        //    TableRow row = new TableRow();

        //    int level = GetGroupLevel(group);
        //    if (level > 0)
        //    {
        //        TableCell cell = new TableCell();
        //        cell.PreferredWidth = new TableWidthUnit(level * 20);
        //        cell.Background = Colors.White;
        //        row.Cells.Add(cell);
        //    }

        //    TableCell aggregatesCell = new TableCell();
        //    aggregatesCell.Background = Colors.White;
        //    aggregatesCell.ColumnSpan = columns.Count() + (grid.GroupDescriptors.Count() > 0 ? 1 : 0) - (level > 0 ? 1 : 0);

        //    AddCellValue(aggregatesCell, group.Key != null ? group.Key.ToString() : string.Empty);

        //    foreach (AggregateResult result in group.AggregateResults)
        //    {
        //        AddCellValue(aggregatesCell, result.FormattedValue != null ? result.FormattedValue.ToString() : string.Empty);
        //    }

        //    row.Cells.Add(aggregatesCell);

        //    table.Rows.Add(row);

        //    if (group.HasSubgroups)
        //    {
        //        for (int i = 0; i < group.Subgroups.Count(); i++)
        //        {
        //            AddGroupRow(table, group.Subgroups[i] as QueryableCollectionViewGroup, columns, grid);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < group.ItemCount; i++)
        //        {
        //            AddDataRows(table, group.Items, columns, grid);
        //        }
        //    }
        //}

        //private void AddCellValue(TableCell cell, string value)
        //{
        //    Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
        //    cell.Blocks.Add(paragraph);
        //    Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
        //    span.Text = value;
        //    paragraph.Inlines.Add(span);
        //}

        //private int GetGroupLevel(IGroup group)
        //{
        //    int level = 0;
        //    IGroup parent = group.ParentGroup;
        //    while (parent != null)
        //    {
        //        level++;
        //        parent = parent.ParentGroup;
        //    }
        //    return level;
        //} 

        //#endregion

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
