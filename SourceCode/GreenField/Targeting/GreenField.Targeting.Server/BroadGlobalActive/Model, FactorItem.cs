using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract(Name = "BgaFactorItemModel")]
    public class FactorItemModel
    {
        [DebuggerStepThrough]
        public FactorItemModel()
        {
        }

        [DebuggerStepThrough]
        public FactorItemModel(BottomUpPortfolioModel bottomUpPortfolio, EditableExpressionModel editableExpression)
        {
            this.BottomUpPortfolio = bottomUpPortfolio;
            this.OverlayFactor = editableExpression;
        }

        [DataMember]
        public BottomUpPortfolioModel BottomUpPortfolio { get; set; }

        [DataMember]
        public EditableExpressionModel OverlayFactor { get; set; }
    }
}
