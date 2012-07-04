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

namespace GreenField.Gadgets.Models
{
    /// <summary>
    /// Financial Statement display data for six year annual and quarter period.
    /// </summary>
    public class PeriodColumnDisplayData
    {
        public Int32 DATA_ID { get; set; }

        /// <summary>
        /// Data Description
        /// </summary>
        public String DATA_DESC { get; set; }

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
        /// Data - Year Seven annual
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
    }

    public class PeriodRecord
    {
        public int YearOne;
        public bool YearOneIsHistorical;

        public int YearTwo;
        public bool YearTwoIsHistorical;

        public int YearThree;
        public bool YearThreeIsHistorical;

        public int YearFour;
        public bool YearFourIsHistorical;

        public int YearFive;
        public bool YearFiveIsHistorical;

        public int YearSix;
        public bool YearSixIsHistorical;

        public int YearSeven;
        public bool YearSevenIsHistorical;

        public int QuarterOneYear;
        public int QuarterOneQuarter;
        public bool QuarterOneIsHistorical;

        public int QuarterTwoYear;
        public int QuarterTwoQuarter;
        public bool QuarterTwoIsHistorical;

        public int QuarterThreeYear;
        public int QuarterThreeQuarter;
        public bool QuarterThreeIsHistorical;

        public int QuarterFourYear;
        public int QuarterFourQuarter;
        public bool QuarterFourIsHistorical;

        public int QuarterFiveYear;
        public int QuarterFiveQuarter;
        public bool QuarterFiveIsHistorical;

        public int QuarterSixYear;
        public int QuarterSixQuarter;
        public bool QuarterSixIsHistorical;
    }

    public class PeriodColumnCEDisplayData
    {
        public Int32 DATA_ID { get; set; }

        /// <summary>
        /// Data Description
        /// </summary>
        public string DATA_DESC { get; set; }

        /// <summary>
        /// Data Description
        /// </summary>
        public string SUB_DATA_DESC { get; set; }

        /// <summary>
        /// Data - Year one annual
        /// </summary>
        public string YEAR_ONE { get; set; }

        /// <summary>
        /// Data - Year two annual
        /// </summary>
        public string YEAR_TWO { get; set; }

        /// <summary>
        /// Data - Year three annual
        /// </summary>
        public string YEAR_THREE { get; set; }

        /// <summary>
        /// Data - Year four annual
        /// </summary>
        public string YEAR_FOUR { get; set; }

        /// <summary>
        /// Data - Year five annual
        /// </summary>
        public string YEAR_FIVE { get; set; }

        /// <summary>
        /// Data - Year six annual
        /// </summary>
        public string YEAR_SIX { get; set; }

        /// <summary>
        /// Data - Year Seven annual
        /// </summary>
        public string YEAR_SEVEN { get; set; }

        /// <summary>
        /// Data - Quarter one
        /// </summary>
        public string QUARTER_ONE { get; set; }

        /// <summary>
        /// Data - Quarter two
        /// </summary>
        public string QUARTER_TWO { get; set; }

        /// <summary>
        /// Data - Quarter three
        /// </summary>
        public string QUARTER_THREE { get; set; }

        /// <summary>
        /// Data - Quarter four
        /// </summary>
        public string QUARTER_FOUR { get; set; }

        /// <summary>
        /// Data - Quarter five
        /// </summary>
        public string QUARTER_FIVE { get; set; }

        /// <summary>
        /// Data - Quarter six
        /// </summary>
        public string QUARTER_SIX { get; set; }
    }

}
