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
using GreenField.Targeting.Only.Backend.Targeting;
using System.Collections.ObjectModel;

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    public class EditorViewModel : ErrorCapableViewModel
    {
        private IClientFactory clientFactory;
        private Visibility commodityGridVisibility = Visibility.Collapsed;
        private Boolean isBusyIndicatorStatus;
        private ModelTraverser traverser;
        private DefaultExpandCollapseStateSetter defaultExpandCollapseStateSetter;

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

        public Visibility CommodityGridVisibility
        {
            get { return commodityGridVisibility; }
            set
            {
                commodityGridVisibility = value;
                this.RaisePropertyChanged(() => this.CommodityGridVisibility);
            }
        }

        public Boolean IsBusyIndicatorStatus
        {
            get { return this.isBusyIndicatorStatus; }
            set
            {
                this.isBusyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.IsBusyIndicatorStatus);
            }
        }

        public void Initialize(Int32 targetingTypeId, String broadGlobalActivePortfolioId, DateTime benchmarkDate)
        {
            this.RequestData(targetingTypeId, broadGlobalActivePortfolioId, benchmarkDate);
            this.IsBusyIndicatorStatus = true;
        }

        private void RequestData(Int32 targetingTypeId, String broadGlobalActivePortfolioId, DateTime benchmarkDate)
        {
            var client = this.clientFactory.CreateClient();
            client.GetBroadGlobalActiveModelCompleted += (sender, args) => this.TakeCareOfResult("Getting data for the editor.", args, x => x.Result, this.TakeData);
            client.GetBroadGlobalActiveModelAsync(targetingTypeId, broadGlobalActivePortfolioId, benchmarkDate);
        }

        protected void TakeData(BgaRootModel data)
        {
            this.TakeDataUnsafe(data);
        }

        protected void TakeDataUnsafe(BgaRootModel data)
        {
            var model = data.Globe;
            this.defaultExpandCollapseStateSetter.SetDefaultCollapseExpandState(model);
            var residents = this.traverser.Traverse(model);
            
            // we need an observable collection to make filtering (collapsing/expanding) work, because it is triggered by the CollectionChanged event
            var observedResidents = new PokableObservableCollection<IGlobeResident>(residents);
            this.Residents = observedResidents;
            this.RootModel = data;
            this.IsBusyIndicatorStatus = false;
        }

        private PokableObservableCollection<IGlobeResident> residents;
        /// <summary>
        /// Residents of the globe model (from the root model) turned into a flat collection in order to get bound to the grid.
        /// </summary>
        public PokableObservableCollection<IGlobeResident> Residents
        {
            get { return this.residents; }
            protected set { this.residents = value; this.RaisePropertyChanged(() => this.Residents); }
        }

        /// <summary>
        /// Original model from the backend service.
        /// </summary>
        public BgaRootModel RootModel { get; protected set; }

        public void Dispose()
        {
        }
    }
}
