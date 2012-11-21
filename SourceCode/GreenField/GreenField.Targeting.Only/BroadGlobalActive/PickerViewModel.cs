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
using GreenField.Targeting.Only.Backend.Targeting;

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    public class PickerViewModel : ViewModelBase
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
            set { this.selectedTargetingType = value; this.RaisePropertyChanged(() => this.SelectedTargetingType); }
        }

        private BgaPortfolioPickerModel selectedPortfolio;
        public BgaPortfolioPickerModel SelectedPortfolio
        {
            get { return this.selectedPortfolio; }
            set
            {
                this.selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolio);

                if (this.selectedTargetingType != null && this.selectedPortfolio != null)
                {
                    var args = new PortfolioPickedEventArgs(
                        this.selectedTargetingType,
                        this.selectedPortfolio
                    );
                    this.OnPortfolioPicked(args);
                }
            }
        }

        public void Initialize()
        {
            this.RequestData();
        }

        protected void RequestData()
        {
            this.IsLoading = true;
            var client = this.clientFactory.CreateClient();
            client.GetTargetingTypePortfolioPickerCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult("Getting data for the picker", args, x => x.Result, this.TakeData);
            client.GetTargetingTypePortfolioPickerAsync();
        }

        protected void TakeData(IEnumerable<BgaTargetingTypePickerModel> targetingTypes)
        {
            this.TargetingTypes.Clear();
            foreach (var targetingType in targetingTypes)
            {
                this.TargetingTypes.Add(targetingType);
            }
            this.IsLoading = false;
        }

        public event PortfolioPickedEventHandler PortfolioPicked;
        protected void OnPortfolioPicked(PortfolioPickedEventArgs args)
        {
            var handler = this.PortfolioPicked;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void Reset()
        {
            this.SelectedPortfolio = null;
            this.SelectedTargetingType = null;
        }
    }
}
