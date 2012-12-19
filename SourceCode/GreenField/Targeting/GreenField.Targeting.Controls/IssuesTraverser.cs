using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.Generic;

namespace GreenField.Targeting.Controls
{
    public class IssuesTraverser
    {
        public IEnumerable<IssueModel> TraverseAll(IEnumerable<IssueModel> issues)
        {
            var result = new List<IssueModel>();
            foreach (var issue in issues)
            {
                var resolver = new Resolver(this, result);
                issue.Accept(resolver);
            }

            return result;
        }

        private class Resolver : IIssueModelResolver
        {
            private IssuesTraverser issuesTraverser;
            private List<IssueModel> result;

            public Resolver(IssuesTraverser issuesTraverser, List<IssueModel> result)
            {
                this.issuesTraverser = issuesTraverser;
                this.result = result;
            }


            public void Resolve(CompoundIssueModel model)
            {
                this.issuesTraverser.Traverse(model, result);
            }

            public void Resolve(WarningModel model)
            {
                this.issuesTraverser.Traverse(model, result);
            }

            public void Resolve(ErrorModel model)
            {
                this.issuesTraverser.Traverse(model, result);
            }
        }

        internal void Traverse(CompoundIssueModel model, List<IssueModel> result)
        {
            result.Add(model);
            result.AddRange(this.TraverseAll(model.Issues));
        }

        internal void Traverse(WarningModel model, List<IssueModel> result)
        {
            result.Add(model);
        }

        internal void Traverse(ErrorModel model, List<IssueModel> result)
        {
            result.Add(model);
        }
    }
}
