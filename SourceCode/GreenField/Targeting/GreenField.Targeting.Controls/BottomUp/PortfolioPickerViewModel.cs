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
using System.Diagnostics;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;

namespace GreenField.Targeting.Controls.BottomUp
{
    public class PortfolioPickerViewModel : PickerViewModelBase
    {
        private IClientFactory clientFactory;
        private ObservableCollection<BottomUpPortfolioModel> bottomUpPortfolios;
        private BottomUpPortfolioModel selectedBottomUpPortfolio;

        [DebuggerStepThrough]
        public PortfolioPickerViewModel(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public void RequestData()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetBottomUpPortfolioPickerCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting picker data for the bottom up editor", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.GetBottomUpPortfolioPickerAsync();
        }

        public void TakeData(BuPickerModel model)
        {
            this.BottomUpPortfolios = model.BottomUpPortfolios;
            this.FinishLoading();
        }

        public ObservableCollection<BottomUpPortfolioModel> BottomUpPortfolios
        {
            get { return this.bottomUpPortfolios; }
            set
            {
                this.bottomUpPortfolios = value;
                this.RaisePropertyChanged(() => this.BottomUpPortfolios);
            }
        }

        public BottomUpPortfolioModel SelectedBottomUpPortfolio
        {
            get { return this.selectedBottomUpPortfolio; }
            set
            {
                if (this.selectedBottomUpPortfolio != value)
                {
                    if (value == null)
                    {
                        var args = new CancellableEventArgs(false);
                        this.OnReseting(args);
                        if (!args.IsCancelled)
                        {
                            this.selectedBottomUpPortfolio = null;
                            this.RaisePropertyChanged(() => this.SelectedBottomUpPortfolio);
                        }
                    }
                    else
                    {
                        var args = new BottomUpPortfolioPickedEventArgs(value, false);
                        this.OnPicked(args);
                        if (!args.IsCancelled)
                        {
                            this.selectedBottomUpPortfolio = value;
                            this.RaisePropertyChanged(() => this.SelectedBottomUpPortfolio);
                        }
                    }
                }
            }
        }

        public event BottomUpPortfolioEventHandler Picked;
        protected virtual void OnPicked(BottomUpPortfolioPickedEventArgs args)
        {
            var handler = this.Picked;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void Deactivate(Boolean silently)
        {
            this.IsSilent = silently;

            this.SelectedBottomUpPortfolio = null;
            this.BottomUpPortfolios = null;
            
            this.IsSilent = false;
        }
    }
}
