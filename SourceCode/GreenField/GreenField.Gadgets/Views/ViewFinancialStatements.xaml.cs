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

namespace GreenField.Gadgets.Views
{
    public partial class ViewFinancialStatements : ViewBaseUserControl
    {
        private int _iterator = 0;

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

        #region Constructor
        public ViewFinancialStatements(ViewModelFinancialStatements dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextFinancialStatements = dataContextSource;
            SetColumnHeaders();
            DataContextFinancialStatements.SetFinancialStatementDisplayInfo();            
        }
        #endregion

        #region Event Handlers
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetColumnHeaders(Convert.ToBoolean(this.rbtnYearly.IsChecked),--_iterator);
            PeriodColumn.RaiseNavigationCompleted();
            //DataContextFinancialStatements.BusyIndicatorNotification(true, "Retrieving data for updated time span");
            //DataContextFinancialStatements.SetFinancialStatementDisplayInfo();
        }

        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SetColumnHeaders(Convert.ToBoolean(this.rbtnYearly.IsChecked),++_iterator);
            PeriodColumn.RaiseNavigationCompleted();
            //DataContextFinancialStatements.BusyIndicatorNotification(true, "Retrieving data for updated time span");
            //DataContextFinancialStatements.SetFinancialStatementDisplayInfo();
        }

        private void RadRadioButton_MouseEnter(object sender, MouseEventArgs e)
        {
            (sender as RadRadioButton).Foreground = new SolidColorBrush(Colors.Black);
        }

        private void RadRadioButton_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as RadRadioButton).Foreground = new SolidColorBrush(Colors.White);
        }

        private void rbtnYearly_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rbtnYearly == null)
                return;
            if (this.rbtnYearly.IsChecked == null)
                return;
            this.rbtnYearly.Foreground = Convert.ToBoolean(this.rbtnYearly.IsChecked)
                ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);

            SetColumnHeaders(true, _iterator = 0);
            UpdateColumnVisibility();            
        }

        private void rbtnQuarterly_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rbtnQuarterly == null)
                return;
            if (this.rbtnQuarterly.IsChecked == null)
                return;
            this.rbtnQuarterly.Foreground = Convert.ToBoolean(this.rbtnQuarterly.IsChecked)
                ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
            SetColumnHeaders(false, _iterator = 0);
            UpdateColumnVisibility(reportTypeIsYearly: false);
        }
        #endregion

        #region Event Unsubscribe
        public override void Dispose()
        {
            this.DataContextFinancialStatements.Dispose();
            this.DataContextFinancialStatements = null;
            this.DataContext = null;
        }
        #endregion

        private int GetQuarter(int month)
        {
            return month < 4 ? 1 : ( month < 7 ? 2 : ( month < 10 ? 3 : 4 ) );
        }

        private void UpdateColumnVisibility(bool reportTypeIsYearly = true)
        {
            this.dgvcYearOne.IsVisible = reportTypeIsYearly;
            this.dgvcYearTwo.IsVisible = reportTypeIsYearly;
            this.dgvcYearThree.IsVisible = reportTypeIsYearly;
            this.dgvcYearFour.IsVisible = reportTypeIsYearly;
            this.dgvcYearFive.IsVisible = reportTypeIsYearly;
            this.dgvcYearSix.IsVisible = reportTypeIsYearly;

            this.dgvcQuarterOne.IsVisible = !(reportTypeIsYearly);
            this.dgvcQuarterTwo.IsVisible = !(reportTypeIsYearly);
            this.dgvcQuarterThree.IsVisible = !(reportTypeIsYearly);
            this.dgvcQuarterFour.IsVisible = !(reportTypeIsYearly);
            this.dgvcQuarterFive.IsVisible = !(reportTypeIsYearly);
            this.dgvcQuarterSix.IsVisible = !(reportTypeIsYearly);

        }

        private void SetColumnHeaders(bool reportTypeIsYearly = true, int incrementFactor = 0)
        {
            int presentYear = DateTime.Today.Year;
            int presentMonth = DateTime.Today.Month;
            int presentQuarter = GetQuarter(presentMonth);

            PeriodRecord.YearOne = presentYear - 3 + incrementFactor;
            PeriodRecord.YearOneIsHistorical = PeriodRecord.YearOne < presentYear;

            PeriodRecord.YearTwo = PeriodRecord.YearOne + 1;
            PeriodRecord.YearTwoIsHistorical = PeriodRecord.YearTwo < presentYear;

            PeriodRecord.YearThree = PeriodRecord.YearTwo + 1;
            PeriodRecord.YearThreeIsHistorical = PeriodRecord.YearThree < presentYear;

            PeriodRecord.YearFour = PeriodRecord.YearThree + 1;
            PeriodRecord.YearFourIsHistorical = PeriodRecord.YearFour < presentYear;

            PeriodRecord.YearFive = PeriodRecord.YearFour + 1;
            PeriodRecord.YearFiveIsHistorical = PeriodRecord.YearFive < presentYear;

            PeriodRecord.YearSix = PeriodRecord.YearFive + 1;
            PeriodRecord.YearSixIsHistorical = PeriodRecord.YearSix < presentYear;

            PeriodRecord.QuarterOneYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 4) * 3).Year;
            PeriodRecord.QuarterOneQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 4) * 3).Month);
            PeriodRecord.QuarterOneIsHistorical = PeriodRecord.QuarterOneYear < presentYear 
                ? true : (PeriodRecord.QuarterOneYear == presentYear ? PeriodRecord.QuarterOneQuarter < presentQuarter : false);

            PeriodRecord.QuarterTwoYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 3) * 3).Year;
            PeriodRecord.QuarterTwoQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 3) * 3).Month);
            PeriodRecord.QuarterTwoIsHistorical = PeriodRecord.QuarterTwoYear < presentYear
                ? true : (PeriodRecord.QuarterTwoYear == presentYear ? PeriodRecord.QuarterTwoQuarter < presentQuarter : false);


            PeriodRecord.QuarterThreeYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 2) * 3).Year;
            PeriodRecord.QuarterThreeQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 2) * 3).Month);
            PeriodRecord.QuarterThreeIsHistorical = PeriodRecord.QuarterThreeYear < presentYear
                ? true : (PeriodRecord.QuarterThreeYear == presentYear ? PeriodRecord.QuarterThreeQuarter < presentQuarter : false);

            PeriodRecord.QuarterFourYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 1) * 3).Year;
            PeriodRecord.QuarterFourQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 1) * 3).Month);
            PeriodRecord.QuarterFourIsHistorical = PeriodRecord.QuarterFourYear < presentYear
                ? true : (PeriodRecord.QuarterFourYear == presentYear ? PeriodRecord.QuarterFourQuarter < presentQuarter : false);

            PeriodRecord.QuarterFiveYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor) * 3).Year;
            PeriodRecord.QuarterFiveQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor) * 3).Month);
            PeriodRecord.QuarterFiveIsHistorical = PeriodRecord.QuarterFiveYear < presentYear
                ? true : (PeriodRecord.QuarterFiveYear == presentYear ? PeriodRecord.QuarterFiveQuarter < presentQuarter : false);

            PeriodRecord.QuarterSixYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor + 1) * 3).Year;
            PeriodRecord.QuarterSixQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor + 1) * 3).Month);
            PeriodRecord.QuarterSixIsHistorical = PeriodRecord.QuarterSixYear < presentYear
                ? true : (PeriodRecord.QuarterSixYear == presentYear ? PeriodRecord.QuarterSixQuarter < presentQuarter : false);

            //PeriodRecord.QuarterOneYear = GetQuarter(DateTime.Today.Month) == 1 ? DateTime.Today.Year - 1

            if (reportTypeIsYearly)
            {
                this.dgFinancialReport.Columns[2].Header = PeriodRecord.YearOne.ToString() + " " + (PeriodRecord.YearOneIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[3].Header = PeriodRecord.YearTwo.ToString() + " " + (PeriodRecord.YearTwoIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[4].Header = PeriodRecord.YearThree.ToString() + " " + (PeriodRecord.YearThreeIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[5].Header = PeriodRecord.YearFour.ToString() + " " + (PeriodRecord.YearFourIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[6].Header = PeriodRecord.YearFive.ToString() + " " + (PeriodRecord.YearFiveIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[7].Header = PeriodRecord.YearSix.ToString() + " " + (PeriodRecord.YearSixIsHistorical ? "A" : "E");
            }
            else
            {
                this.dgFinancialReport.Columns[8].Header = PeriodRecord.QuarterOneYear.ToString() + " Q" + PeriodRecord.QuarterOneQuarter.ToString() + " " + (PeriodRecord.QuarterOneIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[9].Header = PeriodRecord.QuarterTwoYear.ToString() + " Q" + PeriodRecord.QuarterTwoQuarter.ToString() + " " + (PeriodRecord.QuarterTwoIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[10].Header = PeriodRecord.QuarterThreeYear.ToString() + " Q" + PeriodRecord.QuarterThreeQuarter.ToString() + " " + (PeriodRecord.QuarterThreeIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[11].Header = PeriodRecord.QuarterFourYear.ToString() + " Q" + PeriodRecord.QuarterFourQuarter.ToString() + " " + (PeriodRecord.QuarterFourIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[12].Header = PeriodRecord.QuarterFiveYear.ToString() + " Q" + PeriodRecord.QuarterFiveQuarter.ToString() + " " + (PeriodRecord.QuarterFiveIsHistorical ? "A" : "E");
                this.dgFinancialReport.Columns[13].Header = PeriodRecord.QuarterSixYear.ToString() + " Q" + PeriodRecord.QuarterSixQuarter.ToString() + " " + (PeriodRecord.QuarterSixIsHistorical ? "A" : "E");
            }
        }

        private void dgFinancialReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }       

        
    }

    
}
