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

        /// <summary>
        /// Set the period record consisting of six years and quarters based on iteration
        /// </summary>
        /// <param name="incrementFactor">increment factor of iteration in period column year quarter calculation</param>
        /// <returns>PeriodRecord object</returns>
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

        public static List<String> SetColumnHeaders(PeriodRecord periodRecord = null, bool showHistorical = true)
        {
            if (periodRecord == null)
                periodRecord = SetPeriodRecord();

            List<String> periodColumnHeader = new List<string>();

            periodColumnHeader.Add(periodRecord.YearOne.ToString() + (showHistorical ? " " + (periodRecord.YearOneIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.YearTwo.ToString() + (showHistorical ? " " + (periodRecord.YearTwoIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.YearThree.ToString() + (showHistorical ? " " + (periodRecord.YearThreeIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.YearFour.ToString() + (showHistorical ? " " + (periodRecord.YearFourIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.YearFive.ToString() + (showHistorical ? " " + (periodRecord.YearFiveIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.YearSix.ToString() + (showHistorical ? " " + (periodRecord.YearSixIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.QuarterOneYear.ToString() + " Q" + periodRecord.QuarterOneQuarter.ToString() + (showHistorical ? " " + (periodRecord.QuarterOneIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.QuarterTwoYear.ToString() + " Q" + periodRecord.QuarterTwoQuarter.ToString() + (showHistorical ? " " + (periodRecord.QuarterTwoIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.QuarterThreeYear.ToString() + " Q" + periodRecord.QuarterThreeQuarter.ToString() + (showHistorical ? " " + (periodRecord.QuarterThreeIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.QuarterFourYear.ToString() + " Q" + periodRecord.QuarterFourQuarter.ToString() + (showHistorical ? " " + (periodRecord.QuarterFourIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.QuarterFiveYear.ToString() + " Q" + periodRecord.QuarterFiveQuarter.ToString() + (showHistorical ? " " + (periodRecord.QuarterFiveIsHistorical ? "A" : "E") : ""));
            periodColumnHeader.Add(periodRecord.QuarterSixYear.ToString() + " Q" + periodRecord.QuarterSixQuarter.ToString() + (showHistorical ? " " + (periodRecord.QuarterSixIsHistorical ? "A" : "E") : ""));

            return periodColumnHeader;
        }

        public static List<PeriodColumnDisplayData> SetPeriodColumnDisplayInfo(object periodColumnInfo, out PeriodRecord periodRecord, PeriodRecord periodRecordInfo)
        {
            List<PeriodColumnDisplayData> result = new List<PeriodColumnDisplayData>();
            PeriodRecord period = periodRecordInfo;

            #region Financial Statements
            if (periodColumnInfo is List<FinancialStatementData>)
            {
                List<FinancialStatementData> financialStatementInfo = (periodColumnInfo as List<FinancialStatementData>)
                    .OrderBy(record => record.SORT_ORDER)
                    .ThenBy(record => record.PERIOD_TYPE)
                    .ThenBy(record => record.PERIOD)
                    .ToList();

                List<String> distinctDataDescriptors = financialStatementInfo.Select(record => record.DATA_DESC).Distinct().ToList();

                foreach (string dataDesc in distinctDataDescriptors)
                {
                    FinancialStatementData recordData = financialStatementInfo.Where(record => record.DATA_DESC == dataDesc).FirstOrDefault();

                    Int32 dataId = recordData == null ? -1 : recordData.Data_ID;

                    #region Annual
                    #region Year One
                    FinancialStatementData yearOneData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearOne.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearOneIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A")
                                    .FirstOrDefault();

                    if (yearOneData == null)
                    {
                        yearOneData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearOne.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearOneIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A")
                            .FirstOrDefault();
                        if (yearOneData != null)
                        {
                            period.YearOneIsHistorical = !(period.YearOneIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Two
                    FinancialStatementData yearTwoData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearTwo.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearTwoIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearTwoData == null)
                    {
                        yearTwoData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearTwo.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearTwoIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearTwoData != null)
                        {
                            period.YearTwoIsHistorical = !(period.YearTwoIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Three
                    FinancialStatementData yearThreeData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearThree.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearThreeIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearThreeData == null)
                    {
                        yearThreeData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearThree.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearThreeIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearThreeData != null)
                        {
                            period.YearThreeIsHistorical = !(period.YearThreeIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Four
                    FinancialStatementData yearFourData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearFour.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFourIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearFourData == null)
                    {
                        yearFourData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearFour.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFourIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearFourData != null)
                        {
                            period.YearFourIsHistorical = !(period.YearFourIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Five
                    FinancialStatementData yearFiveData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearFive.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFiveIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearFiveData == null)
                    {
                        yearFiveData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearFive.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFiveIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearFiveData != null)
                        {
                            period.YearFiveIsHistorical = !(period.YearFiveIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Six
                    FinancialStatementData yearSixData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearSix.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearSixIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearSixData == null)
                    {
                        yearSixData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearSix.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearSixIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearSixData != null)
                        {
                            period.YearSixIsHistorical = !(period.YearSixIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Seven
                    FinancialStatementData yearSevenData = financialStatementInfo
                                    .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.YearSeven.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearSevenIsHistorical ? "A" : "E") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearSevenData == null)
                    {
                        yearSevenData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD.ToUpper().Trim() == period.YearSeven.ToString().ToUpper().Trim() &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearSevenIsHistorical ? "E" : "A") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearSevenData != null)
                        {
                            period.YearSevenIsHistorical = !(period.YearSevenIsHistorical);
                        }
                    }
                    #endregion
                    #endregion

                    #region Quarterly
                    #region Quarter One
                    FinancialStatementData quarterOneData = financialStatementInfo
                                                .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD.ToUpper().Trim() == period.QuarterOneYear.ToString().ToUpper().Trim() &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterOneIsHistorical ? "A" : "E") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterOneQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterOneData == null)
                    {
                        quarterOneData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.QuarterOneYear.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterOneIsHistorical ? "E" : "A") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterOneQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterOneData != null)
                        {
                            period.QuarterOneIsHistorical = !(period.QuarterOneIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Two
                    FinancialStatementData quarterTwoData = financialStatementInfo
                                                .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD.ToUpper().Trim() == period.QuarterTwoYear.ToString().ToUpper().Trim() &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterTwoIsHistorical ? "A" : "E") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterTwoQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterTwoData == null)
                    {
                        quarterTwoData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.QuarterTwoYear.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterTwoIsHistorical ? "E" : "A") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterTwoQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterTwoData != null)
                        {
                            period.QuarterTwoIsHistorical = !(period.QuarterTwoIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Three
                    FinancialStatementData quarterThreeData = financialStatementInfo
                                                .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD.ToUpper().Trim() == period.QuarterThreeYear.ToString().ToUpper().Trim() &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterThreeIsHistorical ? "A" : "E") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterThreeQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterThreeData == null)
                    {
                        quarterThreeData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.QuarterThreeYear.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterThreeIsHistorical ? "E" : "A") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterThreeQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterThreeData != null)
                        {
                            period.QuarterThreeIsHistorical = !(period.QuarterThreeIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Four
                    FinancialStatementData quarterFourData = financialStatementInfo
                                                .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD.ToUpper().Trim() == period.QuarterFourYear.ToString().ToUpper().Trim() &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFourIsHistorical ? "A" : "E") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFourQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterFourData == null)
                    {
                        quarterFourData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.QuarterFourYear.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFourIsHistorical ? "E" : "A") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFourQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterFourData != null)
                        {
                            period.QuarterFourIsHistorical = !(period.QuarterFourIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Five
                    FinancialStatementData quarterFiveData = financialStatementInfo
                                                .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD.ToUpper().Trim() == period.QuarterFiveYear.ToString().ToUpper().Trim() &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFiveIsHistorical ? "A" : "E") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFiveQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterFiveData == null)
                    {
                        quarterFiveData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.QuarterFiveYear.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFiveIsHistorical ? "E" : "A") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFiveQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterFiveData != null)
                        {
                            period.QuarterFiveIsHistorical = !(period.QuarterFiveIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Six
                    FinancialStatementData quarterSixData = financialStatementInfo
                                                .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD.ToUpper().Trim() == period.QuarterSixYear.ToString().ToUpper().Trim() &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterSixIsHistorical ? "A" : "E") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterSixQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterSixData == null)
                    {
                        quarterSixData = financialStatementInfo
                            .Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD.ToUpper().Trim() == period.QuarterSixYear.ToString().ToUpper().Trim() &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterSixIsHistorical ? "E" : "A") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterSixQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterSixData != null)
                        {
                            period.QuarterSixIsHistorical = !(period.QuarterSixIsHistorical);
                        }
                    }
                    #endregion
                    #endregion

                    result.Add(new PeriodColumnDisplayData()
                    {
                        DATA_ID = dataId,
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
            }
            #endregion

            #region Consensus Estimate Detail
            if (periodColumnInfo is List<ConsensusEstimateDetailedData>)
            {
                List<ConsensusEstimateDetailedData> consensusEstimateDetailedData = (periodColumnInfo as List<ConsensusEstimateDetailedData>);

                List<String> distinctDataDescriptors = consensusEstimateDetailedData.Select(record => record.ESTIMATE_TYPE).Distinct().ToList();

                foreach (string dataDesc in distinctDataDescriptors)
                {
                    //ConsensusEstimateDetailedData recordData = consensusEstimateDetailedData.Where(record => record.ESTIMATE_TYPE == dataDesc).FirstOrDefault();

                    #region Annual
                    #region Year One

                    //ConsensusEstimateDetailedData yearOneData = consensusEstimateDetailedData
                    //                .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                    //                    record.PERIOD_YEAR.ToUpper().Trim() == period.YearOne.ToString().ToUpper().Trim() &&
                    //                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearOneIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                    //                    record.PERIOD_TYPE.ToUpper().Trim() == "A")
                    //                .FirstOrDefault();

                    //if (yearOneData == null)
                    //{
                    //    yearOneData = consensusEstimateDetailedData
                    //        .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                    //            record.PERIOD_YEAR.ToUpper().Trim() == period.YearOne.ToString().ToUpper().Trim() &&
                    //            record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearOneIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                    //            record.PERIOD_TYPE.ToUpper().Trim() == "A")
                    //        .FirstOrDefault();
                    //    if (yearOneData != null)
                    //    {
                    //        period.YearOneIsHistorical = !(period.YearOneIsHistorical);
                    //    }
                    //}
                    #endregion

                    #region Year Two
                    ConsensusEstimateDetailedData yearTwoData = consensusEstimateDetailedData
                                    .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.YearTwo &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearTwoIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearTwoData == null)
                    {
                        yearTwoData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD_YEAR == period.YearTwo &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearTwoIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearTwoData != null)
                        {
                            period.YearTwoIsHistorical = !(period.YearTwoIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Three
                    ConsensusEstimateDetailedData yearThreeData = consensusEstimateDetailedData
                                    .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.YearThree &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearThreeIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearThreeData == null)
                    {
                        yearThreeData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD_YEAR == period.YearThree &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearThreeIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearThreeData != null)
                        {
                            period.YearThreeIsHistorical = !(period.YearThreeIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Four
                    ConsensusEstimateDetailedData yearFourData = consensusEstimateDetailedData
                                    .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.YearFour &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFourIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearFourData == null)
                    {
                        yearFourData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD_YEAR == period.YearFour &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFourIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearFourData != null)
                        {
                            period.YearFourIsHistorical = !(period.YearFourIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Five
                    ConsensusEstimateDetailedData yearFiveData = consensusEstimateDetailedData
                                    .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.YearFive &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFiveIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearFiveData == null)
                    {
                        yearFiveData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD_YEAR == period.YearFive &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearFiveIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearFiveData != null)
                        {
                            period.YearFiveIsHistorical = !(period.YearFiveIsHistorical);
                        }
                    }
                    #endregion

                    #region Year Six
                    ConsensusEstimateDetailedData yearSixData = consensusEstimateDetailedData
                                    .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.YearSix &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearSixIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                        )
                                    .FirstOrDefault();

                    if (yearSixData == null)
                    {
                        yearSixData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                record.PERIOD_YEAR == period.YearSix &&
                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.YearSixIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                record.PERIOD_TYPE.ToUpper().Trim() == "A"
                                )
                            .FirstOrDefault();
                        if (yearSixData != null)
                        {
                            period.YearSixIsHistorical = !(period.YearSixIsHistorical);
                        }
                    }
                    #endregion

                    #endregion

                    #region Quarterly
                    #region Quarter One
                    //ConsensusEstimateDetailedData quarterOneData = consensusEstimateDetailedData
                    //                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                    //                                record.PERIOD_YEAR.ToUpper().Trim() == period.QuarterOneYear.ToString().ToUpper().Trim() &&
                    //                                record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterOneIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                    //                                record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterOneQuarter.ToString().ToUpper().Trim()
                    //                                )
                    //                            .FirstOrDefault();

                    //if (quarterOneData == null)
                    //{
                    //    quarterOneData = consensusEstimateDetailedData
                    //        .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                    //                    record.PERIOD_YEAR.ToUpper().Trim() == period.QuarterOneYear.ToString().ToUpper().Trim() &&
                    //                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterOneIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                    //                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterOneQuarter.ToString().ToUpper().Trim()
                    //                    )
                    //                .FirstOrDefault();
                    //    if (quarterOneData != null)
                    //    {
                    //        period.QuarterOneIsHistorical = !(period.QuarterOneIsHistorical);
                    //    }
                    //}
                    #endregion

                    #region Quarter Two
                    ConsensusEstimateDetailedData quarterTwoData = consensusEstimateDetailedData
                                                .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD_YEAR == period.QuarterTwoYear &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterTwoIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterTwoQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterTwoData == null)
                    {
                        quarterTwoData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.QuarterTwoYear &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterTwoIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterTwoQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterTwoData != null)
                        {
                            period.QuarterTwoIsHistorical = !(period.QuarterTwoIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Three
                    ConsensusEstimateDetailedData quarterThreeData = consensusEstimateDetailedData
                                                .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD_YEAR == period.QuarterThreeYear &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterThreeIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterThreeQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterThreeData == null)
                    {
                        quarterThreeData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.QuarterThreeYear &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterThreeIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterThreeQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterThreeData != null)
                        {
                            period.QuarterThreeIsHistorical = !(period.QuarterThreeIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Four
                    ConsensusEstimateDetailedData quarterFourData = consensusEstimateDetailedData
                                                .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD_YEAR == period.QuarterFourYear &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFourIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFourQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterFourData == null)
                    {
                        quarterFourData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.QuarterFourYear &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFourIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFourQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterFourData != null)
                        {
                            period.QuarterFourIsHistorical = !(period.QuarterFourIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Five
                    ConsensusEstimateDetailedData quarterFiveData = consensusEstimateDetailedData
                                                .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD_YEAR == period.QuarterFiveYear &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFiveIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFiveQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterFiveData == null)
                    {
                        quarterFiveData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.QuarterFiveYear &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterFiveIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterFiveQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterFiveData != null)
                        {
                            period.QuarterFiveIsHistorical = !(period.QuarterFiveIsHistorical);
                        }
                    }
                    #endregion

                    #region Quarter Six
                    ConsensusEstimateDetailedData quarterSixData = consensusEstimateDetailedData
                                                .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                                    record.PERIOD_YEAR == period.QuarterSixYear &&
                                                    record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterSixIsHistorical ? "ACTUAL" : "ESTIMATE") &&
                                                    record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterSixQuarter.ToString().ToUpper().Trim()
                                                    )
                                                .FirstOrDefault();

                    if (quarterSixData == null)
                    {
                        quarterSixData = consensusEstimateDetailedData
                            .Where(record => record.ESTIMATE_TYPE.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                                        record.PERIOD_YEAR == period.QuarterSixYear &&
                                        record.AMOUNT_TYPE.ToUpper().Trim() == (period.QuarterSixIsHistorical ? "ESTIMATE" : "ACTUAL") &&
                                        record.PERIOD_TYPE.ToUpper().Trim() == "Q" + period.QuarterSixQuarter.ToString().ToUpper().Trim()
                                        )
                                    .FirstOrDefault();
                        if (quarterSixData != null)
                        {
                            period.QuarterSixIsHistorical = !(period.QuarterSixIsHistorical);
                        }
                    }
                    #endregion
                    #endregion

                    #region Result Addition

                    #region #Estimates
                    PeriodColumnDisplayData estimatePeriodColumnData = new PeriodColumnDisplayData()
                                {
                                    DATA_DESC = dataDesc,
                                    SUB_DATA_DESC = "# Of Estimates",
                                };


                    if (yearTwoData != null)
                        estimatePeriodColumnData.YEAR_TWO = yearTwoData.NUMBER_OF_ESTIMATES;
                    if (yearThreeData != null)
                        estimatePeriodColumnData.YEAR_THREE = yearThreeData.NUMBER_OF_ESTIMATES;
                    if (yearFourData != null)
                        estimatePeriodColumnData.YEAR_FOUR = yearFourData.NUMBER_OF_ESTIMATES;
                    if (yearFiveData != null)
                        estimatePeriodColumnData.YEAR_FIVE = yearFiveData.NUMBER_OF_ESTIMATES;
                    if (yearSixData != null)
                        estimatePeriodColumnData.YEAR_SIX = yearSixData.NUMBER_OF_ESTIMATES;
                    if (quarterTwoData != null)
                        estimatePeriodColumnData.QUARTER_TWO = quarterTwoData.NUMBER_OF_ESTIMATES;
                    if (quarterThreeData != null)
                        estimatePeriodColumnData.QUARTER_THREE = quarterThreeData.NUMBER_OF_ESTIMATES;
                    if (quarterFourData != null)
                        estimatePeriodColumnData.QUARTER_FOUR = quarterFourData.NUMBER_OF_ESTIMATES;
                    if (quarterFiveData != null)
                        estimatePeriodColumnData.QUARTER_FIVE = quarterFiveData.NUMBER_OF_ESTIMATES;
                    if (quarterSixData != null)
                        estimatePeriodColumnData.QUARTER_SIX = quarterSixData.NUMBER_OF_ESTIMATES;

                    result.Add(estimatePeriodColumnData); 
                    #endregion

                    #region High
                    PeriodColumnDisplayData highPeriodColumnData = new PeriodColumnDisplayData()
                    {
                        DATA_DESC = dataDesc,
                        SUB_DATA_DESC = "High",
                    };


                    if (yearTwoData != null)
                        highPeriodColumnData.YEAR_TWO = yearTwoData.HIGH;
                    if (yearThreeData != null)
                        highPeriodColumnData.YEAR_THREE = yearThreeData.HIGH;
                    if (yearFourData != null)
                        highPeriodColumnData.YEAR_FOUR = yearFourData.HIGH;
                    if (yearFiveData != null)
                        highPeriodColumnData.YEAR_FIVE = yearFiveData.HIGH;
                    if (yearSixData != null)
                        highPeriodColumnData.YEAR_SIX = yearSixData.HIGH;
                    if (quarterTwoData != null)
                        highPeriodColumnData.QUARTER_TWO = quarterTwoData.HIGH;
                    if (quarterThreeData != null)
                        highPeriodColumnData.QUARTER_THREE = quarterThreeData.HIGH;
                    if (quarterFourData != null)
                        highPeriodColumnData.QUARTER_FOUR = quarterFourData.HIGH;
                    if (quarterFiveData != null)
                        highPeriodColumnData.QUARTER_FIVE = quarterFiveData.HIGH;
                    if (quarterSixData != null)
                        highPeriodColumnData.QUARTER_SIX = quarterSixData.HIGH;

                    result.Add(highPeriodColumnData);
                    #endregion

                    #endregion
                }
            }
            #endregion

            periodRecord = period;
            return result;
        }

        public static void UpdateColumnInformation(RadGridView gridView, PeriodColumnUpdateEventArg e, bool IsFinancial = true)
        {
            if (IsFinancial)
            {
                for (int i = 0; i < 12; i++)
                {
                    gridView.Columns[i + 2].Header = e.PeriodColumnHeader[i];
                    gridView.Columns[i + 2].IsVisible = i < 6 ? e.PeriodIsYearly : !(e.PeriodIsYearly);
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    gridView.Columns[i + 2].Header = e.PeriodColumnHeader[i + (i < 5 ? 1 : 2)];
                    gridView.Columns[i + 2].IsVisible = i < 5 ? e.PeriodIsYearly : !(e.PeriodIsYearly);
                }
            }
            
        }

        private static int GetQuarter(int month)
        {
            return month < 4 ? 1 : (month < 7 ? 2 : (month < 10 ? 3 : 4));
        }
    }
}
