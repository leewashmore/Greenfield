using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class WarningModel : IssueModel
    {
        [DebuggerStepThrough]
        public WarningModel()
        {
        }

        [DebuggerStepThrough]
        public WarningModel(String message)
            : base(message)
        {
        }
    }
}
