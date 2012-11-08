using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.Helpers
{
    public class EMSummHarmonicData
    {
        public string SecurityId { get; set; }

        public string IssuerId { get; set; }
       
        public string Country { get; set; }

        public Decimal BenWeight { get; set; }

        public Decimal Amount { get; set; }

        public Decimal InvAmount { get; set; }

        public int PeriodYear { get; set; }

        public int DataId { get; set; }

        public string DataType { get; set; }       
    }
}