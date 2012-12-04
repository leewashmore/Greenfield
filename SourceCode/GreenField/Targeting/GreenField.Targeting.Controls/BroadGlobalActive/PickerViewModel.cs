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
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using TopDown.FacingServer.Backend.Targeting;

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    public class PickerViewModel : CommunicatingViewModelBase
    {
        private IClientFactory clientFactory;

        public PickerViewModel(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            this.TargetingTypes = new ObservableCollection<BgaTargetingTypePickerModel>();
        }

        public ObservableCollection<BgaTargetingTypePickerModel> TargetingTypes { get; private set; }

        private BgaTargetingTypePickerModel selectedTargetingType;
        public BgaTargetingTypePickerModel SelectedTargetingType
        {
            get { return this.selectedTargetingType; }
            set
            {
                this.selectedTargetingType = value;
                this.RaisePropertyChanged(() => this.SelectedTargetingType);
                if (value == null)
                {
                    this.OnReset();
                }
            }
        }

        private BgaPortfolioPickerModel selectedPortfolio;
        public BgaPortfolioPickerModel SelectedPortfolio
        {
            get { return this.selectedPortfolio; }
            set
            {
                this.selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolio);

                if (value == null)
                {
                    this.OnReset();
                }
                else
                {
                    if (this.selectedTargetingType != null)
                    {
                        var args = new PortfolioPickedEventArgs(
                            this.selectedTargetingType,
                            value
                        );
                        this.OnPortfolioPicked(args);
                    }
                }
            }
        }

        public void RequestData()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetTargetingTypePortfolioPickerCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting data for the picker", args, x => x.Result, this.TakeData, this.FinishLoading);
            client.GetTargetingTypePortfolioPickerAsync();
        }

        protected void TakeData(IEnumerable<BgaTargetingTypePickerModel> targetingTypes)
        {
            this.TargetingTypes.Clear();
            foreach (var targetingType in targetingTypes)
            {
                this.TargetingTypes.Add(targetingType);
            }
            this.FinishLoading();
        }

        public event PortfolioPickedEventHandler Picked;
        protected void OnPortfolioPicked(PortfolioPickedEventArgs args)
        {
            var handler = this.Picked;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler Reset;
        protected void OnReset()
        {
            var handler = this.Reset;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }


        public void Deactivate()
        {
            this.SelectedPortfolio = null;
            this.SelectedTargetingType = null;
        }
    }
}
