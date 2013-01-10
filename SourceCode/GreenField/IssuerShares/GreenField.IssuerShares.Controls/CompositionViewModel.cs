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
using Aims.Controls;
using GreenField.IssuerShares.Client.Backend.IssuerShares;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using System.Linq;

namespace GreenField.IssuerShares.Controls
{
    public class CompositionViewModel : CommunicatingViewModelBase
    {
        private IClientFactory clientFactory;
        public DelegateCommand SaveCompositionCommand { get; private set; }

        private bool isChanged;
        public bool IsChanged {
            get
            {
                return isChanged;
            }

            set
            {
                isChanged = value;
                this.RaisePropertyChanged(() => this.IsChanged);
                this.SaveCompositionCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand ClickToAssociateUserCommand { get; private set; }
        

        public CompositionViewModel(IClientFactory clientFactory, IEventAggregator aggregator)
        {
            this.aggregator = aggregator;
            this.clientFactory = clientFactory;
            this.SaveCompositionCommand = new DelegateCommand(SaveComposition, () => this.IsChanged);
            this.IsChanged = false;
            this.RaisePropertyChanged(() => this.IsChanged);
        }

        public void HasChangedPreferred(ItemModel item)
        {
            this.IsChanged = true;
        }

        public void SaveComposition()
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.UpdateIssueSharesCompositionCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Updateing composition for issuer (ID: " + this.Issuer.Id + ")", args, x => x.Result, ReloadModel, FinishLoading);
            client.UpdateIssueSharesCompositionAsync(new RootModel { Issuer = this.Issuer, Items = this.Items });
        }

        public void ReloadModel(RootModel model)
        {
            this.IsChanged = false;
            this.FinishLoading();
        }

        public IssuerModel Issuer { get; private set; }

        internal void RequestData(String securityShortName)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetRootModelCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting composition for security issuer (short name: " + securityShortName + ")", args, x => x.Result, InitializeDataGrid, FinishLoading);

            client.GetRootModelAsync(securityShortName);
            
        }



        private IEventAggregator aggregator;
        public ObservableCollection<ItemModel> Items { get; private set; }

        internal void InitializeDataGrid(RootModel model)
        {

            this.Issuer = model.Issuer;
            this.Items = model.Items;
            foreach (var item in this.Items)
            {
                item.InitializeRemoveCommand(new DelegateCommand<ItemModel>(RemoveItemFromComposition));
                item.InitializeChangedPreferredCommand(new DelegateCommand<ItemModel>(HasChangedPreferred));
            }
            this.RaisePropertyChanged(() => this.Items);
            CompositionChangedEventInfo info = new CompositionChangedEventInfo { Securities = this.Items.Select(x => Int32.Parse(x.Security.Id)).ToList() };
            this.aggregator.GetEvent<CompositionChangedEvent>().Publish(info);
            
        }

        public void RemoveItemFromComposition(ItemModel item)
        {
            this.Items.Remove(item);
            this.IsChanged = true;
            CompositionChangedEventInfo info = new CompositionChangedEventInfo { Securities = this.Items.Select(x => Int32.Parse(x.Security.Id)).ToList() };
            this.aggregator.GetEvent<CompositionChangedEvent>().Publish(info);
        }

        

        internal void AddSecurity(Aims.Data.Client.ISecurity security)
        {
            var item = new ItemModel { Security = security.ToSecurityModel(), Preferred = false };
            item.InitializeRemoveCommand(new DelegateCommand<ItemModel>(RemoveItemFromComposition));

            if (this.Items.Count(x => x.Security.Id == item.Security.Id) != 0)
            {
                MessageBox.Show("Security is already in the composition.");
            }
            else
            {
                this.Items.Add(item);
            }
            this.IsChanged = true;
            CompositionChangedEventInfo info = new CompositionChangedEventInfo { Securities = this.Items.Select(x => Int32.Parse(x.Security.Id)).ToList() };
            this.aggregator.GetEvent<CompositionChangedEvent>().Publish(info);
            
        }



        private void HandleErrors(Exception e)
        { 
        
        }
    }
}
