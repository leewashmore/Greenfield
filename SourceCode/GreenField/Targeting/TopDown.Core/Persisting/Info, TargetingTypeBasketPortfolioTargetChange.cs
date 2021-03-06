﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	/// <summary>
	/// Represents a record of the TARGETING_TYPE_BASKET_PORTFOLIO_TARGET_CHANGE table.
	/// </summary>
    public class TargetingTypeBasketPortfolioTargetChangeInfo : IChangeInfoBase
    {
        [DebuggerStepThrough]
        public TargetingTypeBasketPortfolioTargetChangeInfo()
        {
        }

        [DebuggerStepThrough]
        public TargetingTypeBasketPortfolioTargetChangeInfo(
			Int32 id,
			Int32 targetingTypeId,
			Int32 basketId,
			String portfolioId,
			Decimal? target_Before,
			Decimal? target_After,
			String comment,
			Int32 changesetId
		)
        {
            this.Id = id;
            this.TargetingTypeId = targetingTypeId;
            this.BasketId = basketId;
            this.PortfolioId = portfolioId;
            this.TargetBefore = target_Before;
            this.TargetAfter = target_After;
			this.Comment = comment;
            this.ChangesetId = changesetId;
        }

		/// <summary>
		/// ID column.
		/// </summary>
        public Int32 Id { get; set; }

		/// <summary>
		/// TARGETING_TYPE_ID column.
		/// </summary>
        public Int32 TargetingTypeId { get; set; }

		/// <summary>
		/// BASKET_ID column.
		/// </summary>
        public Int32 BasketId { get; set; }

		/// <summary>
		/// PORTFOLIO_ID column.
		/// </summary>
        public String PortfolioId { get; set; }

		/// <summary>
		/// TARGET_BEFORE column.
		/// </summary>
        public Decimal? TargetBefore { get; set; }

		/// <summary>
		/// TARGET_AFTER column.
		/// </summary>
        public Decimal? TargetAfter { get; set; }

		/// <summary>
		/// CHANGESET_ID column.
		/// </summary>
        public Int32 ChangesetId { get; set; }

		/// <summary>
		/// COMMENT column.
		/// </summary>
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
