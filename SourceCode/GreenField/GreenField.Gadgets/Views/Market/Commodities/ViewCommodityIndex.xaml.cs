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
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;



namespace GreenField.Gadgets.Views
{
    public partial class ViewCommodityIndex : ViewBaseUserControl
    {
        # region PRIVATE FIELDS

        /// <summary>
        /// Private variable to hold data
        /// </summary>
        private List<FXCommodityData> commodityInfo;

        /// <summary>
        /// Private variable to hold next year's value
        /// </summary>
        private int nextYear = DateTime.Now.Year + 1;

        /// <summary>
        /// Private variable to hold next to next year's value
        /// </summary>
        private int twoYearsFuture = DateTime.Now.Year + 2;

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Private variable to hold ViewModel property
        /// </summary>
        private ViewModelCommodityIndex dataContextSource = null;        
        public ViewModelCommodityIndex DataContextSource
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
        /// <summary>
        /// Private variable to hold IsActive property of parent user control
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSource != null)
                {
                    DataContextSource.IsActive = isActive;
                }
            }
        }

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCommodityIndex(ViewModelCommodityIndex dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
            dataContextSource.RetrieveCommodityDataCompleteEvent += new RetrieveCommodityDataCompleteEventHandler(RetrieveCommodityDataCompletedEvent);

        }

        #endregion
        #region Event
        /// <summary>
        /// Data completed Event
        /// </summary>
        /// <param name="e"></param>
        public void RetrieveCommodityDataCompletedEvent(RetrieveCommodityDataCompleteEventArgs e)
        {
            commodityInfo = e.CommodityInfo;
            if (commodityInfo != null)
            {                
                dgCommodity.Columns[5].Header = "Price(" + nextYear.ToString() + ")";
                dgCommodity.Columns[6].Header = "Price(" + twoYearsFuture.ToString() + ")";

                dgCommodity.Columns[5].UniqueName = "Price(" + nextYear.ToString() + ")";
                dgCommodity.Columns[6].UniqueName = "Price(" + twoYearsFuture.ToString() + ")";
            }
        }
        #endregion

        private void dgCommodity_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           
        }
        #region ExportToExcel

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
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Commodity Data",
                    Element = this.dgCommodity,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

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
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Commodity Data",
                    Element = this.dgCommodity,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN);
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);                
            }
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {

                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Commodity Data", Element = this.dgCommodity, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }


        #endregion
    }
}
