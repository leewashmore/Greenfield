using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net;


namespace MarketingCalc.DataContract
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
        public decimal? percentFactorOwned { get; set; }
    }
}
