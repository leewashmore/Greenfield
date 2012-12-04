using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server
{
    [DataContract]
    public class TargetingTypeGroupModel
    {
        [DebuggerStepThrough]
        public TargetingTypeGroupModel()
        {
            this.TargetingTypes = new List<TargetingTypeModel>();
        }

        [DebuggerStepThrough]
        public TargetingTypeGroupModel(Int32 id, String name, String benchmarkIdOpt, IEnumerable<TargetingTypeModel> targetingTypes)
            : this()
        {
            this.Id = id;
            this.Name = name;
            this.BenchmarkIdOpt = benchmarkIdOpt;
            this.TargetingTypes.AddRange(targetingTypes);
        }

        [DataMember]
        public Int32 Id { get; private set; }
        [DataMember]
        public String Name { get; private set; }
        [DataMember]
        public String BenchmarkIdOpt { get; private set; }
        [DataMember]
        public List<TargetingTypeModel> TargetingTypes { get; private set; }
    }
}
