using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;

namespace GreenField.Web.Helpers
{
    public class HistoricalValuationCompararer : EqualityComparer<GetPRevenueData_Result>
    {
        public override bool Equals(GetPRevenueData_Result objGetPRevenueData_Result1, GetPRevenueData_Result objGetPRevenueData_Result2)
        {
            if (objGetPRevenueData_Result1 == null && objGetPRevenueData_Result2 == null)
                return true;
            if ((objGetPRevenueData_Result1.Period_Type.ToUpper().Trim() == objGetPRevenueData_Result2.Period_Type.ToUpper().Trim())
                && (objGetPRevenueData_Result1.Period_Year == objGetPRevenueData_Result2.Period_Year))
                return true;
            else
                return false;           
        }
        public override int GetHashCode(GetPRevenueData_Result objGetPRevenueData_Result)
        {
            if (objGetPRevenueData_Result.Period_Type == null && objGetPRevenueData_Result.Period_Year == null)
                return 0;
            int hCodePeriodType = objGetPRevenueData_Result.Period_Type.GetHashCode();
            int hCodePeriodYear = objGetPRevenueData_Result.Period_Year.GetHashCode();
            return hCodePeriodType ^ hCodePeriodYear;
        }
    }
    public class HistoricalValEV_EBITDACompararer : EqualityComparer<GetEV_EBITDAData_Result>
    {
        public override bool Equals(GetEV_EBITDAData_Result objGetEV_EBITDAData_Result1, GetEV_EBITDAData_Result objGetEV_EBITDAData_Result2)
        {
            if (objGetEV_EBITDAData_Result1 == null && objGetEV_EBITDAData_Result2 == null)
                return true;
            if ((objGetEV_EBITDAData_Result1.PeriodLabel.ToUpper().Trim() == objGetEV_EBITDAData_Result2.PeriodLabel.ToUpper().Trim()))                
                return true;
            else
                return false;
        }
        public override int GetHashCode(GetEV_EBITDAData_Result objGetEV_EBITDAData_Result)
        {
            if (objGetEV_EBITDAData_Result.PeriodLabel == null && objGetEV_EBITDAData_Result.PeriodLabel == null)
                return 0;
            int hCodePeriodType = objGetEV_EBITDAData_Result.PeriodLabel.GetHashCode();
            int hCodePeriodYear = objGetEV_EBITDAData_Result.PeriodLabel.GetHashCode();
            return hCodePeriodType ^ hCodePeriodYear;
        }
    }
}