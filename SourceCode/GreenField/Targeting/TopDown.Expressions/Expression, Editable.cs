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
            return this.EditedValue;
        }
        public String Comment { get; set; }
        public Boolean LastOneModified { get; set; }

        IEnumerable<IValidationIssue> IExpression<Decimal?>.Validate(CalculationTicket ticket)
        {
            return this.Validate();
        }

        public IEnumerable<IValidationIssue> Validate()
        {
            return this.validator(this);
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
