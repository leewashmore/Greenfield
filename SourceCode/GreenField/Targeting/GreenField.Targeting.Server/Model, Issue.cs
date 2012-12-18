using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    [KnownType(typeof(ErrorModel))]
    [KnownType(typeof(WarningModel))]
    [KnownType(typeof(CompoundIssueModel))]
    public class IssueModel
    {
        [DataMember]
        public String Message { get; set; }

        [DebuggerStepThrough]
        public IssueModel(String message) : this()
        {
            this.Message = message;
        }

        [DebuggerStepThrough]
        public IssueModel()
        {
        }
    }
}
