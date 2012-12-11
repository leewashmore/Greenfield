using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core;
using System.Diagnostics;

namespace GreenField.IssuerShares.Core
{
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel(Issuer issuer, IEnumerable<ItemModel> items)
        {
            this.Items = items.ToList();
        }

        public IEnumerable<ItemModel> Items { get; private set; }
    }
}
