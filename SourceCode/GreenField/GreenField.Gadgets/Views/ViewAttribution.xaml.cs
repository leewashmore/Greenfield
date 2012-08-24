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
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Class for the Attribution View
    /// </summary>
    public partial class ViewAttribution : ViewBaseUserControl
    {
        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DataContextSource">ViewModelAttribution as Data context for this View</param>
        public ViewAttribution(ViewModelAttribution dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextAttribution = dataContextSource;
            dataContextSource.attributionDataLoadedEvent +=
        new DataRetrievalProgressIndicatorEventHandler(dataContextSource_attributionDataLoadedEvent);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelAttribution _dataContextAttribution;
        public ViewModelAttribution DataContextAttribution
        {
            get { return _dataContextAttribution; }
            set { _dataContextAttribution = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextAttribution != null)
                    DataContextAttribution.IsActive = _isActive;
            }
        }

        #endregion

        #region EventHandler

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_attributionDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }
        #endregion

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextAttribution.attributionDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_attributionDataLoadedEvent);
            this.DataContextAttribution.Dispose();
            this.DataContextAttribution = null;
            this.DataContext = null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// When row gets loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttribution_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           
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
                if (this.dgAttribution.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                  
                      new RadExportOptions() { ElementName = "Performance Attribution", Element = this.dgAttribution, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_ATTRIBUTION);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void dgAttribution_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion
    }
}
