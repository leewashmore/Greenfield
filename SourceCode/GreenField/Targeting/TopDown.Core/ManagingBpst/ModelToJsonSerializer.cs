using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst
{
    public class ModelToJsonSerializer : ModelToJsonSerializerBase
    {
        private ExpressionToJsonSerializer expressionSerializer;
        private SecurityToJsonSerializer securitySerializer;

        [DebuggerStepThrough]
        public ModelToJsonSerializer(
            ExpressionToJsonSerializer expressionSerializer,
            SecurityToJsonSerializer securitySerializer
        )
        {
            this.expressionSerializer = expressionSerializer;
            this.securitySerializer = securitySerializer;
        }

        public void SerializeRoot(RootModel root, CalculationTicket ticket, IJsonWriter writer)
        {
            writer.Write(JsonNames.LatestBaseChangeset, delegate
            {
                this.SerializeLatestBaseChangeset(root.LatestBaseChangeset, writer);
            });
            writer.Write(JsonNames.LatestPortfolioTargetChangeset, delegate
            {
                this.SerializeLatestPortfolioTargetChangeset(root.LatestPortfolioTargetChangeset, writer);
            });
            writer.Write(root.Core.TargetingTypeGroup.Id, JsonNames.TargetingTypeGroupId);
            writer.Write(root.Core.Basket.Id, JsonNames.BasketId);
            writer.Write(root.BenchmarkDate, JsonNames.BenchmarkDate);
            writer.WriteArray(root.Core.Securities, JsonNames.Securities, security =>
            {
                writer.Write(delegate
                {
                    this.SerializeItem(security, ticket, writer);
                });
            });
            writer.WriteArray(root.Core.Portfolios, JsonNames.Portfolios, portfolio =>
            {
                writer.Write(delegate
                {
                    this.SerializePortfolio(portfolio, writer, ticket);
                });
            });
            this.expressionSerializer.SerializeOnceResolved(root.Core.BaseTotal, JsonNames.BaseTotal, writer, ticket);
        }

        public void SerializePortfolio(PortfolioModel portfolio, IJsonWriter writer, CalculationTicket ticket)
        {
            writer.Write(portfolio.Portfolio.Id, JsonNames.Id);
            this.expressionSerializer.SerializeOnceResolved(
                portfolio.PortfolioTargetTotal,
                JsonNames.PortfolioTargetTotal,
                writer,
                ticket
            );
        }

        public void SerializeLatestBaseChangeset(
            TargetingTypeGroupBasketSecurityBaseValueChangesetInfo latestChangesetInfo,
            IJsonWriter writer)
        {

            this.SerializeChangeset(latestChangesetInfo, writer);
        }

        public void SerializeLatestPortfolioTargetChangeset(
            BasketPortfolioSecurityTargetChangesetInfo latestChangesetInfo,
            IJsonWriter writer
        )
        {
            this.SerializeChangeset(latestChangesetInfo, writer);
        }

        public void SerializeItem(SecurityModel item, CalculationTicket ticket, IJsonWriter writer)
        {
            writer.Write("security", delegate
            {
                this.securitySerializer.SerializeSecurityOnceResolved(item.Security, writer);
            });
            this.expressionSerializer.SerializeEditable(item.Base, JsonNames.Base, writer);
            this.expressionSerializer.Serialize(item.Benchmark, JsonNames.Benchmark, writer, ticket);
            writer.WriteArray(item.PortfolioTargets, JsonNames.PortfolioTargets, portfolioTarget =>
            {
                writer.Write(delegate
                {
                    this.SerializePortfolioTarget(item.Base, portfolioTarget, writer);
                });
            });
        }

        public void SerializePortfolioTarget(EditableExpression baseExpression, PortfolioTargetModel portfolioTarget, IJsonWriter writer)
        {
            writer.Write(portfolioTarget.BroadGlobalActivePortfolio.Id, JsonNames.PortfolioId);
            this.expressionSerializer.Serialize(portfolioTarget.Target, JsonNames.Target, writer, baseExpression.EditedValue);
        }
    }
}
