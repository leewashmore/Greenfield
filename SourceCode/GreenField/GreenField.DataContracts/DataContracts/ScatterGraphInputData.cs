using System;
using System.Net;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace GreenField.DataContracts
{
    [DataContract(Name = "ScatterGraphValuationRatio")]
    public enum ScatterGraphValuationRatio
    {
        [EnumMember(Value = "P/Revenue")]
        [DescriptionAttribute("P/Revenue")]
        PRICE_TO_REVENUE,

        [EnumMember(Value = "P/E")]
        [DescriptionAttribute("P/E")]
        PRICE_TO_EQUITY,

        [EnumMember(Value = "EV/EBITDA")]
        [DescriptionAttribute("EV/EBITDA")]
        ENTERPRISE_VALUE_TO_EBITDA,

        [EnumMember(Value = "P/BV")]
        [DescriptionAttribute("P/BV")]
        PRICE_TO_BOOK_VALUE,

        [EnumMember(Value = "P/CE")]
        [DescriptionAttribute("P/CE")]
        PRICE_TO_CAPITAL_EXPENDITURE,

        [EnumMember(Value = "Free Cash Flow Yield")]
        [DescriptionAttribute("Free Cash Flow Yield")]
        FREE_CASH_FLOW_YIELD
    }

    [DataContract(Name = "ScatterGraphFinancialRatio")]
    public enum ScatterGraphFinancialRatio
    {
        [EnumMember(Value = "ROE")]
        [DescriptionAttribute("ROE")]
        RETURN_ON_EQUITY,

        [EnumMember(Value = "Revenue Growth")]
        [DescriptionAttribute("Revenue Growth")]
        REVENUE_GROWTH,

        [EnumMember(Value = "Net Income Growth")]
        [DescriptionAttribute("Net Income Growth")]
        NET_INCOME_GROWTH,

        [EnumMember(Value = "Net Debt/Equity")]
        [DescriptionAttribute("Net Debt/Equity")]
        NET_DEBT_TO_EQUITY,

        [EnumMember(Value = "ROE Growth")]
        [DescriptionAttribute("ROE Growth")]
        RETURN_ON_EQUITY_GROWTH,

        [EnumMember(Value = "Free Cash Flow Margin")]
        [DescriptionAttribute("Free Cash Flow Margin")]
        FREE_CASH_FLOW_MARGIN
    }

    [DataContract(Name = "ScatterGraphContext")]
    public enum ScatterGraphContext
    {
        [EnumMember(Value = "Region")]
        [DescriptionAttribute("Region")]
        REGION,

        [EnumMember(Value = "Country")]
        [DescriptionAttribute("Country")]
        COUNTRY,

        [EnumMember(Value = "Sector")]
        [DescriptionAttribute("Sector")]
        SECTOR,

        [EnumMember(Value = "Industry")]
        [DescriptionAttribute("Industry")]
        INDUSTRY
    }

    [DataContract(Name = "ScatterGraphPeriod")]
    public enum ScatterGraphPeriod
    {
        [EnumMember(Value = "Trailing")]
        [DescriptionAttribute("Trailing")]
        TRAILING,

        [EnumMember(Value = "Forward")]
        [DescriptionAttribute("Forward")]
        FORWARD,

        [EnumMember(Value = "Year")]
        [DescriptionAttribute("Year")]
        YEAR,

        [EnumMember(Value = "Year + 1")]
        [DescriptionAttribute("Year + 1")]
        YEAR_PLUS_ONE,

        [EnumMember(Value = "Year + 2")]
        [DescriptionAttribute("Year + 2")]
        YEAR_PLUS_TWO,

        [EnumMember(Value = "Year + 3")]
        [DescriptionAttribute("Year + 3")]
        YEAR_PLUS_THREE
    }
}
