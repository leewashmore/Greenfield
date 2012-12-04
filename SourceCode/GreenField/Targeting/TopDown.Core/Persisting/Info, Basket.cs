using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the BASKET table.
    /// </summary>
    public class BasketInfo
    {
        [DebuggerStepThrough]
        public BasketInfo()
        {
        }

        [DebuggerStepThrough]
        public BasketInfo(Int32 id, String type)
        {
            this.Id = id;
            this.Type = type;
        }

        public Int32 Id { get; set; }
        public String Type { get; set; }
    }
}
