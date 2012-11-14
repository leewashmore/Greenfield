using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for ConsensusEstimates-Valuations Gadget
    /// </summary>
    public partial class ViewValuations : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Property of View-Model
        /// </summary>
        private ViewModelValuations dataContextValuations;
        public ViewModelValuations DataContextValuations
        {
            get { return dataContextValuations; }
            set { dataContextValuations = value; }
        }

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData entitySelectionData;
        private bool _periodIsYearly = true;

        #endregion

        #region ActiveDashboard
        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextValuations != null)
                {
                    DataContextValuations.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewValuations(ViewModelValuations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextValuations = dataContextSource;
            InitializeComponent();
            this.DataContext = dataContextSource;

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimateValuations, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false),
                PeriodIsYearly = true
            });

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelValuations).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimateValuations, e);
                    this.btnExportExcel.IsEnabled = true;
                    this.dgConsensusEstimateValuations.Columns[0].Header = "Consensus Valuations in " + this.DataContextValuations.SelectedCurrency;
                }
            };
        }

        #endregion

        #region GridNavigationButtons

        /// <summary>
        /// Left Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);
            try
            {
                PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
                    {
                        PeriodColumnNamespace = typeof(ViewModelValuations).FullName,
                        PeriodColumnNavigationDirection = NavigationDirection.LEFT
                    });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }

        /// <summary>
        /// Right Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);
            try
            {
                PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
                    {
                        PeriodColumnNamespace = typeof(ViewModelValuations).FullName,
                        PeriodColumnNavigationDirection = NavigationDirection.RIGHT
                    });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }
        #endregion

        #region EventsUnsusbcribe
        /// <summary>
        /// Dispose method to unsubscribe Events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelValuations).Dispose();
            this.DataContext = null;
        }
        #endregion

        #region Export
        /// <summary>
        /// Excel exporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgConsensusEstimateValuations_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 12 });
        }
        
        /// <summary>
        /// Export to Excel Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                String elementName = "Consensus Estimate - " + (this.DataContextValuations.EntitySelectionInfo).LongName + 
                    " (" + (this.DataContextValuations.EntitySelectionInfo).ShortName + ") " +
                    (_periodIsYearly ? this.dgConsensusEstimateValuations.Columns[2].Header : this.dgConsensusEstimateValuations.Columns[6].Header) + " - " +
                    (_periodIsYearly ? this.dgConsensusEstimateValuations.Columns[7].Header : this.dgConsensusEstimateValuations.Columns[11].Header);
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = elementName,
                    Element = this.dgConsensusEstimateValuations,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);
            try
            {
                if (this.dgConsensusEstimateValuations.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgConsensusEstimateValuations, 6);
                    }));

                    this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                    RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
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
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);
            try
            {
                if (this.dgConsensusEstimateValuations.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgConsensusEstimateValuations);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }
        #endregion
    }
}