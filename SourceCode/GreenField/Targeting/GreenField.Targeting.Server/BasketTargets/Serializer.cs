using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core = TopDown.Core.ManagingBpst;
using Aims.Expressions;
using TopDown.Core.ManagingBpst;
using Picker = TopDown.Core.Gadgets.BasketPicker;

namespace GreenField.Targeting.Server.BasketTargets
{
    public class Serializer
    {
        private Server.Serializer serializer;
        private ModelChangeDetector modelChangeDetector;
        
        [DebuggerStepThrough]
        public Serializer(Server.Serializer serializer, Core.ModelChangeDetector modelChangeDetector)
        {
            this.serializer = serializer;
            this.modelChangeDetector = modelChangeDetector;
        }

        public RootModel SerializeRoot(Core.RootModel model, CalculationTicket ticket)
        {
            var result = new RootModel(
                this.serializer.SerializeChangeset(model.LatestBaseChangeset),
                this.serializer.SerializeChangeset(model.LatestPortfolioTargetChangeset),
                this.serializer.SerializeTargetingTypeGroup( model.Core.TargetingTypeGroup),
                this.serializer.SerializeBasketOnceResolved(model.Core.Basket),
                this.SerializePortfolios(model.Core.Portfolios, ticket),
                this.SerializeSecurities(model.Core.Securities, ticket),
                this.serializer.SerializeNullableExpression(model.Core.BaseTotal, ticket),
                this.serializer.SerializeExpression(model.Core.BenchmarkTotal, ticket),
                this.serializer.SerializeNullableExpression(model.Core.BaseActiveTotal, ticket),
                this.modelChangeDetector.HasChanged(model),
                model.BenchmarkDate
            );
            return result;
        }

        protected IEnumerable<SecurityModel> SerializeSecurities(IEnumerable<Core.SecurityModel> models, CalculationTicket ticket)
        {
            var result = models.Select(x => this.SerializeSecurity(x, ticket)).ToList();
            return result;
        }

        protected SecurityModel SerializeSecurity(Core.SecurityModel model, CalculationTicket ticket)
        {
            var result = new SecurityModel(
                this.serializer.SerializeSecurityOnceResolved(model.Security),
                this.serializer.SerializeEditableExpression(model.Base),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.SerializePortfolioTargets(model.PortfolioTargets, model.Base),
                this.serializer.SerializeNullableExpression(model.BaseActive, ticket)
            );
            return result;
        }

        protected IEnumerable<PortfolioTargetModel> SerializePortfolioTargets(IEnumerable<Core.PortfolioTargetModel> models, EditableExpression baseExpression)
        {
            var result = models.Select(x => this.SerializePortfolioTarget(baseExpression, x)).ToList();
            return result;
        }

        private PortfolioTargetModel SerializePortfolioTarget(EditableExpression baseExpression, Core.PortfolioTargetModel model)
        {
            var result = new PortfolioTargetModel(
                this.serializer.SerializeBroadGlobalActivePorfolio(model.BroadGlobalActivePortfolio),
                this.serializer.SerializeEditableExpression(model.Target, baseExpression.EditedValue)
            );
            return result;
        }

        protected IEnumerable<PortfolioModel> SerializePortfolios(IEnumerable<Core.PortfolioModel> models, CalculationTicket ticket)
        {
            var result = models.Select(x => this.SerializePortfolio(x, ticket)).ToList();
            return result;
        }

        protected PortfolioModel SerializePortfolio(Core.PortfolioModel model, CalculationTicket ticket)
        {
            var result = new PortfolioModel(
                this.serializer.SerializeBroadGlobalActivePorfolio(model.Portfolio),
                this.serializer.SerializeNullableExpression(model.PortfolioTargetTotal, ticket)
            );
            return result;
        }

        public PickerModel SerializePicker(Picker.RootModel model)
        {
            var models = model.GetGroups();
            var result = new PickerModel(
                this.SerializePickerGroups(models)
            );
            return result;
        }

        protected IEnumerable<PickerTargetingGroupModel> SerializePickerGroups(IEnumerable<Picker.TargetingGroupModel> models)
        {
            var result = models.Select(x => this.SerializePickerGroup(x)).ToList();
            return result;
        }

        protected PickerTargetingGroupModel SerializePickerGroup(Picker.TargetingGroupModel model)
        {
            var baskets = this.SerializeBaskets(model.GetBaskets());
            var result = new PickerTargetingGroupModel(model.TargetingGroupId, model.TargetingGroupName, baskets);
            return result;
        }

        protected IEnumerable<PickerBasketModel> SerializeBaskets(IEnumerable<Picker.BasketModel> models)
        {
            var result = models.Select(x => this.SerializeBasket(x)).ToList();
            return result;
        }

        protected PickerBasketModel SerializeBasket(Picker.BasketModel model)
        {
            var result = new PickerBasketModel(model.Id, model.Name);
            return result;
        }
    }
}
