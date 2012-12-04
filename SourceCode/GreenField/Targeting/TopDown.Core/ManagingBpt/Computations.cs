using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    /// <summary>
    /// Carries common values required in computations.
    /// </summary>
    public class Computations
    {
        public IExpression<Decimal?> PortfolioAdjustmentTotal { get; set; }
        public IExpression<Decimal?> BaseWherePortfoioAdjustmentSetTotal { get; set; }
        public IExpression<Decimal?> BaseLessOverlayPositiveTotal { get; set; }
        public IModelFormula<IModel, Decimal?> BaseLessOverlayFormula { get; set; }
        public IExpression<Decimal?> BaseLessOverlayTotal { get; set; }
        public IModelFormula<IModel, Decimal?> PortfolioScaledFormula { get; set; }
        public IExpression<Decimal?> CashScaled { get; set; }
        public IModelFormula<IModel, Decimal?> BaseActiveFormula { get; set; }
        public IExpression<Decimal?> CashBase { get; set; }
        public IExpression<Decimal?> CashRescaledBase { get; set; }
        public IModelFormula<IModel, Decimal?> TrueExposureFormula { get; set; }
        public IModelFormula<IModel, Decimal?> TrueActiveFormula { get; set; }
    }
}
