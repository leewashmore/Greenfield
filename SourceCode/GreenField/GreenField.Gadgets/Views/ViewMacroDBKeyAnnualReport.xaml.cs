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
            dataContextSource.RetrieveMacroDataCompletedEvent +=new Common.RetrieveMacroCountrySummaryDataCompleteEventHandler(dataContextSource_RetrieveMacroDataCompletedEvent);
            dataContextSource.macroDBKeyAnnualReportCountryDataLoadedEvent +=
           new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportCountryDataLoadedEvent);

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

        public void dataContextSource_RetrieveMacroDataCompletedEvent(Common.RetrieveMacroCountrySummaryDataCompleteEventArgs e)
        {            
            _macroInfo = e.MacroInfo;
            this.dgMacroDBKeyReport.ItemsSource = ((ViewModelMacroDBKeyAnnualReport)this.DataContext).FiveYearMacroCountryData;
            this.dgMacroDBKeyReport.Columns.Clear();
            this.dgMacroDBKeyReport.GroupDescriptors.Clear();
            this.dgMacroDBKeyReport.SortDescriptors.Clear();
            _currentYear = ((ViewModelMacroDBKeyAnnualReport)this.DataContext).CurrentYear;
            if (_macroInfo != null)
            {



    //            <helpers:ViewBaseUserControl.Resources>
    //    <ResourceDictionary>
    //        <ResourceDictionary.MergedDictionaries>
    //            <ResourceDictionary Source="/GreenField.Gadgets;component/Assets/Styles.xaml"/>
    //        </ResourceDictionary.MergedDictionaries>
    //    </ResourceDictionary>
    //</helpers:ViewBaseUserControl.Resources>
                //ViewBaseUserControl v = new ViewBaseUserControl();
                
                //ResourceDictionary r = new ResourceDictionary();
                //r.Source = new Uri("/GreenField.Gadgets;component/Assets/Styles.xaml");
                //r.MergedDictionaries.Add(r);
                //v.Resources.Add(

               
             //Telerik.Windows.Controls.GridView.GridViewHeaderCell.BackgroundProperty.

               //GridViewHeaderCell.BackgroundProperty  = 
                GroupDescriptor descriptor = new GroupDescriptor();
                descriptor.Member = "CATEGORY_NAME";
                descriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.GroupDescriptors.Add(descriptor);
                SortDescriptor sdescriptor = new SortDescriptor();
                sdescriptor.Member = "SORT_ORDER";
                sdescriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.SortDescriptors.Add(sdescriptor);              
                GridViewDataColumn column = new GridViewDataColumn();
                column.DataMemberBinding = new Binding("DESCRIPTION");
                column.Header = "DESCRIPTION";
                column.UniqueName = "MyColumn";
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                this.dgMacroDBKeyReport.Columns.Add(column);
                GridViewDataColumn column1 = new GridViewDataColumn();
                column1.DataMemberBinding = new Binding("YEAR_ONE");
                column1.Header = _currentYear.ToString();
                column1.UniqueName = "MyColumn1";                 
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                this.dgMacroDBKeyReport.Columns.Add(column1);
                GridViewDataColumn column2 = new GridViewDataColumn();
                column2.DataMemberBinding = new Binding("YEAR_TWO");
                column2.Header = (_currentYear + 1).ToString();
                column2.UniqueName = "MyColumn2";
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                this.dgMacroDBKeyReport.Columns.Add(column2);
                GridViewDataColumn column3 = new GridViewDataColumn();
                column3.DataMemberBinding = new Binding("YEAR_THREE");
                column3.Header = (_currentYear + 2).ToString();
                column3.UniqueName = "MyColumn3";
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                this.dgMacroDBKeyReport.Columns.Add(column3);
                GridViewDataColumn column4 = new GridViewDataColumn();
                column4.DataMemberBinding = new Binding("YEAR_FOUR");
                column4.Header = (_currentYear + 3).ToString();
                column4.UniqueName = "MyColumn4";
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                this.dgMacroDBKeyReport.Columns.Add(column4);
                GridViewDataColumn column5 = new GridViewDataColumn();
                column5.DataMemberBinding = new Binding("YEAR_FIVE");
                column5.Header = (_currentYear + 4).ToString();
                column5.UniqueName = "MyColumn5";
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                this.dgMacroDBKeyReport.Columns.Add(column5);

            }
        }
    }
}
