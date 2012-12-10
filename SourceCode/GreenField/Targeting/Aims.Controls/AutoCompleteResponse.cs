using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Aims.Controls
{
    public class AutoCompleteResponse<TResult>
    {
        [DebuggerStepThrough]
        public AutoCompleteResponse(AutoCompleteRequest request, TResult result)
        {
            this.Request = request;
            this.Result = result;
        }

        public AutoCompleteRequest Request { get; private set; }

        public TResult Result { get; private set; }
    }
}
