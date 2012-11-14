using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View for RelativePerformance class
    /// </summary>
    public partial class ViewRelativePerformance : ViewBaseUserControl
    {
        #region Fields
        //Gadget Data
        private List<RelativePerformanceSectorData> relativePerformanceSectorInfo;

        //MEF Singletons
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;

        //private details
        Canvas canvas;
        double totalHeight;
        RadGridView grid;
        double offsetY;

        //total rows data
        private string countryTotal;

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextRelativePerformance != null)
                {
                    DataContextRelativePerformance.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformance(ViewModelRelativePerformance dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.RelativePerformanceGridBuildEvent += new RelativePerformanceGridBuildEventHandler(DataContextSource_RelativePerformanceGridBuildEvent);
            this.DataContextRelativePerformance = dataContextSource;
            this.AddHandler(GridViewHeaderCell.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MouseDownOnHeaderCell), true);
        }
        #endregion

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformance dataContextRelativePerformance;
        public ViewModelRelativePerformance DataContextRelativePerformance
        {
            get { return dataContextRelativePerformance; }
            set { dataContextRelativePerformance = value; }
        }

        /// <summary>
        /// contains data to be displayed in the grid
        /// </summary>
        private List<RelativePerformanceData> relativePerformanceInfo;
        public List<RelativePerformanceData> RelativePerformanceInfo
        {
            get { return relativePerformanceInfo; }
            set
            {
                relativePerformanceInfo = value;
                this.dgRelativePerformance.ItemsSource = value;
            }
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// setting tooltip on grid cells when is getting grid loaded with data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRelativePerformance_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);

            if (e.Row is GridViewHeaderRow)
            { return; }

            if (e.Row.Cells[0] is GridViewFooterCell)
            { return; }

            foreach (GridViewCell cell in e.Row.Cells)
            {
                //null check
                if (cell.Value == null)
                { continue; }

                //no tooltip service for CountryId column
                if (cell.Column.DisplayIndex == 0)
                { continue; }

                //no tooltip service for Totals column
                if (cell.Column.DisplayIndex == this.dgRelativePerformance.Columns.Count - 1)
                { continue; }

                //no toolTip service for blank cells
                if ((cell.Value as RelativePerformanceCountrySpecificData).Alpha == null)
                { continue; }

                decimal? activePosition = (cell.Value as RelativePerformanceCountrySpecificData).ActivePosition;

                ToolTip toolTip = new ToolTip()
                {
                    Content = activePosition
                };

                ToolTipService.SetToolTip(cell, toolTip);
            }
        }

        /// <summary>
        /// catch click on header cells
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MouseDownOnHeaderCell(object sender, MouseEventArgs args)
        {
            if (dgRelativePerformance.Items.Count != 0)
            {
                GridViewHeaderCell cellClicked = ((FrameworkElement)args.OriginalSource).ParentOfType<GridViewHeaderCell>();
                if (cellClicked == null)
                {
                    GridViewFooterCell footerTotalClicked = ((FrameworkElement)args.OriginalSource).ParentOfType<GridViewFooterCell>();
                    if (footerTotalClicked == null)
                    {
                        return;
                    }

                    if ((args.OriginalSource as TextBlock).Text == "Total")
                    {
                        eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
                        {
                            CountryID = null,
                            SectorID = null,
                        });
                        this.dgRelativePerformance.SelectedItems.Clear();
                    }
                }
                else
                {
                    if (cellClicked.Column.UniqueName == "Total")
                    {
                        eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
                        {
                            CountryID = null,
                            SectorID = null,
                        });
                        this.dgRelativePerformance.SelectedItems.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// identifying the cell clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRelativePerformance_SelectedCellsChanged(object sender, GridViewSelectedCellsChangedEventArgs e)
        {
            //ignore involuntary selection event
            if (e.AddedCells.Count == 0)
            {
                return;
            }

            int selectedColumnIndex = e.AddedCells[0].Column.DisplayIndex;

            //when cells on Column ID column, toggled grid is displayed
            if (selectedColumnIndex == 0)
            {
                return;
            }

            //ignore null cells
            if (selectedColumnIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                if (((e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[selectedColumnIndex - 1]
                    as RelativePerformanceCountrySpecificData).Alpha == null)
                {
                    return; 
                }
            }

            //catching cells clicked in sector columns
            string countryID = (e.AddedCells[0].Item as RelativePerformanceData).CountryId;
            string sectorID = null;
            if (selectedColumnIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                sectorID = ((e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[e.AddedCells[0].Column.DisplayIndex - 1] 
                    as RelativePerformanceCountrySpecificData).SectorId;
            }

            eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
            {
                CountryID = countryID,
                SectorID = sectorID,
            });
        }

        /// <summary>
        /// building grid for the gadget
        /// </summary>
        /// <param name="e"></param>
        void DataContextSource_RelativePerformanceGridBuildEvent(RelativePerformanceGridBuildEventArgs e)
        {
            relativePerformanceSectorInfo = e.RelativePerformanceSectorInfo;

            //clear grid of previous sector info
            for (int columnIndex = 1; columnIndex < this.dgRelativePerformance.Columns.Count - 1; )
            {
                this.dgRelativePerformance.Columns.RemoveAt(columnIndex);
            }

            int cIndex = 0;

            foreach (RelativePerformanceSectorData sectorData in e.RelativePerformanceSectorInfo)
            {
                    Telerik.Windows.Controls.GridViewDataColumn dataColumn = new Telerik.Windows.Controls.GridViewDataColumn();
                    dataColumn.Header = sectorData.SectorName;
                    dataColumn.UniqueName = sectorData.SectorName;
                    dataColumn.DataMemberBinding = new System.Windows.Data.Binding("RelativePerformanceCountrySpecificInfo[" + cIndex + "]");

                    StringBuilder CellTemp = new StringBuilder();
                    CellTemp.Append("<DataTemplate");
                    CellTemp.Append(" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'");
                    CellTemp.Append(" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'");

                    //Be sure to replace "YourNamespace" and "YourAssembly" with your app's 
                    //actual namespace and assembly here
                    //      Changed the following block to use converter and publish in Basis Points - Lane - 2012-09-26
                    CellTemp.Append(" xmlns:lanes='clr-namespace:GreenField.Gadgets.Helpers;assembly=GreenField.Gadgets'");
                    CellTemp.Append(" xmlns:local='clr-namespace:GreenField.Gadgets.Views;assembly=GreenField.Gadgets'>");
                    CellTemp.Append("<StackPanel Orientation='Horizontal'>");
                    CellTemp.Append("<TextBlock>");
                    CellTemp.Append("   <TextBlock.Text>");
                    CellTemp.Append("       <Binding Path='RelativePerformanceCountrySpecificInfo[" + cIndex + "].Alpha'>");
                    CellTemp.Append("           <Binding.Converter>");
                    CellTemp.Append("               <lanes:BasisPointsConverter/>");
                    CellTemp.Append("           </Binding.Converter>");
                    CellTemp.Append("       </Binding>");
                    CellTemp.Append("   </TextBlock.Text>");
                    CellTemp.Append("</TextBlock>");
                    CellTemp.Append("</StackPanel>");
                    CellTemp.Append("</DataTemplate>");

                    dataColumn.CellTemplate = XamlReader.Load(CellTemp.ToString()) as DataTemplate;

                    decimal? aggregateSectorAlphaValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex))
                                                                                  .Sum(t => t.Alpha == null ? 0 : t.Alpha);
                    string aggregateSectorAlpha = aggregateSectorAlphaValue == null ? String.Empty : Decimal.Parse(aggregateSectorAlphaValue.ToString()).ToString();
                    decimal? aggregateSectorActiviePositionValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex))
                                                                                            .Sum(t => t.ActivePosition == null ? 0 : t.ActivePosition);
                    string aggregateSectorActiviePosition = aggregateSectorActiviePositionValue == null ? String.Empty
                                                                                         : (Decimal.Parse(aggregateSectorActiviePositionValue.ToString())).ToString();

                    var aggregateAlphaSumFunction = new AggregateFunction<RelativePerformanceData, string>
                    {
                        AggregationExpression = Models => aggregateSectorAlpha,
                        FunctionName = sectorData.SectorId.ToString()
                    };

                    dataColumn.FooterTextAlignment = TextAlignment.Right;
                    dataColumn.Width = GridViewLength.Auto;
                    dataColumn.AggregateFunctions.Add(aggregateAlphaSumFunction);

                    TextBlock spFunctions = new TextBlock()
                    {
                        Text = aggregateSectorActiviePosition.ToString(),
                        Tag = sectorData.SectorId,
                        TextAlignment = TextAlignment.Right,
                        FontSize = 9
                    };

                    TextBlock footerText = new TextBlock()
                    {
                        Text = (Convert.ToDecimal(aggregateSectorAlpha) * 10000).ToString("n0"), //Needs to be displayed in basis points - Lane - 2012-09-25
                        Tag = sectorData.SectorId,
                        TextAlignment = TextAlignment.Right,
                        FontSize = 9
                    };

                    ToolTipService.SetToolTip(footerText, spFunctions);
                    dataColumn.Footer = footerText;
                    dataColumn.HeaderCellStyle = this.Resources["GridViewHeaderCellClickable"] as Style;
                    dataColumn.FooterCellStyle = this.Resources["GridViewCustomFooterCellStyle"] as Style;
                    dataColumn.CellStyle = this.Resources["GridViewCellStyle"] as Style;
                    dgRelativePerformance.Columns.Insert(++cIndex, dataColumn);
            }

            RelativePerformanceInfo = e.RelativePerformanceInfo;

            dbInteractivity = (this.DataContext as ViewModelRelativePerformance).dbInteractivity;
            eventAggregator = (this.DataContext as ViewModelRelativePerformance).eventAggregator;

            //design grid for sector specific top alpha security grid

            if (this.dpTopActivePositionSecurity.Children != null)
            {
                this.dpTopActivePositionSecurity.Children.Clear();
            }

            if (e.RelativePerformanceSectorInfo != null)
            {
                ScrollViewer svc = new ScrollViewer() 
                { 
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Hidden
                };

                Grid grd = new Grid() { ShowGridLines = false, UseLayoutRounding = true };
                grd.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grd.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grd.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                grd.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                int sectorNum = 0;

                for (int i = 0; i < e.RelativePerformanceSectorInfo.Count; i++)
                {
                    int securityNum = 1;

                    List<SecurityDetail> sectorSpecificTopAlphaSecurityNames = e.RelativePerformanceSecurityInfo
                        .Where(record => record.SecuritySectorName == e.RelativePerformanceSectorInfo[i].SectorName)
                        .OrderByDescending(record => record.SecurityAlpha)
                        .Take(3)
                        .Select(record => new SecurityDetail()
                            {
                                SecurityName = record.SecurityName,
                                SecurityAlpha = record.SecurityAlpha == null
                                ? "Null" : String.Format("{0}", record.SecurityAlpha.Value)
                            })
                        .ToList();

                    TextBox sectorHeader = new TextBox()
                    {
                        Text = e.RelativePerformanceSectorInfo[i].SectorName,
                        FontWeight = FontWeights.Bold,
                        FontFamily = new FontFamily("Arial"),
                        IsReadOnly = true,
                        Background = new SolidColorBrush(Color.FromArgb(255, 203, 212, 241)),
                        Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)),
                        FontSize = 9,
                        Margin = new Thickness(2)
                    };

                    grd.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    int colIndex = grd.ColumnDefinitions.Count() - 1;

                    sectorHeader.SetValue(Grid.RowProperty, 0);
                    sectorHeader.SetValue(Grid.ColumnProperty, colIndex);
                    grd.Children.Add(sectorHeader);

                    foreach (SecurityDetail securityName in sectorSpecificTopAlphaSecurityNames)
                    {
                        TextBlock txtSecurityName = new TextBlock()
                        {
                            Text = securityName.SecurityName
                                + " (" + (Convert.ToDecimal(securityName.SecurityAlpha) * 10000).ToString("n0") + ")",  //Needs to be displayed in basis points - Lane - 2012-09-25
                            FontSize = 9,
                            FontWeight = FontWeights.Normal,
                            FontFamily = new FontFamily("Arial")
                        };
                        txtSecurityName.SetValue(Grid.ColumnProperty, sectorNum);
                        txtSecurityName.SetValue(Grid.RowProperty, securityNum);
                        grd.Children.Add(txtSecurityName);
                        securityNum++;
                    }
                    sectorNum++;
                }

                svc.ScrollIntoView(grd);
                svc.Content = grd;
                this.dpTopActivePositionSecurity.Children.Add(svc);
            }
        }
        #endregion

        #region Export To Excel Methods
        /// <summary>
        /// catches export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE);
            childExportOptions.Show();
        }

        /// <summary>
        /// handles element exporting for grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRelativePerformance_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, () =>
            {
                if (e.Value is RelativePerformanceData)
                {
                    RelativePerformanceData value = e.Value as RelativePerformanceData;
                    if (value != null)
                    {
                        decimal totalValue = value.AggregateCountryAlpha.HasValue? 
                            value.AggregateCountryAlpha.Value:0;
                        countryTotal = GetValueInBasisPoints(totalValue.ToString());               
                    }

                    return value;
                }

                if (e.Value is RelativePerformanceCountrySpecificData)
                {
                    RelativePerformanceCountrySpecificData value = e.Value as RelativePerformanceCountrySpecificData;

                    string result = String.Empty;
                    if (value.Alpha != null)
                    {
                        decimal totalValue = 0M;

                        result = Decimal.TryParse(value.Alpha.ToString(), out totalValue) ?
                            GetValueInBasisPoints(totalValue.ToString()) : String.Empty;                                             
                    }
                    return result;
                }

                if (e.Value == null)
                {
                    GridViewDataColumn column = (e.Context as GridViewDataColumn);
                    if (column != null)
                    {
                        if ((!String.IsNullOrEmpty(column.Header.ToString()) && String.Equals(column.Header.ToString(), "Total",
                            StringComparison.CurrentCultureIgnoreCase)))
                        {
                            return countryTotal;
                        }
                    }
                }

                if (e.Element == ExportElement.FooterCell)
                {
                    decimal totalValue = 0M;

                    string value = Decimal.TryParse(e.Value.ToString(), out totalValue) ?
                        GetValueInBasisPoints(totalValue.ToString()) : "Total";

                    return value;
                }

                return e.Value;
            });
        }
        #endregion

        #region Helper Methods/Class
        /// <summary>
        /// class to contain security name and alpha
        /// </summary>
        private class SecurityDetail
        {
            public String SecurityName { get; set; }
            public String SecurityAlpha { get; set; }
        }

        /// <summary>
        /// return number of rows in the grid
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public int GetMaxRows(RelativePerformanceToggledSectorGridBuildEventArgs e)
        {
            int counter = 0;
            foreach (string countryName in e.RelativePerformanceCountryNameInfo)
            {
                List<string> s1 = e.RelativePerformanceSecurityInfo.Where(t => t.SecurityCountryId == countryName).Select(t => t.SecurityName).ToList();
                if (counter < s1.Count())
                { 
                    counter = s1.Count(); 
                }
            }
            return counter;
        }

        /// <summary>
        /// click on footer cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FooterCellBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                string sectorID = ((e.OriginalSource as TextBlock).Tag).ToString();
                eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
                {
                    SectorID = sectorID
                });
            }
        }

        /// <summary>
        /// Convert values to basis points
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValueInBasisPoints(string value)
        {
            decimal decimalVal = Convert.ToDecimal(value) * 10000;
            
            return decimalVal.ToString("n0");
        }

        #endregion

        #region Export to Pdf/Print
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                RadDocument document = CreateDocument(dgRelativePerformance);

                document.LayoutMode = DocumentLayoutMode.Paged;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));

                PdfFormatProvider provider = new PdfFormatProvider();
                using (Stream output = dialog.OpenFile())
                {
                    provider.Export(document, output);
                }
            }
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RichTextBox.Document = CreateDocument(dgRelativePerformance);
            }));

            this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
            RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        }

        /// <summary>
        /// method for pdf exporting
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        private RadDocument CreateDocument(RadGridView grid)
        {
            List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
                                                     orderby c.DisplayIndex
                                                     select c).ToList();
            Table table = new Table();
            RadDocument document = new RadDocument();
            Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
            section.Blocks.Add(table);
            document.Sections.Add(section);

            if (grid.ShowColumnHeaders)
            {
                TableRow headerRow = new TableRow();
                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = Colors.Gray;
                    headerRow.Cells.Add(indentCell);
                }

                for (int i = 0; i < columns.Count(); i++)
                {
                    TableCell cell = new TableCell();
                    cell.Background = Colors.White;
                    AddCellValue(cell, columns[i].Header.ToString());
                    cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                    headerRow.Cells.Add(cell);
                }

                table.Rows.Add(headerRow);
            }

            if (grid.Items.Groups != null)
            {
                for (int i = 0; i < grid.Items.Groups.Count(); i++)
                {
                    AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid);
                }
            }
            else
            {
                AddDataRows(table, grid.Items, columns, grid);
            }

            if (grid.ShowColumnFooters)
            {
                TableRow footerRow = new TableRow();
                for (int i = 0; i < columns.Count(); i++)
                {
                    TableCell cell = new TableCell();
                    cell.Background = Colors.Gray;
                    string value = ((columns[i].Footer) as TextBlock).Text.ToString();
                    AddCellValue(cell, value != null ? value.ToString() : string.Empty);
                    cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                    footerRow.Cells.Add(cell);
                }
                table.Rows.Add(footerRow);
            }
            return document;
        }

        /// <summary>
        /// method for adding rows to exported pdf
        /// </summary>
        /// <param name="table"></param>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <param name="grid"></param>
        private void AddDataRows(Table table, IList items, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            for (int i = 0; i < items.Count; i++)
            {
                TableRow row = new TableRow();

                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = Colors.White;
                    row.Cells.Add(indentCell);
                }

                for (int j = 0; j < columns.Count(); j++)
                {
                    TableCell cell = new TableCell();
                    object value = null;
                    if (j == 0)
                    {
                        value = columns[j].GetValueForItem(items[i]) != null ? (columns[j].GetValueForItem(items[i])) : null;
                    }
                    else if (j == columns.Count - 1)
                    {
                        value = ((items[i]) as RelativePerformanceData) != null ?
                            ((items[i]) as RelativePerformanceData).AggregateCountryAlpha.ToString() : null;
                    }

                    else
                        value = columns[j].GetValueForItem(items[i]) != null ?
                            ((columns[j].GetValueForItem(items[i])) as RelativePerformanceCountrySpecificData).Alpha.ToString() : null;

                    AddCellValue(cell, value != null ? value.ToString() : string.Empty);

                    cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                    cell.Background = Colors.White;

                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }
        }

        /// <summary>
        /// adding group rows to exported pdf
        /// </summary>
        /// <param name="table"></param>
        /// <param name="group"></param>
        /// <param name="columns"></param>
        /// <param name="grid"></param>
        private void AddGroupRow(Table table, QueryableCollectionViewGroup group, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            TableRow row = new TableRow();

            int level = GetGroupLevel(group);
            if (level > 0)
            {
                TableCell cell = new TableCell();
                cell.PreferredWidth = new TableWidthUnit(level * 20);
                cell.Background = Colors.White;
                row.Cells.Add(cell);
            }

            TableCell aggregatesCell = new TableCell();
            aggregatesCell.Background = Colors.White;
            aggregatesCell.ColumnSpan = columns.Count() + (grid.GroupDescriptors.Count() > 0 ? 1 : 0) - (level > 0 ? 1 : 0);

            AddCellValue(aggregatesCell, group.Key != null ? group.Key.ToString() : string.Empty);

            foreach (AggregateResult result in group.AggregateResults)
            {
                AddCellValue(aggregatesCell, result.FormattedValue != null ? result.FormattedValue.ToString() : string.Empty);
            }

            row.Cells.Add(aggregatesCell);

            table.Rows.Add(row);

            if (group.HasSubgroups)
            {
                for (int i = 0; i < group.Subgroups.Count(); i++)
                {
                    AddGroupRow(table, group.Subgroups[i] as QueryableCollectionViewGroup, columns, grid);
                }
            }
            else
            {
                for (int i = 0; i < group.ItemCount; i++)
                {
                    AddDataRows(table, group.Items, columns, grid);
                }
            }
        }

        /// <summary>
        /// adding cell value to exported pdf
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        private void AddCellValue(TableCell cell, string value)
        {
            if (value != null)
            {
                Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
                cell.Blocks.Add(paragraph);
                Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
                if (value == "")
                    value = " ";
                span.Text = value;
                span.FontFamily = new System.Windows.Media.FontFamily("Arial");
                span.FontSize = 9;
                paragraph.Inlines.Add(span);
            }
        }

        /// <summary>
        /// returns group level
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private int GetGroupLevel(IGroup group)
        {
            int level = 0;
            IGroup parent = group.ParentGroup;
            while (parent != null)
            {
                level++;
                parent = parent.ParentGroup;
            }
            return level;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformance.Dispose();
            this.DataContextRelativePerformance.RelativePerformanceGridBuildEvent -= 
                new RelativePerformanceGridBuildEventHandler(DataContextSource_RelativePerformanceGridBuildEvent);
            this.DataContextRelativePerformance = null;
            this.DataContext = null;
        }
        #endregion
    }
}
