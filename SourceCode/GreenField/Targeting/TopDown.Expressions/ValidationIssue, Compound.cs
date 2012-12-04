using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class CompoundValidationIssue : IValidationIssue
    {
        [DebuggerStepThrough]
        public CompoundValidationIssue(String message, IEnumerable<IValidationIssue> issues)
        {
            this.Message = message;
            this.Issues = issues.ToList();
        }

        public String Message { get; private set; }
        public IEnumerable<IValidationIssue> Issues { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IValidationIssueResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
