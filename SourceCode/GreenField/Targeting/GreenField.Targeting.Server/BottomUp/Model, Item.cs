using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Aims.Data.Server;

namespace GreenField.Targeting.Server.BottomUp
{
    [DataContract(Name = "BuItemModel")]
    public class ItemModel
    {
        [DebuggerStepThrough]
        public ItemModel()
        {
        }

        [DebuggerStepThrough]
        public ItemModel(SecurityModel security, EditableExpressionModel targetExpression)
            : this()
        {
            this.Security = security;
            this.Target = targetExpression;
        }

        [DataMember]
        public SecurityModel Security { get; set; }

        [DataMember]
        public EditableExpressionModel Target { get; set; }

        [DebuggerStepThrough]
        public void Accept(IBuLineModelResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
