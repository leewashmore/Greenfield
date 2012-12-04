using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    [Serializable]
    public class ValidationException : Exception
    {
        private IValidationIssue issue;
        public ValidationException(IValidationIssue issue) : base(issue.Message) { this.issue = issue; }
        public ValidationException(IValidationIssue issue, Exception inner) : base(issue.Message, inner) { this.issue = issue; }
        public IValidationIssue Issue
        {
            [DebuggerStepThrough]
            get { return this.issue; }
        }
        protected ValidationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
