using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.Overlaying
{
    public class ModelBuilder
    {
        private CommonParts commonParts;
        private Decimal? defaultOverlayFactor;
        
        public ModelBuilder(Decimal? defaultOverlayFactor, CommonParts commonParts)
        {
            this.defaultOverlayFactor = defaultOverlayFactor;
            this.commonParts = commonParts;
        }

        /// <summary>
        /// Number that is used in the overlay pannel on the top.
        /// </summary>
        /// <returns></returns>
        public EditableExpression CreateOverlayFactorExpression(String portfolioName)
        {
			var result = commonParts.CreateInputExpression(
				String.Format(ValueNames.OverlayFactorFormat, portfolioName),
				this.defaultOverlayFactor
			);
			return result;
        }

    }
}
