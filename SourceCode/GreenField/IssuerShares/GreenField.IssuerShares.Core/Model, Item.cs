using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using System.Diagnostics;

namespace GreenField.IssuerShares.Core
{
    public class ItemModel
    {
        [DebuggerStepThrough]
        public ItemModel(ISecurity security, Boolean preferred)
        {
            this.Security = security;
            this.Preferred = preferred;
        }

        public ISecurity Security { get; private set; }

        public Boolean Preferred { get; private set; }
    }
}
