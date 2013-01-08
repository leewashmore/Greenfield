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
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using Aims.Controls;
using GreenField.IssuerShares.Client.Backend.IssuerShares;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Events;

namespace GreenField.IssuerShares.Controls
{
    public class HistoryViewModel : CommunicatingViewModelBase
    {
        private IClientFactory clientFactory;
        private ObservableCollection<GreenField.IssuerShares.Client.Backend.IssuerShares.IssuerSecurityShareRecordModel> itemStorage;
        public ObservableCollection<GreenField.IssuerShares.Client.Backend.IssuerShares.IssuerSecurityShareRecordModel> Items { get; private set; }
        private ObservableCollection<HistoryLineModel> lines;
        public ObservableCollection<HistoryLineModel> Lines { get; private set; }
       

        public HistoryViewModel(IClientFactory clientFactory, IEventAggregator aggregator)
        {
            this.clientFactory = clientFactory;
            aggregator.GetEvent<CompositionChangedEvent>().Subscribe(this.ShowHistory);
        }



        private List<Int32> securities;

        internal void RequestData(string securityShortName)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.GetIssuerSharesBySecurityShortNameCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                    "Getting history for security issuer (short name: " + securityShortName + ")", args, x => x.Result, StoreData, FinishLoading);

            client.GetIssuerSharesBySecurityShortNameAsync(securityShortName);
        }

        public void StoreData(ObservableCollection<IssuerSecurityShareRecordModel> items)
        {
            itemStorage = items;
            lines = IssuerSharesHelper.MakeRectangleCollection(items);
            this.Lines = lines;
            if (this.securities != null)
            {
                ShowHistory();
            }
            this.FinishLoading();
            
        }

        public void ShowHistory(CompositionChangedEventInfo info)
        {
            this.securities = info.Securities;
            ShowHistory();
        }

        public void ShowHistory()
        {
            if (itemStorage != null)
            {
                this.Items = Helper.ToObservableCollection<IssuerSecurityShareRecordModel>(itemStorage.Where(x => securities.Contains(x.SecurityId.Value)));
                foreach (var line in lines)
                {
                    decimal? total = 0;
                    foreach (var id in securities)
                    {
                        total += line.Values[id] ?? 0;
                    }
                    line.Values[0] = total;
                }
                this.Lines = lines;
                this.Items.Add(new IssuerSecurityShareRecordModel { SecurityTicker = "Total", SecurityId = 0 });
                this.RaisePropertyChanged(() => this.Items);
                this.RaisePropertyChanged(() => this.Lines);
            }
        }

        

        
    }
}
