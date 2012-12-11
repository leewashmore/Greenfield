using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;

namespace TopDown.Core.ManagingTaxonomies
{
    public class RegionNode : Repository<IRegionNodeResident>, ITaxonomyResident, IRegionNodeResident, INode
    {
        [DebuggerStepThrough]
        public RegionNode(String name)
        {
            this.Name = name;
        }

        public String Name { get; private set; }

        [DebuggerStepThrough]
        public void RegisterResident(IRegionNodeResident resident)
        {
            base.RegisterValue(resident);
        }

        [DebuggerStepThrough]
        public IEnumerable<IRegionNodeResident> GetResidents()
        {
            return base.GetValues();
        }

        [DebuggerStepThrough]
        public void Accept(ITaxonomyResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
        
        [DebuggerStepThrough]
        public void Accept(IRegionNodeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(INodeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
