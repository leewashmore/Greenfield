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
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFinstat : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelFinstat _dataContextFinstat;
        public ViewModelFinstat DataContextFinstat
        {
            get { return _dataContextFinstat; }
            set { _dataContextFinstat = value; }
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
                if (DataContextFinstat != null) //DataContext instance
                    DataContextFinstat.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        public ViewFinstat(ViewModelFinstat dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextFinstat = dataContextSource;
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 4, netColumnCount: 7, isQuarterImplemented: false);
            PeriodColumns.UpdateColumnInformation(this.dgFinstat, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnNamespace = typeof(ViewModelFinstat).FullName,                
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false),
                PeriodIsYearly = true
            }, isQuarterImplemented: false, navigatingColumnStartIndex: 1);

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelConsensusEstimatesDetails).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgFinstat, e, false, 1);
                    //this.btnExportExcel.IsEnabled = true;
                }
            };
        } 
        #endregion

        private void dgFinstat_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }

        #region Dispose Method
        public override void Dispose()
        {
            (this.DataContext as ViewModelFinstat).Dispose();
            this.DataContext = null;
        } 
        #endregion
    }
}
