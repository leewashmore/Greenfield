﻿using System;
using System.Net;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace GreenField.DataContracts
{
    [DataContract(Name = "FinancialStatementDataSource")]
    public enum FinancialStatementDataSource
    {
        [EnumMember(Value = "REUTERS")]
        [DescriptionAttribute("REUTERS")]
        REUTERS,

        [EnumMember(Value = "PRIMARY")]
        [DescriptionAttribute("PRIMARY")]
        PRIMARY,

        [EnumMember(Value = "INDUSTRY")]
        [DescriptionAttribute("INDUSTRY")]
        INDUSTRY
    }

    [DataContract(Name = "FinancialStatementPeriodType")]
    public enum FinancialStatementPeriodType
    {
        [EnumMember(Value = "ANNUAL")]
        [DescriptionAttribute("ANNUAL")]
        ANNUAL,
        [EnumMember(Value = "QUARTER")]
        [DescriptionAttribute("QUARTER")]
        QUARTERLY
    }

    [DataContract(Name = "FinancialStatementFiscalType")]
    public enum FinancialStatementFiscalType
    {
        [EnumMember(Value = "FISCAL")]
        [DescriptionAttribute("FISCAL")]
        FISCAL,

        [EnumMember(Value = "CALENDAR")]
        [DescriptionAttribute("CALENDAR")]
        CALENDAR
    }

    [DataContract(Name = "FinancialStatementStatementType")]
    public enum FinancialStatementType
    {
        [EnumMember(Value = "BAL")]
        [DescriptionAttribute("BAL")]
        BALANCE_SHEET,

        [EnumMember(Value = "CAS")]
        [DescriptionAttribute("CAS")]
        CASH_FLOW_STATEMENT,

        [EnumMember(Value = "INC")]
        [DescriptionAttribute("INC")]
        INCOME_STATEMENT,

        [EnumMember(Value = "FUN")]
        [DescriptionAttribute("FUN")]
        FUNDAMENTAL_SUMMARY
    }
}
