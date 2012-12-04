using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core
{
    public class ValidationIssueToJsonSerializer
    {
        public void SerializeValidationIssuesIfAny(IEnumerable<IValidationIssue> issues, IJsonWriter writer)
        {
            if (issues.Any())
            {
                writer.WriteArray(issues, JsonNames.Issues, issue =>
                {
                    writer.Write(delegate
                    {
                        this.SerializeValidationIssueOnceResolved(issue, writer);
                    });
                });
            }
        }

        public void SerializeValidationIssueOnceResolved(IValidationIssue issue, IJsonWriter writer)
        {
            var resolver = new SerializeIssueOnceResolved_IValidationIssueResolver(this, writer);
            issue.Accept(resolver);
        }

        private class SerializeIssueOnceResolved_IValidationIssueResolver : IValidationIssueResolver
        {
            private IJsonWriter writer;
            private ValidationIssueToJsonSerializer serializer;

            public SerializeIssueOnceResolved_IValidationIssueResolver(ValidationIssueToJsonSerializer serializer, IJsonWriter writer)
            {
                this.writer = writer;
                this.serializer = serializer;
            }

            public void Resolve(ValidationIssue issue)
            {
                this.serializer.SerializeValidationIssue(issue, writer);
            }

            public void Resolve(CompoundValidationIssue issue)
            {
                this.serializer.SerializeCompoundValidationIssue(issue, writer);
            }
        }

        public void SerializeValidationIssue(ValidationIssue issue, IJsonWriter writer)
        {
            this.Serialize(issue.Message, No.ValidationIssues, writer);
        }

        public void SerializeCompoundValidationIssue(CompoundValidationIssue issue, IJsonWriter writer)
        {
            this.Serialize(issue.Message, issue.Issues, writer);
        }

        protected void Serialize(String message, IEnumerable<IValidationIssue> issues, IJsonWriter writer)
        {
			writer.Write(message, JsonNames.Message);
			writer.WriteArray(issues, JsonNames.Issues, issue =>
			{
				writer.Write(delegate
				{
					this.SerializeValidationIssueOnceResolved(issue, writer);
				});
			});
        }
    }
}
