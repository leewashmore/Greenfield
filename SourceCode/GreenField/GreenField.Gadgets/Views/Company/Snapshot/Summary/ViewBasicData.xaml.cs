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
    public partial class ViewBasicData : ViewBaseUserControl
    {
        #region PROPERTIES

        /// <summary>
        /// Private variable to hold data
        /// </summary>
        private ViewModelBasicData dataContextSource = null;
        public ViewModelBasicData DataContextSourceModel
        {
            get
            {
                return dataContextSource;
            }
            set
            {
                if (value != null)
                    dataContextSource = value;
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
                if (DataContextSourceModel != null) //DataContext instance
                    DataContextSourceModel.IsActive = isActive;
            }
        }
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DataContextSource"></param>
        public ViewBasicData(ViewModelBasicData DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
            this.DataContextSourceModel = DataContextSource;
            DataContextSource.BasicDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceBasicDataLoadEvent);
            
        }
        #endregion

        #region Event

        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceBasicDataLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.busyIndicatorGrid.IsBusy = true;
            else
                this.busyIndicatorGrid.IsBusy = false;
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
                if (this.dgBasicData.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                    {
                          new RadExportOptions() { ElementName = "Market Data", Element = this.dgBasicData, 
                              ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " +
                        GadgetNames.HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        /// <summary>
        /// Styles added to export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgBasicData_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion
    }
}
