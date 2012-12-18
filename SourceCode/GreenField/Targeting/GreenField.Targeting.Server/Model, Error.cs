using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class ErrorModel : IssueModel
    {
        [DebuggerStepThrough]
        public ErrorModel()
        {
        }

        [DebuggerStepThrough]
        public ErrorModel(String message) : base(message)
        {
        }


    }
}
