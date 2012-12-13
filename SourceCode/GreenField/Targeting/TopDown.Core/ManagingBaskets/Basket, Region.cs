using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingCountries;
using System.Diagnostics;
using Aims.Core;

namespace TopDown.Core.ManagingBaskets
{
    public class RegionBasket : IBasket
    {
        [DebuggerStepThrough]
        public RegionBasket(Int32 id, String name, IEnumerable<Country> countries)
        {
            this.Id = id;
            this.Name = name;
            this.Countries = new List<Country>(countries);
        }
        
        public Int32 Id { get; private set; }
        public String Name { get; private set; }
        public IEnumerable<Country> Countries { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IBasketResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
