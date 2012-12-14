using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	/// <summary>
	/// Represents a record of the BGA_PORTFOLIO_SECURITY_FACTOR_CHANGE table.
	/// </summary>
    public class BgaPortfolioSecurityFactorChangeInfo : IChangeInfoBase
    {
        [DebuggerStepThrough]
        public BgaPortfolioSecurityFactorChangeInfo()
        {
        }

        [DebuggerStepThrough]
        public BgaPortfolioSecurityFactorChangeInfo(
			Int32 id,
			String broadGlobalActivePortfolioId,
			String securityId,
			Decimal? factorBefore,
			Decimal? factorAfter,
			String comment,
			Int32 changesetId
		)
        {
            this.Id = id;
            this.BroadGlobalActivePortfolioId = broadGlobalActivePortfolioId;
            this.SecurityId = securityId;
            this.FactorBefore = factorBefore;
            this.FactorAfter = factorAfter;
			this.Comment = comment;
            this.ChangesetId = changesetId;
        }

		/// <summary>
		/// ID
		/// </summary>
		public Int32 Id { get; private set; }

		/// <summary>
		/// PORTFOLIO_ID
		/// </summary>
		public String BroadGlobalActivePortfolioId { get; private set; }

		/// <summary>
		/// SECURITY_ID
		/// </summary>
		public String SecurityId { get; private set; }
		
		/// <summary>
		/// FACTOR_BEFORE
		/// </summary>
		public Decimal? FactorBefore { get; private set; }
		
		/// <summary>
		/// FACTOR_AFTER
		/// </summary>
		public Decimal? FactorAfter { get; private set; }
		
		/// <summary>
		/// COMMENT
		/// </summary>
		public String Comment { get; private set; }

		/// <summary>
		/// CHANGESET_ID
		/// </summary>
		public Int32 ChangesetId { get; private set; }


        String IChangeInfoBase.Comment
        {
            get { return this.Comment; }
        }

        Decimal? IChangeInfoBase.Before
        {
            get { return this.FactorBefore; }
        }

        Decimal? IChangeInfoBase.After
        {
            get { return this.FactorAfter; }
        }
    }
}
