using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Linq;
using System.Diagnostics;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    public class EditorInput
    {
        [DebuggerStepThrough]
        public EditorInput(Int32 targetingTypeId, String broadGlobalActivePortfolioId)
        {
            this.TargetingTypeId = targetingTypeId;
            this.BroadGlobalActivePortfolioId = broadGlobalActivePortfolioId;
        }

        public Int32 TargetingTypeId { get; private set; }
        public String BroadGlobalActivePortfolioId { get; private set; }
    }

    public class EditorViewModel : EditorViewModelBase<EditorInput>, IValueChangeWatcher
    {
        private IClientFactory clientFactory;
        private ModelTraverser traverser;
        private DefaultExpandCollapseStateSetter defaultExpandCollapseStateSetter;
        private ObservableCollection<IGlobeResident> residents;
        private ObservableCollection<BgaFactorItemModel> factors;


        public EditorViewModel(
            IClientFactory clientFactory,
            ModelTraverser traverser,
            DefaultExpandCollapseStateSetter defaultExpandCollapseStateSetter
        )
        {

            this.traverser = traverser;
            this.defaultExpandCollapseStateSetter = defaultExpandCollapseStateSetter;
            this.clientFactory = clientFactory;
        }

        // talking to the server

        public void RequestData(Int32 targetingTypeId, String broadGlobalActivePortfolioId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetBroadGlobalActiveCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                "Getting data for the editor", args, x => x.Result,
                data =>
                {
                    this.SetProvenValidInput(new EditorInput(targetingTypeId, broadGlobalActivePortfolioId));
                    this.TakeData(data);
                },
                this.FinishLoading);
            client.GetBroadGlobalActiveAsync(targetingTypeId, broadGlobalActivePortfolioId, this.clientFactory.GetUsername());
        }

        public override void RequestRecalculating()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RecalculateBroadGlobalActiveCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Recalculating data for the editor", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.RecalculateBroadGlobalActiveAsync(this.RootModel);
        }

        protected void TakeData(BgaRootModel data)
        {
            this.defaultExpandCollapseStateSetter.SetDefaultCollapseExpandState(data.Globe);
            var residents = this.traverser.Traverse(data);

            // register for listening to any change to base and portfolio adjustment, if change happens we need to initiate recalculation
            this.RegisterResidentsForValueChangeWatch(residents);
            this.RegisterOveralyFactorsForValueChangeWatch(data.Factors);

            // we need an observable collection to make filtering (collapsing/expanding) work, because it is triggered by the CollectionChanged event
            this.Residents = new ObservableCollection<IGlobeResident>(residents);
            this.Factors = new ObservableCollection<BgaFactorItemModel>(data.Factors.Items);
            this.RootModel = data;

            var targetingTypeId = this.LastValidInput.TargetingTypeId;
            var portfolioId = this.LastValidInput.BroadGlobalActivePortfolioId; 

            var registeredExpressions = new List<EditableExpressionModel>();
            foreach (var factor in factors)
            {
                EditableExpressionModel factorExpression = factor.OverlayFactor;
                var securityId = factor.BottomUpPortfolio.Id;
                var requestOverlayFactorCommentsCommand = new DelegateCommand(delegate
                {
                    this.RequestOverlayFactorComments(portfolioId, securityId);
                });
                factorExpression.RegisterForBeingWatched(this, requestOverlayFactorCommentsCommand);
                registeredExpressions.Add(factorExpression);

            }

            foreach (var resident in residents)
            {
                var basketId = -1;
                
                EditableExpressionModel baseExpression = null;
                EditableExpressionModel portfolioAdjustmentExpression = null;
                if (resident is BasketCountryModel)
                {
                    var r = resident as BasketCountryModel;
                    basketId = r.Basket.Id;
                    baseExpression = r.Base;
                    portfolioAdjustmentExpression = r.PortfolioAdjustment;

                }
                if (resident is BasketRegionModel)
                {
                    var r = resident as BasketRegionModel;
                    basketId = r.Basket.Id;
                    baseExpression = r.Base;
                    portfolioAdjustmentExpression = r.PortfolioAdjustment;
                }

                if (basketId > -1)
                {
                    var requestBaseCommentsCommand = new DelegateCommand(delegate
                    {
                        this.RequestBaseComments(targetingTypeId, basketId);
                    });
                    baseExpression.RegisterForBeingWatched(this, requestBaseCommentsCommand);
                    registeredExpressions.Add(baseExpression);

                    var requestPortfolioAdjustmentCommentsCommand = new DelegateCommand(delegate
                    {
                        this.RequestPortfolioAdjustmentComments(targetingTypeId, portfolioId, basketId);
                    });
                    portfolioAdjustmentExpression.RegisterForBeingWatched(this, requestPortfolioAdjustmentCommentsCommand);
                    registeredExpressions.Add(portfolioAdjustmentExpression);
                }
            }

            this.FinishLoading();
            this.OnGotData();
        }

        private void RequestOverlayFactorComments(string portfolioId, string securityId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RequestCommentsForBgaPortfolioSecurityFactorCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting comments", args, x => x.Result, this.TakeComments, this.FinishLoading);
            client.RequestCommentsForBgaPortfolioSecurityFactorAsync(portfolioId, securityId);
        }

        private void RequestPortfolioAdjustmentComments(int targetingTypeId, string portfolioId, int basketId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RequestCommentsForTargetingTypeBasketPortfolioTargetCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting comments", args, x => x.Result, this.TakeComments, this.FinishLoading);
            client.RequestCommentsForTargetingTypeBasketPortfolioTargetAsync(targetingTypeId, portfolioId, basketId);
        }

        private void RequestBaseComments(Int32 targetingTypeId, Int32 basketId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RequestCommentsForTargetingTypeBasketBaseValueCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting comments", args, x => x.Result, this.TakeComments, this.FinishLoading);
            client.RequestCommentsForTargetingTypeBasketBaseValueAsync(targetingTypeId, basketId);
        }

        public void TakeComments(ObservableCollection<CommentModel> comments)
        {
            this.FinishLoading();
            this.Comments = comments;
        }

        private ObservableCollection<CommentModel> comments;
        public ObservableCollection<CommentModel> Comments
        {
            get { return this.comments; }
            set { this.comments = value; this.RaisePropertyChanged(() => this.Comments); }
        }

        public void RequestSaving()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.SaveBroadGlobalActiveCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Saving data from the editor", args, x => x.Result, this.FinishSaving, this.FinishLoading);
            client.SaveBroadGlobalActiveAsync(this.RootModel, this.clientFactory.GetUsername());
        }

        protected override void RequestReloading(EditorInput input)
        {
            this.RequestData(input.TargetingTypeId, input.BroadGlobalActivePortfolioId);
        }

        // handling data

        private void RegisterOveralyFactorsForValueChangeWatch(BgaFactorModel factorsModel)
        {
            foreach (var item in factorsModel.Items)
            {
                item.OverlayFactor.RegisterForBeingWatched(this, null);
            }
        }

        private void RegisterResidentsForValueChangeWatch(IEnumerable<IGlobeResident> residents)
        {
            foreach (var resident in residents)
            {
                var resolver = new RegisterResidentsForValueChangeWatch_IGlobeResidentResolver(this);
                resident.Accept(resolver);
            }
        }

        private class RegisterResidentsForValueChangeWatch_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private IValueChangeWatcher watcher;
            public RegisterResidentsForValueChangeWatch_IGlobeResidentResolver(IValueChangeWatcher watcher)
            {
                this.watcher = watcher;
            }
            public void Resolve(BasketCountryModel model)
            {
                model.Base.RegisterForBeingWatched(this.watcher, null);
                model.PortfolioAdjustment.RegisterForBeingWatched(this.watcher, null);
            }
            public void Resolve(BasketRegionModel model)
            {
                model.Base.RegisterForBeingWatched(this.watcher, null);
                model.PortfolioAdjustment.RegisterForBeingWatched(this.watcher, null);
            }
            public void Resolve(OtherModel model)
            {
                // do nothing
            }
            public void Resolve(RegionModel model)
            {
                // do nothing
            }
            public void Resolve(UnsavedBasketCountryModel model)
            {
                model.Base.RegisterForBeingWatched(this.watcher, null);
                model.PortfolioAdjustment.RegisterForBeingWatched(this.watcher, null);
            }
            public void Resolve(BgaCountryModel model)
            {
                // do nothing
            }
            public void Resolve(CashLineModel model)
            {
                // do nothing
            }
            public void Resolve(TotalLineModel model)
            {
                // do nothing
            }
        }

        public void GetNotifiedAboutChangedValue(EditableExpressionModel model)
        {
            base.ResetRecalculationTimer();
        }

        /// <summary>
        /// Residents of the globe model (from the root model) turned into a flat collection in order to get bound to the grid.
        /// </summary>
        public ObservableCollection<IGlobeResident> Residents
        {
            get { return this.residents; }
            protected set
            {
                this.residents = value;
                this.RaisePropertyChanged(() => this.Residents);
            }
        }

        public ObservableCollection<BgaFactorItemModel> Factors
        {
            get { return this.factors; }
            set
            {
                this.factors = value;
                this.RaisePropertyChanged(() => this.Factors);
            }
        }

        /// <summary>
        /// Original model from the backend service.
        /// </summary>
        private BgaRootModel model;
        public BgaRootModel RootModel
        {
            get { return this.model; }
            set
            {
                this.model = value;
                this.RaisePropertyChanged(() => this.RootModel);
            }
        }

        public void Discard()
        {
            this.Residents = null;
            this.Factors = null;
            this.RootModel = null;
        }

        public void Dispose()
        {
        }

    }
}
