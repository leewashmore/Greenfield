using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core = TopDown.Core.ManagingPst;
using TopDown.Core.Persisting;

namespace GreenField.Targeting.Server.BottomUp
{
    public class Deserializer
    {
        private Server.Deserializer deserializer;
        private Core.ModelBuilder modelBuilder;

        [DebuggerStepThrough]
        public Deserializer(
            Server.Deserializer deserializer,
            Core.ModelBuilder modelBuilder
        )
        {
            this.deserializer = deserializer;
            this.modelBuilder = modelBuilder;
        }

        public Core.RootModel DeserializerRoot(RootModel model)
        {
            var items = new List<Core.ItemModel>();
            this.PopulateItems(model.Items, items);
            
            if (model.SecurityToBeAddedOpt != null)
            {
                var item = this.DeserializeAdditionalItem(model.SecurityToBeAddedOpt);
                items.Insert(0, item);
            }

            var result = new Core.RootModel(
                model.BottomUpPortfolioId,
                this.DeserializeBuPortfolioSecurityTargetChangesetInfo(model.ChangesetModel),
                items,
                this.modelBuilder.CreateTargetTotalExpression(items)
            );
            return result;
        }

        protected Core.ItemModel DeserializeAdditionalItem(SecurityModel securityModel)
        {
            var security = this.deserializer.DeserializeSecurity(securityModel);
            var targetExpression = this.modelBuilder.CreateTargetExpression();
            var result = new Core.ItemModel(
                security,
                targetExpression
            );
            return result;
        }

        protected void PopulateItems(IEnumerable<ItemModel> models, ICollection<Core.ItemModel> result)
        {
            foreach (var model in models)
            {
                var deserializedModel = this.DeserializeItem(model);
                result.Add(deserializedModel);
            }
        }

        

        protected Core.ItemModel DeserializeItem(ItemModel model)
        {
            var totalExpression = this.modelBuilder.CreateTargetExpression();
            this.deserializer.PopulateEditableExpression(totalExpression, model.Target);
            var result = new Core.ItemModel(
                this.deserializer.DeserializeSecurity(model.Security),
                totalExpression
            );
            return result;
        }

        protected BuPortfolioSecurityTargetChangesetInfo DeserializeBuPortfolioSecurityTargetChangesetInfo(ChangesetModel model)
        {
            var result = new BuPortfolioSecurityTargetChangesetInfo(
                model.Id,
                model.Username,
                model.Timestamp,
                model.CalculationId
            );
            return result;
        }
    }
}
