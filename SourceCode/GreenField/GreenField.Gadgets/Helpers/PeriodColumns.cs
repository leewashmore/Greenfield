using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Period Column Navigation Implementation
    /// </summary>
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
        /// <param name="netColumnCount">Total count of columns :[Max] 7 [Default] 6</param>
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

            if (++columnCount > netColumnCount)
                goto QUARTER_INFO;
            periodRecord.YearSeven = periodRecord.YearSix + 1;
            periodRecord.YearSevenIsHistorical = periodRecord.YearSeven < presentYear;
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

            if (++columnCount > netColumnCount)
                goto FINISH;
            periodRecord.QuarterSevenYear = (new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 6) * 3).Year;
            periodRecord.QuarterSevenQuarter = GetQuarter((new DateTime(presentYear, presentMonth, 1)).AddMonths((incrementFactor - defaultHistoricalQuarterCount + 6) * 3).Month);
            periodRecord.QuarterSevenIsHistorical = periodRecord.QuarterSevenYear < presentYear
                ? true : (periodRecord.QuarterSevenYear == presentYear ? periodRecord.QuarterSevenQuarter < presentQuarter : false);
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
            if (++columnCount > periodRecord.NetColumnCount)
                goto QUARTER_INFO;
            periodColumnHeader.Add(periodRecord.YearSeven.ToString() + (displayPeriodType ? " " + (periodRecord.YearSevenIsHistorical ? "A" : "E") : ""));

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
            if (++columnCount > periodRecord.NetColumnCount)
                goto FINISH;
            periodColumnHeader.Add(periodRecord.QuarterSevenYear.ToString() + " Q" + periodRecord.QuarterSevenQuarter.ToString() + (displayPeriodType ? " " + (periodRecord.QuarterSevenIsHistorical ? "A" : "E") : ""));

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
            , PeriodRecord periodRecordInfo, List<PeriodColumnGroupingDetail> subGroups = null, bool updatePeriodRecord = true, bool uniqueByGroupDesc = false
            , String additionalFirstDescPropertyName = null, String additionalSecondDescPropertyName = null)
        {
            List<PeriodColumnDisplayData> result = new List<PeriodColumnDisplayData>();
            PeriodRecord period = periodRecordInfo;

            if (data == null)
                goto FINISH;

            if (data.Count.Equals(0))
                goto FINISH;

            PropertyInfo[] propertyInfo = typeof(T).GetProperties();

            if (!propertyInfo.Any(record => record.Name == "Description")
                || !propertyInfo.Any(record => record.Name == "PeriodYear")
                || !propertyInfo.Any(record => record.Name == "PeriodType")
                || !propertyInfo.Any(record => record.Name == "Amount")
                || !propertyInfo.Any(record => record.Name == "AmountType"))
                throw new InvalidOperationException("Data type is missing requisite columns");

            List<String> distinctPeriodDataGroupDescriptors = new List<string>() { "" };
            if (uniqueByGroupDesc)
            {
                if (!propertyInfo.Any(record => record.Name == "GroupDescription"))
                    throw new InvalidOperationException("Data type is missing requisite columns");
                distinctPeriodDataGroupDescriptors = data
                .Select(record => (String)record.GetType().GetProperty("GroupDescription").GetValue(record, null)).Distinct().ToList();
            }

            foreach (String gDesc in distinctPeriodDataGroupDescriptors)
            {
                List<String> distinctPeriodDataDescriptors = uniqueByGroupDesc
                    ? data.Where(record => (String)record.GetType().GetProperty("GroupDescription").GetValue(record, null) == gDesc)
                        .Select(record => (String)record.GetType().GetProperty("Description").GetValue(record, null)).Distinct().ToList()
                    : data.Select(record => (String)record.GetType().GetProperty("Description").GetValue(record, null)).Distinct().ToList();

                foreach (String dataDesc in distinctPeriodDataDescriptors)
                {
                    T defaultRecord = uniqueByGroupDesc
                    ? data.Where(record => ((String)record.GetType().GetProperty("Description").GetValue(record, null)) == dataDesc
                        && ((String)record.GetType().GetProperty("GroupDescription").GetValue(record, null)) == gDesc).FirstOrDefault()
                    : data.Where(record => ((String)record.GetType().GetProperty("Description").GetValue(record, null)) == dataDesc).FirstOrDefault();

                    String groupDescription = null;
                    if (propertyInfo.Any(record => record.Name == "GroupDescription"))
                        groupDescription = (String)defaultRecord.GetType().GetProperty("GroupDescription").GetValue(defaultRecord, null);

                    Int32? dataId = null;
                    if (propertyInfo.Any(record => record.Name == "DataId"))
                        dataId = (Int32?)defaultRecord.GetType().GetProperty("DataId").GetValue(defaultRecord, null);

                    Boolean? dataBold = false;
                    if (propertyInfo.Any(record => record.Name == "BoldFont"))
                        dataBold = ((String)defaultRecord.GetType().GetProperty("BoldFont").GetValue(defaultRecord, null)).Trim().ToUpper() == "Y";

                    Boolean? dataPercentage = false;
                    if (propertyInfo.Any(record => record.Name == "IsPercentage"))
                        dataPercentage = ((String)defaultRecord.GetType().GetProperty("IsPercentage").GetValue(defaultRecord, null)).Trim().ToUpper() == "Y";

                    Int32? dataDecimal = null;
                    if (propertyInfo.Any(record => record.Name == "Decimals"))
                        dataDecimal = (Int32?)defaultRecord.GetType().GetProperty("Decimals").GetValue(defaultRecord, null);

                    Int32 sortOrder = 0;
                    if (propertyInfo.Any(record => record.Name == "SortOrder"))
                        sortOrder = (Int32)defaultRecord.GetType().GetProperty("SortOrder").GetValue(defaultRecord, null);

                    Int32 columnCount = 0;
                    T yearOneData = default(T);
                    T yearTwoData = default(T);
                    T yearThreeData = default(T);
                    T yearFourData = default(T);
                    T yearFiveData = default(T);
                    T yearSixData = default(T);
                    T yearSevenData = default(T);
                    T quarterOneData = default(T);
                    T quarterTwoData = default(T);
                    T quarterThreeData = default(T);
                    T quarterFourData = default(T);
                    T quarterFiveData = default(T);
                    T quarterSixData = default(T);
                    T quarterSevenData = default(T);

                    #region Annual
                    #region Year One
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearOneData = GetPeriodData<T>(data, dataDesc, period.YearOne.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearOneData != null && updatePeriodRecord)
                        period.YearOneIsHistorical = ConvertAmountTypeToBoolean(yearOneData.GetType().GetProperty("AmountType").GetValue(yearOneData, null).ToString());
                    #endregion

                    #region Year Two
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearTwoData = GetPeriodData<T>(data, dataDesc, period.YearTwo.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearTwoData != null && updatePeriodRecord)
                        period.YearTwoIsHistorical = ConvertAmountTypeToBoolean(yearTwoData.GetType().GetProperty("AmountType").GetValue(yearTwoData, null).ToString());
                    #endregion

                    #region Year Three
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearThreeData = GetPeriodData<T>(data, dataDesc, period.YearThree.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearThreeData != null && updatePeriodRecord)
                        period.YearThreeIsHistorical = ConvertAmountTypeToBoolean(yearThreeData.GetType().GetProperty("AmountType").GetValue(yearThreeData, null).ToString());
                    #endregion

                    #region Year Four
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearFourData = GetPeriodData<T>(data, dataDesc, period.YearFour.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearFourData != null && updatePeriodRecord)
                        period.YearFourIsHistorical = ConvertAmountTypeToBoolean(yearFourData.GetType().GetProperty("AmountType").GetValue(yearFourData, null).ToString());
                    #endregion

                    #region Year Five
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearFiveData = GetPeriodData<T>(data, dataDesc, period.YearFive.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearFiveData != null && updatePeriodRecord)
                        period.YearFiveIsHistorical = ConvertAmountTypeToBoolean(yearFiveData.GetType().GetProperty("AmountType").GetValue(yearFiveData, null).ToString());
                    #endregion

                    #region Year Six
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearSixData = GetPeriodData<T>(data, dataDesc, period.YearSix.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearSixData != null && updatePeriodRecord)
                        period.YearSixIsHistorical = ConvertAmountTypeToBoolean(yearSixData.GetType().GetProperty("AmountType").GetValue(yearSixData, null).ToString());
                    #endregion

                    #region Year Seven
                    if (++columnCount > period.NetColumnCount)
                        goto QUARTERLY_INFO;
                    yearSevenData = GetPeriodData<T>(data, dataDesc, period.YearSeven.ToString(), "A", groupDescription, uniqueByGroupDesc);
                    if (yearSevenData != null && updatePeriodRecord)
                        period.YearSevenIsHistorical = ConvertAmountTypeToBoolean(yearSevenData.GetType().GetProperty("AmountType").GetValue(yearSevenData, null).ToString());
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
                    quarterOneData = GetPeriodData<T>(data, dataDesc, period.QuarterOneYear.ToString(), "Q" + period.QuarterOneQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterOneData != null && updatePeriodRecord)
                        period.QuarterOneIsHistorical = ConvertAmountTypeToBoolean(quarterOneData.GetType().GetProperty("AmountType").GetValue(quarterOneData, null).ToString());
                    #endregion

                    #region Quarter Two
                    if (++columnCount > period.NetColumnCount)
                        goto GROUPING;
                    quarterTwoData = GetPeriodData<T>(data, dataDesc, period.QuarterTwoYear.ToString(), "Q" + period.QuarterTwoQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterTwoData != null && updatePeriodRecord)
                        period.QuarterTwoIsHistorical = ConvertAmountTypeToBoolean(quarterTwoData.GetType().GetProperty("AmountType").GetValue(quarterTwoData, null).ToString());
                    #endregion

                    #region Quarter Three
                    if (++columnCount > period.NetColumnCount)
                        goto GROUPING;
                    quarterThreeData = GetPeriodData<T>(data, dataDesc, period.QuarterThreeYear.ToString(), "Q" + period.QuarterThreeQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterThreeData != null && updatePeriodRecord)
                        period.QuarterThreeIsHistorical = ConvertAmountTypeToBoolean(quarterThreeData.GetType().GetProperty("AmountType").GetValue(quarterThreeData, null).ToString());
                    #endregion

                    #region Quarter Four
                    if (++columnCount > period.NetColumnCount)
                        goto GROUPING;
                    quarterFourData = GetPeriodData<T>(data, dataDesc, period.QuarterFourYear.ToString(), "Q" + period.QuarterFourQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterFourData != null && updatePeriodRecord)
                        period.QuarterFourIsHistorical = ConvertAmountTypeToBoolean(quarterFourData.GetType().GetProperty("AmountType").GetValue(quarterFourData, null).ToString());
                    #endregion

                    #region Quarter Five
                    if (++columnCount > period.NetColumnCount)
                        goto GROUPING;
                    quarterFiveData = GetPeriodData<T>(data, dataDesc, period.QuarterFiveYear.ToString(), "Q" + period.QuarterFiveQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterFiveData != null && updatePeriodRecord)
                        period.QuarterFiveIsHistorical = ConvertAmountTypeToBoolean(quarterFiveData.GetType().GetProperty("AmountType").GetValue(quarterFiveData, null).ToString());
                    #endregion

                    #region Quarter Six
                    if (++columnCount > period.NetColumnCount)
                        goto GROUPING;
                    quarterSixData = GetPeriodData<T>(data, dataDesc, period.QuarterSixYear.ToString(), "Q" + period.QuarterSixQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterSixData != null && updatePeriodRecord)
                        period.QuarterSixIsHistorical = ConvertAmountTypeToBoolean(quarterSixData.GetType().GetProperty("AmountType").GetValue(quarterSixData, null).ToString());
                    #endregion

                    #region Quarter Six
                    if (++columnCount > period.NetColumnCount)
                        goto GROUPING;
                    quarterSevenData = GetPeriodData<T>(data, dataDesc, period.QuarterSevenYear.ToString(), "Q" + period.QuarterSevenQuarter.ToString()
                        , groupDescription, uniqueByGroupDesc);
                    if (quarterSevenData != null && updatePeriodRecord)
                        period.QuarterSevenIsHistorical = ConvertAmountTypeToBoolean(quarterSevenData.GetType().GetProperty("AmountType").GetValue(quarterSevenData, null).ToString());
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

                        if (++countDisplayColumns > period.NetColumnCount)
                            goto GROUPING_QUARTER_INFO;
                        displayData.YEAR_SEVEN = GetGroupedData<T>(yearSevenData, item);

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

                            if (++countDisplayColumns > period.NetColumnCount)
                                goto GROUPING_RECORD_ENTRY;
                            displayData.QUARTER_SEVEN = GetGroupedData<T>(quarterSevenData, item);
                        }

                    GROUPING_RECORD_ENTRY:
                        result.Add(displayData);
                    }

                    continue;
                    #endregion

                RECORD_ENTRY:
                    object dataFirstAdditionalInfo = null;
                    if (additionalFirstDescPropertyName != null)
                    {
                        dataFirstAdditionalInfo = yearFourData == null ? null : GetFormattedValue(yearFourData.GetType()
                            .GetProperty("HarmonicFirst").GetValue(yearFourData, null), dataDecimal, dataPercentage);
                    }
                    object dataSecondAdditionalInfo = null;
                    if (additionalSecondDescPropertyName != null)
                    {
                        dataSecondAdditionalInfo = yearFourData == null ? null : GetFormattedValue(yearFourData.GetType()
                            .GetProperty("HarmonicSecond").GetValue(yearFourData, null), dataDecimal, dataPercentage);
                    }
                    result.Add(new PeriodColumnDisplayData()
                    {
                        DATA_ID = dataId,
                        DATA_BOLD = dataBold,
                        DATA_PERCENTAGE = dataPercentage,
                        DATA_DECIMALS = dataDecimal,
                        SUB_DATA_DESC = groupDescription,
                        SORT_ORDER = sortOrder,
                        ADDITIONAL_DESC_FIRST = dataFirstAdditionalInfo,
                        ADDITIONAL_DESC_SECOND = dataSecondAdditionalInfo,
                        YEAR_ONE_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearOneData, "RootSource"),
                        YEAR_TWO_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearTwoData, "RootSource"),
                        YEAR_THREE_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearThreeData, "RootSource"),
                        YEAR_FOUR_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearFourData, "RootSource"),
                        YEAR_FIVE_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearFiveData, "RootSource"),
                        YEAR_SIX_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearSixData, "RootSource"),
                        YEAR_SEVEN_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(yearSevenData, "RootSource"),
                        QUARTER_ONE_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterOneData, "RootSource"),
                        QUARTER_TWO_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterTwoData, "RootSource"),
                        QUARTER_THREE_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterThreeData, "RootSource"),
                        QUARTER_FOUR_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterFourData, "RootSource"),
                        QUARTER_FIVE_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterFiveData, "RootSource"),
                        QUARTER_SIX_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterSixData, "RootSource"),
                        QUARTER_SEVEN_DATA_ROOT_SOURCE = GetFormatPrecursors<T, String>(quarterSevenData, "RootSource"),

                        YEAR_ONE_DATA_SOURCE = GetFormatPrecursors<T, String>(yearOneData, "DataSource"),
                        YEAR_TWO_DATA_SOURCE = GetFormatPrecursors<T, String>(yearTwoData, "DataSource"),
                        YEAR_THREE_DATA_SOURCE = GetFormatPrecursors<T, String>(yearThreeData, "DataSource"),
                        YEAR_FOUR_DATA_SOURCE = GetFormatPrecursors<T, String>(yearFourData, "DataSource"),
                        YEAR_FIVE_DATA_SOURCE = GetFormatPrecursors<T, String>(yearFiveData, "DataSource"),
                        YEAR_SIX_DATA_SOURCE = GetFormatPrecursors<T, String>(yearSixData, "DataSource"),
                        QUARTER_ONE_DATA_SOURCE = GetFormatPrecursors<T, String>(quarterOneData, "DataSource"),
                        QUARTER_TWO_DATA_SOURCE = GetFormatPrecursors<T, String>(quarterTwoData, "DataSource"),
                        QUARTER_THREE_DATA_SOURCE = GetFormatPrecursors<T, String>(quarterThreeData, "DataSource"),
                        QUARTER_FOUR_DATA_SOURCE = GetFormatPrecursors<T, String>(quarterFourData, "DataSource"),
                        QUARTER_FIVE_DATA_SOURCE = GetFormatPrecursors<T, String>(quarterFiveData, "DataSource"),
                        QUARTER_SIX_DATA_SOURCE = GetFormatPrecursors<T, String>(quarterSixData, "DataSource"),

                        YEAR_ONE_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearOneData, "RootSourceDate"),
                        YEAR_TWO_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearTwoData, "RootSourceDate"),
                        YEAR_THREE_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearThreeData, "RootSourceDate"),
                        YEAR_FOUR_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearFourData, "RootSourceDate"),
                        YEAR_FIVE_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearFiveData, "RootSourceDate"),
                        YEAR_SIX_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearSixData, "RootSourceDate"),
                        YEAR_SEVEN_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(yearSevenData, "RootSourceDate"),
                        QUARTER_ONE_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterOneData, "RootSourceDate"),
                        QUARTER_TWO_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterTwoData, "RootSourceDate"),
                        QUARTER_THREE_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterThreeData, "RootSourceDate"),
                        QUARTER_FOUR_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterFourData, "RootSourceDate"),
                        QUARTER_FIVE_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterFiveData, "RootSourceDate"),
                        QUARTER_SIX_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterSixData, "RootSourceDate"),
                        QUARTER_SEVEN_DATA_ROOT_SOURCE_DATE = GetFormatPrecursors<T, DateTime?>(quarterSevenData, "RootSourceDate"),

                        YEAR_ONE_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearOneData, "LastUpdateDate"),
                        YEAR_TWO_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearTwoData, "LastUpdateDate"),
                        YEAR_THREE_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearThreeData, "LastUpdateDate"),
                        YEAR_FOUR_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearFourData, "LastUpdateDate"),
                        YEAR_FIVE_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearFiveData, "LastUpdateDate"),
                        YEAR_SIX_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearSixData, "LastUpdateDate"),
                        YEAR_SEVEN_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(yearSevenData, "LastUpdateDate"),
                        QUARTER_ONE_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterOneData, "LastUpdateDate"),
                        QUARTER_TWO_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterTwoData, "LastUpdateDate"),
                        QUARTER_THREE_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterThreeData, "LastUpdateDate"),
                        QUARTER_FOUR_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterFourData, "LastUpdateDate"),
                        QUARTER_FIVE_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterFiveData, "LastUpdateDate"),
                        QUARTER_SIX_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterSixData, "LastUpdateDate"),
                        QUARTER_SEVEN_LAST_UPDATE_DATE = GetFormatPrecursors<T, string>(quarterSevenData, "LastUpdateDate"),

                        YEAR_ONE_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearOneData, "ReportedCurrency"),
                        YEAR_TWO_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearTwoData, "ReportedCurrency"),
                        YEAR_THREE_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearThreeData, "ReportedCurrency"),
                        YEAR_FOUR_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearFourData, "ReportedCurrency"),
                        YEAR_FIVE_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearFiveData, "ReportedCurrency"),
                        YEAR_SIX_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearSixData, "ReportedCurrency"),
                        YEAR_SEVEN_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(yearSevenData, "ReportedCurrency"),
                        QUARTER_ONE_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterOneData, "ReportedCurrency"),
                        QUARTER_TWO_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterTwoData, "ReportedCurrency"),
                        QUARTER_THREE_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterThreeData, "ReportedCurrency"),
                        QUARTER_FOUR_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterFourData, "ReportedCurrency"),
                        QUARTER_FIVE_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterFiveData, "ReportedCurrency"),
                        QUARTER_SIX_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterSixData, "ReportedCurrency"),
                        QUARTER_SEVEN_REPORTED_CURRENCY = GetFormatPrecursors<T, string>(quarterSevenData, "ReportedCurrency"),

                        DATA_DESC = dataDesc,
                        YEAR_ONE = yearOneData == null ? null : GetFormattedValue(yearOneData.GetType().GetProperty("Amount")
                        .GetValue(yearOneData, null), dataDecimal, dataPercentage),
                        YEAR_TWO = yearTwoData == null ? null : GetFormattedValue(yearTwoData.GetType().GetProperty("Amount")
                        .GetValue(yearTwoData, null), dataDecimal, dataPercentage),
                        YEAR_THREE = yearThreeData == null ? null : GetFormattedValue(yearThreeData.GetType().GetProperty("Amount")
                        .GetValue(yearThreeData, null), dataDecimal, dataPercentage),
                        YEAR_FOUR = yearFourData == null ? null : GetFormattedValue(yearFourData.GetType().GetProperty("Amount")
                        .GetValue(yearFourData, null), dataDecimal, dataPercentage),
                        YEAR_FIVE = yearFiveData == null ? null : GetFormattedValue(yearFiveData.GetType().GetProperty("Amount")
                        .GetValue(yearFiveData, null), dataDecimal, dataPercentage),
                        YEAR_SIX = yearSixData == null ? null : GetFormattedValue(yearSixData.GetType().GetProperty("Amount")
                        .GetValue(yearSixData, null), dataDecimal, dataPercentage),
                        YEAR_SEVEN = yearSevenData == null ? null : GetFormattedValue(yearSevenData.GetType().GetProperty("Amount")
                        .GetValue(yearSevenData, null), dataDecimal, dataPercentage),
                        QUARTER_ONE = quarterOneData == null ? null : GetFormattedValue(quarterOneData.GetType().GetProperty("Amount")
                        .GetValue(quarterOneData, null), dataDecimal, dataPercentage),
                        QUARTER_TWO = quarterTwoData == null ? null : GetFormattedValue(quarterTwoData.GetType().GetProperty("Amount")
                        .GetValue(quarterTwoData, null), dataDecimal, dataPercentage),
                        QUARTER_THREE = quarterThreeData == null ? null : GetFormattedValue(quarterThreeData.GetType().GetProperty("Amount")
                        .GetValue(quarterThreeData, null), dataDecimal, dataPercentage),
                        QUARTER_FOUR = quarterFourData == null ? null : GetFormattedValue(quarterFourData.GetType().GetProperty("Amount")
                        .GetValue(quarterFourData, null), dataDecimal, dataPercentage),
                        QUARTER_FIVE = quarterFiveData == null ? null : GetFormattedValue(quarterFiveData.GetType().GetProperty("Amount")
                        .GetValue(quarterFiveData, null), dataDecimal, dataPercentage),
                        QUARTER_SIX = quarterSixData == null ? null : GetFormattedValue(quarterSixData.GetType().GetProperty("Amount")
                        .GetValue(quarterSixData, null), dataDecimal, dataPercentage),
                        QUARTER_SEVEN = quarterSevenData == null ? null : GetFormattedValue(quarterSevenData.GetType().GetProperty("Amount")
                        .GetValue(quarterSevenData, null), dataDecimal, dataPercentage),
                    });
                }
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
        public static void UpdateColumnInformation(RadGridView gridView, PeriodColumnUpdateEventArg e, bool isQuarterImplemented = true
            , Int32 navigatingColumnStartIndex = 2)
        {
            for (int i = 0; i < e.PeriodColumnHeader.Count; i++)
            {
                //prevent exceeding gridview column count
                if (i + navigatingColumnStartIndex >= gridView.Columns.Count)
                    return;

                //Description and left navigation columns of period column gadgets ignored
                gridView.Columns[i + navigatingColumnStartIndex].Header = e.PeriodColumnHeader[i];
                gridView.Columns[i + navigatingColumnStartIndex].UniqueName = e.PeriodColumnHeader[i];

                //update column visibility if quarterly data display is implemented
                if (isQuarterImplemented)
                {
                    bool columnVisibility = (i < (e.PeriodColumnHeader.Count / 2)) ? e.PeriodIsYearly : !(e.PeriodIsYearly);

                    if (gridView.Columns[i + navigatingColumnStartIndex].IsVisible != columnVisibility)
                        gridView.Columns[i + navigatingColumnStartIndex].IsVisible = columnVisibility;
                }
            }
        }

        /// <summary>
        /// Set Bold/Percentage formats on data and place tooltips
        /// </summary>
        /// <param name="e">RowLoadedEventArgs</param>
        public static void RowDataCustomization(RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow)
            {
                var row = e.Row as GridViewRow;
                if (row != null)
                {
                    PeriodColumnDisplayData rowContext = row.DataContext as PeriodColumnDisplayData;
                    if (rowContext != null)
                    {
                        if (rowContext.DATA_BOLD != null)
                            row.FontWeight = Convert.ToBoolean(rowContext.DATA_BOLD) ? FontWeights.ExtraBold : FontWeights.Normal;
                        foreach (GridViewCell cell in row.Cells)
                        {
                            //Null Check
                            if (cell.Value == null)
                            {
                                continue;
                            }
                            //No toolTip service for Description and left navigation
                            if (cell.Column.DisplayIndex <= 1)
                            {
                                continue;
                            }
                            //No toolTip service for right navigation column
                            if (cell.Column.DisplayIndex == e.GridViewDataControl.Columns.Count - 1)
                            {
                                continue;
                            }
                            String toolTipContent = GetToolTipContent(rowContext, cell.DataColumn.DataMemberBinding.Path.Path);

                            if (toolTipContent != null)
                            {
                                ToolTip toolTip = new ToolTip()
                                {
                                    Content = toolTipContent,
                                    FontSize = 7,
                                    FontFamily = new FontFamily("Arial")
                                };

                                ToolTipService.SetToolTip(cell, toolTip);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Bold/Percentage formats on data and place tooltips
        /// </summary>
        /// <param name="e">RowLoadedEventArgs</param>
        public static void RowDataCustomizationforCOASpecificGadget(RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow)
            {
                var row = e.Row as GridViewRow;
                if (row != null)
                {
                    PeriodColumnDisplayData rowContext = row.DataContext as PeriodColumnDisplayData;
                    if (rowContext != null)
                    {
                        if (rowContext.DATA_BOLD != null)
                            row.FontWeight = Convert.ToBoolean(rowContext.DATA_BOLD) ? FontWeights.ExtraBold : FontWeights.Normal;
                        foreach (GridViewCell cell in row.Cells)
                        {
                            //Null Check
                            if (cell.Value == null)
                            {
                                continue;
                            }
                            //No toolTip service for Description and left navigation
                            if (cell.Column.DisplayIndex <= 1)
                            {
                                continue;
                            }
                            //No toolTip service for right navigation column
                            if (cell.Column.DisplayIndex == e.GridViewDataControl.Columns.Count - 1)
                            {
                                continue;
                            }
                            String toolTipContent = GetToolTipContentForCOASpecificGadget(rowContext, cell.DataColumn.DataMemberBinding.Path.Path);

                            if (toolTipContent != null)
                            {
                                ToolTip toolTip = new ToolTip()
                                {
                                    Content = toolTipContent,
                                    FontSize = 7,
                                    FontFamily = new FontFamily("Arial")
                                };

                                ToolTipService.SetToolTip(cell, toolTip);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Bold/Percentage formats on data and place tooltips
        /// </summary>
        /// <param name="e">RowLoadedEventArgs</param>
        public static void RowDataCustomizationForConsensusDetailedGadget(RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow)
            {
                var row = e.Row as GridViewRow;
                if (row != null)
                {
                    PeriodColumnDisplayData rowContext = row.DataContext as PeriodColumnDisplayData;
                    if (rowContext != null)
                    {
                        if (rowContext.DATA_BOLD != null)
                            row.FontWeight = Convert.ToBoolean(rowContext.DATA_BOLD) ? FontWeights.ExtraBold : FontWeights.Normal;
                        foreach (GridViewCell cell in row.Cells)
                        {
                            //Null Check
                            if (cell.Value == null)
                            {
                                continue;
                            }
                            //No toolTip service for Description and left navigation
                            if (cell.Column.DisplayIndex <= 1)
                            {
                                continue;
                            }
                            //No toolTip service for right navigation column
                            if (cell.Column.DisplayIndex == e.GridViewDataControl.Columns.Count - 1)
                            {
                                continue;
                            }
                            String toolTipContent = GetToolTipContentForConsensusDetailedGadget(rowContext, cell.DataColumn.DataMemberBinding.Path.Path);

                            if (toolTipContent != null)
                            {
                                ToolTip toolTip = new ToolTip()
                                {
                                    Content = toolTipContent,
                                    FontSize = 7,
                                    FontFamily = new FontFamily("Arial")
                                };

                                ToolTipService.SetToolTip(cell, toolTip);
                            }
                        }
                    }
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
        private static T GetPeriodData<T>(List<T> data, String description, String periodYear, String periodType, String groupDescription, bool uniqueByGroupDesc = false)
        {
            try
            {
                if (data == null || description == null || periodYear == null || periodType == null || groupDescription == null)
                {
                    throw new ArgumentNullException();
                }
                T yearData = uniqueByGroupDesc
                    ? data.Where(record =>
                        record.GetType().GetProperty("GroupDescription").GetValue(record, null) != null &&
                        record.GetType().GetProperty("GroupDescription").GetValue(record, null).ToString().ToUpper().Trim() == groupDescription.ToString().ToUpper().Trim() &&
                        record.GetType().GetProperty("Description").GetValue(record, null) != null &&
                        record.GetType().GetProperty("Description").GetValue(record, null).ToString().ToUpper().Trim() == description.ToString().ToUpper().Trim() &&
                        record.GetType().GetProperty("PeriodYear").GetValue(record, null) != null &&
                        record.GetType().GetProperty("PeriodYear").GetValue(record, null).ToString().ToUpper().Trim() == periodYear.ToString().ToUpper().Trim() &&
                        record.GetType().GetProperty("PeriodType").GetValue(record, null) != null &&
                        record.GetType().GetProperty("PeriodType").GetValue(record, null).ToString().ToUpper().Trim() == periodType.ToString().ToUpper().Trim())
                        .FirstOrDefault()
                    : data.Where(record =>
                        record.GetType().GetProperty("Description").GetValue(record, null) != null &&
                        record.GetType().GetProperty("Description").GetValue(record, null).ToString().ToUpper().Trim() == description.ToString().ToUpper().Trim() &&
                        record.GetType().GetProperty("PeriodYear").GetValue(record, null) != null &&
                        record.GetType().GetProperty("PeriodYear").GetValue(record, null).ToString().ToUpper().Trim() == periodYear.ToString().ToUpper().Trim() &&
                        record.GetType().GetProperty("PeriodType").GetValue(record, null) != null &&
                        record.GetType().GetProperty("PeriodType").GetValue(record, null).ToString().ToUpper().Trim() == periodType.ToString().ToUpper().Trim())
                        .FirstOrDefault();
                return yearData;
            }
            catch (Exception ex)
            {
                return default(T);
            }
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
                PropertyInfo[] propertyInfo = data.GetType().GetProperties();
                if (propertyInfo.Any(record => record.Name == groupItem.GroupPropertyName))
                {
                    object groupdata = data.GetType().GetProperty(groupItem.GroupPropertyName).GetValue(data, null);

                    if (groupdata == null)
                        return null;
                    switch (groupItem.GroupDataType)
                    {
                        case PeriodColumnGroupingType.INT:
                            return ((Int32)groupdata).ToString("n");
                        case PeriodColumnGroupingType.DECIMAL:
                            return (Math.Round((Decimal)groupdata, 2)).ToString("n");
                        case PeriodColumnGroupingType.DECIMAL_PERCENTAGE:
                            return (Math.Round((Decimal)groupdata, 2)).ToString("n") + " %";
                        case PeriodColumnGroupingType.SHORT_DATETIME:
                            return ((DateTime)groupdata).ToShortDateString();
                        case PeriodColumnGroupingType.LONG_DATETIME:
                            return ((DateTime)groupdata).ToShortDateString();
                        case PeriodColumnGroupingType.STRING:
                            return (groupdata).ToString();
                        default:
                            return (groupdata).ToString();
                    }
                }

                return null;
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

        /// <summary>
        /// Formats decimal values with requisite decimal places and optionally posts percentage symbol
        /// </summary>
        /// <param name="value">Value to be formatted</param>
        /// <param name="decimals">Decimal places : [Default] 0</param>
        /// <param name="percentage">Percentage option : Default] false</param>
        /// <returns>Formatted string</returns>
        private static String GetFormattedValue(object value, int? decimals = 0, bool? percentage = false)
        {
            if (value == null)
                return null;

            Decimal result;
            Int32 decimalPlaces = Convert.ToInt32(decimals);
            String formattedValue = Decimal.TryParse(value.ToString(), out result) ? String.Format("{0:n" + decimalPlaces.ToString() + "}", result) 
                                                                                       : value.ToString();
            if (percentage == true)
                formattedValue = formattedValue + " %";

            return formattedValue;
        }

        /// <summary>
        /// Gets value for property Name and type casts it into specified type
        /// </summary>
        /// <typeparam name="T1">Data type</typeparam>
        /// <typeparam name="T2">Type to which property value is to be parsed</typeparam>
        /// <param name="data">Data of type T1</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Property value cast into type T2</returns>
        private static T2 GetFormatPrecursors<T1, T2>(T1 data, String propertyName)
        {
            T2 result = default(T2);

            if (data == null)
                return result;

            try
            {
                PropertyInfo[] propertyInfo = typeof(T1).GetProperties();

                if (propertyInfo.Any(record => record.Name == propertyName))
                    result = (T2)typeof(T1).GetProperty(propertyName).GetValue(data, null);
            }
            catch (Exception)
            { }

            return result;
        }

        /// <summary>
        /// Create tooltip content from property name factor - assumed that data source and data source
        /// date property names are tightly linked with column binded property name
        /// </summary>
        /// <param name="data">PeriodColumnDisplayData</param>
        /// <param name="columnBindedPropertyName">Name of the property binded to the cell column</param>
        /// <returns>tool tip content</returns>
        private static String GetToolTipContent(PeriodColumnDisplayData data, String columnBindedPropertyName)
        {
            String result = null;

            if (data == null)
                return result;

            String rootSource = (String)data.GetType().GetProperty(columnBindedPropertyName + "_DATA_ROOT_SOURCE").GetValue(data, null);

            if (rootSource == null)
                return result;

            DateTime? rootSourceDate = (DateTime?)data.GetType().GetProperty(columnBindedPropertyName + "_DATA_ROOT_SOURCE_DATE").GetValue(data, null);

            result = rootSource + (rootSourceDate != null ? " (" + Convert.ToDateTime(rootSourceDate).ToShortDateString() + ")" : "");

            return result;
        }

        //<summary>
        //Create tooltip content from property name factor - assumed that data source and data source
        //date property names are tightly linked with column binded property name
        //</summary>
        //<param name="data">PeriodColumnDisplayData</param>
        //<param name="columnBindedPropertyName">Name of the property binded to the cell column</param>
        //<returns>tool tip content</returns>
        private static String GetToolTipContentForCOASpecificGadget(PeriodColumnDisplayData data, String columnBindedPropertyName)
        {
            String result = null;

            if (data == null)
                return result;

            String rootSource = (String)data.GetType().GetProperty(columnBindedPropertyName + "_DATA_ROOT_SOURCE").GetValue(data, null);

            if (rootSource == null)
                return result;

            String dataSource = (String)data.GetType().GetProperty(columnBindedPropertyName + "_DATA_SOURCE").GetValue(data, null);

            result = "SOURCE:" + dataSource + Environment.NewLine + "ROOT SOURCE:" + rootSource;

            return result;
        }

        //<summary>
        //Create tooltip content from property name factor - assumed that data source and data source
        //date property names are tightly linked with column binded property name
        //</summary>
        //<param name="data">PeriodColumnDisplayData</param>
        //<param name="columnBindedPropertyName">Name of the property binded to the cell column</param>
        //<returns>tool tip content</returns>
        private static String GetToolTipContentForConsensusDetailedGadget(PeriodColumnDisplayData data, String columnBindedPropertyName)
        {
            String result = null;

            if (data == null)
                return result;

            String lastUpdate = (String)data.GetType().GetProperty(columnBindedPropertyName + "_LAST_UPDATE_DATE").GetValue(data, null);

            if (lastUpdate == null)
                return result;

            String reportedCurrency = (String)data.GetType().GetProperty(columnBindedPropertyName + "_REPORTED_CURRENCY").GetValue(data, null);

            result = "LAST UPDATE: " + lastUpdate + Environment.NewLine + "REPROTED CURRENCY: " + reportedCurrency;

            return result;
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
