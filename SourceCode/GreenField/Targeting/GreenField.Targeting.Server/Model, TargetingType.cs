using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class TargetingTypeModel
    {
        [DebuggerStepThrough]
        public TargetingTypeModel()
        {
        }

        [DebuggerStepThrough]
        public TargetingTypeModel(Int32 id, String name)
        {
            this.Id = id;
            this.Name = name;
        }

        [DataMember]
        public Int32 Id { get; set; }

        [DataMember]
        public String Name { get; set; }
    }
}
