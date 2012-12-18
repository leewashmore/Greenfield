using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class EditableExpression : IExpression<Decimal?>
    {
        private Decimal? _editedValue;
        private Func<EditableExpression, IEnumerable<IValidationIssue>> validator;
        private IEnumerable<IValidationIssue> injectedProblemsOpt;

        [DebuggerStepThrough]
        public EditableExpression(
            String name,
			Decimal? value,
            IValueAdapter<Decimal?> adapter,
            Func<EditableExpression, IEnumerable<IValidationIssue>> validator
        )
        {
			this.Name = name;
            this.EditedValue = this.InitialValue = this.DefaultValue = value;
            this.Adapter = adapter;
            this.validator = validator;
        }
		public String Name { get; private set; }
        public IValueAdapter<Decimal?> Adapter { get; private set; }
        public Decimal? DefaultValue { get; private set; }
        public Decimal? InitialValue { get; set; }
        public Boolean IsModified { get; private set; }
        public Decimal? EditedValue
        {
            get
            {
                if (this.IsModified)
                {
                    return this._editedValue;
                }
                else
                {
                    return this.InitialValue;
                }
            }
            set
            {
                this._editedValue = value;
                bool modified;
                if (this.InitialValue.HasValue)
                {
                    if (value.HasValue)
                    {
                        modified = !CalculationHelper.NoDifference(this.InitialValue.Value, value.Value);
                    }
                    else
                    {
                        modified = true;
                    }
                }
                else
                {
                    if (value.HasValue)
                    {
                        modified = true;
                    }
                    else
                    {
                        modified = false;
                    }
                }
                this.IsModified = modified;
            }
        }
        /// <summary>
        /// The value that is displayed on the screen when the value is default.
        /// </summary>
        public Decimal? Value(CalculationTicket ticket)
        {
            var value = this.Value(ticket, No.CalculationTracer, No.ExpressionName);
            return value;
        }
        public Decimal? Value(CalculationTicket ticket, ICalculationTracer tracer, String name)
        {
            var value = this.EditedValue;
            tracer.WriteValue(name ?? this.Name, value);
            return value;
        }


        public String Comment { get; set; }
        public Boolean LastOneModified { get; set; }

        IEnumerable<IValidationIssue> IExpression<Decimal?>.Validate(CalculationTicket ticket)
        {
            return this.Validate();
        }

        /// <summary>
        /// When a value cannot validate itself we validate it from outside.
        /// </summary>
        public void InjectProblems(IEnumerable<IValidationIssue> issues)
        {
            this.injectedProblemsOpt = issues;
        }

        public IEnumerable<IValidationIssue> Validate()
        {
            var result = new List<IValidationIssue>();
            result.AddRange(this.validator(this));
            if (this.injectedProblemsOpt != null)
            {
                result.AddRange(this.injectedProblemsOpt);
            }
            return result;
        }

        [DebuggerStepThrough]
        public void Accept(IExpressionResolver<Decimal?> resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(IExpressionResolver resolver)
        {
            resolver.Resolve(this);
        }


      
    }
}
