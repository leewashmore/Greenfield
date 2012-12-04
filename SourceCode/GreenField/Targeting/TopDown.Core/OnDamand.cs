using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core
{
    public interface IOnDamand<TValue> : IDisposable
    {
        TValue Claim();
    }
}
