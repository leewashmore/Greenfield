using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BottomUp
{
    public class TotalLineModel : IBuLineModel
    {
        [DebuggerStepThrough]
        public void Accept(IBuLineModelResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
