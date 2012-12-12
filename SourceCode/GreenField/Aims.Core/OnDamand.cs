using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core
{
    public interface IOnDemand<out TValue> : IDisposable
    {
        TValue Claim();
    }
}
