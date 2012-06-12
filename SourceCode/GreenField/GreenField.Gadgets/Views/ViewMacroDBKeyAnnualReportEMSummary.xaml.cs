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
using GreenField.DataContracts;
using Telerik.Windows.Data;
using System.ComponentModel;
using Telerik.Windows.Controls;
using System.Windows.Data;
using GreenField.Common;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMacroDBKeyAnnualReportEMSummary : ViewBaseUserControl
    {
        private List<MacroDatabaseKeyAnnualReportData> _macroInfo;
        private int _currentYear = DateTime.Now.Year;
        public ViewMacroDBKeyAnnualReportEMSummary(ViewModelMacroDBKeyAnnualReportEMSummary dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.RetrieveMacroEMSummaryDataCompletedEvent += new Common.RetrieveMacroCountrySummaryDataCompleteEventHandler(dataContextSource_RetrieveMacroDataCompletedEvent);
            dataContextSource.macroDBKeyAnnualReportEMSummaryDataLoadedEvent +=
           new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportEMSummaryDataLoadedEvent);
            SetGridColumnHeaders();

        }

        /// <summary>
        /// Event Handler for LeftNavigation Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                (this.DataContext as ViewModelMacroDBKeyAnnualReportEMSummary).MoveLeftCommandMethod(null);
        }

        /// <summary>
        /// Event Handler for Right navigation click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                (this.DataContext as ViewModelMacroDBKeyAnnualReportEMSummary).MoveRightCommandMethod(null);
        }

         /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_macroDBKeyAnnualReportEMSummaryDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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

        public void dataContextSource_RetrieveMacroDataCompletedEvent(Common.RetrieveMacroCountrySummaryDataCompleteEventArgs e)
        {
            //if (_currentYear == 2022)
            //    return;

            _macroInfo = e.MacroInfo;
            this.dgMacroDBKeyReport.ItemsSource = ((ViewModelMacroDBKeyAnnualReportEMSummary)this.DataContext).FiveYearMacroCountryData;
            this.dgMacroDBKeyReport.GroupDescriptors.Clear();
            this.dgMacroDBKeyReport.SortDescriptors.Clear();
            _currentYear = ((ViewModelMacroDBKeyAnnualReportEMSummary)this.DataContext).CurrentYear;
            if (_currentYear >= 2024 || _currentYear <= 1989)
                return;
            if (_macroInfo != null)
            {
                GroupDescriptor descriptor = new GroupDescriptor();
                descriptor.Member = "CategoryName";
                descriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.GroupDescriptors.Add(descriptor);
                SortDescriptor sdescriptor = new SortDescriptor();
                sdescriptor.Member = "SortOrder";
                sdescriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.SortDescriptors.Add(sdescriptor);

                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                dgMacroDBKeyReport.Columns[2].Header = (_currentYear - 3).ToString();
                dgMacroDBKeyReport.Columns[3].Header = (_currentYear - 2).ToString();
                dgMacroDBKeyReport.Columns[4].Header = (_currentYear - 1).ToString();
                dgMacroDBKeyReport.Columns[5].Header = (_currentYear).ToString();
                dgMacroDBKeyReport.Columns[6].Header = (_currentYear + 1).ToString();
                dgMacroDBKeyReport.Columns[7].Header = (_currentYear + 2).ToString();
                int currentYear = DateTime.Today.Year;
                dgMacroDBKeyReport.Columns[8].Header = "Five Year Average" + "(" + (currentYear - 4).ToString() + "-" + (currentYear).ToString() + ")";
            }
        }


        public void SetGridColumnHeaders()
        {
            int currentYear = DateTime.Today.Year;
            dgMacroDBKeyReport.Columns[2].Header = (currentYear - 3).ToString();
            dgMacroDBKeyReport.Columns[3].Header = (currentYear - 2).ToString();
            dgMacroDBKeyReport.Columns[4].Header = (currentYear - 1).ToString();
            dgMacroDBKeyReport.Columns[5].Header = (currentYear).ToString();
            dgMacroDBKeyReport.Columns[6].Header = (currentYear + 1).ToString();
            dgMacroDBKeyReport.Columns[7].Header = (currentYear + 2).ToString();
            dgMacroDBKeyReport.Columns[8].Header = "Five Year Average" + "(" + (currentYear - 4).ToString() + "-" + (currentYear).ToString() + ")";

        }

        private void dgMacroDBKeyReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
