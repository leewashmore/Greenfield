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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ModelFXDefinitions;
using System.Collections.Generic;
using GreenField.Common.Helper;
using GreenField.DataContracts;
using GreenField.ServiceCaller.TargetingDefinitions;
using GreenField.Gadgets.ViewModels;


namespace GreenField.Gadgets.Views
{
    public partial class ViewTargetingBroadGlobalActive : Helpers.ViewBaseUserControl
    {
        public ViewTargetingBroadGlobalActive(ViewModelTargetingBroadGlobalActive viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
