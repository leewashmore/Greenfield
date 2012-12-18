using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Aims.Expressions;
using System.Diagnostics;
using Core = TopDown.Core;
using Aims.Data.Server;

namespace GreenField.Targeting.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.PerCall)]
    public class Facade : IFacade
    {
        [Obsolete("Hack!")]
        public const String Username = "bykova";

        private Core.Facade facade;
        private BroadGlobalActive.Serializer bgaSerializer;
        private BroadGlobalActive.Deserializer bgaDeserializer;
        private Boolean shouldDropRepositoriesOnEveryReload;
        private BasketTargets.Serializer btSerializer;
        private BasketTargets.Deserializer btDeserializer;
        private Serializer serializer;
        private BottomUp.Deserializer buDeserializer;
        private BottomUp.Serializer buSerializer;

        [DebuggerStepThrough]
        public Facade(FacadeSettings settings)
            : this(
            settings.Facade,
            settings.Serializer,
            settings.BgaSerializer,
            settings.BgaDeserializer,
            settings.BtSerializer,
            settings.BtDeserializer,
            settings.BuSerializer,
            settings.BuDeserializer,
            settings.ShouldDropRepositoriesOnEveryReload
        )
        {
        }

        [DebuggerStepThrough]
        public Facade(
            Core.Facade facade,
            Serializer serializer,
            BroadGlobalActive.Serializer bgaSerializer,
            BroadGlobalActive.Deserializer bgaDeserializer,
            BasketTargets.Serializer btSerializer,
            BasketTargets.Deserializer btDeserializer,
            BottomUp.Serializer buSerializer,
            BottomUp.Deserializer buDeserializer,
            Boolean shouldDropRepositoriesOnEveryReload
        )
        {
            this.facade = facade;
            this.serializer = serializer;
            this.bgaSerializer = bgaSerializer;
            this.bgaDeserializer = bgaDeserializer;
            this.btSerializer = btSerializer;
            this.btDeserializer = btDeserializer;
            this.buSerializer = buSerializer;
            this.buDeserializer = buDeserializer;
            this.shouldDropRepositoriesOnEveryReload = shouldDropRepositoriesOnEveryReload;
        }

        protected void MakeSureRepositoriesAreDroppedIfNeeded()
        {
            if (this.shouldDropRepositoriesOnEveryReload)
            {
                this.facade.RepositoryManager.DropEverything();
            }
        }

        public BroadGlobalActive.RootModel GetBroadGlobalActive(Int32 targetingTypeId, String bgaPortfolioId)
        {
            this.MakeSureRepositoriesAreDroppedIfNeeded();
            var ticket = new CalculationTicket();
            var model = this.facade.GetBptModel(targetingTypeId, bgaPortfolioId);
            var result = this.bgaSerializer.SerializeRoot(model, ticket);
            return result;
        }

        public IEnumerable<BroadGlobalActive.Picker.TargetingTypeModel> GetTargetingTypePortfolioPicker()
        {
            this.MakeSureRepositoriesAreDroppedIfNeeded();
            var model = this.facade.GetTargetingTypePortfolioPickerModel();
            var result = this.bgaSerializer.SerializePicker(model);
            return result;
        }

        public BroadGlobalActive.RootModel RecalculateBroadGlobalActive(BroadGlobalActive.RootModel serializedModel)
        {
            this.MakeSureRepositoriesAreDroppedIfNeeded();
            var ticket = new CalculationTicket();
            var model = this.bgaDeserializer.DeserializeRoot(serializedModel);
            this.facade.RecalculateBptModel(model, ticket);
            serializedModel = this.bgaSerializer.SerializeRoot(model, ticket);
            return serializedModel;
        }

        public IEnumerable<SecurityModel> PickSecurities(String pattern, Int32 atMost)
        {
            var result = this.facade.GetSecurities(pattern, atMost);
            var serializedSecurities = this.serializer.SerializeSecurities(result);
            return serializedSecurities;
        }

        public IEnumerable<SecurityModel> PickSecuritiesFromBasket(String pattern, Int32 atMost, Int32 basketId)
        {
            var result = this.facade.GetSecurities(pattern, atMost, basketId);
            var serializedSecurities = this.serializer.SerializeSecurities(result);
            return serializedSecurities;
        }

        public IEnumerable<IssueModel> SaveBroadGlobalActive(BroadGlobalActive.RootModel serializedModel)
        {
            var ticket = new CalculationTicket();
            var model = this.bgaDeserializer.DeserializeRoot(serializedModel);
            var issues = this.facade.ApplyBroadGlobalActiveModelIfValid(model, Username, ticket);
            var serializedIssues = this.serializer.SerializeValidationIssues(issues);
            return serializedIssues;
        }

        // basket targets

        public BasketTargets.PickerModel GetBasketPicker()
        {
            var model = this.facade.GetBasketPickerRootModel();
            var serializedModel = this.btSerializer.SerializePicker(model);
            return serializedModel;
        }

        public BasketTargets.RootModel GetBasketTargets(Int32 targetingTypeGroupId, Int32 basketId)
        {
            var ticket = new CalculationTicket();
            var model = this.facade.GetBpstModel(targetingTypeGroupId, basketId);
            var serializedModel = this.btSerializer.SerializeRoot(model, ticket);
            return serializedModel;
        }

        public BasketTargets.RootModel RecalculateBasketTargets(BasketTargets.RootModel model)
        {
            var deserializedModel = this.btDeserializer.DeserializeRoot(model);
            var ticket = new CalculationTicket();
            var serializedModel = this.btSerializer.SerializeRoot(deserializedModel, ticket);
            return serializedModel;
        }

        public IEnumerable<IssueModel> SaveBasketTargets(BasketTargets.RootModel model)
        {
            var deserializedModel = this.btDeserializer.DeserializeRoot(model);
            var ticket = new CalculationTicket();
            var issues = this.facade.ApplyBpstModelIfValid(deserializedModel, Username, ticket);
            var serializedIssues = this.serializer.SerializeValidationIssues(issues);
            return serializedIssues;
        }

        // bottom-up

        public BottomUp.PickerModel GetBottomUpPortfolioPicker()
        {
            var models = this.facade.GetBottomUpPortfolios(Username);
            var result = this.buSerializer.SerializePicker(models);
            return result;
        }

        public BottomUp.RootModel GetBottomUpModel(String bottomUpPortfolioId)
        {
            var model = this.facade.GetPstModel(bottomUpPortfolioId);
            var ticket = new CalculationTicket();
            var serializeModel = this.buSerializer.SerializeRoot(model, ticket);
            return serializeModel;
        }

        public BottomUp.RootModel RecalculateBottomUp(BottomUp.RootModel model)
        {
            var deserializedModel = this.buDeserializer.DeserializerRoot(model);
            var ticket = new CalculationTicket();
            var serializedModel = this.buSerializer.SerializeRoot(deserializedModel, ticket);
            return serializedModel;
        }

        public IEnumerable<IssueModel> SaveBottomUp(BottomUp.RootModel model)
        {
            var deserializedModel = this.buDeserializer.DeserializerRoot(model);
            var ticket = new CalculationTicket();
            var issues = this.facade.ApplyPstModelIfValid(deserializedModel, Username, ticket);
            var serializedIssues = this.serializer.SerializeValidationIssues(issues);
            return serializedIssues;
        }


        // comments

        public IEnumerable<CommentModel> RequestCommentsForBasketPortfolioSecurityTarget(Int32 basketId, String broadGlbalActivePortfolioId, String securityId)
        {
            var comments = this.facade.GetCommentsForBasketPortfolioSecurityTarget(basketId, broadGlbalActivePortfolioId, securityId);
            var serializedComments = this.serializer.SerializeComments(comments);
            return serializedComments;
            //return new CommentModel[] { new CommentModel("Hey!") };
        }

        public IEnumerable<CommentModel> RequestCommentsForTargetingTypeBasketBase(int targetingTypeGroupId, int basketId, string securityId)
        {
            return null;
            //return new CommentModel[] { new CommentModel("Hey yourself!") };
        }
    }
}
