using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Overlaying
{
    /// <summary>
    /// Collection of overlays for the portfolio.
    /// </summary>
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel(IEnumerable<ItemModel> items)
        {
            this.Items = items.ToList();
        }
        public IEnumerable<ItemModel> Items { get; private set; }
    }
}
