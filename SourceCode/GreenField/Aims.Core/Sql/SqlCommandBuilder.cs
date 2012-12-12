using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Aims.Core.Sql
{
    public class SqlCommandBuilder : SqlCommandBuilderBase, IDisposable
    {
        private SqlCommand command;
        
        [DebuggerStepThrough]
        public SqlCommandBuilder(SqlCommand command)
            : base(command)
        {
            this.command = command;
        }
        
        public new SqlCommandBuilder Text(String text)
        {
            base.Text(text);
            return this;
        }

        public new SqlCommandBuilder Parameter(String value)
        {
            base.Parameter(value);
            return this;
        }
    
        public SqlCommandBuilder Parameter(Decimal value)
        {
            base.ParameterNonNullable(value);
            return this;
        }

        public SqlCommandBuilder Parameter(Decimal? value)
        {
            base.ParameterNullable(value);
            return this;
        }

        public void Dispose()
        {
            if (this.command != null)
            {
                this.command.Dispose();
                this.command = null;
            }
        }

        public Int32 Execute()
        {
            return this.command.ExecuteNonQuery();
        }
    }
}
