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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.DataContracts.DataContracts;
using GreenField.ServiceCaller;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Views
{
    public partial class ViewEMSummaryMarketData : ViewBaseUserControl
    {
        List<EMSummaryMarketData> data = new List<EMSummaryMarketData>();
        public ViewEMSummaryMarketData(ViewModelEMSummaryMarketData dataContextSource)
        {
            InitializeComponent();
            this.txtCurrDate.Text = DateTime.Today.ToString("MMMM dd, yyyy");
            this.DataContext = dataContextSource;
            dataContextSource.RetrieveEMSummaryDataCompletedEvent += new
                RetrieveEMSummaryDataCompleteEventHandler(dataContextSource_RetrieveEMSummaryDataCompletedEvent);
        }
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        private void dataContextSource_RetrieveEMSummaryDataCompletedEvent(Common.RetrieveEMSummaryDataCompleteEventArgs e)
        {
            data = e.EMSummaryInfo;

            if (data != null)
            {
                for (int i = 4; i < this.dgEMSummaryMarketData.Columns.Count; i++)
                {
                    String propertyName = ((GridViewDataColumn)this.dgEMSummaryMarketData.Columns[i])
                        .DataMemberBinding.Path.Path.ToString();
                    this.dgEMSummaryMarketData.Columns[i].AggregateFunctions.Add(new AggregateFunctionEMDataSummary() { SourceField = propertyName });
                }
                this.dgEMSummaryMarketData.ItemsSource = data;

                InitializeGridHeaders();
            }
        }
        private void InitializeGridHeaders()
        {
            //this.dgEMSummaryMarketData.ColumnGroups[1].Header = data.First().BenchmarkId;
            //DateTime portfolioDate = data.First().PortfolioDate;
            //this.dgEMSummaryMarketData.Columns[1].Header = portfolioDate.ToString("M/dd/yyyy");

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

            foreach (GridViewColumnGroup item in this.dgEMSummaryMarketData.ColumnGroups)
            {

            }
        }

        #region Export

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
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = this.dgEMSummaryMarketData.Tag.ToString()
                        , Element = this.dgEMSummaryMarketData, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

                //else if (this.dgEMSummaryMarketSSRData.Visibility == Visibility.Visible)
                //    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = this.dgEMSummaryMarketSSRData.Tag.ToString()
                //        ,Element = this.dgEMSummaryMarketSSRData, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER});

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " 
                    + GadgetNames.MODELS_FX_MACRO_ECONOMICS_EM_DATA_REPORT);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        #endregion

        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgEMSummaryMarketData.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgEMSummaryMarketData, 12);
                }
                //else if (this.dgEMSummaryMarketSSRData.Visibility == Visibility.Visible)
                //{
                //    PDFExporter.btnExportPDF_Click(this.dgEMSummaryMarketSSRData, 12);
                //}
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);                
            }
        }

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
                //else if (this.dgEMSummaryMarketSSRData.Visibility == Visibility.Visible)
                //{
                //    Dispatcher.BeginInvoke((Action)(() =>
                //    {
                //        RichTextBox.Document = PDFExporter.Print(this.dgEMSummaryMarketSSRData, 12);
                //    }));                    
                //}
                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);                
            }
        }


        #region Flipping

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            //if (this.chEVEBITDA.Visibility == System.Windows.Visibility.Visible)
            //{
            //    Flipper.FlipItem(this.chEVEBITDA, this.dgEVEBITDA);
            //}
            //else
            //{
            //    Flipper.FlipItem(this.dgEVEBITDA, this.chEVEBITDA);
            //}
        }

        #endregion

    }
}
