using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    public class ModelValidator : ValidatorBase
    {
        public IEnumerable<IValidationIssue> ValidateRoot(RootModel root, CalculationTicket ticket)
        {
            var result = new List<IValidationIssue>();
            this.ValidateRoot(root, ticket, result);
            return result;
        }

        protected void ValidateRoot(RootModel model, CalculationTicket ticket, List<IValidationIssue> result)
        {
            result.AddRange(model.TargetTotal.Validate(ticket));
            foreach (var item in model.Items)
            {
                result.AddRange(this.ValidateScope(item.Security.Name, item.Target.Validate()));
            }
        }
    }
}
