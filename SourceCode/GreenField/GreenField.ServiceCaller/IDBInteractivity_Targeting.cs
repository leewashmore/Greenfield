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

namespace GreenField.ServiceCaller
{
    public partial interface IDBInteractivity
    {
        void GetBgaModel(Int32 targetingTypeId, String portfolioId, DateTime benchmarkDate, Action<RootModel> callback);
    }
}
