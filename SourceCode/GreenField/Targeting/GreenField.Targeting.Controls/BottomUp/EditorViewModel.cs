using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;
using System.Windows;

namespace GreenField.Targeting.Controls.BottomUp
{
    public class EditorViewModel : EditorViewModelBase, IValueChangeWatcher
    {
        private IClientFactory clientFactory;
        private ObservableCollection<BuItemModel> items;

        [DebuggerStepThrough]
        public EditorViewModel(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        // talking to the server

        public void RequestData(String bottomUpPortfolioId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetBottomUpModelCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting picker data for the bottom-up editor", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.GetBottomUpModelAsync(bottomUpPortfolioId);
        }

        public void ConsiderRecalculating()
        {
            this.RequestRecalculating();
        }

        public void RequestRecalculating()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RecalculateBottomUpCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Recalculating bottom up", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.RecalculateBottomUpAsync(this.KeptRootModel);
        }

        public void TakeData(BuRootModel model)
        {
            this.KeptRootModel = model;
            this.RegisterInChangeWatcher(model);
            this.Items = model.Items;
            this.FinishLoading();
            this.OnGotData();
        }

        public void RequestSaving()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.SaveBottomUpAsync(this.KeptRootModel);
            client.SaveBottomUpCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Saving bottom-up", args, x => x.Result, this.FinishLoading, this.FinishLoading);
        }

        private void RegisterInChangeWatcher(BuRootModel model)
        {
            model.Items.ForEach(x => x.Target.RegisterForBeingWatched(this));
        }
        
        public void GetNotifiedAboutChangedValue(EditableExpressionModel model)
        {
            this.ConsiderRecalculating();
        }

        public ObservableCollection<BuItemModel> Items
        {
            get { return this.items; }
            set { this.items = value; this.RaisePropertyChanged(() => this.Items); }
        }

        /// <summary>
        /// Required when the data is being sent back.
        /// </summary>
        internal BuRootModel KeptRootModel { get; private set; }

        public void AddSecurity(SecurityModel security)
        {
            this.KeptRootModel.SecurityToBeAddedOpt = security;
            this.RequestRecalculating();
        }


        public void Deactivate()
        {
            this.Items = null;
            this.KeptRootModel = null;
        }


        // saving
    }
}
