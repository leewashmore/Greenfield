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
    public class PickerViewModel : PickerViewModelBase
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
                if (this.selectedTargetingType != value)
                {
                    // no matter if the new value is NULL or something real
                    // we fire the reseting event anyways as long as the value before is not the same as the value after
                    var args = new CancellableEventArgs(false);
                    this.OnReseting(args);
                    if (!args.IsCancelled)
                    {
                        this.selectedTargetingType = value;
                        this.RaisePropertyChanged(() => this.SelectedTargetingType);
                    }                    
                }
            }
        }

        private BgaPortfolioPickerModel selectedPortfolio;
        public BgaPortfolioPickerModel SelectedPortfolio
        {
            get { return this.selectedPortfolio; }
            set
            {
                if (this.selectedPortfolio != value)
                {
                    if (value == null)
                    {
                        var args = new CancellableEventArgs(false);
                        this.OnReseting(args);
                        if (!args.IsCancelled)
                        {
                            this.selectedPortfolio = null;
                            this.RaisePropertyChanged(() => this.SelectedPortfolio);
                        }
                    }
                    else
                    {
                        if (this.selectedTargetingType != null)
                        {
                            var args = new PortfolioPickedEventArgs(this.selectedTargetingType, value, false);
                            this.OnPicking(args);
                            if (!args.IsCancelled)
                            {
                                this.selectedPortfolio = value;
                                this.RaisePropertyChanged(() => this.SelectedPortfolio);
                            }
                        }
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

        public event PortfolioPickedEventHandler Picking;
        protected void OnPicking(PortfolioPickedEventArgs args)
        {
            var handler = this.Picking;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void Deactivate(Boolean silently)
        {
            this.IsSilent = silently;
            this.SelectedPortfolio = null;
            this.SelectedTargetingType = null;
            this.IsSilent = false;
        }
    }
}
