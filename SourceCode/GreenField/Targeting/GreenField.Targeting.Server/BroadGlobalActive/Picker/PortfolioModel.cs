using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive.Picker
{
    [DataContract(Name = "BgaPortfolioPickerModel")]
    public class PortfolioModel
    {
        [DebuggerStepThrough]
        public PortfolioModel()
        {
        }


        [DebuggerStepThrough]
        public PortfolioModel(String id, String name)
        {
            this.Id = id;
            this.Name = name;
        }

        [DataMember]
        public String Id { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
