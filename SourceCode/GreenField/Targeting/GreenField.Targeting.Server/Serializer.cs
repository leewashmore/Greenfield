using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;
using Core = TopDown.Core;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBaskets;
using Aims.Core;
using Aims.Data.Server;

namespace GreenField.Targeting.Server
{
    public class Serializer
    {
        public BottomUpPortfolioModel SerializeBottomUpPortfolio(BottomUpPortfolio portfolio)
        {
            var result = new BottomUpPortfolioModel(
                portfolio.Id,
                portfolio.Name,
                this.SerializeFund(portfolio.Fund)
            );
            return result;
        }

        public IEnumerable<SecurityModel> SerializeSecurities(IEnumerable<ISecurity> securities)
        {
            var result = securities.Select(x => this.SerializeSecurityOnceResolved(x)).ToArray();
            return result;
        }

        public SecurityModel SerializeSecurityOnceResolved(ISecurity security)
        {
            var resolver = new SerializeSecurityOnceResolved_ISecurityResolver(this);
            security.Accept(resolver);
            return resolver.Result;
        }

        private class SerializeSecurityOnceResolved_ISecurityResolver : ISecurityResolver
        {
            private Serializer serializer;

            public SerializeSecurityOnceResolved_ISecurityResolver(Serializer serializer)
            {
                this.serializer = serializer;
            }

            public SecurityModel Result { get; private set; }

            public void Resolve(CompanySecurity stock)
            {
                this.Result = this.serializer.SerializeCompanySecurity(stock);
            }

            public void Resolve(Fund fund)
            {
                this.Result = this.serializer.SerializeFund(fund);
            }
        }


        private FundModel SerializeFund(Fund fund)
        {
            if (fund != null)
            {
                var result = new FundModel(
                    fund.Id,
                    fund.Name,
                    fund.ShortName,
                    fund.Ticker,
                    fund.IssuerId,
                    fund.SecurityType
                );
                return result;
            }
            else
                return null;
        }

        public CompanySecurityModel SerializeCompanySecurity(CompanySecurity security)
        {
            var result = new CompanySecurityModel(
                security.Id,
                security.Name,
                security.ShortName,
                security.Ticker,
                this.SerializeCountry(security.Country),
                security.IssuerId,
                security.SecurityType
            );
            return result;
        }

        public CountryModel SerializeCountry(Country country)
        {
            var result = new CountryModel(
                country.IsoCode,
                country.Name
            );
            return result;
        }

        public EditableExpressionModel SerializeEditableExpression(Aims.Expressions.EditableExpression expression)
        {
            var result = new EditableExpressionModel(
                expression.DefaultValue,
                expression.InitialValue,
                expression.EditedValue,
                expression.Comment,
                this.SerializeValidationIssues(expression.Validate()),
                expression.LastOneModified
            );
            return result;
        }

        public EditableExpressionModel SerializeEditableExpression(Aims.Expressions.EditableExpression expression, Decimal? displayValue)
        {
            var result = this.SerializeEditableExpression(expression);
            result.DisplayValue = displayValue;
            return result;
        }
		
		public NullableExpressionModel SerializeNullableExpression(IExpression<Decimal?> expression, CalculationTicket ticket)
		{
			var value = expression.Value(ticket);
			var issues = this.SerializeValidationIssues(expression.Validate(ticket));
			var result = new NullableExpressionModel(value, issues);
			return result;
		}

		public ExpressionModel SerializeExpression(IExpression<Decimal> expression, CalculationTicket ticket)
		{
			var value = expression.Value(ticket);
			var issues = this.SerializeValidationIssues(expression.Validate(ticket));
			var result = new ExpressionModel(value, issues);
			return result;
		}

        public IEnumerable<IssueModel> SerializeValidationIssues(IEnumerable<Aims.Expressions.IValidationIssue> issues)
        {
            var result = issues.Select(issue => this.SerializeValidationIssueOnceResolved(issue)).ToArray();
            return result;
        }

        public IssueModel SerializeValidationIssueOnceResolved(Aims.Expressions.IValidationIssue issue)
        {
            var resolver = new SerializeValidationIssueOnceResolved_IValidationIssueResolver(this);
            issue.Accept(resolver);
            return resolver.Result;

        }

        private class SerializeValidationIssueOnceResolved_IValidationIssueResolver : Aims.Expressions.IValidationIssueResolver
        {
            private Serializer serializer;

            public SerializeValidationIssueOnceResolved_IValidationIssueResolver(Serializer serializer)
            {
                this.serializer = serializer;
            }

            public IssueModel Result { get; private set; }

            public void Resolve(Aims.Expressions.ErrorIssue issue)
            {
                this.Result = this.serializer.SerializeErrorIssue(issue);
            }

            public void Resolve(Aims.Expressions.CompoundValidationIssue issue)
            {
                this.Result = this.serializer.SerializeComoundValidationIssue(issue);
            }

            public void Resolve(WariningIssue issue)
            {
                this.Result = this.serializer.SerializeWarningIssue(issue);
            }
        }

        public ErrorModel SerializeErrorIssue(Aims.Expressions.ErrorIssue issue)
        {
            var result = new ErrorModel(issue.Message);
            return result;
        }

        public WarningModel SerializeWarningIssue(Aims.Expressions.WariningIssue issue)
        {
            var result = new WarningModel(issue.Message);
            return result;
        }

        public CompoundIssueModel SerializeComoundValidationIssue(Aims.Expressions.CompoundValidationIssue issue)
        {
            var result = new CompoundIssueModel(issue.Message, this.SerializeValidationIssues(issue.Issues));
            return result;
        }

        public BroadGlobalActivePortfolioModel SerializeBroadGlobalActivePorfolio(BroadGlobalActivePortfolio broadGlobalActivePortfolio)
        {
            var result = new BroadGlobalActivePortfolioModel(
                broadGlobalActivePortfolio.Id,
                broadGlobalActivePortfolio.Name
            );
            return result;
        }

        public TargetingTypeModel SerializeTargetingType(Core.ManagingTargetingTypes.TargetingType targetingType)
        {
            var result = new TargetingTypeModel(
                targetingType.Id,
                targetingType.Name
            );
            return result;
        }

        public CountryBasketModel SerializeCountryBasket(Core.ManagingBaskets.CountryBasket model)
        {
            var result = new CountryBasketModel(
                model.Id,
                this.SerializeCountry(model.Country)
            );
            return result;
        }

        public RegionBasketModel SerializeRegionBasket(Core.ManagingBaskets.RegionBasket model)
        {
            var result = new RegionBasketModel(
                model.Id,
                model.Name,
                this.SerializeCountries(model.Countries)
            );
            return result;
        }

        public IEnumerable<CountryModel> SerializeCountries(IEnumerable<Country> models)
        {
            var result = models.Select(x => this.SerializeCountry(x)).ToArray();
            return result;
        }

        public ChangesetModel SerializeChangeset(Core.Persisting.ChangesetInfoBase changesetInfo)
        {
            var result = new ChangesetModel(
                changesetInfo.Id,
                changesetInfo.Username,
                changesetInfo.Timestamp,
                changesetInfo.CalculationId
            );
            return result;
        }

        public TargetingTypeGroupModel SerializeTargetingTypeGroup(TargetingTypeGroup model)
        {
            var result = new TargetingTypeGroupModel(
                model.Id,
                model.Name,
                model.BenchmarkIdOpt,
                this.SerializeTargetingTypes(model.GetTargetingTypes())
            );
            return result;
        }

        public IEnumerable<TargetingTypeModel> SerializeTargetingTypes(IEnumerable<TargetingType> models)
        {
            var result = models.Select(x => this.SerializeTargetingType(x)).ToList();
            return result;
        }

        public BasketModel SerializeBasketOnceResolved(IBasket model)
        {
            var resolver = new SerializeBasketOnceResolved_IBasketResolver(this);
            model.Accept(resolver);
            return resolver.Result;
        }

        private class SerializeBasketOnceResolved_IBasketResolver : IBasketResolver
        {
            private Serializer serializer;

            public SerializeBasketOnceResolved_IBasketResolver(Serializer serializer)
            {
                this.serializer = serializer;
            }

            public BasketModel Result { get; private set; }

            public void Resolve(CountryBasket basket)
            {
                this.Result = this.serializer.SerializeCountryBasket(basket);
            }

            public void Resolve(RegionBasket basket)
            {
                this.Result = this.serializer.SerializeRegionBasket(basket);
            }
        }


        public IEnumerable<CommentModel> SerializeComments(IEnumerable<Core.ManagingComments.CommentModel> comments)
        {
            var result = comments.Select(x => new CommentModel(
                x.ChangeInfo.Comment,
                x.ChangeInfo.Before,
                x.ChangeInfo.After,
                x.ChangesetInfo.Username,
                x.ChangesetInfo.Timestamp
            )).ToList();
            return result;
        }
    }
}