using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BottomUp
{
    [DataContract(Name = "BuRootModel")]
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel()
        {
            this.Items = new List<ItemModel>();
        }

        [DebuggerStepThrough]
        public RootModel(
            String bottomUpPortfolioId,
            ChangesetModel changesetModel,
            IEnumerable<ItemModel> items,
            NullableExpressionModel nullableExpressionModel,
            Boolean isModified
        )
            : this()
        {
            this.BottomUpPortfolioId = bottomUpPortfolioId;
            this.ChangesetModel = changesetModel;
            this.NullableExpressionModel = nullableExpressionModel;
            this.Items.AddRange(items);
            this.IsModified = isModified;
        }

        [DataMember]
        public String BottomUpPortfolioId { get; set; }
        [DataMember]
        public ChangesetModel ChangesetModel { get; set; }
        [DataMember]
        public List<ItemModel> Items { get; set; }
        [DataMember]
        public NullableExpressionModel NullableExpressionModel { get; set; }
        [DataMember]
        public Boolean IsModified { get; set; }
        [DataMember]
        public Server.SecurityModel SecurityToBeAddedOpt { get; set; }
    }
}
