using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBpt;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core
{
    public class ExpressionToJsonSerializer
    {
        private ValidationIssueToJsonSerializer validationSerializer;

        [DebuggerStepThrough]
        public ExpressionToJsonSerializer(
            ValidationIssueToJsonSerializer validationSerializer
        )
        {
            this.validationSerializer = validationSerializer;
        }

        public void Write(Decimal value, String name, IJsonWriter writer)
        {
            writer.Write(value, name);
        }

        public void SerializeEditable(EditableExpression expression, String name, IJsonWriter writer)
        {
            writer.Write(name, delegate
            {
                expression.Adapter.Take(new Write_NamedValueAdapter(JsonNames.InitialValue, writer), expression.InitialValue);
                expression.Adapter.Take(new Write_NamedValueAdapter(JsonNames.EditedValue, writer), expression.EditedValue);
                writer.Write(expression.LastOneModified, JsonNames.LastOneModified);
                writer.Write(expression.Comment, JsonNames.Comment);
                var issues = expression.Validate();
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }

        public void Serialize(EditableExpression expression, String name, IJsonWriter writer, Decimal? displayValue)
        {
            writer.Write(name, delegate
            {
                writer.Write(expression.InitialValue, JsonNames.InitialValue);
                writer.Write(expression.EditedValue, JsonNames.EditedValue);
                writer.Write(displayValue, JsonNames.DisplayValue);
                writer.Write(expression.LastOneModified, JsonNames.LastOneModified);
                writer.Write(expression.Comment, JsonNames.Comment);
                var issues = expression.Validate();
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }

        

        public void Write<TValue>(Expression<TValue> expression, String name, IJsonWriter writer, CalculationTicket ticket)
        {
            writer.Write(name, delegate
            {
                var issues = expression.Validate(ticket);
                expression.Adapter.Take(new Write_NamedValueAdapter("value", writer), expression.Value(ticket));
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }

        public void Write<TModel, TValue>(ModelFormulaExpression<TModel, TValue> expression, String name, IJsonWriter writer, CalculationTicket ticket)
        {
            writer.Write(name, delegate
            {
                var value = expression.Value(ticket);
                var issues = expression.Validate(ticket);
                expression.Adapter.Take(new Write_NamedValueAdapter("value", writer), value);
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }

        public void Write(NullableSumExpression expression, String name, IJsonWriter writer, CalculationTicket ticket)
        {
            writer.Write(name, delegate
            {
                var value = expression.Value(ticket);
                var issues = expression.Validate(ticket);
                writer.Write(value, "value");
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }
        public void Write(SumExpression expression, String name, IJsonWriter writer, CalculationTicket ticket)
        {
            writer.Write(name, delegate
            {
                var value = expression.Value(ticket);
                var issues = expression.Validate(ticket);
                writer.Write(value, "value");
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }
        public void Serialize<TValue>(UnchangableExpression<TValue> expression, String name, IJsonWriter writer, CalculationTicket ticket)
        {
            writer.Write(name, delegate
            {
                var value = expression.Value(ticket);
                var issues = expression.Validate(ticket);
                expression.Adapter.Take(new Write_NamedValueAdapter("value", writer), value);
                this.validationSerializer.SerializeValidationIssuesIfAny(issues, writer);
            });
        }

        public void SerializeOnceResolved<TValue>(IExpression<TValue> expression, String name, IJsonWriter writer, CalculationTicket ticket)
        {
            expression.Accept(new WriteOnceResolved_IExpressionResolver<TValue>(this, name, ticket, writer));
        }

        private class WriteOnceResolved_IExpressionResolver<TValue> : IExpressionResolver<TValue>
        {
            private IJsonWriter writer;
            private String name;
            private ExpressionToJsonSerializer parent;
            private CalculationTicket ticket;

            public WriteOnceResolved_IExpressionResolver(ExpressionToJsonSerializer parent, String name, CalculationTicket ticket ,IJsonWriter writer)
            {
                this.parent = parent;
                this.name = name;
                this.ticket = ticket;
                this.writer = writer;
            }

            public void Resolve(SumExpression expression)
            {
                this.parent.Write(expression, this.name, this.writer, this.ticket);
            }

            public void Resolve(EditableExpression expression)
            {
                this.parent.SerializeEditable(expression, this.name, this.writer);
            }

            public void Resolve(Expression<TValue> expression)
            {
                this.parent.Write(expression, this.name, this.writer, this.ticket);
            }

            public void Resolve<TModel>(ModelFormulaExpression<TModel, TValue> expression)
            {
                this.parent.Write(expression, this.name, this.writer, this.ticket);
            }

            public void Resolve(NullableSumExpression expression)
            {
                this.parent.Write(expression, this.name, this.writer, this.ticket);
            }

            public void Resolve(UnchangableExpression<TValue> expression)
            {
                this.parent.Serialize(expression, this.name, this.writer, this.ticket);
            }
        }

        private class Write_NamedValueAdapter : IValueTaker
        {
            private String name;
            private IJsonWriter writer;

            public Write_NamedValueAdapter(String name, IJsonWriter writer)
            {
                this.name = name;
                this.writer = writer;
            }

            public void TakeDecimal(Decimal value)
            {
                this.writer.Write(value, this.name);
            }

            public void TakeNullableDecimal(Decimal? value)
            {
                this.writer.Write(value, this.name);
            }

            public void TakeString(String value)
            {
                this.writer.Write(value, this.name);
            }
        }
    }
}
