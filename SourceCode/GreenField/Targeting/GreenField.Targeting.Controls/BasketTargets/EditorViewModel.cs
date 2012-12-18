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
using Aims.Data.Client;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Threading;

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

        public DelegateCommand ContextMenuCommand { get; private set; }

        public void HandleContextMenu()
        {
            MessageBox.Show("Hey!");
        }

        public Int32? LastBasketId
        {
            get
            {
                var lastInputOpt = base.LastValidInput;
                return lastInputOpt != null ? lastInputOpt.BasketId : (Int32?)null;
            }
        }

        public Int32? LastTargetingTypeGroupId
        {
            get
            {
                var lastInputOpt = base.LastValidInput;
                return lastInputOpt != null ? lastInputOpt.TargetingTypeGroupId : (Int32?)null;
            }
        }

        public EditorViewModel(IClientFactory clientFactory)
        {
            this.areEmptyColumnsShown = true;
            this.clientFactory = clientFactory;
            this.ContextMenuCommand = new DelegateCommand(this.HandleContextMenu);
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
        private BtRootModel model;
        public BtRootModel KeptRootModel
        {
            get { return this.model; }
            set
            {
                this.model = value;
                this.RaisePropertyChanged(() => this.KeptRootModel);
            }
        }

        public IEnumerable<IBtLineModel> Lines
        {
            get { return this.lines; }
            set { this.lines = value; this.RaisePropertyChanged(() => this.Lines); }
        }

        public void AddSecurity(ISecurity security)
        {
            this.KeptRootModel.SecurityToBeAddedOpt = security.ToSecurityModel();
            this.RequestRecalculating();
        }

        public override void RequestRecalculating()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RecalculateBasketTargetsCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Recalculating basket targets", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.RecalculateBasketTargetsAsync(this.KeptRootModel);
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
            client.GetBasketTargetsAsync(targetingTypeGroupId, basketId);
        }

        public void RequestSaving()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.SaveBasketTargetsCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Saving basket targets", args, x => x.Result, this.FinishSaving, this.FinishLoading);
            client.SaveBasketTargetsAsync(this.KeptRootModel);
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

            var registeredExpressions = new List<EditableExpressionModel>();
            foreach (var security in model.Securities)
            {
                var securityId = security.Security.Id;
                var basketId = this.LastBasketId.Value;
                foreach (var portfolioTarget in security.PortfolioTargets)
                {

                    var portfolioId = portfolioTarget.BroadGlobalActivePortfolio.Id;

                    var requestCommentsCommand = new DelegateCommand(delegate
                    {
                        this.RequestComments(basketId, portfolioId, securityId);
                    });
                    var expression = portfolioTarget.PortfolioTarget;
                    expression.RegisterForBeingWatched(this, requestCommentsCommand);
                    registeredExpressions.Add(expression);
                }
                var requestBaseCommentsCommand = new DelegateCommand(delegate
                {
                    this.RequestComments(this.LastTargetingTypeGroupId.Value, basketId, securityId);
                });
                var baseExpression = security.Base;
                baseExpression.RegisterForBeingWatched(this, requestBaseCommentsCommand);
                registeredExpressions.Add(baseExpression);
            }

            this.OnGotData();




            // setting the focus
            foreach (var expression in registeredExpressions)
            {
                if (expression.IsLastEdited)
                {
                    expression.IsFocusSet = true;
                    expression.IsLastEdited = false;
                }
            }
        }

        private void RequestComments(Int32 targetingTypeGroupId, Int32 basketId, String securityId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RequestCommentsForTargetingTypeBasketBaseCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting comments", args, x => x.Result, this.TakeComments, this.FinishLoading);
            client.RequestCommentsForTargetingTypeBasketBaseAsync(targetingTypeGroupId, basketId, securityId);
        }

        public void RequestComments(Int32 basketId, String portfolioId, String securityId)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.RequestCommentsForBasketPortfolioSecurityTargetCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting comments", args, x => x.Result, this.TakeComments, this.FinishLoading);
            client.RequestCommentsForBasketPortfolioSecurityTargetAsync(basketId, portfolioId, securityId);
        }

        public void TakeComments(ObservableCollection<CommentModel> comments)
        {
            this.FinishLoading();
            var comment = String.Join(", ", comments.Select(x => String.Format(
                "{4} at {3} {1} => {2}: {0}",
                x.Comment,
                x.Before,
                x.After,
                x.Timestamp,
                x.Username
            )));
            this.Comments = comments;
        }

        private ObservableCollection<CommentModel> comments;
        public ObservableCollection<CommentModel> Comments
        {
            get { return this.comments; }
            set { this.comments = value; this.RaisePropertyChanged(() => this.Comments); }
        }

        public void Discard()
        {
            this.Portfolios = null;
            this.Lines = null;
            this.KeptRootModel = null;
            this.Comments = null;
        }


        private EditableExpressionModel lastExpressionModel;
        public void GetNotifiedAboutChangedValue(EditableExpressionModel model)
        {
            if (this.lastExpressionModel != null)
            {
                this.lastExpressionModel.IsLastEdited = false;
            }

            model.IsLastEdited = true;
            this.ResetRecalculationTimer();
            this.lastExpressionModel = model;
        }
    }
}
