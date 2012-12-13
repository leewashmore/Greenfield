using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Aims.Data.Server;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract(Name = "BgaRootModel")]
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel()
        {
        }

        [DebuggerStepThrough]
        public RootModel(
            TargetingTypeModel targetingType,
            BroadGlobalActivePortfolioModel broadGlobalActiveProtfolio,
            ChangesetModel latestTtbbvChangeset,
            ChangesetModel latestTtbptChangeset,
            ChangesetModel latestPstoChangeset,
            ChangesetModel latestPstChangeset,
            GlobeModel globe,
            CashModel cash,
            DateTime benchmarkDate,
            Boolean isModified
        )
        {
            this.TargetingType = targetingType;
            this.BroadGlobalActiveProtfolio = broadGlobalActiveProtfolio;
            this.LatestTtbbvChangeset = latestTtbbvChangeset;
            this.LatestTtbptChangeset = latestTtbptChangeset;
            this.LatestBgapsfChangeset = latestPstoChangeset;
            this.LatestBupstChangeset = latestPstChangeset;
            this.Globe = globe;
            this.Cash = cash;
            this.IsModified = isModified;
            this.BenchmarkDate = benchmarkDate;
        }

        [DataMember]
        public DateTime BenchmarkDate { get; set; }

        [DataMember]
        public TargetingTypeModel TargetingType { get; set; }

        [DataMember]
        public BroadGlobalActivePortfolioModel BroadGlobalActiveProtfolio { get; set; }

        [DataMember]
        public FactorModel Factors { get; set; }

        [DataMember]
        public ChangesetModel LatestTtbbvChangeset { get; set; }

        [DataMember]
        public ChangesetModel LatestTtbptChangeset { get; set; }

        [DataMember]
        public ChangesetModel LatestBgapsfChangeset { get; set; }

        [DataMember]
        public ChangesetModel LatestBupstChangeset { get; set; }

        [DataMember]
        public GlobeModel Globe { get; set; }

        [DataMember]
        public CashModel Cash { get; set; }

        [DataMember]
        public Boolean IsModified { get; set; }
    }
}
