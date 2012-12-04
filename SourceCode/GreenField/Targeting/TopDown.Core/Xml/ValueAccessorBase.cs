using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace TopDown.Core.Xml
{
	internal class ValueAccessorBase
	{
		private Consumable consumable;

		[DebuggerStepThrough]
		public ValueAccessorBase(Consumable consumable)
		{
			this.consumable = consumable;
		}

		public String ReadAttributeAsNotEmptyString(String name)
		{
			return this.consumable.ReadAttributeAsNotEmptyString(name);
		}

		public String ReadContentAsNotEmptyString()
		{
			var name = this.consumable.LocalName;
			var content = this.consumable.ReadContentAsString().AsNotEmpty("Content of \"" + name + "\".");
			return content;
		}

		public Double ReadContentAsDouble()
		{
			var name = this.consumable.LocalName;
			var content = this.consumable.ReadContentAsDouble();
			return content;
		}

		public String ReadAttributeAsString(String name, String defaultValue)
		{
			return this.consumable.ReadAttributeAsString(name, defaultValue);
		}

		public Double ReadAttributeAsDouble(String name)
		{
			return this.consumable.ReadAttributeAsDouble(name);
		}
        
        public Int32 ReadAttributeAsInt32(String name)
        {
            return this.consumable.ReadAttributeAsInt32(name);
        }

		public Boolean ReadAttributeAsBoolean(String name, Boolean defaultValue)
		{
			return this.consumable.ReadAttributeAsBoolean(name, defaultValue);
		}

		public virtual IElement TryLockOn(String name)
		{
			this.Consumable.MakeSureCharged();
			if (this.Consumable.IsContentElement(name)) return new ContentElement(this.Consumable, name);
			if (this.Consumable.IsEmptyElement(name)) return new AtomicElement(this.Consumable);
			return null;
		}

		public virtual IElement TryLockOn(String name, out String considerInstead)
		{
			this.Consumable.MakeSureCharged();
			considerInstead = null;
			if (this.Consumable.IsContentElement(name)) return new ContentElement(this.Consumable, name);
			if (this.Consumable.IsEmptyElement(name)) return new AtomicElement(this.Consumable);
			considerInstead = this.Consumable.LocalName;
			return null;
		}

		public virtual IElement LockOn(String name)
		{
			String considerInstead;
			var locked = this.TryLockOn(name, out considerInstead);
            if (this.Consumable.IsEndElement(this.Consumable.LocalName)) throw new ApplicationException("Element \"" + name + "\" is expected, but the closing \"" + this.Consumable.LocalName + "\" has been reached instead.");
			if (locked == null) throw new ApplicationException("Element \"" + name + "\" is expected, but not found. However there is \"" + considerInstead + "\" which you may want to consider.");
			return locked;
		}

		protected Consumable Consumable
		{
			[DebuggerStepThrough]
			get { return this.consumable; }
		}
	}
}