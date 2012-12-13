using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Aims.Data.Server;

namespace GreenField.Targeting.Server.BottomUp
{
    [DataContract(Name = "BuPickerModel")]
    public class PickerModel
    {
        [DebuggerStepThrough]
        public PickerModel()
        {
            this.BottomUpPortfolios = new List<BottomUpPortfolioModel>();
        }

        [DebuggerStepThrough]
        public PickerModel(IEnumerable<BottomUpPortfolioModel> portfolios)
            : this()
        {
            this.BottomUpPortfolios.AddRange(portfolios);
        }

        [DataMember]
        public List<BottomUpPortfolioModel> BottomUpPortfolios { get; set; }
    }
}
