using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core
{
	public class JsonPropertyValueGiver : IValueGiver
	{
		private String propertyName;
		private JsonReader reader;

		[DebuggerStepThrough]
		public JsonPropertyValueGiver(String propertyName, JsonReader reader)
		{
			this.propertyName = propertyName;
			this.reader = reader;
		}

		public Decimal GiveDecimal()
		{
			var value = this.reader.ReadAsDecimal(this.propertyName);
			return value;
		}

		public Decimal? GiveNullableDecimal()
		{
			var value = this.reader.ReadAsNullableDecimal(this.propertyName);
			return value;
		}

		public String GiveString()
		{
			var value = this.reader.ReadAsString();
			return value;
		}
	}
}
