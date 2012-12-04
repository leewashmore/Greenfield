using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core
{
	public abstract class ValidatorBase
	{
		protected IEnumerable<IValidationIssue> ValidateScope(String message, Action<List<IValidationIssue>> handler)
		{
			var issues = new List<IValidationIssue>();
			handler(issues);
			if (issues.Any())
			{
				return new IValidationIssue[] { new CompoundValidationIssue(message, issues) };
			}
			else
			{
				return No.ValidationIssues;
			}
		}
		protected IEnumerable<IValidationIssue> ValidateScope(Action<List<IValidationIssue>> handler)
		{
			var issues = new List<IValidationIssue>();
			handler(issues);
			return issues;
		}
		protected IEnumerable<IValidationIssue> ValidateScope(String message, IEnumerable<IValidationIssue> issues)
		{
			if (issues.Any())
			{
				return new IValidationIssue[] { new CompoundValidationIssue(message, issues) };
			}
			else
			{
				return No.ValidationIssues;
			}
		}
		protected IEnumerable<IValidationIssue> ValidateScope(String message, Func<IEnumerable<IValidationIssue>> handler)
		{
			var issues = handler();
			if (issues.Any())
			{
				return new IValidationIssue[] { new CompoundValidationIssue(message, issues) };
			}
			else
			{
				return No.ValidationIssues;
			}
		}
	}
}
