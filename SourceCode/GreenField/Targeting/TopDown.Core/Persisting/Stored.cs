using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class Stored<TId, TValue>
    {
        [DebuggerStepThrough]
        public Stored(TId id, TValue value)
        {
            this.Id = id;
            this.Value = value;
        }

        public TId Id { get; private set; }
        public TValue Value { get; private set; }
    }
}
