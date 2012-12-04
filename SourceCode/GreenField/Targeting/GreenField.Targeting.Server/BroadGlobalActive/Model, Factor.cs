using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract(Name = "BgaFactorModel")]
    public class FactorModel
    {
		[DebuggerStepThrough]
        public FactorModel()
        {
            this.Items = new List<FactorItemModel>();
        }

		[DebuggerStepThrough]
		public FactorModel(IEnumerable<FactorItemModel> items)
		{
			this.Items.AddRange(items);
		}
		
		[DataMember]
        public List<FactorItemModel> Items { get; set; }
    }
}
