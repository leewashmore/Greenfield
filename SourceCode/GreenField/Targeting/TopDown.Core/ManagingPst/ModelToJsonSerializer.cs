using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using TopDown.Core.ManagingSecurities;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
	public class ModelToJsonSerializer
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

        public void SerializeRoot(JsonWriter writer, RootModel root, CalculationTicket ticket)
		{
            writer.Write(root.PortfolioId, JsonNames.PortfolioId);
			writer.Write(JsonNames.LatestChangeset, delegate
			{
				this.Write(writer, root.LatestChangeset);
			});
            writer.WriteArray(root.Items, JsonNames.Items, item =>
            {
                writer.Write(delegate
                {
                    this.Write(writer, item);
                });
            });
            this.expressionSerializer.Write(root.TargetTotal, JsonNames.TargetTotal, writer, ticket);
		}

		public void Write(JsonWriter writer, ItemModel item)
		{
			writer.Write(JsonNames.Security, delegate
			{
				this.securitySerializer.SerializeSecurityOnceResolved(item.Security, writer);
			});
            this.expressionSerializer.SerializeEditable(item.Target, JsonNames.Target, writer);
		}

		public void Write(JsonWriter writer, BuPortfolioSecurityTargetChangesetInfo changesetInfo)
		{
            writer.Write(changesetInfo.Id, JsonNames.Id);
            writer.Write(changesetInfo.Username, JsonNames.Username);
			writer.Write(changesetInfo.Timestamp, JsonNames.Timestamp);
		}
	}
}
