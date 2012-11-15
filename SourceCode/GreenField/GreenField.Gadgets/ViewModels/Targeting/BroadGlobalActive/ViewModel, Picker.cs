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
using GreenField.ServiceCaller.TargetingDefinitions;
using Microsoft.Practices.Prism.Commands;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Gadgets.ViewModels.Targeting.BroadGlobalActive
{
    public class PickerViewModel : NotificationObject
    {
        private IDBInteractivity repository;

        public PickerViewModel(IDBInteractivity repository)
        {
            this.repository = repository;
            this.TargetingTypes = new ObservableCollection<BgaTargetingTypePickerModel>();
            this.Initialize();
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
            this.repository.GetTargetingTypes(this.TakeData);
        }

        protected void TakeData(IEnumerable<BgaTargetingTypePickerModel> targetingTypes)
        {
            this.TargetingTypes.Clear();
            foreach (var targetingType in targetingTypes)
            {
                this.TargetingTypes.Add(targetingType);
            }
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
    }
}
