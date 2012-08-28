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
using System.Windows.Data;
using System.Windows.Printing;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRelativePerformance : ViewBaseUserControl
    {
        #region Fields
        //Gadget Data
        private List<RelativePerformanceSectorData> _relativePerformanceSectorInfo;

        //MEF Singletons
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;

        //private details
        Canvas canvas;
        double totalHeight;
        RadGridView grid;
        double offsetY;

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextRelativePerformance != null) //DataContext instance
                    DataContextRelativePerformance.IsActive = _isActive;
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
        private ViewModelRelativePerformance _dataContextRelativePerformance;
        public ViewModelRelativePerformance DataContextRelativePerformance
        {
            get { return _dataContextRelativePerformance; }
            set { _dataContextRelativePerformance = value; }
        }

        /// <summary>
        /// contains data to be displayed in the grid
        /// </summary>
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
                return;

            if (e.Row.Cells[0] is GridViewFooterCell)
                return;                     

            foreach (GridViewCell cell in e.Row.Cells)
            {
                //Null Check
                if (cell.Value == null)
                    continue;

                //No toolTip service for CountryId Column
                if (cell.Column.DisplayIndex == 0)
                    continue;

                //No toolTip service for Totals Column
                if (cell.Column.DisplayIndex == this.dgRelativePerformance.Columns.Count - 1)
                    continue;

                //No toolTip service for Blank cells
                if ((cell.Value as RelativePerformanceCountrySpecificData).Alpha == null)
                    continue;

                //decimal? activePosition = (cell.ParentRow.DataContext as RelativePerformanceData).AggregateCountryActivePosition;
                decimal? activePosition = (cell.Value as RelativePerformanceCountrySpecificData).ActivePosition;

                ToolTip toolTip = new ToolTip()
                {
                    Content = activePosition               
                };

                ToolTipService.SetToolTip(cell, toolTip);
            }
        }

        private void MouseDownOnHeaderCell(object sender, MouseEventArgs args)
        {
            if (dgRelativePerformance.Items.Count != 0)
            {
                GridViewHeaderCell cellClicked = ((FrameworkElement)args.OriginalSource).ParentOfType<GridViewHeaderCell>();
                if (cellClicked == null)
                {
                    GridViewFooterCell footerTotalClicked = ((FrameworkElement)args.OriginalSource).ParentOfType<GridViewFooterCell>();
                    if (footerTotalClicked == null)
                        return;

                    if ((args.OriginalSource as TextBlock).Text == "Total")
                    {
                        _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
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
                        _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
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
            //Ignore involuntary selection event
            if (e.AddedCells.Count == 0)
                return;

            int selectedColumnIndex = e.AddedCells[0].Column.DisplayIndex;

            //when cells on Column ID column, toggled grid is displayed
            if (selectedColumnIndex == 0)
            return;

            //Ignore null cells
            if (selectedColumnIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                if (((e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[selectedColumnIndex - 1] as RelativePerformanceCountrySpecificData).Alpha == null)
                    return;
            }

            //Catching cells clicked in sector columns
            string countryID = (e.AddedCells[0].Item as RelativePerformanceData).CountryId;
            string sectorID = null;
            if (selectedColumnIndex != this.dgRelativePerformance.Columns.Count - 1)
            {
                sectorID = ((e.AddedCells[0].Item as RelativePerformanceData).RelativePerformanceCountrySpecificInfo[e.AddedCells[0].Column.DisplayIndex - 1] as RelativePerformanceCountrySpecificData).SectorId;
            }

            _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
            {
                CountryID = countryID,
                SectorID = sectorID,
            });
        }

        void DataContextSource_RelativePerformanceGridBuildEvent(RelativePerformanceGridBuildEventArgs e)
        {
            _relativePerformanceSectorInfo = e.RelativePerformanceSectorInfo;
            
            //Clear grid of previous sector info
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
                CellTemp.Append("</StackPanel>");
                CellTemp.Append("</DataTemplate>");

                dataColumn.CellTemplate = XamlReader.Load(CellTemp.ToString()) as DataTemplate;

                decimal? aggregateSectorAlphaValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex)).Sum(t => t.Alpha == null ? 0 : t.Alpha);
                string aggregateSectorAlpha = aggregateSectorAlphaValue == null ? String.Empty : Decimal.Parse(aggregateSectorAlphaValue.ToString()).ToString();
                decimal? aggregateSectorActiviePositionValue = e.RelativePerformanceInfo.Select(t => t.RelativePerformanceCountrySpecificInfo.ElementAt(cIndex)).Sum(t => t.ActivePosition == null ? 0 : t.ActivePosition);
                string aggregateSectorActiviePosition = aggregateSectorActiviePositionValue == null ? String.Empty : (Decimal.Parse(aggregateSectorActiviePositionValue.ToString())).ToString();

                var aggregateAlphaSumFunction = new AggregateFunction<RelativePerformanceData, string>
                {
                    //AggregationExpression = Models => string.Format("{0} ({1}%)", aggregateSectorAlpha, aggregateSectorActiviePosition),
                    AggregationExpression = Models => aggregateSectorAlpha,
                    FunctionName = sectorData.SectorId.ToString()
                };                

                dataColumn.FooterTextAlignment = TextAlignment.Right;
                dataColumn.Width = GridViewLength.Auto;
                dataColumn.AggregateFunctions.Add(aggregateAlphaSumFunction);
                
                TextBlock spFunctions = new TextBlock() {
                    Text = aggregateSectorActiviePosition.ToString(),
                    Tag = sectorData.SectorId,
                    TextAlignment = TextAlignment.Right,
                    FontSize = 9
                };
                TextBlock footerText = new TextBlock() { 
                    Text = aggregateSectorAlpha.ToString(), 
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

            _dbInteractivity = (this.DataContext as ViewModelRelativePerformance)._dbInteractivity;
            _eventAggregator = (this.DataContext as ViewModelRelativePerformance)._eventAggregator;


            //Design Grid for Sector Specific Top Alpha Security Grid
            //#######################################################

            if (this.dpTopActivePositionSecurity.Children != null)
                this.dpTopActivePositionSecurity.Children.Clear();

            if (e.RelativePerformanceSectorInfo != null)
            {
                ScrollViewer svc = new ScrollViewer() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };

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
                                + " (" + securityName.SecurityAlpha + ")",
                            FontSize = 9
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
                        return value.CountryId;
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

        public int GetMaxRows(RelativePerformanceToggledSectorGridBuildEventArgs e)
        {
            int counter = 0;
            foreach (string countryName in e.RelativePerformanceCountryNameInfo)
            {
                List<string> s1 = e.RelativePerformanceSecurityInfo.Where(t => t.SecurityCountryId == countryName).Select(t => t.SecurityName).ToList();
                if (counter < s1.Count())
                    counter = s1.Count();
            }
            return counter;
        }        

        private void FooterCellBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is TextBlock)
            {
                    string sectorID = ((e.OriginalSource as TextBlock).Tag).ToString();
                    _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Publish(new RelativePerformanceGridCellData()
                    {
                        SectorID = sectorID
                    });           
            }
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

        ///// <summary>
        ///// Printing the Grid
        ///// </summary>
        ///// <param name="grid"></param>
        ///// <param name="fontSize"></param>
        ///// <returns></returns>
        //public static RadDocument Print(RadGridView grid, int fontSize = 8)
        //{
        //    int fontSizePDF = fontSize;
        //    RadDocument document =  CreateDocument(grid);
        //}

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

        #region Printing the DataGrid

       

        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformance.Dispose();
            this.DataContextRelativePerformance.RelativePerformanceGridBuildEvent -= new RelativePerformanceGridBuildEventHandler(DataContextSource_RelativePerformanceGridBuildEvent);
            this.DataContextRelativePerformance= null;
            this.DataContext = null;
        }
        #endregion

        private class SecurityDetail
        {
            public String SecurityName { get; set; }
            public String SecurityAlpha { get; set; }
        }
    }
}
