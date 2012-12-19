using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public class IssueTraverser
    {
        public IEnumerable<IValidationIssue> TraverseAll(IEnumerable<IValidationIssue> issues)
        {
            var result = new List<IValidationIssue>();
            foreach (var issue in issues)
            {
                this.TraverseOnceResolved(issue, result);
            }
            return result;
        }

        private void TraverseOnceResolved(IValidationIssue issue, List<IValidationIssue> result)
        {
            var resolver = new TraverseOnceResolved_IValidationIssueResolver(this, result);
            issue.Accept(resolver);

        }

        private class TraverseOnceResolved_IValidationIssueResolver : IValidationIssueResolver
        {
            private List<IValidationIssue> result;
            private IssueTraverser traverser;

            public TraverseOnceResolved_IValidationIssueResolver(IssueTraverser traverser, List<IValidationIssue> result)
            {
                this.result = result;
                this.traverser = traverser;
            }

            public void Resolve(ErrorIssue issue)
            {
                this.traverser.Traverse(issue, result);
            }

            public void Resolve(CompoundValidationIssue issue)
            {
                this.traverser.Traverse(issue, result);
            }

            public void Resolve(WariningIssue issue)
            {
                this.traverser.Traverse(issue, result);
            }
        }

        protected void Traverse(ErrorIssue issue, List<IValidationIssue> result)
        {
            result.Add(issue);
        }

        protected void Traverse(CompoundValidationIssue issues, List<IValidationIssue> result)
        {
            result.Add(issues);
            foreach (var issue in issues.Issues)
            {
                this.TraverseOnceResolved(issue, result);
            }
        }

        protected void Traverse(WariningIssue issue, List<IValidationIssue> result)
        {
            result.Add(issue);
        }
    }
}
