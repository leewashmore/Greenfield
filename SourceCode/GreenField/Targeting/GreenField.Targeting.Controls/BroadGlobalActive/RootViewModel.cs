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

namespace GreenField.Targeting.Controls.BroadGlobalActive
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

            pickerViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            editorViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;

            this.EditorViewModel = editorViewModel;
            this.PickerViewModel = pickerViewModel;

            pickerViewModel.Picked += (sender, e) => this.ConsiderReloading(e.TargetingType.Id, e.Portfolio.Id, settings.BenchmarkDate);
            pickerViewModel.Reset += (sender, e) => this.ConsiderReseting();

            this.SaveCommand = new DelegateCommand(this.Save, this.CanSave);
            editorViewModel.GotData += (sender, e) => this.SaveCommand.RaiseCanExecuteChanged();
        }

        protected void ConsiderReseting()
        {
            if (this.CanGo())
            {
                this.EditorViewModel.Deactivate();
            }
        }

        protected void ConsiderReloading(Int32 targetingTypeId, String portfolioId, DateTime benchmarkDate)
        {
            if (this.CanGo())
            {
                this.EditorViewModel.RequestData(targetingTypeId, portfolioId, benchmarkDate);
            }
        }

        public void Save()
        {
            this.EditorViewModel.RequestSaving();
        }

        public Boolean CanSave()
        {
            return this.HasUnsavedChanges;
        }

        public DelegateCommand SaveCommand { get; private set; }

        // activating / deactivating
        public override void Activate()
        {
            this.PickerViewModel.RequestData();
        }

        public override void Deactivate()
        {
            this.PickerViewModel.Deactivate(true);
            this.EditorViewModel.Deactivate();
        }



        // reclaculating
        // when anything changes we need to do a roundtrip to the server which will update a model for us according to these changes
        public void Recalculate()
        {
            this.EditorViewModel.RequestRecalculating();
        }

        // components
        public PickerViewModel PickerViewModel { get; private set; }
        public EditorViewModel EditorViewModel { get; private set; }


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
