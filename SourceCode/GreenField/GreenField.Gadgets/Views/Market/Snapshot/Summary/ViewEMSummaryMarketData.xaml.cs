using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.DataContracts.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewEMSummaryMarketData
    /// </summary>
    public partial class ViewEMSummaryMarketData : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelEMSummaryMarketData dataContextEMSummaryMarket;
        public ViewModelEMSummaryMarketData DataContextEMSummaryMarket
        {
            get { return dataContextEMSummaryMarket; }
            set
            {
                dataContextEMSummaryMarket = value;
                this.DataContext = value;
            }
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
                if (DataContextEMSummaryMarket != null)
                {
                    DataContextEMSummaryMarket.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelEMSummaryMarketData</param>
        public ViewEMSummaryMarketData(ViewModelEMSummaryMarketData dataContextSource)
        {
            InitializeComponent();
            this.txtCurrDate.Text = DateTime.Today.ToString("MMMM dd, yyyy");
            DataContextEMSummaryMarket = dataContextSource;
            
            for (int i = 4; i < this.dgEMSummaryMarketData.Columns.Count; i++)
            {
                String propertyName = ((GridViewDataColumn)this.dgEMSummaryMarketData.Columns[i]).DataMemberBinding.Path.Path.ToString();

                switch (i)
                {
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 13:
                        this.dgEMSummaryMarketData.Columns[i].AggregateFunctions
                            .Add(new AggregateFunctionEMDataSummary() { SourceField = propertyName });
                        break;
                    default:
                        this.dgEMSummaryMarketData.Columns[i].AggregateFunctions
                            .Add(new AggregateFunctionPercentageEMDataSummary() { SourceField = propertyName });
                        break;
                }
            }
        }
        #endregion

        #region Event Handlers
        #region Data Load
        /// <summary>
        /// dgEMSummaryMarketData DataLoaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgEMSummaryMarketData_DataLoaded(object sender, EventArgs e)
        {
            InitializeGridHeaders();
        }       
        #endregion  

        #region Export/Print
        /// <summary>
        /// Event for Grid Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementExportingEvent(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (this.dgEMSummaryMarketData.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = this.dgEMSummaryMarketData.Tag.ToString(),
                        Element = this.dgEMSummaryMarketData,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    });

                else if (this.dgEMSummaryMarketSSRData.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = this.dgEMSummaryMarketSSRData.Tag.ToString(),
                        Element = this.dgEMSummaryMarketSSRData,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: "
                    + GadgetNames.MODELS_FX_MACRO_ECONOMICS_EM_DATA_REPORT);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// btnExportPdf Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgEMSummaryMarketData.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgEMSummaryMarketData, 12);
                }
                else if (this.dgEMSummaryMarketSSRData.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgEMSummaryMarketSSRData, 12);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// btnPrint Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgEMSummaryMarketData.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgEMSummaryMarketData, 12);
                    }));
                }
                else if (this.dgEMSummaryMarketSSRData.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgEMSummaryMarketSSRData, 12);
                    }));
                }
                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }
        #endregion

        #region Flipping
        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgEMSummaryMarketData.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.dgEMSummaryMarketData, this.dgEMSummaryMarketSSRData);
            }
            else
            {
                Flipper.FlipItem(this.dgEMSummaryMarketSSRData, this.dgEMSummaryMarketData);
            }
        }
        #endregion
        #endregion        

        #region Helper Methods
        /// <summary>
        /// Initailizes data grid headers
        /// </summary>
        private void InitializeGridHeaders()
        {
            List<EMSummaryMarketData> data = this.dgEMSummaryMarketData.ItemsSource as List<EMSummaryMarketData>;            
            if (data != null && data.Count > 0)
            {
                String benchmarkId = data.First().BenchmarkId;
                DateTime portfolioDate = data.First().PortfolioDate;

                this.dgEMSummaryMarketData.Columns[0].Footer = benchmarkId;

                UpdateBenchmarkColumnGroup(this.dgEMSummaryMarketData, "cgBenchmark", benchmarkId, portfolioDate);
                UpdateBenchmarkColumnGroup(this.dgEMSummaryMarketSSRData, "cgBenchmark1", benchmarkId, portfolioDate);
            }

            #region Update EMM Summary report headers
            this.dgEMSummaryMarketData.Columns[5].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[6].Header = String.Format("{0} C", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[7].Header = String.Format("{0}", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketData.Columns[8].Header = String.Format("{0} C", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketData.Columns[9].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[10].Header = String.Format("{0} C", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[11].Header = String.Format("{0}", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketData.Columns[12].Header = String.Format("{0} C", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketData.Columns[13].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[14].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[15].Header = String.Format("{0}", DateTime.Now.Year - 2000);            
            #endregion

            #region Update EMM Summary SSR report headers
            this.dgEMSummaryMarketSSRData.Columns[3].Header = String.Format("3/{0}e", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketSSRData.Columns[4].Header = String.Format("6/{0}e", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketSSRData.Columns[5].Header = String.Format("9/{0}e", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketSSRData.Columns[6].Header = String.Format("12/{0}e", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketSSRData.Columns[7].Header = String.Format("3/{0}e", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketSSRData.Columns[8].Header = String.Format("6/{0}e", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketSSRData.Columns[9].Header = String.Format("9/{0}e", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketSSRData.Columns[10].Header = String.Format("12/{0}e", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketSSRData.Columns[11].Header = String.Format("-{0}e", DateTime.Now.Year - 2001);
            this.dgEMSummaryMarketSSRData.Columns[12].Header = String.Format("-{0}e", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketSSRData.Columns[13].Header = String.Format("-{0}e", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketSSRData.Columns[14].Header = String.Format("-{0}e", DateTime.Now.Year - 2001);
            this.dgEMSummaryMarketSSRData.Columns[15].Header = String.Format("-{0}e", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketSSRData.Columns[16].Header = String.Format("-{0}e", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketSSRData.Columns[17].Header = String.Format("-{0}e", DateTime.Now.Year - 2001);
            this.dgEMSummaryMarketSSRData.Columns[18].Header = String.Format("-{0}e", DateTime.Now.Year - 2000);

            this.dgEMSummaryMarketSSRData.Columns[19].Header = String.Format("-{0}e", DateTime.Now.Year - 2000);            
            #endregion
        }

        /// <summary>
        /// Updates benchmark column group header and data refresh.
        /// </summary>
        /// <param name="gridView">RadGridView</param>
        /// <param name="columnGroupName">Column Group Name</param>
        /// <param name="benchmarkId">BenchmarkId</param>
        /// <param name="portfolioDate">Portfolio Date</param>
        private void UpdateBenchmarkColumnGroup(RadGridView gridView, String columnGroupName, String benchmarkId, DateTime portfolioDate)
        {
            GridViewColumnGroup benchmarkColumnGroup = gridView.ColumnGroups.Where(g => g.Name == columnGroupName).FirstOrDefault();
            gridView.ColumnGroups.Remove(benchmarkColumnGroup);
            benchmarkColumnGroup = new GridViewColumnGroup()
            {
                Name = columnGroupName,
                Header = benchmarkId,
                HeaderStyle = this.Resources["GridViewColumnGroupHeaderStyle_RightAligned"] as Style
            };
            gridView.ColumnGroups.Add(benchmarkColumnGroup);
            gridView.Columns[1].Header = portfolioDate.ToString("M/dd/yyyy");
        }

        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextEMSummaryMarket.Dispose();
            this.DataContextEMSummaryMarket = null;
            this.DataContext = null;
        }
        #endregion
    }
}
