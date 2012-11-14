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

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelTargetingTypeWithBgaPortfolioPicker
    {
        public ViewModelTargetingTypeWithBgaPortfolioPicker()
        {
            this.TargetingTypes = new ObservableCollection<TargetingTypeModel>();
            this.TargetingTypeChangedCommand = new DelegateCommand<Object>(this.SelectTargetingType);
        }

        public ObservableCollection<TargetingTypeModel> TargetingTypes { get; private set; }
        public DelegateCommand<Object> TargetingTypeChangedCommand { get; private set; }
        
        public void SelectTargetingType(Object whatever)
        {
        }
    }
}
