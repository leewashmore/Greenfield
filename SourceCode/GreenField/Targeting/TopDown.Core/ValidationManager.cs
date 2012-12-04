using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core
{
    public class ValidationManager
    {
        private ValidationIssueToJsonSerializer validationSerializer;

        [DebuggerStepThrough]
        public ValidationManager(ValidationIssueToJsonSerializer validationSerializer)
        {
            this.validationSerializer = validationSerializer;
        }

        public String SerializeToJson(IEnumerable<IValidationIssue> issues)
        {
            var builder = new StringBuilder();
            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.WriteArray(issues, issue =>
                {
					writer.Write(delegate
					{
						this.validationSerializer.SerializeValidationIssueOnceResolved(issue, writer);
					});
                });
            }
            var result = builder.ToString();
            return result;
        }
    }
}
