using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.Helpers
{
    public class EMSummFinalData
    {
        public int PeriodYear { get; set; }

        public int DataId { get; set; }

        public string DataType { get; set; }

        public string CountryCode { get; set; }

        public Decimal HarmonicMean { get; set; }
    }
}