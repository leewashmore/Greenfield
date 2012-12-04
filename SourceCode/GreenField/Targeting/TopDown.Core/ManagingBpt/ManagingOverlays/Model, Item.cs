using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingPortfolios;
using Aims.Expressions;

namespace TopDown.Core.Overlaying
{
    public class ItemModel
    {
        [DebuggerStepThrough]
        public ItemModel(
			BottomUpPortfolio bottomUpPortfolio,
			EditableExpression overlayFactor
		)
        {
			this.BottomUpPortfolio = bottomUpPortfolio;
            this.OverlayFactor = overlayFactor;
        }

		public BottomUpPortfolio BottomUpPortfolio { get; private set; }
		public EditableExpression OverlayFactor { get; private set; }
	}
}
