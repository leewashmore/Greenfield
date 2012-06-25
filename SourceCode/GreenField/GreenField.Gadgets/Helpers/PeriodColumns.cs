using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.Gadgets.Models;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Linq;
using GreenField.Common;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Helpers
{
    public static class PeriodColumns
    {
        #region Period Column Update Event
        public delegate void PeriodColumnUpdateEventHandler(PeriodColumnUpdateEventArg e);

        public class PeriodColumnUpdateEventArg
        {
            public String PeriodColumnNamespace { get; set; }
            public List<String> PeriodColumnHeader { get; set; }
            public EntitySelectionData EntitySelectionData { get; set; }
            public PeriodRecord PeriodRecord { get; set; }
            public bool PeriodIsYearly { get; set; }
        }

        public static event PeriodColumnUpdateEventHandler PeriodColumnUpdate = delegate { };

        public static void RaisePeriodColumnUpdateCompleted(PeriodColumnUpdateEventArg e)
        {
            PeriodColumnUpdateEventHandler periodColumnUpdationEvent = PeriodColumnUpdate;
            if (periodColumnUpdationEvent != null)
            {
                periodColumnUpdationEvent(e);
            }
        } 
        #endregion

        #region Period Column Navigation Event
        public delegate void PeriodColumnNavigationEventHandler(PeriodColumnNavigationEventArg e);

        public enum NavigationDirection
        {
            LEFT,
            RIGHT
        }

        public class PeriodColumnNavigationEventArg
        {
            public String PeriodColumnNamespace { get; set; }
            public NavigationDirection PeriodColumnNavigationDirection { get; set; }
        }

        public static event PeriodColumnNavigationEventHandler PeriodColumnNavigate = delegate { };

        public static void RaisePeriodColumnNavigationCompleted(PeriodColumnNavigationEventArg e)
        {
            PeriodColumnNavigationEventHandler periodColumnNavigationEvent = PeriodColumnNavigate;
            if (periodColumnNavigationEvent != null)
            {
                periodColumnNavigationEvent(e);
            }
        } 
        #endregion

        public static PeriodRecord SetPeriodRecord(int incrementFactor = 0)
        {
            PeriodRecord periodRecord = new PeriodRecord();

            int presentYear = DateTime.Today.Year;
            int presentMonth = DateTime.Today.Month;
            int presentQuarter = GetQuarter(presentMonth);

            periodRecord.YearOne = presentYear - 3 + incrementFactor;
            periodRecord.YearOneIsHistorical = periodRecord.YearOne < presentYear;

            periodRecord.YearTwo = periodRecord.YearOne + 1;
            periodRecord.YearTwoIsHistorical = periodRecord.YearTwo < presentYear;

            periodRecord.YearThree = periodRecord.YearTwo + 1;
            periodRecord.YearThreeIsHistorical = periodRecord.YearThree < presentYear;

            periodRecord.YearFour = periodRecord.YearThree + 1;
            periodRecord.YearFourIsHistorical = periodRecord.YearFour < presentYear;

            periodRecord.YearFive = periodRecord.YearFour + 1;
            periodRecord.YearFiveIsHistorical = periodRecord.YearFive < presentYear;

            periodRecord.YearSix = periodRecord.YearFive + 1;
            periodRecord.YearSixIsHistorical = periodRecord.YearSix < presentYear;

            periodRecord.YearSeven = periodRecord.YearSix + 1;
            periodRecord.YearSevenIsHistorical = periodRecord.YearSeven < presentYear;

            periodRecord.QuarterOneYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 4) * 3).Year;
            periodRecord.QuarterOneQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 4) * 3).Month);
            periodRecord.QuarterOneIsHistorical = periodRecord.QuarterOneYear < presentYear
                ? true : (periodRecord.QuarterOneYear == presentYear ? periodRecord.QuarterOneQuarter < presentQuarter : false);

            periodRecord.QuarterTwoYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 3) * 3).Year;
            periodRecord.QuarterTwoQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 3) * 3).Month);
            periodRecord.QuarterTwoIsHistorical = periodRecord.QuarterTwoYear < presentYear
                ? true : (periodRecord.QuarterTwoYear == presentYear ? periodRecord.QuarterTwoQuarter < presentQuarter : false);


            periodRecord.QuarterThreeYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 2) * 3).Year;
            periodRecord.QuarterThreeQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 2) * 3).Month);
            periodRecord.QuarterThreeIsHistorical = periodRecord.QuarterThreeYear < presentYear
                ? true : (periodRecord.QuarterThreeYear == presentYear ? periodRecord.QuarterThreeQuarter < presentQuarter : false);

            periodRecord.QuarterFourYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 1) * 3).Year;
            periodRecord.QuarterFourQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - 1) * 3).Month);
            periodRecord.QuarterFourIsHistorical = periodRecord.QuarterFourYear < presentYear
                ? true : (periodRecord.QuarterFourYear == presentYear ? periodRecord.QuarterFourQuarter < presentQuarter : false);

            periodRecord.QuarterFiveYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor) * 3).Year;
            periodRecord.QuarterFiveQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor) * 3).Month);
            periodRecord.QuarterFiveIsHistorical = periodRecord.QuarterFiveYear < presentYear
                ? true : (periodRecord.QuarterFiveYear == presentYear ? periodRecord.QuarterFiveQuarter < presentQuarter : false);

            periodRecord.QuarterSixYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor + 1) * 3).Year;
            periodRecord.QuarterSixQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor + 1) * 3).Month);
            periodRecord.QuarterSixIsHistorical = periodRecord.QuarterSixYear < presentYear
                ? true : (periodRecord.QuarterSixYear == presentYear ? periodRecord.QuarterSixQuarter < presentQuarter : false);

            return periodRecord;
        }

        public static List<String> SetColumnHeaders(PeriodRecord periodRecord = null)
        {
            if (periodRecord == null)
                periodRecord = SetPeriodRecord();
            
            List<String> periodColumnHeader = new List<string>();

            periodColumnHeader.Add(periodRecord.YearOne.ToString() + " " + (periodRecord.YearOneIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.YearTwo.ToString() + " " + (periodRecord.YearTwoIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.YearThree.ToString() + " " + (periodRecord.YearThreeIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.YearFour.ToString() + " " + (periodRecord.YearFourIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.YearFive.ToString() + " " + (periodRecord.YearFiveIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.YearSix.ToString() + " " + (periodRecord.YearSixIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.QuarterOneYear.ToString() + " Q" + periodRecord.QuarterOneQuarter.ToString() + " " + (periodRecord.QuarterOneIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.QuarterTwoYear.ToString() + " Q" + periodRecord.QuarterTwoQuarter.ToString() + " " + (periodRecord.QuarterTwoIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.QuarterThreeYear.ToString() + " Q" + periodRecord.QuarterThreeQuarter.ToString() + " " + (periodRecord.QuarterThreeIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.QuarterFourYear.ToString() + " Q" + periodRecord.QuarterFourQuarter.ToString() + " " + (periodRecord.QuarterFourIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.QuarterFiveYear.ToString() + " Q" + periodRecord.QuarterFiveQuarter.ToString() + " " + (periodRecord.QuarterFiveIsHistorical ? "A" : "E"));
            periodColumnHeader.Add(periodRecord.QuarterSixYear.ToString() + " Q" + periodRecord.QuarterSixQuarter.ToString() + " " + (periodRecord.QuarterSixIsHistorical ? "A" : "E"));

            return periodColumnHeader;
        }

        private static List<PeriodColumnDisplayData> GetCalendarizedPeriodColumnDisplayInfo(List<PeriodColumnDisplayData> displayData, int month)
        {
            List<PeriodColumnDisplayData> result = new List<PeriodColumnDisplayData>();
            Decimal pastMonthFactor = Convert.ToDecimal(month)/12;
            Decimal postMonthFactor = (12 - Convert.ToDecimal(month)) / 12;
            foreach (PeriodColumnDisplayData info in displayData)
            {

                result.Add(new PeriodColumnDisplayData()
                {
                    DATA_DESC = info.DATA_DESC,
                    YEAR_ONE = (info.YEAR_ONE * pastMonthFactor) + (info.YEAR_TWO * postMonthFactor),
                    YEAR_TWO = (info.YEAR_ONE * pastMonthFactor) + (info.YEAR_THREE * postMonthFactor),
                    YEAR_THREE = (info.YEAR_ONE * pastMonthFactor) + (info.YEAR_FOUR * postMonthFactor),
                    YEAR_FOUR = (info.YEAR_ONE * pastMonthFactor) + (info.YEAR_FIVE * postMonthFactor),
                    YEAR_FIVE = (info.YEAR_ONE * pastMonthFactor) + (info.YEAR_SIX * postMonthFactor),
                    YEAR_SIX = (info.YEAR_ONE * pastMonthFactor) + (info.YEAR_SEVEN * postMonthFactor),
                    QUARTER_ONE = info.QUARTER_ONE,
                    QUARTER_TWO = info.QUARTER_TWO,
                    QUARTER_THREE = info.QUARTER_THREE,
                    QUARTER_FOUR = info.QUARTER_FOUR,
                    QUARTER_FIVE = info.QUARTER_FIVE,
                    QUARTER_SIX = info.QUARTER_SIX,
                });
            }

            return result;
        }

        public static List<PeriodColumnDisplayData> SetPeriodColumnDisplayInfo(object periodColumnInfo, PeriodRecord periodRecord, String currency = "USD", Int32? calendarizeMonth = 12)
        {
            List<PeriodColumnDisplayData> result = new List<PeriodColumnDisplayData>();
            calendarizeMonth = calendarizeMonth == null ? 12 : calendarizeMonth;


            if (periodColumnInfo is List<FinancialStatementData>)
            {
                List<FinancialStatementData> financialStatementInfo = (periodColumnInfo as List<FinancialStatementData>)
                    .OrderBy(record => record.SORT_ORDER)
                    .ThenBy(record => record.PERIOD_TYPE)
                    .ThenBy(record => record.PERIOD)
                    .ToList(); ;

                List<String> distinctDataDescriptors = financialStatementInfo.Select(record => record.DATA_DESC).Distinct().ToList();

                foreach (string dataDesc in distinctDataDescriptors)
                {
                    FinancialStatementData yearOneData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearOne.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearOneIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData yearTwoData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearTwo.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearTwoIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData yearThreeData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearThree.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearThreeIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData yearFourData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearFour.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearFourIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData yearFiveData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearFive.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearFiveIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData yearSixData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearSix.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearSixIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData yearSevenData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.YearSeven.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.YearSevenIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "A" && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData quarterOneData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.QuarterOneYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.QuarterOneIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + periodRecord.QuarterOneQuarter.ToString().ToUpper().Trim() && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData quarterTwoData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.QuarterTwoYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.QuarterTwoIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + periodRecord.QuarterTwoQuarter.ToString().ToUpper().Trim() && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData quarterThreeData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.QuarterThreeYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.QuarterThreeIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + periodRecord.QuarterThreeQuarter.ToString().ToUpper().Trim() && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData quarterFourData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.QuarterFourYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.QuarterFourIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + periodRecord.QuarterFourQuarter.ToString().ToUpper().Trim() && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData quarterFiveData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.QuarterFiveYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.QuarterFiveIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + periodRecord.QuarterFiveQuarter.ToString().ToUpper().Trim() && record.CURRENCY == currency).FirstOrDefault();

                    FinancialStatementData quarterSixData = financialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                            record.PERIOD.ToUpper().Trim() == periodRecord.QuarterSixYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (periodRecord.QuarterSixIsHistorical ? "A" : "E")
                            && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + periodRecord.QuarterSixQuarter.ToString().ToUpper().Trim() && record.CURRENCY == currency).FirstOrDefault();

                    result.Add(new PeriodColumnDisplayData()
                    {
                        DATA_DESC = dataDesc,
                        YEAR_ONE = yearOneData != null ? yearOneData.AMOUNT : null,
                        YEAR_TWO = yearTwoData != null ? yearTwoData.AMOUNT : null,
                        YEAR_THREE = yearThreeData != null ? yearThreeData.AMOUNT : null,
                        YEAR_FOUR = yearFourData != null ? yearFourData.AMOUNT : null,
                        YEAR_FIVE = yearFiveData != null ? yearFiveData.AMOUNT : null,
                        YEAR_SIX = yearSixData != null ? yearSixData.AMOUNT : null,
                        YEAR_SEVEN = yearSevenData != null ? yearSevenData.AMOUNT : null,
                        QUARTER_ONE = quarterOneData != null ? quarterOneData.AMOUNT : null,
                        QUARTER_TWO = quarterTwoData != null ? quarterTwoData.AMOUNT : null,
                        QUARTER_THREE = quarterThreeData != null ? quarterThreeData.AMOUNT : null,
                        QUARTER_FOUR = quarterFourData != null ? quarterFourData.AMOUNT : null,
                        QUARTER_FIVE = quarterFiveData != null ? quarterFiveData.AMOUNT : null,
                        QUARTER_SIX = quarterSixData != null ? quarterSixData.AMOUNT : null,
                    });
                }

                if (calendarizeMonth != financialStatementInfo[0].REPORTED_MONTH)
                    return GetCalendarizedPeriodColumnDisplayInfo(result, Convert.ToInt32(calendarizeMonth));
            }

            return result;
        }

        public static void UpdateColumnInformation(RadGridView gridView, PeriodColumnUpdateEventArg e)
        {
            for (int i = 0; i < 12; i++)
            {
                gridView.Columns[i + 2].Header = e.PeriodColumnHeader[i];
                gridView.Columns[i + 2].IsVisible = i < 6 ? e.PeriodIsYearly : !(e.PeriodIsYearly);
            }
        }

        //public static void RadRadioButton_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    RadRadioButton radioButton = sender as RadRadioButton;
        //    if (radioButton != null)
        //        radioButton.Foreground = new SolidColorBrush(Colors.Black);
        //}

        //public static void RadRadioButton_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    RadRadioButton radioButton = sender as RadRadioButton;
        //    if (radioButton != null)
        //        if (radioButton.IsChecked == false || radioButton.IsChecked == null)
        //            radioButton.Foreground = new SolidColorBrush(Colors.White);
        //}

        //public static void RadRadioButton_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (sender == null)
        //        return;
        //    RadRadioButton radioButton = sender as RadRadioButton;

        //    if (radioButton == null)
        //        return;

        //    if (radioButton.IsChecked == null)
        //        return;

        //    radioButton.Foreground = Convert.ToBoolean(radioButton.IsChecked)
        //        ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White);
        //}        

        private static int GetQuarter(int month)
        {
            return month < 4 ? 1 : (month < 7 ? 2 : (month < 10 ? 3 : 4));
        }
    }
}
