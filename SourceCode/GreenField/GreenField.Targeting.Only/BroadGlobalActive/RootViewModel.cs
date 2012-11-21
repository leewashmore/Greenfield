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
using System.ComponentModel.Composition;
using System.Threading;
using Microsoft.Practices.Prism.Events;

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    /// <summary>
    /// Orchestrates communications between picker and editor.
    /// </summary>
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        [ImportingConstructor]
        public RootViewModel(Settings settings)
        {
            var editorViewModel = new EditorViewModel(
                settings.ClientFactory,
                settings.ModelTraverser,
                settings.DefaultExpandCollapseStateSetter
            );
            
            var pickerViewModel = new PickerViewModel(settings.ClientFactory);

            pickerViewModel.PortfolioPicked += (sender, e) => editorViewModel.Initialize(e.TargetingType.Id, e.Portfolio.Id, settings.BenchmarkDate);

            pickerViewModel.Loading += (sender, e) => this.CountOneLoading();
            pickerViewModel.Loaded += (sender, e) => this.CountOneLoaded();
            editorViewModel.Loading += (sender, e) => this.CountOneLoading();
            editorViewModel.Loaded += (sender, e) => this.CountOneLoaded();

            this.editorViewModel = editorViewModel;
            this.pickerViewModel = pickerViewModel;
        }

        // activating / deactivating
        public override void Activate()
        {
            this.pickerViewModel.Initialize();
        }

        public override void Deactivate()
        {
            // do nothing
        }


        // taking care of the loading spinner
        private Int32 currentlyLoading;
        protected void CountOneLoaded()
        {
            var total = Interlocked.Decrement(ref this.currentlyLoading);
            this.IsLoading = total > 0;
        }
        protected void CountOneLoading()
        {
            var total = Interlocked.Increment(ref this.currentlyLoading);
            this.IsLoading = total > 0;
        }

        // components
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

        // handling unsaved changes
        /// <summary>
        /// Safe check for modifications.
        /// Takes into account that the view model may not be initialized by the time the check is done.
        /// This property is not supposed to be bound, because it doesn't notify the view about getting changed.
        /// The main purpose of this property is to be used in OnNavigateFrom of the RootView in order to get a confirmation about leaving unsaved modifications.
        /// </summary>
        public override Boolean HasUnsavedChanges
        {
            get
            {
                var result = this.EditorViewModel != null && this.EditorViewModel.RootModel != null && this.EditorViewModel.RootModel.IsModified;
                return result;
            }
        }
    }
}
