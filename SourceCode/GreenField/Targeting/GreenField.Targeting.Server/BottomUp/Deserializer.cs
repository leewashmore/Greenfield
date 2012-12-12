using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core = TopDown.Core.ManagingPst;
using TopDown.Core.Persisting;
using Aims.Data.Server;

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


            var targetTotalExpression = this.modelBuilder.CreateTargetTotalExpression(items);
            var cashExpression = this.modelBuilder.CreateCashExpression(targetTotalExpression);

            var result = new Core.RootModel(
                model.BottomUpPortfolioId,
                this.DeserializeBuPortfolioSecurityTargetChangesetInfo(model.ChangesetModel),
                items,
                targetTotalExpression,
                cashExpression
            );
            return result;
        }

        protected Core.ItemModel DeserializeAdditionalItem(SecurityModel securityModel)
        {
            var security = this.deserializer.DeserializeSecurity(securityModel);
            var targetExpression = this.modelBuilder.CreateTargetExpression();
            var result = this.modelBuilder.CreateItem(security, targetExpression);
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
            var security = this.deserializer.DeserializeSecurity(model.Security);
            var targetExpression = this.modelBuilder.CreateTargetExpression();
            this.deserializer.PopulateEditableExpression(targetExpression, model.Target);
            var result = this.modelBuilder.CreateItem(security, targetExpression);
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
