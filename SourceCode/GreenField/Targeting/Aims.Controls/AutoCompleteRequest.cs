using System;
using System.Diagnostics;

namespace Aims.Controls
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
