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
    public class EditorInput
    {
        [DebuggerStepThrough]
        public EditorInput(string bottomUpPortfolioId)
        {
            this.BottomUpPortfolioId = bottomUpPortfolioId;
        }

        public String BottomUpPortfolioId { get; private set; }
    }

    public class EditorViewModel : EditorViewModelBase<EditorInput>, IValueChangeWatcher
    {
        private IClientFactory clientFactory;
        private ObservableCollection<IBuLineModel> lines;

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
            client.GetBottomUpModelCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                "Getting picker data for the bottom-up editor", args, x => x.Result,
                data =>
                {
                    this.SetProvenValidInput(new EditorInput(bottomUpPortfolioId));
                    this.TakeData(data);
                },
                this.FinishLoading);
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

        protected override void RequestReloading(EditorInput input)
        {
            this.RequestData(input.BottomUpPortfolioId);
        }

        public void TakeData(BuRootModel model)
        {
            this.KeptRootModel = model;
            this.RegisterInChangeWatcher(model);
            var lines = Helper.ToObservableCollection(model.Items.Select(x => Helper.As<IBuLineModel>(x)));
            lines.Add(new BuTotalModel(model.TargetTotal));
            lines.Add(new BuCashModel(model.Cash));
            this.Lines = lines;
            this.FinishLoading();
            this.OnGotData();
        }

        public void RequestSaving()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.SaveBottomUpAsync(this.KeptRootModel);
            client.SaveBottomUpCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Saving bottom-up", args, x => x.Result, this.FinishSaving, this.FinishLoading);
        }

        private void RegisterInChangeWatcher(BuRootModel model)
        {
            model.Items.ForEach(x => x.Target.RegisterForBeingWatched(this));
        }

        public void GetNotifiedAboutChangedValue(EditableExpressionModel model)
        {
            this.ConsiderRecalculating();
        }

        public ObservableCollection<IBuLineModel> Lines
        {
            get { return this.lines; }
            set { this.lines = value; this.RaisePropertyChanged(() => this.Lines); }
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

        public void Discard()
        {
            this.Lines = null;
            this.KeptRootModel = null;
        }

        // saving
    }
}
