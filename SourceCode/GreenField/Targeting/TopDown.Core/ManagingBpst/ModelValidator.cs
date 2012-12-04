using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst
{
    public class ModelValidator
    {
        public IEnumerable<IValidationIssue> ValidateRoot(RootModel model, CalculationTicket ticket)
        {
			var issues = this.ValidateCore(model.Core, ticket);
			return issues;
        }

		public IEnumerable<IValidationIssue> ValidateCore(CoreModel model, CalculationTicket ticket)
		{
			var issues = new List<IValidationIssue>();
			issues.AddRange(model.BaseTotal.Validate(ticket));
			model.Portfolios.ForEach(x => this.Validate(x, ticket, issues));
			model.Securities.ForEach(x => this.Validate(x, issues));
			return issues;
		}

        protected void Validate(PortfolioModel portfolio, CalculationTicket ticket, List<IValidationIssue> issues)
        {
            issues.AddRange(portfolio.PortfolioTargetTotal.Validate(ticket));
        }

        protected void Validate(SecurityModel security, List<IValidationIssue> issues)
        {
            issues.AddRange(security.Base.Validate());
            security.PortfolioTargets.ForEach(portfolioTarget => this.Validate(portfolioTarget, issues));
        }

        protected void Validate(PortfolioTargetModel portfolioTarget, List<IValidationIssue> issues)
        {
            issues.AddRange(portfolioTarget.Target.Validate());
        }
    }
}
