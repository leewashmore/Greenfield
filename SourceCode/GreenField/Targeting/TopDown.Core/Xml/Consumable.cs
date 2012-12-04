using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace TopDown.Core.Xml
{
	public class Consumable
	{
		private XmlReader reader;
		private Boolean _consumed;
		private String name;

		[DebuggerStepThrough]
		public Consumable(XmlReader reader)
		{
			this.MarkConsumed();
			this.reader = reader;
		}

		public void MakeSureCharged()
		{
			if (this._consumed)
			{
				if (!this.reader.Read()) throw new ApplicationException();
				this.reader.MoveToContent();
				this.name = this.reader.IsStartElement() ? this.reader.LocalName : String.Empty;
				this._consumed = false;
			}
		}

		public Boolean IsElement { get { return this.reader.NodeType == XmlNodeType.Element; } }

		public Boolean IsEmptyElement(String localName)
		{
			return this.reader.IsStartElement(localName) && this.reader.IsEmptyElement;
		}

		public Boolean IsContentElement(String localName)
		{
			return this.reader.IsStartElement(localName) && !this.reader.IsEmptyElement;
		}

		public Boolean IsEndElement(String name)
		{
			return this.reader.NodeType == XmlNodeType.EndElement && this.reader.LocalName == name;
		}

		public String LocalName
		{
			get { return this.reader.LocalName; }
		}

		public String ReadContentAsString()
		{
			this.MakeSureCharged();
			var content = this.reader.ReadContentAsString();
			// ReadContentAsString advances the reader to the next element (not node), so the consumable is charged (not yet consumed)
			//this._consumed = false;
			return content;
		}

		public Double ReadContentAsDouble()
		{
			this.MakeSureCharged();
			var content = this.reader.ReadContentAsDouble();
			return content;
		}

		public void MarkConsumed()
		{
			this._consumed = true;
		}


		public String ReadAttributeAsNotEmptyString(String name)
		{
			if (!this.reader.MoveToAttribute(name)) throw new ApplicationException("There is no attribute \"" + name + "\" at the element \"" + this.name + "\".");
			var value = this.reader.Value;
			if (string.IsNullOrWhiteSpace(value)) throw new ApplicationException("Value of the \"" + name + "\" attribute is empty.");
			return value;
		}

		public Boolean ReadAttributeAsBoolean(String name, Boolean defaultValue)
		{
			if (!this.reader.MoveToAttribute(name)) return defaultValue;
			var value = this.reader.Value;
			Boolean result;
			try
			{
				result = XmlConvert.ToBoolean(value);
			}
			catch (Exception exception)
			{
				throw new ApplicationException("Unable to read the \"" + value + "\" value of the \"" + name + "\" as " + typeof(Boolean).Name + ".", exception);
			}
			return result;
		}
		
		public Double ReadAttributeAsDouble(String name)
		{
			if (!this.reader.MoveToAttribute(name)) throw new ApplicationException("There is no \"" + name + "\" attribute.");
			var value = this.reader.Value;
			Double result;
			try
			{
				result = XmlConvert.ToDouble(value);
			}
			catch (Exception exception)
			{
				throw new ApplicationException("Unable to read the \"" + value + "\" value of the \"" + name + "\" as " + typeof(Double).Name + ".", exception);
			}
			return result;
		}

        public Int32 ReadAttributeAsInt32(String name)
        {
            if (!this.reader.MoveToAttribute(name)) throw new ApplicationException("There is no \"" + name + "\" attribute.");
            var value = this.reader.Value;
            Int32 result;
            try
            {
                result = XmlConvert.ToInt32(value);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to read the \"" + value + "\" value of the \"" + name + "\" as " + typeof(Int32).Name + ".", exception);
            }
            return result;
        }

		public String ReadAttributeAsString(String name, String defaultValue)
		{
			if (!this.reader.MoveToAttribute(name)) return defaultValue;
			return this.reader.Value;
		}
	}

}