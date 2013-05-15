using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace GreenField.Web.Helpers
{
    /// <summary>
    /// class to store original value and scrubbed value for each security
    /// </summary>
    public class SecurityDataIdScrub 
    {
        public string IssuerId { get; set; }
        public string IssuerName { get; set; }
        public string IsoCountryCode { get; set; }
        public String SecurityId { get; set; }
        public int? DataId { get; set; }
        public int? PeriodYear { get; set; }
        public String GICS_Industry { get; set; }
        public String GICS_Industry_Name { get; set; }
        public String GICS_Sector { get; set; }
        public String GICS_Sector_Name { get; set; }

        [System.ComponentModel.DefaultValue(null)]
        public decimal? OriginalValue { get; set; }
        [System.ComponentModel.DefaultValue(null)]
        public decimal? ScrubbedValue { get; set; }
    }
}
