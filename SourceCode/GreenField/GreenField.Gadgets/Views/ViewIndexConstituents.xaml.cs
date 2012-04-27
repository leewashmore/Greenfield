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
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Data;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

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
            dataContextSource.IndexConstituentDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceIndexConstituentLoadEvent);
            this.DataContextIndexConstituents = dataContextSource;
        }
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceIndexConstituentLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
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
            this.DataContextIndexConstituents.IndexConstituentDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceIndexConstituentLoadEvent);
            this.DataContextIndexConstituents = null;
            this.DataContext = null;
        }
        #endregion
    }
}
