using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class IssueModel
    {
        [DebuggerStepThrough]
        public IssueModel()
        {
        }

        [DebuggerStepThrough]
        public IssueModel(String message)
        {
            this.Message = message;
        }


        [DebuggerStepThrough]
        public IssueModel(String message, IEnumerable<IssueModel> issues)
        {
            this.Message = message;
            this.Issues = issues.ToList();
        }

        [DataMember]
        public String Message { get; set; }

        [DataMember]
        public IEnumerable<IssueModel> Issues { get; set; }
    }
}
