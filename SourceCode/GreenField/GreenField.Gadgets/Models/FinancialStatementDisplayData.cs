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
        public Decimal? YEAR_ONE { get; set; }

        /// <summary>
        /// Data - Year two annual
        /// </summary>
        public Decimal? YEAR_TWO { get; set; }

        /// <summary>
        /// Data - Year three annual
        /// </summary>
        public Decimal? YEAR_THREE { get; set; }

        /// <summary>
        /// Data - Year four annual
        /// </summary>
        public Decimal? YEAR_FOUR { get; set; }

        /// <summary>
        /// Data - Year five annual
        /// </summary>
        public Decimal? YEAR_FIVE { get; set; }

        /// <summary>
        /// Data - Year six annual
        /// </summary>
        public Decimal? YEAR_SIX { get; set; }

        /// <summary>
        /// Data - Year Seven annual
        /// </summary>
        public Decimal? YEAR_SEVEN { get; set; }

        /// <summary>
        /// Data - Quarter one
        /// </summary>
        public Decimal? QUARTER_ONE { get; set; }

        /// <summary>
        /// Data - Quarter two
        /// </summary>
        public Decimal? QUARTER_TWO { get; set; }

        /// <summary>
        /// Data - Quarter three
        /// </summary>
        public Decimal? QUARTER_THREE { get; set; }

        /// <summary>
        /// Data - Quarter four
        /// </summary>
        public Decimal? QUARTER_FOUR { get; set; }

        /// <summary>
        /// Data - Quarter five
        /// </summary>
        public Decimal? QUARTER_FIVE { get; set; }

        /// <summary>
        /// Data - Quarter six
        /// </summary>
        public Decimal? QUARTER_SIX { get; set; }
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
}
