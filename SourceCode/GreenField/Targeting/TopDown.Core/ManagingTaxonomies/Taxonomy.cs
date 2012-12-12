using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;

namespace TopDown.Core.ManagingTaxonomies
{
    public class Taxonomy : Repository<ITaxonomyResident>
    {
        [DebuggerStepThrough]
        public Taxonomy(Int32 id)
        {
            this.Id = id;
        }

        public Int32 Id { get; private set; }

        [DebuggerStepThrough]
        public void RegisterResident(ITaxonomyResident resident)
        {
            base.RegisterValue(resident);
        }

        [DebuggerStepThrough]
        public IEnumerable<ITaxonomyResident> GetResidents()
        {
            return base.GetValues();
        }
    }
}
