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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller.ModelFXDefinitions;
using Telerik.Windows.Data;
using System.ComponentModel;
using Telerik.Windows.Controls;
using System.Windows.Data;
using GreenField.Common;
using Telerik.Windows.Controls.GridView;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMacroDBKeyAnnualReport : ViewBaseUserControl
    {
        private List<MacroDatabaseKeyAnnualReportData> _macroInfo;
        private int _currentYear;


        public ViewMacroDBKeyAnnualReport(ViewModelMacroDBKeyAnnualReport dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.RetrieveMacroDataCompletedEvent += new Common.RetrieveMacroCountrySummaryDataCompleteEventHandler(dataContextSource_RetrieveMacroDataCompletedEvent);
            dataContextSource.macroDBKeyAnnualReportCountryDataLoadedEvent +=
           new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportCountryDataLoadedEvent);
            SetGridColumnHeaders();

        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_macroDBKeyAnnualReportCountryDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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

        /// <summary>
        /// Event Handler for LeftNavigation Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                (this.DataContext as ViewModelMacroDBKeyAnnualReport).MoveLeftCommandMethod(null);
        }

        /// <summary>
        /// Event Handler for Right navigation click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
                (this.DataContext as ViewModelMacroDBKeyAnnualReport).MoveRightCommandMethod(null);
        }

        public void dataContextSource_RetrieveMacroDataCompletedEvent(Common.RetrieveMacroCountrySummaryDataCompleteEventArgs e)
        {
            if (_currentYear == 2022)
                return;

            _macroInfo = e.MacroInfo;
            this.dgMacroDBKeyReport.ItemsSource = ((ViewModelMacroDBKeyAnnualReport)this.DataContext).FiveYearMacroCountryData;                      
            this.dgMacroDBKeyReport.GroupDescriptors.Clear();
            this.dgMacroDBKeyReport.SortDescriptors.Clear();
            _currentYear = ((ViewModelMacroDBKeyAnnualReport)this.DataContext).CurrentYear;
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
                dgMacroDBKeyReport.Columns[2].Header = _currentYear.ToString();
                dgMacroDBKeyReport.Columns[3].Header = (_currentYear + 1).ToString();
                dgMacroDBKeyReport.Columns[4].Header = (_currentYear + 2).ToString();
                dgMacroDBKeyReport.Columns[5].Header = (_currentYear + 3).ToString();
                dgMacroDBKeyReport.Columns[6].Header = (_currentYear + 4).ToString();
                dgMacroDBKeyReport.Columns[7].Header = "Five Year Average" + "(" + _currentYear.ToString() + "-" + (_currentYear + 4).ToString() + ")";
            }
        }

        public void SetGridColumnHeaders()
        {
            int currentYear = DateTime.Today.Year;
            dgMacroDBKeyReport.Columns[2].Header = currentYear.ToString();
            dgMacroDBKeyReport.Columns[3].Header = (currentYear + 1).ToString();
            dgMacroDBKeyReport.Columns[4].Header = (currentYear + 2).ToString();
            dgMacroDBKeyReport.Columns[5].Header = (currentYear + 3).ToString();
            dgMacroDBKeyReport.Columns[6].Header = (currentYear + 4).ToString();
            dgMacroDBKeyReport.Columns[7].Header = "Five Year Average"+"(" + currentYear.ToString() + "-" + (currentYear + 4).ToString() + ")";

        }
    }
}
