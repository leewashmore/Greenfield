using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using System.Diagnostics;

namespace Aims.Core
{
    public class No : Aims.Expressions.No
    {
        private readonly static IEnumerable<ISecurity> securities = new ISecurity[] { };
        public static IEnumerable<ISecurity> Securities
        {
            [DebuggerStepThrough]
            get { return securities; }
        }
    }
}
