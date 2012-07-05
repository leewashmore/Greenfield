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
using System.IO;
using Telerik.Windows.Controls.GridView;
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Data;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;

namespace GreenField.Gadgets.Views
{
    public partial class ViewIndexConstituents : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelIndexConstituents _dataContextIndexConstituents;
        public ViewModelIndexConstituents DataContextIndexConstituents
        {
            get { return _dataContextIndexConstituents; }
            set { _dataContextIndexConstituents = value; }
        }

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
                if (DataContextIndexConstituents != null) //DataContext instance
                    DataContextIndexConstituents.IsActive = _isActive;
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

        #region Event
       
        /// <summary>
        /// Handling row loaded event of grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgIndexConstituents_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e); 
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
                        Element = this.dgIndexConstituents,
                        ElementName = "Index Constituent Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.BENCHMARK_INDEX_CONSTITUENTS);
            childExportOptions.Show();
           }
        private void dgIndexConstituents_ElementExporting(object sender, GridViewElementExportingEventArgs e)
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
            this.DataContextIndexConstituents.Dispose();
            this.DataContextIndexConstituents = null;
            this.DataContext = null;
        }
        #endregion

        
    }
}
