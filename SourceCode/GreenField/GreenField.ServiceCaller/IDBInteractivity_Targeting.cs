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
using System.Collections.Generic;

namespace GreenField.ServiceCaller
{
    public partial interface IDBInteractivity
    {
        void GetBgaModel(Int32 targetingTypeId, String portfolioId, DateTime benchmarkDate, Action<BgaRootModel> callback);
        void GetTargetingTypes(Action<IEnumerable<TargetingDefinitions.BgaTargetingTypePickerModel>> callback);
    }
}
