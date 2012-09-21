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
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    //public class MeasuresList
    //{
    //    private string _measures;
    //    public string Measures
    //    {
    //        get { return _measures; }
    //        set { _measures = value; }
    //    }

    //    private int _dataId;
    //    public int DataId
    //    {
    //        get { return _dataId; }
    //        set { _dataId = value; }
    //    }
    //}

    public class CurrentFairValueMeasures
    {
        public Dictionary<string, int> MeasureList;
        public static String FORWARD_DIVIDEND_YIELD = "Forward Dividend Yield";
        public static String FORWARD_EV_EBITDA = "Forward EV/EBITDA";
        public static String FORWARD_EV_EBITDA_RELATIVE_TO_COUNTRY = "Forward EV/EBITDA relative to Country";
        public static String FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY = "Forward EV/EBITDA relative to Industry";
        public static String FORWARD_EV_EBITDA_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY = "Forward EV/EBITDA Relative to Country Industry";
        public static String FORWARD_EV_REVENUE = "Forward EV/Revenue";
        public static String FORWARD_P_NAV = "Forward P/NAV";
        public static String FORWARD_P_APPRAISAL_VALUE = "Forward P/Appraisal Value";
        public static String FORWARD_P_BV = "Forward P/BV";
        public static String FORWARD_P_BV_RELATIVE_TO_COUNTRY = "Forward P/BV relative to Country";
        public static String FORWARD_P_BV_RELATIVE_TO_INDUSTRY = "Forward P/BV relative to Industry";
        public static String FORWARD_P_BV_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY = "Forward P/BV Relative to Country Industry";
        public static String FORWARD_P_CE = "Forward P/CE";
        public static String FORWARD_P_E = "Forward P/E";
        public static String FORWARD_P_E_RELATIVE_TO_COUNTRY = "Forward P/E relative to Country";
        public static String FORWARD_P_E_RELATIVE_TO_INDUSTRY = "Forward P/E relative to Industry";
        public static String FORWARD_P_E_RELATIVE_TO_INDUSTRY_WITHIN_COUNTRY = "Forward P/E Relative to Country Industry";
        public static String FORWARD_P_E_TO_2_YEAR_EARNINGS_GROWTH = "Forward P/E to 2 Year Growth";
        public static String FORWARD_P_E_TO_3_YEAR_EARNINGS_GROWTH = "Forward P/E to 3 Year Growth";
        public static String FORWARD_P_EMBEDDED_VALUE = "Forward P/Embedded Value";
        public static String FORWARD_P_REVENUE = "Forward P/Revenue";          
    }
}
