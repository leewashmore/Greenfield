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
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using GreenField.Gadgets.Models;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFinancialStatements : ViewBaseUserControl
    {
        #region Property
        private ViewModelFinancialStatements _dataContextFinancialStatements;
        public ViewModelFinancialStatements DataContextFinancialStatements
        {
            get
            {
                return _dataContextFinancialStatements;
            }
            set
            {
                _dataContextFinancialStatements = value;
            }
        }
        #endregion

        private EntitySelectionData _entitySelectionData;
        private bool _periodIsYearly = true;
        
        #region Constructor
        public ViewFinancialStatements(ViewModelFinancialStatements dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextFinancialStatements = dataContextSource;

            PeriodColumns.UpdateColumnInformation(this.dgFinancialReport, new PeriodColumns.PeriodColumnUpdateEventArg()
            {
                PeriodRecord = PeriodColumns.SetPeriodRecord(),
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(),
                PeriodIsYearly = true
            });
            
            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelFinancialStatements).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgFinancialReport, e);
                    _entitySelectionData = e.EntitySelectionData;
                    _periodIsYearly = e.PeriodIsYearly;
                    this.btnExportExcel.IsEnabled = true;
                }
            };

            //this.rbtnQuarterly.MouseEnter += PeriodColumns.RadRadioButton_MouseEnter;
            //this.rbtnYearly.MouseEnter += PeriodColumns.RadRadioButton_MouseEnter;

            //this.rbtnQuarterly.MouseLeave += PeriodColumns.RadRadioButton_MouseLeave;
            //this.rbtnYearly.MouseLeave += PeriodColumns.RadRadioButton_MouseLeave;

            //this.rbtnQuarterly.Checked += PeriodColumns.RadRadioButton_Checked;
            //this.rbtnYearly.Checked += PeriodColumns.RadRadioButton_Checked;
        }
        #endregion

        #region Event Handlers
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumns.PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodColumnNavigationDirection = PeriodColumns.NavigationDirection.LEFT
            });
            e.Handled = true;
        }

        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumns.PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodColumnNavigationDirection = PeriodColumns.NavigationDirection.RIGHT
            });
            e.Handled = true;
        }

        private void dgFinancialReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
        #endregion

        #region Event Unsubscribe
        public override void Dispose()
        {
            //this.rbtnQuarterly.MouseEnter -= PeriodColumns.RadRadioButton_MouseEnter;
            //this.rbtnYearly.MouseEnter -= PeriodColumns.RadRadioButton_MouseEnter;

            //this.rbtnQuarterly.MouseLeave -= PeriodColumns.RadRadioButton_MouseLeave;
            //this.rbtnYearly.MouseLeave -= PeriodColumns.RadRadioButton_MouseLeave;

            //this.rbtnQuarterly.Checked -= PeriodColumns.RadRadioButton_Checked;
            //this.rbtnYearly.Checked -= PeriodColumns.RadRadioButton_Checked;

            this.DataContextFinancialStatements.Dispose();
            this.DataContextFinancialStatements = null;
            this.DataContext = null;
        }
        #endregion

        private void dgFinancialReport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 14 });
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            String elementName = "Balance Sheet - " + _entitySelectionData.LongName + " (" + _entitySelectionData.ShortName + ") " +
                (_periodIsYearly ? this.dgFinancialReport.Columns[2].Header : this.dgFinancialReport.Columns[8].Header) + " - " +
                (_periodIsYearly ? this.dgFinancialReport.Columns[7].Header : this.dgFinancialReport.Columns[13].Header);
            RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = elementName, Element = this.dgFinancialReport
                , ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_BALANCE_SHEET);
            childExportOptions.Show();
        }

        

        

        


    }


}
