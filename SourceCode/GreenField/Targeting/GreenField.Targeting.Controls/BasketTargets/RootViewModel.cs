using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Targeting.Controls.BasketTargets
{
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        public const Int32 MaxNumberOfSecurities = 20;

        [ImportingConstructor]
        public RootViewModel(Settings settings)
        {
            var pickerViewModel = new PickerViewModel(settings.ClientFactory);
            pickerViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.PickerViewModel = pickerViewModel;

            var editorViewModel = new EditorViewModel(settings.ClientFactory, settings.BenchmarkDate);
            editorViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.EditorViewModel = editorViewModel;

            var securityPickerViewModel = new SecurityPickerViewModel(settings.ClientFactory, MaxNumberOfSecurities);
            this.SecurityPickerViewModel = securityPickerViewModel;

            pickerViewModel.Picked += (s, e) => this.ConsiderReloading(e.TargetingTypeGroupId, e.BasketId);
            pickerViewModel.Reset += (s, e) => this.ConsiderReseting();
            securityPickerViewModel.SecurityPicked += (s, e) =>
            {
                this.EditorViewModel.AddSecurity(e.Security);
                this.SecurityPickerViewModel.Clear();
            };

            this.SaveCommand = new DelegateCommand(this.Save, this.CanSave);
            editorViewModel.GotData += (s, e) => this.SaveCommand.RaiseCanExecuteChanged();
        }

        protected void ConsiderReseting()
        {
            if (this.CanGo())
            {
                this.EditorViewModel.Deactivate();
            }
        }

        protected void ConsiderReloading(Int32 targetingTypeGroupId, Int32 basketId)
        {
            if (this.CanGo())
            {
                this.EditorViewModel.RequestData(targetingTypeGroupId, basketId);
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


        public PickerViewModel PickerViewModel { get; private set; }
        public EditorViewModel EditorViewModel { get; private set; }
        public SecurityPickerViewModel SecurityPickerViewModel { get; set; }

        public override Boolean HasUnsavedChanges
        {
            get
            {
                return this.EditorViewModel != null && this.EditorViewModel.KeptRootModel != null && this.EditorViewModel.KeptRootModel.IsModified;
            }
        }

        public override void Activate()
        {
            this.PickerViewModel.RequestData();
        }

        public override void Deactivate()
        {
            this.PickerViewModel.Deactivate();
            this.EditorViewModel.Deactivate();
        }



        
    }
}
