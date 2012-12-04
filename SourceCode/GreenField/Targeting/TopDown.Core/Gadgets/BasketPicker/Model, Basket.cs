using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.BasketPicker
{
    public class BasketModel
    {
        [DebuggerStepThrough]
        public BasketModel(Int32 id, String name)
        {
            this.Id = id;
            this.Name = name;
        }

        public Int32 Id { get; private set; }
        public String Name { get; private set; }
    }
}
