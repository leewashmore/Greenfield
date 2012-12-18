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

        public IExpression<Decimal> CreateBenchmarkTotalExpression(List<SecurityModel> securities)
        {
            return new SumExpression(
                ValueNames.BenchmarkTotal,
                securities.Select(x => x.Benchmark),
                this.defaultValues.DefaultBenchmark,
                this.commonParts.ValidateWhatever
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




        public Expression<Decimal?> CreateBaseActiveExpression(EditableExpression baseExpression, UnchangableExpression<decimal> benchmarkExpression)
        {
            return new Expression<decimal?>(ValueNames.BaseActive, new BaseActiveFormula(baseExpression, benchmarkExpression), this.commonParts.NullableDecimalValueAdapter, this.commonParts.ValidateWhatever);
        }

        private class BaseActiveFormula : IFormula<Decimal?>
        {
            public BaseActiveFormula(EditableExpression baseExpression, UnchangableExpression<Decimal> benchmarkExpression)
            {
                this.BaseExpression = baseExpression;
                this.BenchmarkExpression = benchmarkExpression;
            }

            public Decimal? Calculate(CalculationTicket ticket)
            {
                var result = this.Calculate(ticket, No.CalculationTracer);
                return result;
            }

            public UnchangableExpression<Decimal> BenchmarkExpression { get; set; }
            public EditableExpression BaseExpression { get; set; }


            public Decimal? Calculate(CalculationTicket ticket, ICalculationTracer tracer)
            {
                tracer.WriteLine("Base active formula");
                tracer.Indent();
                var baseValue = BaseExpression.Value(ticket, tracer, "Base");
                var benchmarkValue = BenchmarkExpression.Value(ticket, tracer, "Benchmark");
                var result = baseValue.HasValue ? baseValue - benchmarkValue : null;
                tracer.WriteValue("Base - Benchmark", result);
                tracer.Unindent();
                return result;
            }
        }

        public IExpression<Decimal?> CreateBaseActiveTotalExpression(List<SecurityModel> securityModels)
        {
            var baseActiveTotalExpression = new NullableSumExpression(ValueNames.BaseActiveTotal, securityModels.Select(s => s.BaseActive), this.defaultValues.DefaultBaseActive, this.commonParts.ValidateWhatever);
            return baseActiveTotalExpression;
        }
    }
}
