using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace TopDown.Core.Xml
{
	internal class ContentElement : ValueAccessorBase, IElement
	{
		private readonly String name;

		public ContentElement(Consumable consumable, String name)
			: base(consumable)
		{
			this.name = name;
			if (!this.Consumable.IsContentElement(name)) throw new ApplicationException();
			this.Consumable.MarkConsumed();
		}

		public String Name
		{
			[DebuggerStepThrough]
			get { return this.name; }
		}

		public Boolean IsAtomic
		{
			[DebuggerStepThrough]
			get { return false; }
		}


		public void Release(String name)
		{
			if (this.name != name) throw new ApplicationException("Element was locked as \"" + this.name + "\", but released as \"" + name + "\".");
			this.Consumable.MakeSureCharged();
			if (!this.Consumable.IsEndElement(this.name))
			{
				throw new ApplicationException("End element \"" + this.name + "\" is expected, but \"" + this.Consumable.LocalName + "\" was found instead.");
			}
			this.Consumable.MarkConsumed();
		}
	}
}
