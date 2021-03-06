﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the BU_PORTFOLIO_SECURITY_TARGET_CHANGE table.
    /// Before NULL, after SOMETHING: a record was created.
    /// Before SOMETHING, after SOMETHING ELSE: a record was updated.
    /// Before SOMETHING, after NULL: a record was deleted.
    /// </summary>
    public class BuPortfolioSecurityTargetChangeInfo : IChangeInfoBase
    {
        [DebuggerStepThrough]
        public BuPortfolioSecurityTargetChangeInfo()
        { }

        public BuPortfolioSecurityTargetChangeInfo(
            Int32 id,
            String bottomUpPortfolioId,
            String securityId,
            Decimal? targetBefore,
            Decimal? targetAfter,
            Int32 changeSetId,
            String comment
        )
        {
            this.Id = id;
            this.BottomUpPortfolioId = bottomUpPortfolioId;
            this.SecurityId = securityId;
            this.TargetBefore = targetBefore;
            this.TargetAfter = targetAfter;
            this.ChangesetId = changeSetId;
            this.Comment = comment;
        }
        public Int32 Id { get; set; }
        public String BottomUpPortfolioId { get; set; }
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