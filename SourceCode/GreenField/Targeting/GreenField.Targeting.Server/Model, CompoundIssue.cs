using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class CompoundIssueModel : IssueModel
    {
        [DebuggerStepThrough]
        public CompoundIssueModel()
        {
        }

        [DebuggerStepThrough]
        public CompoundIssueModel(String message, IEnumerable<IssueModel> issues)
            : base(message)
        {
            this.Issues = issues.ToList();
        }

        [DataMember]
        public IEnumerable<IssueModel> Issues { get; set; }
    }
}
