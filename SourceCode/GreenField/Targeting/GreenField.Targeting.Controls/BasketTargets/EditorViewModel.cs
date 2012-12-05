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
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class EditorInput
    {
        [DebuggerStepThrough]
        public EditorInput(Int32 targetingTypeGroupId, Int32 basketId)
        {
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.BasketId = basketId;
        }

        public Int32 TargetingTypeGroupId { get; private set; }
        public Int32 BasketId { get; private set; }
    }

    public class EditorViewModel : EditorViewModelBase<EditorInput>, IValueChangeWatcher
    {
        private IClientFactory clientFactory;
        private IEnumerable<BtPorfolioModel> portfolios;
        private IEnumerable<IBtLineModel> lines;
        private DateTime benchmarkDate;
        private ValueTraverser valueTraverser;

        public EditorViewModel(IClientFactory clientFactory, DateTime benchmarkDate)
            : this(clientFactory, benchmarkDate, new ValueTraverser())
        {

        }

        public EditorViewModel(IClientFactory clientFactory, DateTime benchmarkDate, ValueTraverser valueTraverser)
        {
            this.areEmptyColumnsShown = true;
            this.clientFactory = clientFactory;
            this.benchmarkDate = benchmarkDate;
            this.valueTraverser = valueTraverser;
        }

        public IEnumerable<BtPorfolioModel> Portfolios
        {
            get { return this.portfolios; }
            set
            {
                this.portfolios = value;
                this.RaisePropertyChanged(() => this.Portfolios);
            }
        }

        private Boolean areEmptyColumnsShown;
        public Boolean AreEmptyColumnsShown
        {
            get { return this.areEmptyColumnsShown; }
            set
            {
                this.areEmptyColumnsShown = value;
                this.RaisePropertyChanged("AreEmptyColumnsShown");
            }
        }


        // for later use when we send data back to the service
        internal BtRootModel KeptRootModel { get; private set; }

        public IEnumerable<IBtLineModel> Lines
        {
            get { return this.lines; }
            set { this.lines = value; this.RaisePropertyChanged(() => this.Lines); }
        }

        public void AddSecurity(SecurityModel security)
        {
            this.KeptRootModel.SecurityToBeAddedOpt = security;
            this.RequestRecalculating();
        }

        private void RequestRecalculating()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RecalculateBasketTargetsCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Recalculating basket targets", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.RecalculateBasketTargetsAsync(this.KeptRootModel, this.benchmarkDate);
        }

        public void RequestData(Int32 targetingTypeGroupId, Int32 basketId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetBasketTargetsCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                "Getting basket targets", args, x => x.Result,
                data =>
                {
                    this.SetProvenValidInput(new EditorInput(targetingTypeGroupId, basketId));
                    this.TakeData(data);
                },
                this.FinishLoading);
            client.GetBasketTargetsAsync(targetingTypeGroupId, basketId, this.benchmarkDate);
        }

        public void RequestSaving()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.SaveBasketTargetsCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Saving basket targets", args, x => x.Result, this.FinishSaving, this.FinishLoading);
            client.SaveBasketTargetsAsync(this.KeptRootModel, this.benchmarkDate);
        }

        protected override void RequestReloading(EditorInput input)
        {
            this.RequestData(input.TargetingTypeGroupId, input.BasketId);
        }

        public void TakeData(BtRootModel model)
        {
            this.FinishLoading();

            // important step: datagrid needs to know how many columns are required to fit all the portfolios
            this.Portfolios = model.Portfolios;

            var lines = model.Securities.Select(x => Helper.As<IBtLineModel>(x)).ToList();
            lines.Add(model);
            this.Lines = lines;

            this.KeptRootModel = model;
            this.valueTraverser.TraverseValues(model).ForEach(x => x.RegisterForBeingWatched(this));
            this.OnGotData();
        }

        public void Discard()
        {
            this.Portfolios = null;
            this.Lines = null;
            this.KeptRootModel = null;
        }

        public void GetNotifiedAboutChangedValue(EditableExpressionModel model)
        {
            this.RequestRecalculating();
        }


    }
}
