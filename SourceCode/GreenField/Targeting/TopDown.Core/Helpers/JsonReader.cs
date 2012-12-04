using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TopDown.Core
{
    public class JsonReader : IDisposable
    {
        private JsonTextReader reader;

        public JsonReader(JsonTextReader reader)
        {
            this.reader = reader;
        }

		protected ApplicationException UnexpectedlyEnded()
		{
			return new ApplicationException("Reader has unexpectedly ended.");
		}

		protected ApplicationException CreateError(String message, Exception exception)
		{
			return new ApplicationException(message + " At " + this.reader.Path + ", " + this.reader.Value + ", " + this.reader.TokenType, exception);
		}

        protected void ReadProperty(String propertyName)
        {
			try
			{
				if (!this.reader.Read()) throw this.UnexpectedlyEnded();
				if (reader.TokenType != JsonToken.PropertyName) throw new ApplicationException("Unexpected token: " + reader.TokenType);
				if ((reader.Value as String) != propertyName) throw new ApplicationException("Property name doesn't match the expected one.");
			}
			catch (Exception exception)
			{
				throw this.CreateError("Unable to read the \"" + propertyName + "\" property.", exception);
			}
        }

        public TValue Read<TValue>(Func<TValue> handler)
        {
            if (!this.reader.Read()) throw new ApplicationException();
			if (reader.TokenType != JsonToken.StartObject) throw new ApplicationException("Start object token is expected, but " + reader.Value + " has neen encountered.");
            var result = handler();
            if (!this.reader.Read() || reader.TokenType != JsonToken.EndObject) throw new ApplicationException();
            return result;
        }

        public void Read(Action handler)
        {
            if (!this.reader.Read() || reader.TokenType != JsonToken.StartObject) throw new ApplicationException();
            handler();
            if (!this.reader.Read() || reader.TokenType != JsonToken.EndObject) throw new ApplicationException();
        }

        public void Read(String propertyName, Action handler)
        {
            this.ReadProperty(propertyName);
            this.Read(handler);
        }

        public TValue Read<TValue>(String propertyName, Func<TValue> handler)
        {
            this.ReadProperty(propertyName);
            var value = this.Read(handler);
            return value;
        }

        public void Dispose()
        {
            if (this.reader != null)
            {
                this.reader.Close();
                this.reader = null;
            }
        }

        public Decimal ReadAsDecimal(String propertyName)
        {
            this.ReadProperty(propertyName);
            var value = this.ReadAsDecimal();
            return value;
        }

        public Decimal ReadAsDecimal()
        {
            var value = this.reader.ReadAsDecimal();
            if (!value.HasValue) throw new ApplicationException();
            return value.Value;
        }

        public Decimal? ReadAsNullableDecimal(String propertyName)
        {
            this.ReadProperty(propertyName);
            var value = this.reader.ReadAsDecimal();
            return value;
        }

        public IEnumerable<TValue> ReadArray<TValue>(String propertyName, Func<TValue> handler)
        {
            this.ReadProperty(propertyName);
            var values = this.ReadArray(handler);
            return values;
        }

        public IEnumerable<TValue> ReadArray<TValue>(Func<TValue> handler)
        {
            if (!this.reader.Read() || reader.TokenType != JsonToken.StartArray) throw new ApplicationException();
            if (!this.reader.Read()) throw new ApplicationException();
            var result = new List<TValue>();

            while (this.reader.TokenType == JsonToken.StartObject)
            {
                var value = handler();
                result.Add(value);
                if (!this.reader.Read() || reader.TokenType != JsonToken.EndObject) throw new ApplicationException();
                if (!this.reader.Read()) throw new ApplicationException();
            }

            if (this.reader.TokenType == JsonToken.EndArray)
            {
                return result;
            }
            else
            {
                throw new ApplicationException();
            }
        }


        public void ReadArray(String propertyName, Action handler)
        {
            this.ReadProperty(propertyName);
            this.ReadArray(handler);
        }

        public void ReadArray(Action handler)
        {
            if (!this.reader.Read() || reader.TokenType != JsonToken.StartArray) throw new ApplicationException();
            if (!this.reader.Read()) throw new ApplicationException();
            while (this.reader.TokenType == JsonToken.StartObject)
            {
                handler();
                if (!this.reader.Read() || reader.TokenType != JsonToken.EndObject) throw new ApplicationException();
                if (!this.reader.Read()) throw new ApplicationException();
            }

            if (this.reader.TokenType != JsonToken.EndArray)
            {
                throw new ApplicationException();
            }
        }

		public IEnumerable<String> ReadArrayOfStrings(String propertyName)
		{
			this.ReadProperty(propertyName);
			return this.ReadArrayOfStrings();
		}
		public IEnumerable<String> ReadArrayOfStrings()
		{
			if (!this.reader.Read() || reader.TokenType != JsonToken.StartArray) throw new ApplicationException();
			
			var result = new List<String>();
            while (true)
            {
                var value = this.reader.ReadAsString();
                if (value == null && this.reader.TokenType == JsonToken.EndArray)
                {
                    break;
                }
                result.Add(value);
            }
			if (this.reader.TokenType == JsonToken.EndArray)
			{
				return result;
			}
			else
			{
				throw new ApplicationException();
			}
		}

        public String ReadAsString(String propertyName)
        {
            this.ReadProperty(propertyName);
            var value = this.ReadAsString();
            return value;
        }

        public String ReadAsString()
        {
            var value = this.reader.ReadAsString();
            if (value == null) throw new ApplicationException("String value cannot be NULL.");
            return value;
        }

        public Int32 ReadAsInt32(String propertyName)
        {
            this.ReadProperty(propertyName);
            var value = this.ReadAsInt32();
            return value;
        }

        public Int32 ReadAsInt32()
        {
            var value = this.reader.ReadAsInt32();
            if (!value.HasValue) throw new ApplicationException();
            return value.Value;
        }

        public Boolean ReadAsBoolean(String propertyName)
        {
            this.ReadProperty(propertyName);
            var value = this.ReadAsBoolean();
            return value;
        }

        public Boolean ReadAsBoolean()
        {
            var read = (this.reader.ReadAsString() ?? String.Empty).ToLowerInvariant();
            if (read == "true")
            {
                return true;
            }
            else if (read == "false")
            {
                return false;
            }
            else
            {
                throw new ApplicationException("Unexpected value: " + read);
            }
        }

		public DateTime ReadAsDatetime(String propertyName)
		{
			this.ReadProperty(propertyName);
			var value = this.ReadAsDatetime();
			return value;
		}

		private DateTime ReadAsDatetime()
		{
			var value = this.reader.ReadAsDateTime();
			if (value.HasValue)
			{
				return value.Value;
			}
			else
			{
				throw new ApplicationException();
			}
		}

        public String ReadAsNullableString(String propertyName)
        {
            this.ReadProperty(propertyName);
            return this.ReadAsNullableString();
        }

        public String ReadAsNullableString()
        {
            return this.reader.ReadAsString();
        }
    }
}
