using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BasketTargets
{
    [DataContract(Name = "BtPickerTargetingGroupModel")]
    public class PickerTargetingGroupModel
    {
        [DebuggerStepThrough]
        public PickerTargetingGroupModel()
        {
            this.Baskets = new List<PickerBasketModel>();
        }

        [DebuggerStepThrough]
        public PickerTargetingGroupModel(Int32 targetingTypeGroupId, String targetingTypeGroupName, IEnumerable<PickerBasketModel> baskets)
            : this()
        {
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.TargetingTypeGroupName = targetingTypeGroupName;
            this.Baskets.AddRange(baskets);
        }

        [DataMember]
        public Int32 TargetingTypeGroupId { get; set; }
        
        [DataMember]
        public String TargetingTypeGroupName { get; set; }

        [DataMember]
        public List<PickerBasketModel> Baskets { get; set; }
    }
}
