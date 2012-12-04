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
using System.ComponentModel.Composition;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class PickerViewModel : CommunicatingViewModelBase
    {
        private IClientFactory clientFactory;
        private ObservableCollection<BtPickerTargetingGroupModel> targetingTypeGroups;
        private BtPickerTargetingGroupModel selectedTargetingTypeGroup;
        private ObservableCollection<BtPickerBasketModel> baskets;
        private BtPickerBasketModel selectedBasket;

        [DebuggerStepThrough]
        public PickerViewModel(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public ObservableCollection<BtPickerTargetingGroupModel> TargetingTypeGroups
        {
            get { return this.targetingTypeGroups; }
            set
            {
                this.targetingTypeGroups = value;
                this.RaisePropertyChanged(() => this.TargetingTypeGroups);
            }
        }

        public BtPickerTargetingGroupModel SelectedTargetingTypeGroup
        {
            get { return this.selectedTargetingTypeGroup; }
            set
            {
                this.selectedTargetingTypeGroup = value;
                this.RaisePropertyChanged(() => this.SelectedTargetingTypeGroup);
                if (value != null)
                {
                    this.Baskets = value.Baskets;
                }
                else
                {
                    this.Baskets = null;
                }
                this.OnReset();
            }
        }

        public ObservableCollection<BtPickerBasketModel> Baskets
        {
            get { return this.baskets; }
            set
            {
                this.baskets = value;
                this.RaisePropertyChanged(() => this.Baskets);
            }
        }

        public BtPickerBasketModel SelectedBasket
        {
            get { return this.selectedBasket; }
            set
            {
                this.selectedBasket = value;
                this.RaisePropertyChanged(() => this.SelectedBasket);
                if (value != null)
                {
                    var targetingTypeGroup = this.SelectedTargetingTypeGroup;
                    var basket = value;
                    this.OnBasketPicked(new BasketPickedEventArgs(targetingTypeGroup.TargetingTypeGroupId, basket.Id));
                }
                else
                {
                    this.OnReset();
                }
            }
        }

        public void RequestData()
        {
            var client = this.clientFactory.CreateClient();
            client.GetBasketPickerCompleted += (sender, e) => RuntimeHelper.TakeCareOfResult("Getting baskets", e, x => x.Result, this.TakeData, this.FinishLoading);
            client.GetBasketPickerAsync();
        }

        public void TakeData(BtPickerModel data)
        {
            this.TargetingTypeGroups = data.TargetingGroups;
        }

        public event BasketPickedEventHandler Picked;
        protected virtual void OnBasketPicked(BasketPickedEventArgs args)
        {
            var handler = this.Picked;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler Reset;
        protected virtual void OnReset()
        {
            var handler = this.Reset;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public void Deactivate()
        {
            this.TargetingTypeGroups = null;
            this.SelectedTargetingTypeGroup = null;
            this.SelectedBasket = null;
            this.Baskets = null;
        }
    }
}
