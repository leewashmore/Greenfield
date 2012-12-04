using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.PortfolioPicker
{
    public class PortfolioModel
    {
        [DebuggerStepThrough]
        public PortfolioModel(String id, String name)
        {
            this.Id = id;
            this.Name = name;
        }

        public String Id { get; private set; }
        public String Name { get; private set; }
    }
}
