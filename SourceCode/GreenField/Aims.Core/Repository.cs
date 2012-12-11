using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core
{
    public class Repository<TValue>
    {
        private List<TValue> values;
        
        public Repository()
        {
            this.values = new List<TValue>();
        }

        protected void RegisterValue(TValue value)
        {
            this.values.Add(value);
        }

        protected IEnumerable<TValue> GetValues()
        {
            return this.values;
        }
    }
}
