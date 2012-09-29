using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchmarkNodeFinancials
{
    class BenchmarkNodeFinancialsData
    {
        public string BenchmarkName { get; set; }

        public string SecurityId { get; set; }
      
        public string IssuerId { get; set; }
      
        public string IssueName { get; set; }
      
        public string AsecShortName { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string Sector { get; set; }

        public string industry { get; set; }

        public Decimal? BenWeight { get; set; }

        public Decimal Amount { get; set; }

        public Decimal InvAmount { get; set; }

        public int PeriodYear { get; set; }

        public int DataId { get; set; }

        public String PeriodType { get; set; }

        public String Currency { get; set; }

        public String TypeNode { get; set; }
        
    }
}
