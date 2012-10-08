using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    public partial class ViewTopHoldings : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelTopHoldings dataContextViewModelTopHoldings;
        public ViewModelTopHoldings DataContextViewModelTopHoldings
        {
            get { return dataContextViewModelTopHoldings; }
            set { dataContextViewModelTopHoldings = value; }
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
                if (DataContextViewModelTopHoldings != null)
                { DataContextViewModelTopHoldings.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewTopHoldings(ViewModelTopHoldings dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelTopHoldings = dataContextSource;
        }
        #endregion

        #region Export To Excel Methods
        /// <summary>
        /// method to export the grid
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
                        Element = this.dgTopHoldings,
                        ElementName = "Top 10 Holdings Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS);
            childExportOptions.Show();
        }

        /// <summary>
        /// handling element exporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgTopHoldings_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelTopHoldings.Dispose();
            this.DataContextViewModelTopHoldings = null;
            this.DataContext = null;
        }
        #endregion
    }
}
