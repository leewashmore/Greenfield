using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Model;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Documents.Layout;
using System.IO;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// class for view for Finstat
    /// </summary>
    public partial class ViewFinstat : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelFinstat dataContextFinstat;
        public ViewModelFinstat DataContextFinstat
        {
            get { return dataContextFinstat; }
            set { dataContextFinstat = value; }
        }

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
                if (DataContextFinstat != null)
                { DataContextFinstat.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewFinstat(ViewModelFinstat dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextFinstat = dataContextSource;
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 4, netColumnCount: 7, isQuarterImplemented: false);
            List<string> periodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false);
            PeriodColumns.UpdateColumnInformation(this.dgFinstat, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnNamespace = typeof(ViewModelFinstat).FullName,
                PeriodColumnHeader = periodColumnHeader,
                PeriodIsYearly = true
            }, isQuarterImplemented: false, navigatingColumnStartIndex: 1);
            dgFinstat.Columns[8].Header = "Avg " + periodColumnHeader[1] + "-" + periodColumnHeader[3];
            dgFinstat.Columns[9].Header = "Avg " + periodColumnHeader[4] + "-" + periodColumnHeader[6];
            SettingGridColumnUniqueNames(periodColumnHeader);

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == this.DataContext.GetType().FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgFinstat, e, false, 1);
                    dgFinstat.Columns[8].Header = "Avg " + e.PeriodColumnHeader[1] + "-" + e.PeriodColumnHeader[3];
                    dgFinstat.Columns[9].Header = "Avg " + periodColumnHeader[4] + "-" + periodColumnHeader[6];

                    SettingGridColumnUniqueNames(e.PeriodColumnHeader);
                    this.btnExportExcel.IsEnabled = true;
                }
            };           
        } 
        #endregion

        /// <summary>
        /// dgFinstat RowLoaded EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RowLoadedEventArgs</param>
        private void dgFinstat_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            PeriodColumns.RowDataCustomization(e, false);
        } 

        #region Export/Pdf/Print
        #region ExportToExcel
        /// <summary>
        /// handles element exporting when exported to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFinstat_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }

        /// <summary>
        /// catch export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

            RadExportOptionsInfo.Add(new RadExportOptions()
            { 
                ElementName = "Finstat Report",
                Element = this.dgFinstat,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER 
            });
            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Finstat Report");
            childExportOptions.Show(); 
        }
        #endregion

        #region PDFExport
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = "Finstat Report",
                Element = this.dgFinstat,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                RichTextBox = this.RichTextBox,
                InitialHeaderBlock = InsertHeaderBlock
            });
            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Finstat Report");
            childExportOptions.Show();
        }

        /// <summary>
        /// Inserting header content to the exported pdf output
        /// </summary>
        /// <returns>Block element</returns>
        private Block InsertHeaderBlock()
        {
            Table resultBlock = new Table();           

            #region Report content
            TableRow resultHeaderRow = new TableRow();
            TableCell resultHeaderCell = new TableCell()
            {
                VerticalAlignment = RadVerticalAlignment.Center
            };
            resultHeaderRow.Cells.Add(resultHeaderCell);
            resultBlock.Rows.Add(resultHeaderRow);

            #region Report Name/Security and Date
            Table headertable = new Table();

            TableRow headerLogoRow = new TableRow();
            TableCell headerLogoCell = new TableCell()
            {
                VerticalAlignment = RadVerticalAlignment.Top
            };

            Paragraph headerLogoParagraph = new Paragraph() { TextAlignment = RadTextAlignment.Right };
            Stream stream = Application.GetResourceStream(new Uri(@"/GreenField.Gadgets;component/Images/AshmoreLogo.png", UriKind.RelativeOrAbsolute)).Stream;
            Size size = new Size(160, 30);
            ImageInline image = new ImageInline(stream, size, "png");
            headerLogoParagraph.Inlines.Add(image);

            headerLogoCell.Blocks.Add(headerLogoParagraph);
            headerLogoRow.Cells.Add(headerLogoCell);
            headertable.Rows.Add(headerLogoRow);

            TableRow headerRow = new TableRow();            
            TableCell headerCell = new TableCell()
            {
                VerticalAlignment = RadVerticalAlignment.Center
            };
            Paragraph headerParagraph = new Paragraph() { TextAlignment = RadTextAlignment.Center };
            if (!String.IsNullOrWhiteSpace(this.txtIssueName.Text))
            {
                Span span = new Span()
                {                    
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + this.txtIssueName.Text,
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 12,
                    FontWeight = FontWeights.Bold
                };
                headerParagraph.Inlines.Add(span);
                Span span2 = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + DateTime.Today.ToString("d"),
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
                headerParagraph.Inlines.Add(span2);
            }
            headerCell.Blocks.Add(headerParagraph);
            headerRow.Cells.Add(headerCell);
            headertable.Rows.Add(headerRow);
            resultHeaderCell.Blocks.Add(headertable);            
            #endregion

            #region Input Region
            TableRow resultInputRow = new TableRow();
            TableCell resultInputCell = new TableCell()
            {
                VerticalAlignment = RadVerticalAlignment.Center
            };
            resultInputRow.Cells.Add(resultInputCell);
            resultBlock.Rows.Add(resultInputRow);

            Table inputtable = new Table();
            TableRow inputRow = new TableRow();

            #region Input Section 1
            TableCell inputSection1Cell = new TableCell()
                {
                    VerticalAlignment = RadVerticalAlignment.Top
                };

            Table inputSection1Table = new Table();
            TableRow inputSection1Row = new TableRow();
            TableCell inputSection1Part1Cell = new TableCell()
                {
                    VerticalAlignment = RadVerticalAlignment.Center
                };
            inputSection1Part1Cell.PreferredWidth = new TableWidthUnit(300);
            Paragraph inputSection1Part1Paragraph = new Paragraph() { TextAlignment = RadTextAlignment.Left };
            #region Country
            Span inputSection1Part1Item1Span = new Span()
                {
                    Text = "Country :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part1Item1Span);
            Span inputSection1Part2Item1Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtCountry.Text) ? "-" : this.txtCountry.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part2Item1Span);
            #endregion

            #region Sector
            Span inputSection1Part1Item2Span = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + "Sector :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part1Item2Span);
            Span inputSection1Part2Item2Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtSector.Text) ? "-" : this.txtSector.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part2Item2Span); 
            #endregion

            #region Industry
            Span inputSection1Part1Item3Span = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + "Industry :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part1Item3Span);
            Span inputSection1Part2Item3Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtIndustry.Text) ? "-" : this.txtIndustry.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part2Item3Span); 
            #endregion

            #region Sub-Industry
		    Span inputSection1Part1Item4Span = new Span()
            {
                Text = FormattingSymbolLayoutBox.LINE_BREAK + "Sub-Industry :",
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10,
                FontWeight = FontWeights.Bold
            };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part1Item4Span);
            Span inputSection1Part2Item4Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtSubIndustry.Text) ? "-" : this.txtSubIndustry.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection1Part1Paragraph.Inlines.Add(inputSection1Part2Item4Span);
	        #endregion

            inputSection1Part1Cell.Blocks.Add(inputSection1Part1Paragraph);
            inputSection1Row.Cells.Add(inputSection1Part1Cell);
            inputSection1Table.Rows.Add(inputSection1Row);
            inputSection1Cell.Blocks.Add(inputSection1Table);
            inputRow.Cells.Add(inputSection1Cell);           
            #endregion 

            //dummy Cell to create distance between input sections
            inputRow.Cells.Add(new TableCell());            

            #region Input Section 2
            TableCell inputSection2Cell = new TableCell()
            {
                VerticalAlignment = RadVerticalAlignment.Top
            };

            Table inputSection2Table = new Table();
            TableRow inputSection2Row = new TableRow();
            TableCell inputSection2Part1Cell = new TableCell()
            {
                VerticalAlignment = RadVerticalAlignment.Center
            };
            Paragraph inputSection2Part1Paragraph = new Paragraph() { TextAlignment = RadTextAlignment.Left };

            #region Ticker
            Span inputSection2Part1Item1Span = new Span()
                {
                    Text = "Ticker :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part1Item1Span);
            Span inputSection2Part2Item1Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtTicker.Text) ? "-" : this.txtTicker.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part2Item1Span); 
            #endregion

            #region Currency
            
            Span nameTODO1 = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + "Data Currency :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection2Part1Paragraph.Inlines.Add(nameTODO1);
            Span nameTODO2 = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtDataCurrency.Text) ? "-" : this.txtDataCurrency.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection2Part1Paragraph.Inlines.Add(nameTODO2); 

            /*
            Span inputSection2Part1Item2Span = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + "Trading Currency :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part1Item2Span);
            Span inputSection2Part2Item2Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtCurrency.Text) ? "-" : this.txtCurrency.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part2Item2Span); 
             * */
            #endregion

            #region Primary Analyst
            Span inputSection2Part1Item3Span = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + "Primary Analyst :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part1Item3Span);
            Span inputSection2Part2Item3Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtPrimaryAnalyst.Text) ? "-" : this.txtPrimaryAnalyst.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part2Item3Span); 
            #endregion

            #region Industry Analyst
            Span inputSection2Part1Item4Span = new Span()
                {
                    Text = FormattingSymbolLayoutBox.LINE_BREAK + "Industry Analyst :",
                    FontFamily = new System.Windows.Media.FontFamily("Arial"),
                    FontSize = 10,
                    FontWeight = FontWeights.Bold
                };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part1Item4Span);
            Span inputSection2Part2Item4Span = new Span()
            {
                Text = String.IsNullOrWhiteSpace(this.txtIndustryAnalyst.Text) ? "-" : this.txtIndustryAnalyst.Text,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 10
            };
            inputSection2Part1Paragraph.Inlines.Add(inputSection2Part2Item4Span); 
            #endregion

            inputSection2Part1Cell.Blocks.Add(inputSection2Part1Paragraph);
            inputSection2Row.Cells.Add(inputSection2Part1Cell);
            inputSection2Table.Rows.Add(inputSection2Row);
            inputSection2Cell.Blocks.Add(inputSection2Table);
            inputRow.Cells.Add(inputSection2Cell);
            #endregion 

            inputtable.Rows.Add(inputRow);
            resultInputCell.Blocks.Add(inputtable);
            #endregion                     
            #endregion

            return resultBlock;
        }
        #endregion

        #region Printing the DataGrid
        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = "Finstat Report",
                Element = this.dgFinstat,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                RichTextBox = this.RichTextBox,
                InitialHeaderBlock = InsertHeaderBlock
            });
            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Finstat Report");
            childExportOptions.Show();
        }
        #endregion       
        #endregion

        #region Helper Method
        /// <summary>
        /// Method to set unique names for grid columns required in Pdf and Print
        /// </summary>
        /// <param name="periodColumnHeader">List<string> periodColumnHeader</param>
        public void SettingGridColumnUniqueNames(List<string> periodColumnHeader)
        {
            dgFinstat.Columns[0].UniqueName = "";
            dgFinstat.Columns[1].UniqueName = periodColumnHeader[0];
            dgFinstat.Columns[2].UniqueName = periodColumnHeader[1];
            dgFinstat.Columns[3].UniqueName = periodColumnHeader[2];
            dgFinstat.Columns[4].UniqueName = periodColumnHeader[3];
            dgFinstat.Columns[5].UniqueName = periodColumnHeader[4];
            dgFinstat.Columns[6].UniqueName = periodColumnHeader[5];
            dgFinstat.Columns[7].UniqueName = periodColumnHeader[6];
            dgFinstat.Columns[8].UniqueName = "Avg " + periodColumnHeader[1] + "-" + periodColumnHeader[3];
            dgFinstat.Columns[9].UniqueName = "Avg " + periodColumnHeader[4] + "-" + periodColumnHeader[6];
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelFinstat).Dispose();
            this.DataContext = null;
        } 
        #endregion
    }
}
