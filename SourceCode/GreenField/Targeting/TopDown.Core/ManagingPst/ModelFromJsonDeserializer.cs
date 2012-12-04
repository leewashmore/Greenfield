using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using TopDown.Core.ManagingSecurities;

namespace TopDown.Core.ManagingPst
{
    public class ModelFromJsonDeserializer
    {
        private ExpressionFromJsonDeserializer expressionDeserializer;
        private ModelBuilder modelBuilder;

        [DebuggerStepThrough]
        public ModelFromJsonDeserializer(
            ModelBuilder modelBuilder,
            ExpressionFromJsonDeserializer expressionDeserializer
        )
        {
            this.modelBuilder = modelBuilder;
            this.expressionDeserializer = expressionDeserializer;
        }

        public RootModel DeserializeRoot(JsonReader reader, SecurityRepository securityRepository)
        {
            return reader.Read(delegate
            {
                var protfolioId = reader.ReadAsString(JsonNames.PortfolioId);

                var latestChangeset = reader.Read(JsonNames.LatestChangeset, delegate
                {
                    return this.ReadLastChangeset(reader);
                });


                var items = reader.ReadArray(JsonNames.Items, delegate
                {
                    return this.ReadItem(reader, securityRepository);
                });

                var targetTotalExpression = this.modelBuilder.CreateTargetTotalExpression(items);

                var result = new RootModel(
                    protfolioId,
                    latestChangeset,
                    items,
                    targetTotalExpression
                );

                return result;
            });
        }

        public BuPortfolioSecurityTargetChangesetInfo ReadLastChangeset(JsonReader reader)
        {
            var result = new BuPortfolioSecurityTargetChangesetInfo
            {
                Id = reader.ReadAsInt32(JsonNames.Id),
                Username = reader.ReadAsString(JsonNames.Username),
                Timestamp = reader.ReadAsDatetime(JsonNames.Timestamp),
            };
            return result;
        }

        public ItemModel ReadItem(JsonReader reader, SecurityRepository securityRepository)
        {
            var securityId = reader.ReadAsString(JsonNames.SecurityId);
			var security = securityRepository.GetSecurity(securityId);

            var targetExpression = this.modelBuilder.CreateTargetExpression();
            reader.Read(JsonNames.Target, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, targetExpression);
            });

            var result = new ItemModel(security, targetExpression);
            return result;
        }
    }
}
