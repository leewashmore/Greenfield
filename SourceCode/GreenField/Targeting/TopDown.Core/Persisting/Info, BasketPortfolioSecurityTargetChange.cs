using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class BasketPortfolioSecurityTargetChangeInfo : IChangeInfoBase
    {
        [DebuggerStepThrough]
        public BasketPortfolioSecurityTargetChangeInfo()
        {
        }

        [DebuggerStepThrough]
        public BasketPortfolioSecurityTargetChangeInfo(
            Int32 id,
            Int32 basketId,
            String portfolioId,
            String securityId,
            Decimal? targetBefore,
            Decimal? targetAfter,
            Int32 changesetId,
            String comment
        )
        {
            this.Id = id;
            this.BasketId = basketId;
            this.PortfolioId = portfolioId;
            this.SecurityId = securityId;
            this.TargetBefore = targetBefore;
            this.TargetAfter = targetAfter;
            this.ChangesetId = changesetId;
            this.Comment = comment;
        }


        public Int32 Id { get; set; }
        public Int32 BasketId { get; set; }
        public String PortfolioId { get; set; }
        public String SecurityId { get; set; }
        public Decimal? TargetBefore { get; set; }
        public Decimal? TargetAfter { get; set; }
        public Int32 ChangesetId { get; set; }
        public String Comment { get; set; }


        String IChangeInfoBase.Comment
        {
            get { return this.Comment; }
        }

        Decimal? IChangeInfoBase.Before
        {
            get { return this.TargetBefore; }
        }

        Decimal? IChangeInfoBase.After
        {
            get { return this.TargetAfter; }
        }
    }
}
