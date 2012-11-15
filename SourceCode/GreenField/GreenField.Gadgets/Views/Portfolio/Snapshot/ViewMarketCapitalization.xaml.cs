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
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;


namespace GreenField.Gadgets.Views
{
    public partial class ViewMarketCapitalization : ViewBaseUserControl
    {

        #region CONSTRUCTOR
        public ViewMarketCapitalization(ViewModelMarketCapitalization dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
            dataContextSource.MarketCapitalizationDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceMarketCapitalizationLoadEvent);
        }
        #endregion

        #region PROPERTIES

        private ViewModelMarketCapitalization dataContextSource = null;
        public ViewModelMarketCapitalization DataContextSource
        {
            get
            {
                return dataContextSource;
            }
            set
            {
                if (value != null)
                {
                    dataContextSource = value;
                }
            }
        }

        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSource != null) //DataContext instance
                {
                    DataContextSource.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceMarketCapitalizationLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.gridBusyIndicator.IsBusy = true;
            }
            else
            {
                this.gridBusyIndicator.IsBusy = false;
            }
        }

        /// <summary>
        /// catches export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = GadgetNames.HOLDINGS_MARKET_CAPITALIZATION,
                Element = this.dgMarketCapData,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_MARKET_CAPITALIZATION);
            childExportOptions.Show();
        }

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
                ElementName = GadgetNames.HOLDINGS_MARKET_CAPITALIZATION,
                Element = this.dgMarketCapData,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_MARKET_CAPITALIZATION);
            childExportOptions.Show();
        }

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
                ElementName = GadgetNames.HOLDINGS_MARKET_CAPITALIZATION,
                Element = this.dgMarketCapData,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_MARKET_CAPITALIZATION);
            childExportOptions.Show();
        }

        /// <summary>
        /// Styles added to export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgMarketCapData_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion
    }
}
