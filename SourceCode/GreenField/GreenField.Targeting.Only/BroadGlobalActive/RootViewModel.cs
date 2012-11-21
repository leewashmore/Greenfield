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
using Microsoft.Practices.Prism.Commands;
using GreenField.Targeting.Only.BroadGlobalActive;
using System.ComponentModel.Composition;

namespace GreenField.DashBoardModule.ViewModels.Targeting
{
    /// <summary>
    /// Orchestrates communications between picker and editor.
    /// </summary>

    [Export]
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

        /// <summary>
        /// Safe check for modifications.
        /// Takes into account that the view model may not be initialized by the time the check is done.
        /// This property is not supposed to be bound, because it doesn't notify the view about getting changed.
        /// The main purpose of this property is to be used in OnNavigateFrom of the RootView in order to get a confirmation about leaving unsaved modifications.
        /// </summary>
        public Boolean IsModified
        {
            get
            {
                var result = this.EditorViewModel != null && this.EditorViewModel.RootModel != null && this.EditorViewModel.RootModel.IsModified;
                return result;
            }
        }
    }
}
