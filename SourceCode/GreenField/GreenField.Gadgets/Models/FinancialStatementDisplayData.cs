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
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Models
{
    /// <summary>
    /// Financial Statement display data for six year annual and quarter period.
    /// </summary>
    public class PeriodColumnDisplayData
    {
        public Int32? DATA_ID { get; set; }

        public Boolean? DATA_BOLD { get; set; }

        public Boolean? DATA_PERCENTAGE { get; set; }

        public Int32? DATA_DECIMALS { get; set; }

        public String YEAR_ONE_DATA_ROOT_SOURCE { get; set; }
        public String YEAR_TWO_DATA_ROOT_SOURCE { get; set; }
        public String YEAR_THREE_DATA_ROOT_SOURCE { get; set; }
        public String YEAR_FOUR_DATA_ROOT_SOURCE { get; set; }
        public String YEAR_FIVE_DATA_ROOT_SOURCE { get; set; }
        public String YEAR_SIX_DATA_ROOT_SOURCE { get; set; }
        public String YEAR_SEVEN_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_ONE_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_TWO_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_THREE_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_FOUR_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_FIVE_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_SIX_DATA_ROOT_SOURCE { get; set; }
        public String QUARTER_SEVEN_DATA_ROOT_SOURCE { get; set; }

        public String YEAR_ONE_DATA_SOURCE { get; set; }
        public String YEAR_TWO_DATA_SOURCE { get; set; }
        public String YEAR_THREE_DATA_SOURCE { get; set; }
        public String YEAR_FOUR_DATA_SOURCE { get; set; }
        public String YEAR_FIVE_DATA_SOURCE { get; set; }
        public String YEAR_SIX_DATA_SOURCE { get; set; }
        public String YEAR_SEVEN_DATA_SOURCE { get; set; }
        public String QUARTER_ONE_DATA_SOURCE { get; set; }
        public String QUARTER_TWO_DATA_SOURCE { get; set; }
        public String QUARTER_THREE_DATA_SOURCE { get; set; }
        public String QUARTER_FOUR_DATA_SOURCE { get; set; }
        public String QUARTER_FIVE_DATA_SOURCE { get; set; }
        public String QUARTER_SIX_DATA_SOURCE { get; set; }
        public String QUARTER_SEVEN_DATA_SOURCE { get; set; }

        public DateTime? YEAR_ONE_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? YEAR_TWO_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? YEAR_THREE_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? YEAR_FOUR_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? YEAR_FIVE_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? YEAR_SIX_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? YEAR_SEVEN_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_ONE_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_TWO_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_THREE_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_FOUR_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_FIVE_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_SIX_DATA_ROOT_SOURCE_DATE { get; set; }
        public DateTime? QUARTER_SEVEN_DATA_ROOT_SOURCE_DATE { get; set; }        

        /// <summary>
        /// Data Description
        /// </summary>
        public String DATA_DESC { get; set; }

        /// <summary>
        /// Data Description
        /// </summary>
        public object ADDITIONAL_DESC_FIRST { get; set; }

        /// <summary>
        /// Data Description
        /// </summary>
        public object ADDITIONAL_DESC_SECOND { get; set; }


        /// <summary>
        /// Data Description
        /// </summary>
        public Int32 SORT_ORDER { get; set; }

        /// <summary>
        /// Data Description
        /// </summary>
        public String SUB_DATA_DESC { get; set; }

        /// <summary>
        /// Data - Year one annual
        /// </summary>
        public object YEAR_ONE { get; set; }

        /// <summary>
        /// Data - Year two annual
        /// </summary>
        public object YEAR_TWO { get; set; }

        /// <summary>
        /// Data - Year three annual
        /// </summary>
        public object YEAR_THREE { get; set; }

        /// <summary>
        /// Data - Year four annual
        /// </summary>
        public object YEAR_FOUR { get; set; }

        /// <summary>
        /// Data - Year five annual
        /// </summary>
        public object YEAR_FIVE { get; set; }

        /// <summary>
        /// Data - Year six annual
        /// </summary>
        public object YEAR_SIX { get; set; }

        /// <summary>
        /// Data - Year seven annual
        /// </summary>
        public object YEAR_SEVEN { get; set; }

        /// <summary>
        /// Data - Quarter one
        /// </summary>
        public object QUARTER_ONE { get; set; }

        /// <summary>
        /// Data - Quarter two
        /// </summary>
        public object QUARTER_TWO { get; set; }

        /// <summary>
        /// Data - Quarter three
        /// </summary>
        public object QUARTER_THREE { get; set; }

        /// <summary>
        /// Data - Quarter four
        /// </summary>
        public object QUARTER_FOUR { get; set; }

        /// <summary>
        /// Data - Quarter five
        /// </summary>
        public object QUARTER_FIVE { get; set; }

        /// <summary>
        /// Data - Quarter six
        /// </summary>
        public object QUARTER_SIX { get; set; }

        /// <summary>
        /// Data - Quarter seven
        /// </summary>
        public object QUARTER_SEVEN { get; set; }
    }

    public class PeriodColumnGroupingDetail
    {
        public PeriodColumnGroupingType GroupDataType { get; set; }
        public String GroupPropertyName { get; set; }
        public String GroupDisplayName { get; set; }

        public PeriodColumnGroupingDetail()
        {
        }

        public PeriodColumnGroupingDetail(PeriodColumnGroupingType groupDataType, String groupPropertyName, String groupDisplayName)
        {
            GroupDataType = groupDataType;
            GroupPropertyName = groupPropertyName;
            GroupDisplayName = groupDisplayName;
        }
    }

    public enum PeriodColumnGroupingType
    {
        INT,
        DECIMAL,
        DECIMAL_PERCENTAGE,
        SHORT_DATETIME,
        LONG_DATETIME,
        STRING
    }

    public class PeriodRecord
    {
        public int YearOne { get; set; }
        public bool YearOneIsHistorical { get; set; }

        public int YearTwo { get; set; }
        public bool YearTwoIsHistorical { get; set; }

        public int YearThree { get; set; }
        public bool YearThreeIsHistorical { get; set; }

        public int YearFour { get; set; }
        public bool YearFourIsHistorical { get; set; }

        public int YearFive { get; set; }
        public bool YearFiveIsHistorical { get; set; }

        public int YearSix { get; set; }
        public bool YearSixIsHistorical { get; set; }

        public int YearSeven { get; set; }
        public bool YearSevenIsHistorical { get; set; }

        public int QuarterOneYear { get; set; }
        public int QuarterOneQuarter { get; set; }
        public bool QuarterOneIsHistorical { get; set; }

        public int QuarterTwoYear { get; set; }
        public int QuarterTwoQuarter { get; set; }
        public bool QuarterTwoIsHistorical { get; set; }

        public int QuarterThreeYear { get; set; }
        public int QuarterThreeQuarter { get; set; }
        public bool QuarterThreeIsHistorical { get; set; }

        public int QuarterFourYear { get; set; }
        public int QuarterFourQuarter { get; set; }
        public bool QuarterFourIsHistorical { get; set; }

        public int QuarterFiveYear { get; set; }
        public int QuarterFiveQuarter { get; set; }
        public bool QuarterFiveIsHistorical { get; set; }

        public int QuarterSixYear { get; set; }
        public int QuarterSixQuarter { get; set; }
        public bool QuarterSixIsHistorical { get; set; }

        public int QuarterSevenYear { get; set; }
        public int QuarterSevenQuarter { get; set; }
        public bool QuarterSevenIsHistorical { get; set; }

        public int DefaultHistoricalYearCount { get; set; }
        public int DefaultHistoricalQuarterCount { get; set; }
        public int NetColumnCount { get; set; }
        public bool IsQuarterImplemented { get; set; }
    }

    //public class PeriodColumnCEDisplayData
    //{
    //    public Int32 DATA_ID { get; set; }

    //    /// <summary>
    //    /// Data Description
    //    /// </summary>
    //    public string DATA_DESC { get; set; }

    //    /// <summary>
    //    /// Data Description
    //    /// </summary>
    //    public string SUB_DATA_DESC { get; set; }

    //    /// <summary>
    //    /// Data - Year one annual
    //    /// </summary>
    //    public string YEAR_ONE { get; set; }

    //    /// <summary>
    //    /// Data - Year two annual
    //    /// </summary>
    //    public string YEAR_TWO { get; set; }

    //    /// <summary>
    //    /// Data - Year three annual
    //    /// </summary>
    //    public string YEAR_THREE { get; set; }

    //    /// <summary>
    //    /// Data - Year four annual
    //    /// </summary>
    //    public string YEAR_FOUR { get; set; }

    //    /// <summary>
    //    /// Data - Year five annual
    //    /// </summary>
    //    public string YEAR_FIVE { get; set; }

    //    /// <summary>
    //    /// Data - Year six annual
    //    /// </summary>
    //    public string YEAR_SIX { get; set; }

    //    /// <summary>
    //    /// Data - Year Seven annual
    //    /// </summary>
    //    public string YEAR_SEVEN { get; set; }

    //    /// <summary>
    //    /// Data - Quarter one
    //    /// </summary>
    //    public string QUARTER_ONE { get; set; }

    //    /// <summary>
    //    /// Data - Quarter two
    //    /// </summary>
    //    public string QUARTER_TWO { get; set; }

    //    /// <summary>
    //    /// Data - Quarter three
    //    /// </summary>
    //    public string QUARTER_THREE { get; set; }

    //    /// <summary>
    //    /// Data - Quarter four
    //    /// </summary>
    //    public string QUARTER_FOUR { get; set; }

    //    /// <summary>
    //    /// Data - Quarter five
    //    /// </summary>
    //    public string QUARTER_FIVE { get; set; }

    //    /// <summary>
    //    /// Data - Quarter six
    //    /// </summary>
    //    public string QUARTER_SIX { get; set; }
    //}

}
