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

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    public class EditorViewModel : EditorViewModelBase, IValueChangeWatcher
    {
        public const Int32 NumberOfMillisecondsBetweenLastChangeAndRecalcualtion = 0;

        private IClientFactory clientFactory;
        private ModelTraverser traverser;
        private DefaultExpandCollapseStateSetter defaultExpandCollapseStateSetter;
        private ObservableCollection<IGlobeResident> residents;
        private ObservableCollection<BgaFactorItemModel> factors;
        private DispatcherTimer waitBeforeRecalculating;

        public EditorViewModel(
            IClientFactory clientFactory,
            ModelTraverser traverser,
            DefaultExpandCollapseStateSetter defaultExpandCollapseStateSetter
        )
        {
            this.waitBeforeRecalculating = new DispatcherTimer();
            this.waitBeforeRecalculating.Interval = TimeSpan.FromMilliseconds(NumberOfMillisecondsBetweenLastChangeAndRecalcualtion);
            this.waitBeforeRecalculating.Stop();
            this.waitBeforeRecalculating.Tick += this.ConsiderRecalculating;

            this.traverser = traverser;
            this.defaultExpandCollapseStateSetter = defaultExpandCollapseStateSetter;
            this.clientFactory = clientFactory;
        }

        // talking to the server

        public void RequestData(Int32 targetingTypeId, String broadGlobalActivePortfolioId, DateTime benchmarkDate)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetBroadGlobalActiveCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting data for the editor", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.GetBroadGlobalActiveAsync(targetingTypeId, broadGlobalActivePortfolioId, benchmarkDate);
        }

        public void RequestRecalculating()
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

            this.FinishLoading();
            this.OnGotData();
        }

        public void RequestSaving()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.SaveBroadGlobalActiveCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Saving data from the editor", args, x => x.Result, this.FinishLoading, this.FinishLoading);
            client.SaveBroadGlobalActiveAsync(this.RootModel);
        }

        // handling data
        
        private void RegisterOveralyFactorsForValueChangeWatch(BgaFactorModel factorsModel)
        {
            foreach (var item in factorsModel.Items)
            {
                item.OverlayFactor.RegisterForBeingWatched(this);
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
                model.Base.RegisterForBeingWatched(this.watcher);
                model.PortfolioAdjustment.RegisterForBeingWatched(this.watcher);
            }
            public void Resolve(BasketRegionModel model)
            {
                model.Base.RegisterForBeingWatched(this.watcher);
                model.PortfolioAdjustment.RegisterForBeingWatched(this.watcher);
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
                model.Base.RegisterForBeingWatched(this.watcher);
                model.PortfolioAdjustment.RegisterForBeingWatched(this.watcher);
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
            this.waitBeforeRecalculating.Stop();
            this.waitBeforeRecalculating.Start();
        }

        protected void ConsiderRecalculating(Object sender, EventArgs e)
        {
            this.waitBeforeRecalculating.Stop();
            this.RequestRecalculating();
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
        public BgaRootModel RootModel { get; protected set; }

        public void Deactivate()
        {
            this.Residents = null;
            this.RootModel = null;
        }

        public void Dispose()
        {
        }


    }
}
