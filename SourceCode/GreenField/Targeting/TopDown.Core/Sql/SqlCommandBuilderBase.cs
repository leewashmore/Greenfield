using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
    public class SqlCommandBuilderBase
    {
        private SqlCommand command;
        private StringBuilder builder;

        [DebuggerStepThrough]
        public SqlCommandBuilderBase(SqlCommand command)
        {
            this.command = command;
            this.builder = new StringBuilder();
        }

        protected void Text(String text)
        {
            this.builder.Append(text);
            this.command.CommandText = this.builder.ToString();
        }

        protected void ParameterNullable<TValue>(TValue? value) where TValue : struct
        {
            if (value.HasValue)
            {
                this.ParameterNonNullable(value.Value);
            }
            else
            {
                this.ParameterNull();
            }
        }

        protected void ParameterNonNullable<TValue>(TValue value) where TValue : struct
        {
            this.ParameterValue(value);
        }

        protected void Parameter(String value)
        {
            if (value == null)
            {
                this.ParameterNull();
            }
            else
            {
                this.ParameterValue(value);
            }
        }

        private void ParameterNull()
        {
            var parameter = this.command.CreateParameter();
            parameter.ParameterName = "@p" + this.command.Parameters.Count;
            parameter.Value = DBNull.Value;
            this.command.Parameters.Add(parameter);
            this.Text(parameter.ParameterName);
        }

        private void ParameterValue<TValue>(TValue value)
        {
            var parameter = this.command.CreateParameter();
            parameter.ParameterName = "@p" + this.command.Parameters.Count;
            parameter.Value = value;
            this.command.Parameters.Add(parameter);
            this.Text(parameter.ParameterName);
        }

    }
}
