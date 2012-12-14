using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.Persisting
{
    public interface IChangeInfoBase
    {
        String Comment { get; }
        Decimal? Before { get; }
        Decimal? After { get; }
        Int32 ChangesetId { get; }
    }
}
