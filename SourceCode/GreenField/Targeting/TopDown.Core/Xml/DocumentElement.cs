using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TopDown.Core.Xml
{
	public class DocumentElement
	{
		private Consumable consumable;
		
		public DocumentElement(XmlReader reader)
		{
			this.consumable = new Consumable(reader);
		}

        public virtual IElement TryLockOn(String name)
        {
            this.consumable.MakeSureCharged();
            if (this.consumable.IsContentElement(name)) return new ContentElement(this.consumable, name);
            if (this.consumable.IsEmptyElement(name)) return new AtomicElement(this.consumable);
            return null;
        }

        public virtual IElement TryLockOn(String name, out String considerInstead)
        {
            this.consumable.MakeSureCharged();
            considerInstead = null;
            if (this.consumable.IsContentElement(name)) return new ContentElement(this.consumable, name);
            if (this.consumable.IsEmptyElement(name)) return new AtomicElement(this.consumable);
            considerInstead = this.consumable.LocalName;
            return null;
        }

        public virtual IElement LockOn(String name)
        {
            String considerInstead;
            var locked = this.TryLockOn(name, out considerInstead);
            if (this.consumable.IsEndElement(this.consumable.LocalName)) throw new ApplicationException("Element \"" + name + "\" is expected, but the closing \"" + this.consumable.LocalName + "\" has been reached instead.");
            if (locked == null) throw new ApplicationException("Element \"" + name + "\" is expected, but not found. However there is \"" + considerInstead + "\" which you may want to consider.");
            return locked;
        }


        public IElement TryMultilockOn(IEnumerable<String> names)
        {
            foreach (var name in names)
            {
                var locked = this.TryLockOn(name);
                if (locked != null)
                {
                    return locked;
                }
            }
            return null;
        }

        public IElement MultilockOn(IEnumerable<String> names)
        {
            var locked = this.TryMultilockOn(names);
            if (locked == null) throw new ApplicationException();
            return locked;
        }
    }
}
