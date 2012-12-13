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
using System.Linq;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class PickerViewModel : PickerViewModelBase
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
                if (this.selectedTargetingTypeGroup != value)
                {
                    var args = new CancellableEventArgs(false);
                    this.OnReseting(args);
                    if (!args.IsCancelled)
                    {
                        this.selectedTargetingTypeGroup = value;
                        if (value == null)
                        {
                            this.Baskets = null;
                        }
                        else
                        {
                            var sortedBaskets = this.SortBaskets(value.Baskets);
                            this.Baskets = sortedBaskets;
                        }
                        this.RaisePropertyChanged(() => this.SelectedTargetingTypeGroup);
                    }
                }
            }
        }

        public ObservableCollection<BtPickerBasketModel> SortBaskets(ObservableCollection<BtPickerBasketModel> baskets)
        {
            var result = Helper.ToObservableCollection(baskets.OrderBy(x => x.Name));
            return result;
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
                if (this.selectedBasket != value)
                {
                    if (value != null)
                    {
                        var targetingTypeGroup = this.SelectedTargetingTypeGroup;
                        var basket = value;
                        var args = new BasketPickedEventArgs(targetingTypeGroup.TargetingTypeGroupId, basket.Id, false);
                        this.OnPicking(args);
                        if (!args.IsCancelled)
                        {
                            this.selectedBasket = value;
                            this.RaisePropertyChanged(() => this.SelectedBasket);
                        }
                    }
                    else
                    {
                        var args = new CancellableEventArgs(false);
                        this.OnReseting(args);
                        if (!args.IsCancelled)
                        {
                            this.selectedBasket = value;
                            this.RaisePropertyChanged(() => this.SelectedBasket);
                        }
                    }
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

        public event BasketPickedEventHandler Picking;
        protected virtual void OnPicking(BasketPickedEventArgs args)
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
            this.TargetingTypeGroups = null;
            this.SelectedTargetingTypeGroup = null;
            this.SelectedBasket = null;
            this.Baskets = null;
            this.IsSilent = false;
        }
    }
}
