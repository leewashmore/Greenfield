using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.Helpers
{
    public class CalculatedValuesForValuation
    {
        public string IssuerId { get; set; }

        public string SecurityId { get; set; }

        public int DataId { get; set; }

        public Decimal Amount { get; set; }

        public Decimal? PortfolioPercent { get; set; }

        public Decimal InverseAmount { get; set; }

        public Decimal? MultipliedValue { get; set; }
    }
}