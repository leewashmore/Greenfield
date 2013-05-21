using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net;


namespace GreenField.Web.Helpers
{
    
    public class PortfolioValuation 
    {
        public String portfolio_id {get;set;}
            
        public String issuer_id { get; set; }
            
        public String Asec_Sec_short_name { get; set; }
            
        public String Security_id { get; set; }
           
        public decimal? dirtvaluepc{ get; set; }
           
        public decimal? dirty_price { get; set; }
           
        public decimal? marketcap { get; set; }
        
        public decimal? earnings { get; set; }

        public decimal? fwdearnings { get; set; }

        public decimal? nextYearEarnings { get; set; }
         
        public decimal? trailingearnings { get; set; }
           
        public decimal? percentFactorOwned { get; set; }

        public decimal? fwdpercentFactorOwned { get; set; }

        public decimal? nextYearPercentFactorOwned { get; set; }

        public decimal? trailingpercentFactorOwned { get; set; }

        public decimal? weight { get; set; }
        
        public decimal? currentYearPE {get;set;}

        public decimal? currYearPEContr { get; set; }

        public decimal? fwdWeight{ get; set; }

        public decimal? fwdPE { get; set; }

        public decimal? fwdPEContr { get; set; }

        public decimal? nextYearWeight { get; set; }

        public decimal? nextYearPE { get; set; }

        public decimal? nextYearPEContr { get; set; }

        public decimal? equity { get; set; }

        public decimal? equityFactorOwned { get; set; }

        public decimal? fwdEquity { get; set; }

        public decimal? fwdEquityFactorOwned { get; set; }

        public decimal? nextYearEquity { get; set; }

        public decimal? nextYearEquityFactorOwned { get; set; }

        public decimal? weightPB { get; set; }

        public decimal? currentYearPB { get; set; }

        public decimal? currYearPBContr { get; set; }

        public decimal? fwdWeightPB { get; set; }

        public decimal? fwdPB { get; set; }

        public decimal? fwdPBContr { get; set; }

        public decimal? nextYearWeightPB { get; set; }

        public decimal? nextYearPB { get; set; }

        public decimal? nextYearPBContr { get; set; }

        public decimal? weightMktCap { get; set; }

        public decimal? mktCapContr { get; set; }

        public decimal? Dividend { get; set; }

        public decimal? divFactorOwned { get; set; }

        public decimal? currentYearWeightDY { get; set; }

        public decimal? currentYearDY { get; set; }

        public decimal? currYearDYContr { get; set; }

        public decimal? nextYearDividend { get; set; }

        public decimal? nextYearDivFactorOwned { get; set; }

        public decimal? nextYearWeightDY { get; set; }

        public decimal? nextYearDY { get; set; }

        public decimal? nextYearDYContr { get; set; }

        public decimal? currYearWeightEGrowth { get; set; }

        public decimal? currYearEGrowth { get; set; }
        public decimal? currYearEGrowthContr { get; set; }

        public decimal? nextYearWeightEGrowth { get; set; }

        public decimal? nextYearEGrowth { get; set; }
        public decimal? nextYearEGrowthContr { get; set; }

        public decimal? currYearWeightROE { get; set; }

        public decimal? currYearROE { get; set; }
        public decimal? currYearROEContr { get; set; }

        public decimal? nextYearWeightROE { get; set; }

        public decimal? nextYearROE { get; set; }
        public decimal? nextYearROEContr { get; set; }

    }
}
