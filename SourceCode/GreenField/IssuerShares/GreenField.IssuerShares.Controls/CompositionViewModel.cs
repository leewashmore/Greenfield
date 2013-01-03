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

namespace GreenField.IssuerShares.Controls
{
    public class CompositionViewModel : NotificationObject
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

        
        

        public CompositionViewModel(IClientFactory clientFactory)
        {
            // TODO: Complete member initialization
            this.clientFactory = clientFactory;
            this.SaveCompositionCommand = new DelegateCommand(SaveComposition, () => this.IsChanged);
            this.IsChanged = false;
            this.RaisePropertyChanged(() => this.IsChanged);
        }

        public void SaveComposition()
        {
            var client = this.clientFactory.CreateClient();
            client.GetRootModelCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Updateing composition for issuer (ID: " + this.Issuer.Id + ")", args, x => x.Result, null, HandleErrors);
            client.UpdateIssueSharesCompositionAsync(new RootModel { Issuer = this.Issuer, Items = this.Items });
        }

        public IssuerModel Issuer { get; private set; }

        internal void RequestData(String securityShortName, Action<RootModel> callback)
        {
            this.callback = callback;
            var client = this.clientFactory.CreateClient();
            client.GetRootModelCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting composition for security issuer (short name: " + securityShortName + ")", args, x => x.Result, InitializeDataGrid, HandleErrors);

            client.GetRootModelAsync(securityShortName);
            
        }

        private Action<RootModel> callback;
        public ObservableCollection<ItemModel> Items { get; private set; }

        internal void InitializeDataGrid(RootModel model)
        {

            this.Issuer = model.Issuer;
            this.Items = model.Items;
            foreach (var item in this.Items)
            {
                item.InitializeRemoveCommand(new DelegateCommand<ItemModel>(RemoveItemFromComposition));
            }
            this.RaisePropertyChanged(() => this.Items);
            callback(model);
        }

        public void RemoveItemFromComposition(ItemModel item)
        {
            this.Items.Remove(item);
            this.IsChanged = true;
        }

        internal void HandleErrors(Exception e)
        { }

        internal void AddSecurity(Aims.Data.Client.ISecurity security)
        {
            var item = new ItemModel { Security = security.ToSecurityModel(), Preferred = false };
            item.InitializeRemoveCommand(new DelegateCommand<ItemModel>(RemoveItemFromComposition));
            this.Items.Add(item);
            this.IsChanged = true;
            
        }

        
    }
}
