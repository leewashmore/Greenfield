using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core = TopDown.Core.ManagingPst;
using Aims.Expressions;
using Picker = TopDown.Core.ManagingPortfolios;

namespace GreenField.Targeting.Server.BottomUp
{
    public class Serializer
    {
        private Server.Serializer serializer;
        private Core.ModelChangeDetector modelChangeDetector;

        [DebuggerStepThrough]
        public Serializer(
            Server.Serializer serializer,
            Core.ModelChangeDetector modelChangeDetector
        )
        {
            this.serializer = serializer;
            this.modelChangeDetector = modelChangeDetector;
        }

        public RootModel SerializeRoot(Core.RootModel model, CalculationTicket ticket)
        {
            var items = this.SerializeItems(model.Items);
            var result = new RootModel(
                model.PortfolioId,
                this.serializer.SerializeChangeset(model.LatestChangeset),
                items,
                this.serializer.SerializeNullableExpression(model.TargetTotal, ticket),
                this.serializer.SerializeNullableExpression(model.Cash, ticket),
                this.modelChangeDetector.HasChanged(model)
            );
            return result;
        }

        protected IEnumerable<ItemModel> SerializeItems(IEnumerable<Core.ItemModel> models)
        {
            var result = models.Select(x => this.SerializeItem(x)).ToList();
            return result;
        }

        protected ItemModel SerializeItem(Core.ItemModel model)
        {
            var result = new ItemModel(
                this.serializer.SerializeSecurityOnceResolved( model.Security),
                this.serializer.SerializeEditableExpression( model.Target )
            );
            return result;
        }

        public PickerModel SerializePicker(IEnumerable<Picker.BottomUpPortfolio> models)
        {
            var result = new PickerModel(
                models.Select(x => this.serializer.SerializeBottomUpPortfolio(x)).ToList()
            );
            return result;
        }
    }
}
