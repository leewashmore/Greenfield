using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BasketTargets
{
    [DataContract(Name = "BtPickerModel")]
    public class PickerModel
    {
        [DebuggerStepThrough]
        public PickerModel()
        {
            this.TargetingGroups = new List<PickerTargetingGroupModel>();
        }

        [DebuggerStepThrough]
        public PickerModel(IEnumerable<PickerTargetingGroupModel> targetingGroups)
            : this()
        {
            this.TargetingGroups.AddRange(targetingGroups);
        }

        [DataMember]
        public List<PickerTargetingGroupModel> TargetingGroups { get; set; }
    }
}
