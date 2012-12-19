using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Commands;
using Aims.Controls;
using System.Windows.Threading;

namespace GreenField.Targeting.Controls.BasketTargets
{
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        public const Int32 MaxNumberOfSecurities = 20;
        private SecurityPickerClientFactory securityPickerClientFactory;

        [ImportingConstructor ]
        public RootViewModel(Settings settings)
        {
            var pickerViewModel = new PickerViewModel(settings.ClientFactory);
            pickerViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.PickerViewModel = pickerViewModel;

            var editorViewModel = new EditorViewModel(settings.ClientFactory);
            editorViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.EditorViewModel = editorViewModel;

            this.securityPickerClientFactory = new SecurityPickerClientFactory(settings.ClientFactory);
            var securityPickerViewModel = new SecurityPickerViewModel(
                new OnlyErrorCommunicationState(this),
                this.securityPickerClientFactory
            );
            this.SecurityPickerViewModel = securityPickerViewModel;

            pickerViewModel.Picking += (s, e) =>
            {
                e.IsCancelled = !this.ConsiderReloading(e.TargetingTypeGroupId, e.BasketId);
            };

            pickerViewModel.Reseting += (s, e) =>
            {
                e.IsCancelled = !this.ConsiderReseting();
            };

            securityPickerViewModel.SecurityPicked += (s, e) =>
            {
                this.EditorViewModel.AddSecurity(e.Security);
                this.SecurityPickerViewModel.Clear();
            };

            this.SaveCommand = new DelegateCommand(this.Save, this.CanSave);
            editorViewModel.GotData += (s, e) => this.ReactOnDataBeingGotten();
        }

        protected override void TakeDispatcher(Dispatcher dispatcher)
        {
            this.EditorViewModel.Dispatcher = dispatcher;
            this.PickerViewModel.Dispatcher = dispatcher;
        }


        protected void ReactOnDataBeingGotten()
        {
            this.SaveCommand.RaiseCanExecuteChanged(); // poke the save button
            this.SecurityPickerViewModel.IsEnabled = true; // can add securities now
            this.securityPickerClientFactory.BasketId = this.EditorViewModel.LastBasketId; // update the basket id for the picker
        }

        protected Boolean ConsiderReseting()
        {
            var result = this.CanGo();
            if (result)
            {
                this.EditorViewModel.Discard();
            }
            return result;
        }

        protected Boolean ConsiderReloading(Int32 targetingTypeGroupId, Int32 basketId)
        {
            var result = this.CanGo();
            if (result)
            {
                this.EditorViewModel.RequestData(targetingTypeGroupId, basketId);
            }
            return result;
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
            this.PickerViewModel.Deactivate(true);
            this.EditorViewModel.Discard();
            this.SecurityPickerViewModel.IsEnabled = false;
            this.securityPickerClientFactory.BasketId = null;
        }




    }
}
