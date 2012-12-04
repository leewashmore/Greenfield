using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core
{
	public class CommonParts
	{
		[DebuggerStepThrough]
		public CommonParts()
		{
			this.DecimalValueAdapter = new DecimalValueAdapter();
			this.NullableDecimalValueAdapter = new NullableDecimalValueAdapter();
			this.StringValueAdapter = new StringValueAdapter();
		}

		public IValueAdapter<Decimal> DecimalValueAdapter { get; private set; }
		public IValueAdapter<Decimal?> NullableDecimalValueAdapter { get; private set; }
		public IValueAdapter<String> StringValueAdapter { get; private set; }

#warning Make sure everybody uses ValidateWhatever conciously

		[Obsolete]
		public IEnumerable<IValidationIssue> ValidateWhatever<TValue>(IExpression<TValue> expression, CalculationTicket ticket)
		{
			return No.ValidationIssues;
		}

		[Obsolete]
		public IEnumerable<IValidationIssue> ValidateWhatever<TValue>(IExpression<TValue> expression)
		{
			return No.ValidationIssues;
		}

		public IEnumerable<IValidationIssue> ValidateNonNegativeOrNull(IExpression<Decimal?> expression, CalculationTicket ticket)
		{
			var value = expression.Value(ticket);
			return this.ValidateNonNegativeOrNull(expression.Name, value);
		}

		public IEnumerable<IValidationIssue> ValidateNonNegativeOrNull(EditableExpression expression)
		{
			return this.ValidateNonNegativeOrNull(expression.Name, expression.EditedValue);
		}

		public IEnumerable<IValidationIssue> ValidateNonNegative(EditableExpression expression)
		{
			return this.ValidateNonNegative(
				expression.Name,
				expression.EditedValue
			);
		}

		public IEnumerable<IValidationIssue> ValidateNonNegative(String name, Decimal? value)
		{
			if (value.HasValue)
			{
				if (value.Value < 0)
				{
					return new IValidationIssue[] { new ValidationIssue(
						String.Format("{0} cannot be less than 0.", name)
					)};
				}
				else
				{
					return No.ValidationIssues;
				}
			}
			else
			{
				return new IValidationIssue[] { new ValidationIssue(String.Format("{0} is required.", name)) };
			}
		}

		protected IEnumerable<IValidationIssue> ValidateNonNegativeOrNull(String name, Decimal? value)
		{
			if (value.HasValue)
			{
				if (value.Value < 0)
				{
					return new IValidationIssue[] { new ValidationIssue(String.Format("{0} cannot be less than 0.", name)) };
				}
				else
				{
					return No.ValidationIssues;
				}
			}
			else
			{
				return No.ValidationIssues;
			}
		}

		public IEnumerable<IValidationIssue> ValidateEitheNullOr100(IExpression<Decimal?> expression, CalculationTicket ticket)
		{
			var value = expression.Value(ticket);
			if (value.HasValue)
			{
				return this.Validate100(expression.Name, value.Value);
			}
			else
			{
				return No.ValidationIssues;
			}
		}

		private IEnumerable<IValidationIssue> Validate100(String name, Decimal value)
		{
			if (value < 0)
			{
				return new IValidationIssue[] { new ValidationIssue(String.Format("{0} must be greater than 0.", name)) };
			}
			else if ((value - 1.0m) > CalculationHelper.InsignificantDifference)
			{
				return new IValidationIssue[] { new ValidationIssue(String.Format("{0} must be equal to 100% (now greter than that).", name)) };
			}
			else if ((1.0m - value) > CalculationHelper.InsignificantDifference)
			{
				return new IValidationIssue[] { new ValidationIssue(String.Format("{0} must be equal to 100% (now less than that).", name)) };
			}
			else
			{
				return No.ValidationIssues;
			}
		}

		public IEnumerable<IValidationIssue> ValidateComment(EditableExpression expression)
		{
			var issues = new List<IValidationIssue>();
			if (expression.InitialValue != expression.EditedValue)
			{
				// there is a change
				if (String.IsNullOrWhiteSpace(expression.Comment))
				{
					issues.Add(new ValidationIssue(String.Format("{0} needs a comment.", expression.Name)));
				}
			}
			return issues;
		}

		protected IEnumerable<IValidationIssue> ValidateInputExpression(EditableExpression expression)
		{
			var issues = new List<IValidationIssue>();
			issues.AddRange(this.ValidateComment(expression));
			issues.AddRange(this.ValidateNonNegativeOrNull(expression.Name, expression.EditedValue));
			return issues;
		}


		public EditableExpression CreateInputExpression(String name, Decimal? defaultValue)
		{
			return new EditableExpression(
				name,
				defaultValue,
				this.NullableDecimalValueAdapter,
				this.ValidateInputExpression
			);
		}
	}
}
