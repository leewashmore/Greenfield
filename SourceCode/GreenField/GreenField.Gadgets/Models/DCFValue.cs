using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greenfield.Gadgets.Models
{
    /// <summary>
    /// Result Set of DCFSP calculator
    /// </summary>
    public class DCFValue
    {
        public decimal DCFValuePerShare { get; set; }

        public decimal UpsideValue { get; set; }
    }
}
