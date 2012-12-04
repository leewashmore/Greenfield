using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Xml
{
    public interface IElement : IValueAccessor
    {
        String Name { get; }
        Boolean IsAtomic { get; }
        IElement LockOn(String name);
        IElement TryLockOn(String name);
        IElement TryLockOn(String name, out String considerInstead);
        void Release(String name);
    }
}
