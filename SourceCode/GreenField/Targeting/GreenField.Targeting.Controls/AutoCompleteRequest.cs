using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class AutoCompleteRequest
    {
        [DebuggerStepThrough]
        public AutoCompleteRequest(String pattern, Action callback)
        {
            this.Pattern = pattern;
            this.Callback = callback;
        }

        public String Pattern { get; private set; }
        public Action Callback { get; private set; }
    }
}
