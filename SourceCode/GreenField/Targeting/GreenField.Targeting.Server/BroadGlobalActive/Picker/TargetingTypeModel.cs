using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive.Picker
{
    [DataContract(Name="BgaTargetingTypePickerModel")]
    public class TargetingTypeModel
    {
        [DebuggerStepThrough]
        public TargetingTypeModel()
        {
            this.Portfolios = new List<PortfolioModel>();
        }

        [DebuggerStepThrough]
        public TargetingTypeModel(Int32 id, String name, IEnumerable<PortfolioModel> portfolios)
            : this()
        {
            this.Id = id;
            this.Name = name;
            this.Portfolios.AddRange(portfolios);
        }

        [DataMember]
        public Int32 Id { get; set; }

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public List<PortfolioModel> Portfolios { get; set; }
    }
}
