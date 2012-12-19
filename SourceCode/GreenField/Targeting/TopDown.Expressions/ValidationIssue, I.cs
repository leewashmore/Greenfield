using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface IValidationIssue
    {
        String Message { get; }
        void Accept(IValidationIssueResolver resolver);
    }

    public interface IValidationIssueResolver
    {
        void Resolve(ErrorIssue issue);
        void Resolve(CompoundValidationIssue issue);
        void Resolve(WariningIssue issue);
    }
}
