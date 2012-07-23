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
        #region Events
        #region Period Column Update Event
        /// <summary>
        /// Static event for PeriodColumnUpdateEvent
        /// </summary>
        public static event PeriodColumnUpdateEvent PeriodColumnUpdate = delegate { };

        /// <summary>
        /// Raise PeriodColumnUpdateEvent
        /// </summary>
        /// <param name="e">PeriodColumnUpdateEventArg</param>
        public static void RaisePeriodColumnUpdateCompleted(PeriodColumnUpdateEventArg e)
        {
            PeriodColumnUpdateEvent periodColumnUpdationEvent = PeriodColumnUpdate;
            if (periodColumnUpdationEvent != null)
            {
                periodColumnUpdationEvent(e);
            }
        }
        #endregion

        #region Period Column Navigation Event
        /// <summary>
        /// Static event for PeriodColumnNavigationEvent
        /// </summary>
        public static event PeriodColumnNavigationEvent PeriodColumnNavigate = delegate { };

        /// <summary>
        /// Raise PeriodColumnNavigationEvent
        /// </summary>
        /// <param name="e">PeriodColumnNavigationEventArg</param>
        public static void RaisePeriodColumnNavigationCompleted(PeriodColumnNavigationEventArg e)
        {
            PeriodColumnNavigationEvent periodColumnNavigationEvent = PeriodColumnNavigate;
            if (periodColumnNavigationEvent != null)
            {
                periodColumnNavigationEvent(e);
            }
        }                
        #endregion
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the period record consisting of six years and quarters based on iteration
        /// </summary>
        /// <param name="incrementFactor">increment factor of iteration in period column year quarter calculation : [Default] 0</param>
        /// <param name="defaultHistoricalYearCount">year columns from left to default to hostorical data : [Default] 3</param>
        /// <param name="defaultHistoricalQuarterCount">quarter columns from left to default to historical data : [Default] 4</param>
        /// <param name="netColumnCount">Total count of columns :[Max] 6 [Default] 6</param>
        /// <param name="isQuarterImplemented">quarter data implementation : [Default] true</param>
        /// <returns>PeriodRecord</returns>
        public static PeriodRecord SetPeriodRecord(int incrementFactor = 0, int defaultHistoricalYearCount = 3, int defaultHistoricalQuarterCount = 4
            , int netColumnCount = 6, bool isQuarterImplemented = true)
        {
            PeriodRecord periodRecord = new PeriodRecord()
            {
                DefaultHistoricalYearCount = defaultHistoricalYearCount,
                DefaultHistoricalQuarterCount = defaultHistoricalQuarterCount,
                NetColumnCount = netColumnCount,
                IsQuarterImplemented = isQuarterImplemented
            };

            int presentYear = DateTime.Today.Year;
            int presentMonth = DateTime.Today.Month;
            int presentQuarter = GetQuarter(presentMonth);
            int columnCount = 0;

            #region Set Yearly Information
            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearOne = presentYear - defaultHistoricalYearCount + incrementFactor;
            periodRecord.YearOneIsHistorical = periodRecord.YearOne < presentYear;

            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearTwo = periodRecord.YearOne + 1;
            periodRecord.YearTwoIsHistorical = periodRecord.YearTwo < presentYear;

            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearThree = periodRecord.YearTwo + 1;
            periodRecord.YearThreeIsHistorical = periodRecord.YearThree < presentYear;

            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearFour = periodRecord.YearThree + 1;
            periodRecord.YearFourIsHistorical = periodRecord.YearFour < presentYear;

            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearFive = periodRecord.YearFour + 1;
            periodRecord.YearFiveIsHistorical = periodRecord.YearFive < presentYear;

            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearSix = periodRecord.YearFive + 1;
            periodRecord.YearSixIsHistorical = periodRecord.YearSix < presentYear;
            #endregion

        QUARTER_INFO:
            if (!isQuarterImplemented)
                goto FINISH;
            columnCount = 0;

            #region Set Quarterly Information
            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterOneYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount) * 3).Year;
            periodRecord.QuarterOneQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount) * 3).Month);
            periodRecord.QuarterOneIsHistorical = periodRecord.QuarterOneYear < presentYear
                ? true : (periodRecord.QuarterOneYear == presentYear ? periodRecord.QuarterOneQuarter < presentQuarter : false);

            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterTwoYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 1) * 3).Year;
            periodRecord.QuarterTwoQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 1) * 3).Month);
            periodRecord.QuarterTwoIsHistorical = periodRecord.QuarterTwoYear < presentYear
                ? true : (periodRecord.QuarterTwoYear == presentYear ? periodRecord.QuarterTwoQuarter < presentQuarter : false);

            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterThreeYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 2) * 3).Year;
            periodRecord.QuarterThreeQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 2) * 3).Month);
            periodRecord.QuarterThreeIsHistorical = periodRecord.QuarterThreeYear < presentYear
                ? true : (periodRecord.QuarterThreeYear == presentYear ? periodRecord.QuarterThreeQuarter < presentQuarter : false);

            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterFourYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 3) * 3).Year;
            periodRecord.QuarterFourQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 3) * 3).Month);
            periodRecord.QuarterFourIsHistorical = periodRecord.QuarterFourYear < presentYear
                ? true : (periodRecord.QuarterFourYear == presentYear ? periodRecord.QuarterFourQuarter < presentQuarter : false);

            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterFiveYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 4) * 3).Year;
            periodRecord.QuarterFiveQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 4) * 3).Month);
            periodRecord.QuarterFiveIsHistorical = periodRecord.QuarterFiveYear < presentYear
                ? true : (periodRecord.QuarterFiveYear == presentYear ? periodRecord.QuarterFiveQuarter < presentQuarter : false);

            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterSixYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 5) * 3).Year;
            periodRecord.QuarterSixQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 5) * 3).Month);
            periodRecord.QuarterSixIsHistorical = periodRecord.QuarterSixYear < presentYear
                ? true : (periodRecord.QuarterSixYear == presentYear ? periodRecord.QuarterSixQuarter < presentQuarter : false);
            #endregion

        FINISH:
            return periodRecord;
        }

        /// <summary>
        /// Sets Column headers based in PeriodRecord
        /// </summary>
        /// <param name="periodRecord">PeriodRecord : [Default] null</param>
        /// <param name="displayPeriodType">Display period type in headers : [Default] true</param>
        /// <returns>List of column header strings</returns>
        public static List<String> SetColumnHeaders(PeriodRecord periodRecord, bool displayPeriodType = true)
        {
            if (periodRecord == null)
                throw new InvalidOperationException();

            List<String> periodColumnHeader = new List<string>();

            int columnCount = 0;

            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearOne.ToString() + (displayPeriodType ? " " + (periodRecord.YearOneIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearTwo.ToString() + (displayPeriodType ? " " + (periodRecord.YearTwoIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearThree.ToString() + (displayPeriodType ? " " + (periodRecord.YearThreeIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearFour.ToString() + (displayPeriodType ? " " + (periodRecord.YearFourIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearFive.ToString() + (displayPeriodType ? " " + (periodRecord.YearFiveIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearSix.ToString() + (displayPeriodType ? " " + (periodRecord.YearSixIsHistorical ? "A" : "E") : ""));

        QUARTER_INFO:
            if (!periodRecord.IsQuarterImplemented)
                goto FINISH;
            columnCount = 0;

            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterOneYear.ToString() + " Q" + periodRecord.QuarterOneQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterOneIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterTwoYear.ToString() + " Q" + periodRecord.QuarterTwoQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterTwoIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterThreeYear.ToString() + " Q" + periodRecord.QuarterThreeQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterThreeIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterFourYear.ToString() + " Q" + periodRecord.QuarterFourQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterFourIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterFiveYear.ToString() + " Q" + periodRecord.QuarterFiveQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterFiveIsHistorical ? "A" : "E") : ""));
            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterSixYear.ToString() + " Q" + periodRecord.QuarterSixQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterSixIsHistorical ? "A" : "E") : ""));

        FINISH:
            return periodColumnHeader;
        }

        /// <summary>
        /// Set Period Column Information - Pivots data to align data on grid based on PeriodRecord information
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="data">Unpivoted data</param>
        /// <param name="periodRecord">modified PeriodRecord returned to invoker</param>
        /// <param name="periodRecordInfo">original PeriodRecord</param>
        /// <param name="subGroups">List of PeriodColumnGroupingDetail for grouping data : [Default] null</param>
        /// <param name="updatePeriodRecord">update PeriodRecord with AmountType updates : [Default] true</param>
        /// <returns>List of PeriodColumnDisplayData objects</returns>
        public static List<PeriodColumnDisplayData> SetPeriodColumnDisplayInfo<T>(List<T> data, out PeriodRecord periodRecord
            , PeriodRecord periodRecordInfo, List<PeriodColumnGroupingDetail> subGroups = null, bool updatePeriodRecord = true)
        {
            List<PeriodColumnDisplayData> result = new List<PeriodColumnDisplayData>();
            PeriodRecord period = periodRecordInfo;

            if (data == null)
                goto FINISH;

            List<String> distinctPeriodDataDescriptors = data
                .Select(record => (String)record.GetType().GetProperty("Description").GetValue(record, null)).Distinct().ToList();

            foreach (String dataDesc in distinctPeriodDataDescriptors)
            {
                Int32 columnCount = 0;
                T yearOneData = default(T);
                T yearTwoData = default(T);
                T yearThreeData = default(T);
                T yearFourData = default(T);
                T yearFiveData = default(T);
                T yearSixData = default(T);
                T quarterOneData = default(T);
                T quarterTwoData = default(T);
                T quarterThreeData = default(T);
                T quarterFourData = default(T);
                T quarterFiveData = default(T);
                T quarterSixData = default(T);

                #region Annual
                #region Year One
                if (++columnCount > period.NetColumnCount)
                    goto QUARTERLY_INFO;
                yearOneData = GetPeriodData<T>(data, dataDesc, period.YearOne.ToString(), "A");
                if (yearOneData != null && updatePeriodRecord)
                    period.YearOneIsHistorical = ConvertAmountTypeToBoolean(yearOneData.GetType().GetProperty("AmountType").GetValue(yearOneData, null).ToString());
                #endregion

                #region Year Two
                if (++columnCount > period.NetColumnCount)
                    goto QUARTERLY_INFO;
                yearTwoData = GetPeriodData<T>(data, dataDesc, period.YearTwo.ToString(), "A");
                if (yearTwoData != null && updatePeriodRecord)
                    period.YearTwoIsHistorical = ConvertAmountTypeToBoolean(yearTwoData.GetType().GetProperty("AmountType").GetValue(yearTwoData, null).ToString());
                #endregion

                #region Year Three
                if (++columnCount > period.NetColumnCount)
                    goto QUARTERLY_INFO;
                yearThreeData = GetPeriodData<T>(data, dataDesc, period.YearThree.ToString(), "A");
                if (yearThreeData != null && updatePeriodRecord)
                    period.YearThreeIsHistorical = ConvertAmountTypeToBoolean(yearThreeData.GetType().GetProperty("AmountType").GetValue(yearThreeData, null).ToString());
                #endregion

                #region Year Four
                if (++columnCount > period.NetColumnCount)
                    goto QUARTERLY_INFO;
                yearFourData = GetPeriodData<T>(data, dataDesc, period.YearFour.ToString(), "A");
                if (yearFourData != null && updatePeriodRecord)
                    period.YearFourIsHistorical = ConvertAmountTypeToBoolean(yearFourData.GetType().GetProperty("AmountType").GetValue(yearFourData, null).ToString());
                #endregion

                #region Year Five
                if (++columnCount > period.NetColumnCount)
                    goto QUARTERLY_INFO;
                yearFiveData = GetPeriodData<T>(data, dataDesc, period.YearFive.ToString(), "A");
                if (yearFiveData != null && updatePeriodRecord)
                    period.YearFiveIsHistorical = ConvertAmountTypeToBoolean(yearFiveData.GetType().GetProperty("AmountType").GetValue(yearFiveData, null).ToString());
                #endregion

                #region Year Six
                if (++columnCount > period.NetColumnCount)
                    goto QUARTERLY_INFO;
                yearSixData = GetPeriodData<T>(data, dataDesc, period.YearSix.ToString(), "A");
                if (yearSixData != null && updatePeriodRecord)
                    period.YearSixIsHistorical = ConvertAmountTypeToBoolean(yearSixData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion
                #endregion

            QUARTERLY_INFO:
                if (!(period.IsQuarterImplemented))
                    goto GROUPING;
                columnCount = 0;

                #region Quarterly
                #region Quarter One
                if (++columnCount > period.NetColumnCount)
                    goto GROUPING;
                quarterOneData = GetPeriodData<T>(data, dataDesc, period.QuarterOneYear.ToString(), "Q" + period.QuarterOneQuarter.ToString());
                if (quarterOneData != null && updatePeriodRecord)
                    period.QuarterOneIsHistorical = ConvertAmountTypeToBoolean(quarterOneData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion

                #region Quarter Two
                if (++columnCount > period.NetColumnCount)
                    goto GROUPING;
                quarterTwoData = GetPeriodData<T>(data, dataDesc, period.QuarterTwoYear.ToString(), "Q" + period.QuarterTwoQuarter.ToString());
                if (quarterTwoData != null && updatePeriodRecord)
                    period.QuarterTwoIsHistorical = ConvertAmountTypeToBoolean(quarterTwoData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion

                #region Quarter Three
                if (++columnCount > period.NetColumnCount)
                    goto GROUPING;
                quarterThreeData = GetPeriodData<T>(data, dataDesc, period.QuarterThreeYear.ToString(), "Q" + period.QuarterThreeQuarter.ToString());
                if (quarterThreeData != null && updatePeriodRecord)
                    period.QuarterThreeIsHistorical = ConvertAmountTypeToBoolean(quarterThreeData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion

                #region Quarter Four
                if (++columnCount > period.NetColumnCount)
                    goto GROUPING;
                quarterFourData = GetPeriodData<T>(data, dataDesc, period.QuarterFourYear.ToString(), "Q" + period.QuarterFourQuarter.ToString());
                if (quarterFourData != null && updatePeriodRecord)
                    period.QuarterFourIsHistorical = ConvertAmountTypeToBoolean(quarterFourData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion

                #region Quarter Five
                if (++columnCount > period.NetColumnCount)
                    goto GROUPING;
                quarterFiveData = GetPeriodData<T>(data, dataDesc, period.QuarterFiveYear.ToString(), "Q" + period.QuarterFiveQuarter.ToString());
                if (quarterFiveData != null && updatePeriodRecord)
                    period.QuarterFiveIsHistorical = ConvertAmountTypeToBoolean(quarterFiveData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion

                #region Quarter Six
                if (++columnCount > period.NetColumnCount)
                    goto GROUPING;
                quarterSixData = GetPeriodData<T>(data, dataDesc, period.QuarterSixYear.ToString(), "Q" + period.QuarterSixQuarter.ToString());
                if (quarterSixData != null && updatePeriodRecord)
                    period.QuarterSixIsHistorical = ConvertAmountTypeToBoolean(quarterSixData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                #endregion
                #endregion

            GROUPING:

                #region Grouping
                if (subGroups == null)
                    goto RECORD_ENTRY;

                foreach (PeriodColumnGroupingDetail item in subGroups)
                {
                    PeriodColumnDisplayData displayData = new PeriodColumnDisplayData()
                    {
                        DATA_DESC = dataDesc,
                        SUB_DATA_DESC = item.GroupDisplayName
                    };

                    int countDisplayColumns = 0;

                    if (++countDisplayColumns > period.NetColumnCount)
                        goto GROUPING_QUARTER_INFO;
                    displayData.YEAR_ONE = GetGroupedData<T>(yearOneData, item);

                    if (++countDisplayColumns > period.NetColumnCount)
                        goto GROUPING_QUARTER_INFO;
                    displayData.YEAR_TWO = GetGroupedData<T>(yearTwoData, item);

                    if (++countDisplayColumns > period.NetColumnCount)
                        goto GROUPING_QUARTER_INFO;
                    displayData.YEAR_THREE = GetGroupedData<T>(yearThreeData, item);

                    if (++countDisplayColumns > period.NetColumnCount)
                        goto GROUPING_QUARTER_INFO;
                    displayData.YEAR_FOUR = GetGroupedData<T>(yearFourData, item);

                    if (++countDisplayColumns > period.NetColumnCount)
                        goto GROUPING_QUARTER_INFO;
                    displayData.YEAR_FIVE = GetGroupedData<T>(yearFiveData, item);

                    if (++countDisplayColumns > period.NetColumnCount)
                        goto GROUPING_QUARTER_INFO;
                    displayData.YEAR_SIX = GetGroupedData<T>(yearSixData, item);

                GROUPING_QUARTER_INFO:
                    if (period.IsQuarterImplemented)
                    {
                        countDisplayColumns = 0;

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_RECORD_ENTRY;
                        displayData.QUARTER_ONE = GetGroupedData<T>(quarterOneData, item);

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_RECORD_ENTRY;
                        displayData.QUARTER_TWO = GetGroupedData<T>(quarterTwoData, item);

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_RECORD_ENTRY;
                        displayData.QUARTER_THREE = GetGroupedData<T>(quarterThreeData, item);

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_RECORD_ENTRY;
                        displayData.QUARTER_FOUR = GetGroupedData<T>(quarterFourData, item);

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_RECORD_ENTRY;
                        displayData.QUARTER_FIVE = GetGroupedData<T>(quarterFiveData, item);

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_RECORD_ENTRY;
                        displayData.QUARTER_SIX = GetGroupedData<T>(quarterSixData, item);
                    }

                GROUPING_RECORD_ENTRY:
                    result.Add(displayData);
                }

                continue;
                #endregion

            RECORD_ENTRY:
                result.Add(new PeriodColumnDisplayData()
                {
                    DATA_DESC = dataDesc,
                    YEAR_ONE = yearOneData != null ? yearOneData.GetType().GetProperty("Amount").GetValue(yearOneData, null) : null,
                    YEAR_TWO = yearTwoData != null ? yearTwoData.GetType().GetProperty("Amount").GetValue(yearTwoData, null) : null,
                    YEAR_THREE = yearThreeData != null ? yearThreeData.GetType().GetProperty("Amount").GetValue(yearThreeData, null) : null,
                    YEAR_FOUR = yearFourData != null ? yearFourData.GetType().GetProperty("Amount").GetValue(yearFourData, null) : null,
                    YEAR_FIVE = yearFiveData != null ? yearFiveData.GetType().GetProperty("Amount").GetValue(yearFiveData, null) : null,
                    YEAR_SIX = yearSixData != null ? yearSixData.GetType().GetProperty("Amount").GetValue(yearSixData, null) : null,
                    QUARTER_ONE = quarterOneData != null ? quarterOneData.GetType().GetProperty("Amount").GetValue(quarterOneData, null) : null,
                    QUARTER_TWO = quarterTwoData != null ? quarterTwoData.GetType().GetProperty("Amount").GetValue(quarterTwoData, null) : null,
                    QUARTER_THREE = quarterThreeData != null ? quarterThreeData.GetType().GetProperty("Amount").GetValue(quarterThreeData, null) : null,
                    QUARTER_FOUR = quarterFourData != null ? quarterFourData.GetType().GetProperty("Amount").GetValue(quarterFourData, null) : null,
                    QUARTER_FIVE = quarterFiveData != null ? quarterFiveData.GetType().GetProperty("Amount").GetValue(quarterFiveData, null) : null,
                    QUARTER_SIX = quarterSixData != null ? quarterSixData.GetType().GetProperty("Amount").GetValue(quarterSixData, null) : null,
                });
            }

        FINISH:

            periodRecord = period;
            return result;
        }

        /// <summary>
        /// Updates column information of a grid with information created from SetColumnHeaders method
        /// </summary>
        /// <param name="gridView">Gridview for which column headers is to be updated</param>
        /// <param name="e">PeriodColumnUpdateEventArg</param>
        /// <param name="isQuarterImplemented">Quarter data display implemented in view : [Default] true</param>
        public static void UpdateColumnInformation(RadGridView gridView, PeriodColumnUpdateEventArg e, bool isQuarterImplemented = true)
        {
            for (int i = 0; i < e.PeriodColumnHeader.Count; i++)
            {
                //prevent exceeding gridview column count
                if (i + 2 >= gridView.Columns.Count)
                    return;

                //Description and left navigation columns of period column gadgets ignored
                gridView.Columns[i + 2].Header = e.PeriodColumnHeader[i];

                //update column visibility if quarterly data display is implemented
                if (isQuarterImplemented)
                {
                    bool columnVisibility = (i < (e.PeriodColumnHeader.Count / 2)) ? e.PeriodIsYearly : !(e.PeriodIsYearly);

                    if (gridView.Columns[i + 2].IsVisible != columnVisibility)
                        gridView.Columns[i + 2].IsVisible = columnVisibility;
                }
            }
        }       

        #endregion

        #region Private Methods
        /// <summary>
        /// Gets period data for specific Description, period year, period type and AmountType
        /// [Required] Unpivoted data must contain columns "Description", "PeriodYear", "PeriodType" and "AmountType"
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="data">Unpivoted data</param>
        /// <param name="Description">Description column value</param>
        /// <param name="periodYear">PeriodYear column value</param>
        /// <param name="periodType">PeriodType column value</param>
        /// <returns>Generic Type record matching criterion or null</returns>
        private static T GetPeriodData<T>(List<T> data, string Description, string periodYear, string periodType)
        {
            T yearData = data
                .Where(record =>
                    record.GetType().GetProperty("Description").GetValue(record, null).ToString().ToUpper().Trim() == Description.ToString().ToUpper().Trim() &&
                    record.GetType().GetProperty("PeriodYear").GetValue(record, null).ToString().ToUpper().Trim() == periodYear.ToString().ToUpper().Trim() &&
                    record.GetType().GetProperty("PeriodType").GetValue(record, null).ToString().ToUpper().Trim() == periodType.ToString().ToUpper().Trim())
                .FirstOrDefault();

            //if (yearData == null)
            //{
            //    yearData = data
            //    .Where(record =>
            //        ((String)record.GetType().GetProperty("Description").GetValue(record, null)).ToUpper().Trim() == Description.ToUpper().Trim() &&
            //        ((String)record.GetType().GetProperty("PeriodYear").GetValue(record, null)).ToUpper().Trim() == periodYear.ToUpper().Trim() &&
            //        ((String)record.GetType().GetProperty("PeriodType").GetValue(record, null)).ToUpper().Trim() == periodType.ToUpper().Trim() &&
            //        ((String)record.GetType().GetProperty("AmountType").GetValue(record, null)).ToUpper().Trim() == (AmountIsHistorical ? "ESTIMATE" : "ACTUAL"))
            //    .FirstOrDefault();
            //}

            return yearData;
        }

        /// <summary>
        /// Queries data for group item and returns value string in format (PeriodColumnGroupingType)
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="data">Unpivoted data</param>
        /// <param name="groupItem">PeriodColumnGroupingDetails</param>
        /// <returns>Value</returns>
        private static String GetGroupedData<T>(T data, PeriodColumnGroupingDetail groupItem)
        {
            if (data == null)
                return null;

            try
            {
                switch (groupItem.GroupDataType)
                {
                    case PeriodColumnGroupingType.DECIMAL:
                        return (Math.Round((Decimal)data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null), 4)).ToString();
                    case PeriodColumnGroupingType.DECIMAL_PERCENTAGE:
                        return (Math.Round((Decimal)data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null), 4)).ToString() + " %";
                    case PeriodColumnGroupingType.SHORT_DATETIME:
                        return ((DateTime)data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null)).ToShortDateString();
                    case PeriodColumnGroupingType.LONG_DATETIME:
                        return ((DateTime)data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null)).ToShortDateString();
                    case PeriodColumnGroupingType.STRING:
                        return (data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null)).ToString();
                    default:
                        return (data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null)).ToString();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Converts AmountType column value to boolean - ACTUAL (True), ESTIMATED (False)
        /// </summary>
        /// <param name="AmountType">AmountType column value</param>
        /// <returns>True/False</returns>
        private static bool ConvertAmountTypeToBoolean(String AmountType)
        {
            if (AmountType.Trim().ToUpper().Equals("ACTUAL"))
                return true;
            return false;
        }

        /// <summary>
        /// Gets the Quarter based on the month
        /// </summary>
        /// <param name="month">month number e.g. 3 for March</param>
        /// <returns>quarter number i.e. 1/2/3/4</returns>
        private static int GetQuarter(int month)
        {
            if (month <= 0 || month > 12)
                throw new InvalidOperationException("Invalid Month (should be between 1-12)");
            return month < 4 ? 1 : (month < 7 ? 2 : (month < 10 ? 3 : 4));
        }
        #endregion
    }

    /// <summary>
    /// Event raised when column header updation is complete
    /// </summary>
    /// <param name="e">PeriodColumnUpdateEventArg</param>
    public delegate void PeriodColumnUpdateEvent(PeriodColumnUpdateEventArg e);

    /// <summary>
    /// Payload for PeriodColumnUpdateEvent
    /// </summary>
    public class PeriodColumnUpdateEventArg
    {
        /// <summary>
        /// Full namespace of the view model class
        /// </summary>
        public String PeriodColumnNamespace { get; set; }

        /// <summary>
        /// List of period column headers
        /// </summary>
        public List<String> PeriodColumnHeader { get; set; }

        /// <summary>
        /// PeriodRecord storing period column reference data
        /// </summary>
        public PeriodRecord PeriodRecord { get; set; }

        /// <summary>
        /// Period type information
        /// </summary>
        public Boolean PeriodIsYearly { get; set; }

        /// <summary>
        /// Addition Information to be passed to view on column information updation
        /// </summary>
        public Object AdditionInfo { get; set; }
    }

    /// <summary>
    /// PeriodColumnNavigationEvent - raised when period column is navigated
    /// </summary>
    /// <param name="e">PeriodColumnNavigationEventArg</param>
    public delegate void PeriodColumnNavigationEvent(PeriodColumnNavigationEventArg e);

    /// <summary>
    /// Enumeration for navigation in period columns
    /// </summary>
    public enum NavigationDirection
    {
        LEFT,
        RIGHT
    }

    /// <summary>
    /// Payload for PeriodColumnNavigationEvent
    /// </summary>
    public class PeriodColumnNavigationEventArg
    {
        /// <summary>
        /// Full namespace of the view model class
        /// </summary>
        public String PeriodColumnNamespace { get; set; }

        /// <summary>
        /// NavigationDirection - LEFT / RIGHT
        /// </summary>
        public NavigationDirection PeriodColumnNavigationDirection { get; set; }
    }
}
