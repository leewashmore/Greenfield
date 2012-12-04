using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TopDown.Core
{
	public class JsonWriter : IJsonWriter
	{
		private JsonTextWriter writer;

		public JsonWriter(JsonTextWriter writer)
		{
			this.writer = writer;
		}

		public void Write(String propertyName, Action handler)
		{
			writer.WritePropertyName(propertyName);
			this.Write(handler);
		}

		public void Write(Action handler)
		{
			writer.WriteStartObject();
			handler();
			writer.WriteEndObject();
		}

		public void WriteArray<TValue>(IEnumerable<TValue> values, String propertyName, Action<TValue> handler)
		{
			writer.WritePropertyName(propertyName);
			this.WriteArray(values, handler);
		}

		public void WriteArray<TValue>(IEnumerable<TValue> values, Action<TValue> handler)
		{
			writer.WriteStartArray();
			foreach (var value in values)
			{
				handler(value);
			}
			writer.WriteEndArray();
		}

		public void Write(String value, String propertyName)
		{
			try
			{
				writer.WritePropertyName(propertyName);
			}
			catch (Exception exception)
			{
				throw new ApplicationException("Unable to write the name of the \"" + propertyName + "\" property.", exception);
			}

			if (value != null)
			{
				writer.WriteValue(value);
			}
			else
			{
				writer.WriteNull();
			}
		}

		public void Write(Int32 value, String propertyName)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(value);
		}

		public void Write(Decimal value, String propertyName)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(value);
		}


		public void Write(Decimal? value, String name)
		{
			this.writer.WritePropertyName(name);
			if (value.HasValue)
			{
				this.writer.WriteValue(value.Value);
			}
			else
			{
				this.writer.WriteNull();
			}
		}

		public void Dispose()
		{
			if (this.writer != null)
			{
				this.writer.Close();
				this.writer = null;
			}
		}

		public void Write(Decimal value)
		{
			this.writer.WriteValue(value);
		}

		public void Write(Decimal? value)
		{
			this.writer.WriteValue(value);
		}

		public void Write(String value)
		{
			this.writer.WriteValue(value);
		}

		public void Write(Boolean value, String propertyName)
		{
			writer.WritePropertyName(propertyName);
			writer.WriteValue(value);
		}

		public void Write(Boolean value)
		{
			writer.WriteValue(value);
		}

		public void Write(DateTime value, String propertyName)
		{
			this.writer.WritePropertyName(propertyName);
			this.writer.WriteValue(value);
		}

		public void WriteNull(String propertyName)
		{
			this.writer.WritePropertyName(propertyName);
			this.WriteNull();
		}

		public void WriteNull()
		{
			this.writer.WriteNull();
		}

	}
}
