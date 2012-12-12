using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingPortfolios;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingBpst
{
    public class ModelBuilder
    {
        private DefaultValues defaultValues;
        private CommonParts commonParts;

        [DebuggerStepThrough]
        public ModelBuilder(DefaultValues defaultValues, CommonParts commonParts)
        {
            this.defaultValues = defaultValues;
            this.commonParts = commonParts;
        }

        public EditableExpression CreatePortfolioTargetExpression(String porfolioName)
        {
            var result = new EditableExpression(
				String.Format(ValueNames.PortfolioTargetFormat, porfolioName),
                this.defaultValues.DefaultPortfolioTarget,
                commonParts.NullableDecimalValueAdapter,
                this.ValidatePortfolioTargetExpression
            );
            return result;
        }

        public IEnumerable<IValidationIssue> ValidatePortfolioTargetExpression(EditableExpression expression)
        {
            var issues = new List<IValidationIssue>();
            issues.AddRange(this.commonParts.ValidateComment(expression));
            issues.AddRange(this.commonParts.ValidateNonNegativeOrNull(expression));
            return issues;
        }

        public EditableExpression CreateBaseExpression()
        {
            var result = new EditableExpression(
				ValueNames.Base,
                this.defaultValues.DefaultBase,
                this.commonParts.NullableDecimalValueAdapter,
                this.ValidateBaseExpression
            );
            return result;
        }

        public IEnumerable<IValidationIssue> ValidateBaseExpression(EditableExpression expression)
        {
            var issues = new List<IValidationIssue>();
            issues.AddRange(this.commonParts.ValidateComment(expression));
            issues.AddRange(this.commonParts.ValidateNonNegativeOrNull(expression));
            return issues;
        }

        public UnchangableExpression<Decimal> CreateBenchmarkExpression()
        {
            return new UnchangableExpression<Decimal>(
				ValueNames.Benchmark,
                this.defaultValues.DefaultBenchmark,
                this.commonParts.DecimalValueAdapter,
				this.commonParts.ValidateWhatever
            );
        }

        public IExpression<Decimal?> CreateBaseTotalExpression(IEnumerable<SecurityModel> securities)
        {
            return new NullableSumExpression(
				ValueNames.BaseTotal,
                securities.Select(x => x.Base),
                this.defaultValues.DefaultBase,
                this.commonParts.ValidateEitheNullOr100
            );
        }

        public IExpression<Decimal?> CreatePortfolioTargetTotalExpression(
			BroadGlobalActivePortfolio portfolio,
			IEnumerable<SecurityModel> securities
		)
        {
            var expressions = securities.SelectMany(security =>
            {
                return security
                    .PortfolioTargets
                    .Where(x => x.BroadGlobalActivePortfolio.Id == portfolio.Id)
                    .Select(portfolioModel =>
                    {
                        IExpression<Decimal?> result;
						var targetValue = portfolioModel.Target;
                        if (targetValue.EditedValue.HasValue)
                        {
                            result = targetValue;
                        }
                        else
                        {
                            var baseValue = security.Base;
                            if (baseValue.EditedValue.HasValue)
                            {
                                result = baseValue;
                            }
                            else
                            {
                                result = null;
                            }
                        }
                        return result;
                    });
            });

            return new NullableSumExpression(
				String.Format(ValueNames.PortfolioTargetTotalFormat, portfolio.Name),
                expressions,
                this.defaultValues.DefaultPortfolioTarget,
                this.commonParts.ValidateEitheNullOr100
            );
        }
    }
}
