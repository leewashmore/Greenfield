using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core.Sql
{
	public class SqlQueryCommandBuilder<TInfo> : SqlCommandBuilderBase, IDisposable
		where TInfo : class, new()
	{
		private SqlCommand command;
		private List<IField<TInfo>> fields;

		[DebuggerStepThrough]
		public SqlQueryCommandBuilder(SqlCommand command)
			: base(command)
		{
			this.command = command;
			this.fields = new List<IField<TInfo>>();
		}

		public new SqlQueryCommandBuilder<TInfo> Text(String text)
		{
			base.Text(text);
			return this;
		}

		public new SqlQueryCommandBuilder<TInfo> Parameter(String value)
		{
			base.Parameter(value);
			return this;
		}

		public SqlQueryCommandBuilder<TInfo> Parameter(DateTime value)
		{
			base.ParameterNonNullable(value);
			return this;
		}

		public SqlQueryCommandBuilder<TInfo> Parameter(Decimal value)
		{
			base.ParameterNonNullable(value);
			return this;
		}

		/// <summary>
		/// Adds a list of parameters separated by a comma.
		/// </summary>
		public SqlQueryCommandBuilder<TInfo> Parameters(IEnumerable<String> values)
		{
			var value = values.GetEnumerator();
			if (value.MoveNext())
			{
				this.Parameter(value.Current);
				while (value.MoveNext())
				{
					this.Text(", ");
					this.Parameter(value.Current);
				}
			}
			return this;
		}

		protected SqlQueryCommandBuilder<TInfo> AddField(String name, IField<TInfo> field)
		{
			this.Text(name);
			var wrappedField = new SafeFieldWrapper<TInfo>(field);
			this.fields.Add(wrappedField);
			return this;
		}


		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, String> setter, Boolean isMandatory)
		{
			if (isMandatory)
			{
				return this.AddField(name, new MandatoryStringField<TInfo>(setter));
			}
			else
			{
				return this.AddField(name, new OptionalStringField<TInfo>(setter));
			}
		}

		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, Int32?> setter)
		{
			return this.AddField(name, new NullableInt32Field<TInfo>(setter));
		}

		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, Decimal?> setter)
		{
			return this.AddField(name, new NullableDecimalField<TInfo>(setter));
		}

		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, Decimal> setter)
		{
			return this.AddField(name, new DecimalField<TInfo>(setter));
		}

		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, Int32> setter)
		{
			return this.AddField(name, new Int32Field<TInfo>(setter));
		}

		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, DateTime> setter)
		{
			return this.AddField(name, new DatetimeField<TInfo>(setter));
		}

		public SqlQueryCommandBuilder<TInfo> Field(String name, Action<TInfo, DateTime?> setter)
		{
			return this.AddField(name, new NullableDatetimeField<TInfo>(setter));
		}

		public SqlQueryCommandBuilder<TInfo> Outer<TSubInfo>(String name, Func<TInfo, TSubInfo> creator, Action<TSubInfo, Int32> setter)
		{
			return this.AddField(name, new OuterInt32Field<TInfo, TSubInfo>(creator, setter));
		}

		public SqlQueryCommandBuilder<TInfo> Outer<TSubInfo>(String name, Func<TInfo, TSubInfo> creator, Action<TSubInfo, String> setter)
		{
			return this.AddField(name, new OuterStringField<TInfo, TSubInfo>(creator, setter));
		}

		public SqlQueryCommandBuilder<TInfo> Outer<TSubInfo>(String name, Func<TInfo, TSubInfo> creator, Action<TSubInfo, DateTime> setter)
		{
			return this.AddField(name, new OuterDateTimeField<TInfo, TSubInfo>(creator, setter));
		}

		public void Populate(ICollection<TInfo> result)
		{
			var query = this.command.CommandText;
			foreach (SqlParameter parameter in this.command.Parameters)
			{
				query = query.Replace(parameter.ParameterName, "{" + parameter.Value + "}");
			}
			try
			{
				using (var reader = this.command.ExecuteReader())
				{
					while (reader.Read())
					{
						var info = new TInfo();
						for (var index = 0; index < this.fields.Count; index++)
						{
							this.fields[index].Populate(info, reader, index);
						}
						result.Add(info);
					}
				}
			}
			catch (Exception exception)
			{
				throw new ApplicationException("Query \"" + query + "\".", exception);
			}
		}


		public TInfo PullFirstOrDefault()
		{
			using (var reader = this.command.ExecuteReader())
			{
				if (reader.Read())
				{
					var info = new TInfo();
					for (var index = 0; index < this.fields.Count; index++)
					{
						this.fields[index].Populate(info, reader, index);
					}
					return info;
				}
				else
				{
					return null;
				}
			}
		}

		public IEnumerable<TInfo> PullAll()
		{
			var result = new List<TInfo>();
			this.Populate(result);
			return result;
		}

		public void Dispose()
		{
			if (this.command != null)
			{
				this.command.Dispose();
				this.command = null;
			}
		}


	}
}
