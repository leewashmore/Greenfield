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
    public class EditorViewModel : ViewModelBase
    {
        private IClientFactory clientFactory;
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

        public void RequestData(Int32 targetingTypeId, String broadGlobalActivePortfolioId, DateTime benchmarkDate)
        {
            this.IsLoading = true;
            var client = this.clientFactory.CreateClient();
            client.GetBroadGlobalActiveModelCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting data for the editor.", args, x => x.Result, this.TakeData);
            client.GetBroadGlobalActiveModelAsync(targetingTypeId, broadGlobalActivePortfolioId, benchmarkDate);
        }
        
        public void RequestRecalculating()
        {
            this.IsLoading = true;
            var client = this.clientFactory.CreateClient();
            client.RecalculateBroadGlobalActiveComplete +=
        }


        protected void TakeData(BgaRootModel data)
        {
            this.TakeDataUnsafe(data);
            this.IsLoading = false;
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

        public void Reset()
        {
            this.Residents = null;
            this.RootModel = null;
        }


        public void Dispose()
        {
        }


        
    }
}
