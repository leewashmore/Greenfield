using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    public partial class ViewIndexConstituents : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelIndexConstituents dataContextIndexConstituents;
        public ViewModelIndexConstituents DataContextIndexConstituents
        {
            get { return dataContextIndexConstituents; }
            set { dataContextIndexConstituents = value; }
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
                if (DataContextIndexConstituents != null)
                { DataContextIndexConstituents.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewIndexConstituents(ViewModelIndexConstituents dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextIndexConstituents = dataContextSource;
        }
        #endregion

        #region Export To Excel Methods
        /// <summary>
        /// method to catch export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ChildExportOptions childExportOptions = new ChildExportOptions( new List<RadExportOptions>
                { new RadExportOptions() 
                    {
                        Element = this.dgIndexConstituents,
                        ElementName = "Index Constituent Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.BENCHMARK_INDEX_CONSTITUENTS);
            childExportOptions.Show();
        }

        /// <summary>
        /// handles element exporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgIndexConstituents_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextIndexConstituents.Dispose();
            this.DataContextIndexConstituents = null;
            this.DataContext = null;
        }
        #endregion
    }
}
