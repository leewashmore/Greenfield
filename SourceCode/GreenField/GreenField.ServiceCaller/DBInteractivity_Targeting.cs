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
using GreenField.ServiceCaller.TargetingDefinitions;
using System.ComponentModel;

namespace GreenField.ServiceCaller
{
    public partial class DBInteractivity
    {
        public void GetBgaModel(Int32 targetingTypeId, String portfolioId, DateTime benchmarkDate, Action<RootModel> callback)
        {
            var client = new FacadeClient();
            client.GetBgaModelCompleted += (sender, e) => this.MakeSureSuccessful(e, x => callback(x.Result));
            client.GetBgaModelAsync(targetingTypeId, portfolioId, benchmarkDate);
        }

        protected void MakeSureSuccessful<TValue>(TValue e, Action<TValue> callback)
            where TValue : AsyncCompletedEventArgs
        {
            if (e.Cancelled) throw new ApplicationException("Request to the web-serivice has been cancelled.");
            if (e.Error != null) throw new ApplicationException("Exception has been thrown during fulfilling the request: " + e.Error.Message, e.Error);
            callback(e);
        }
    }
}
