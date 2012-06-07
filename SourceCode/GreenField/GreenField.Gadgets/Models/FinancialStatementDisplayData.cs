﻿using System;
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
    public class FinancialStatementDisplayData
    {
        /// <summary>
        /// Data Description
        /// </summary>
        public String DATA_DESC { get; set; }

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

    public static class PeriodRecord
    {
        public static int YearOne;
        public static bool YearOneIsHistorical;

        public static int YearTwo;
        public static bool YearTwoIsHistorical;

        public static int YearThree;
        public static bool YearThreeIsHistorical;

        public static int YearFour;
        public static bool YearFourIsHistorical;

        public static int YearFive;
        public static bool YearFiveIsHistorical;

        public static int YearSix;
        public static bool YearSixIsHistorical;

        public static int QuarterOneYear;
        public static int QuarterOneQuarter;
        public static bool QuarterOneIsHistorical;

        public static int QuarterTwoYear;
        public static int QuarterTwoQuarter;
        public static bool QuarterTwoIsHistorical;

        public static int QuarterThreeYear;
        public static int QuarterThreeQuarter;
        public static bool QuarterThreeIsHistorical;

        public static int QuarterFourYear;
        public static int QuarterFourQuarter;
        public static bool QuarterFourIsHistorical;

        public static int QuarterFiveYear;
        public static int QuarterFiveQuarter;
        public static bool QuarterFiveIsHistorical;

        public static int QuarterSixYear;
        public static int QuarterSixQuarter;
        public static bool QuarterSixIsHistorical;
    }
}