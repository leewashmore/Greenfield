using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BasketTargets
{
    [DataContract(Name = "BtPickerBasketModel")]
    public class PickerBasketModel
    {
        [DebuggerStepThrough]
        public PickerBasketModel()
        {

        }

        [DebuggerStepThrough]
        public PickerBasketModel(Int32 id, String name) : this()
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
