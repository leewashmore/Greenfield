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
using GreenField.Gadgets.ViewModels.Targeting.BroadGlobalActive;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.DashBoardModule.ViewModels.Targeting
{
    public class RootViewModel : NotificationObject
    {
        [Obsolete]
        public RootViewModel(PickerViewModel pickerViewModel, EditorViewModel editorViewModel)
            : this(pickerViewModel, editorViewModel, DateTime.Today.AddDays(-1))
        {
        }
        public RootViewModel(PickerViewModel pickerViewModel, EditorViewModel editorViewModel, DateTime benchmarkDate)
        {
            this.editorViewModel = editorViewModel;
            this.pickerViewModel = pickerViewModel;
            this.pickerViewModel.PortfolioPicked += (sender, e) => this.editorViewModel.Initialize(e.TargetingType.Id, e.Portfolio.Id, benchmarkDate);
        }

        private PickerViewModel pickerViewModel;
        public PickerViewModel PickerViewModel
        {
            get { return this.pickerViewModel; }
            set { this.pickerViewModel = value; this.RaisePropertyChanged(() => this.PickerViewModel); }
        }

        private EditorViewModel editorViewModel;
        public EditorViewModel EditorViewModel
        {
            get { return this.editorViewModel; }
            set { this.editorViewModel = value; this.RaisePropertyChanged(() => this.EditorViewModel); }
        }        
    }
}
