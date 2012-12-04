using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace TopDown.Core.Xml
{
	internal class AtomicElement : ValueAccessorBase, IElement
	{
		private Consumable consumable;
		private String name;
		private Boolean released;

		public AtomicElement(Consumable consumable)
			: base(consumable)
		{
			this.consumable = consumable;
			this.name = this.consumable.LocalName;
			this.consumable.MarkConsumed();
			this.released = false;
		}

		public String Name
		{
			[DebuggerStepThrough]
			get { return this.name; }
		}

		public Boolean IsAtomic
		{
			[DebuggerStepThrough]
			get { return true; }
		}

		public override IElement LockOn(String name)
		{
			if (!this.released) throw new ApplicationException("Unable to lock on \"" + name + "\" being at the empty element \"" + this.name + "\" which hasn't been released yet.");
			return base.LockOn(name);
		}

		public override IElement TryLockOn(String name)
		{
			if (!this.released) throw new ApplicationException("Unable to lock on \"" + name + "\" being at the empty element \"" + this.name + "\" which hasn't been released yet.");
			return base.TryLockOn(name);
		}

		public override IElement TryLockOn(String name, out String considerInstead)
		{
			if (!this.released) throw new ApplicationException("Unable to lock on \"" + name + "\" being at the empty element \"" + this.name + "\" which hasn't been released yet.");
			return base.TryLockOn(name, out considerInstead);
		}

		public void Release(String name)
		{
			if (this.name != name) throw new ApplicationException();
			this.released = true;
		}
	}
}
