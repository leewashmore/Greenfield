using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class WariningIssue : IValidationIssue
    {
        [DebuggerStepThrough]
        public WariningIssue(String message)
        {
            this.Message = message;
        }


        public String Message
        {
            get;
            private set;
        }

        [DebuggerStepThrough]
        public void Accept(IValidationIssueResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
