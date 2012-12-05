﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Targeting.Controls.BottomUp
{
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        public const Int32 MaxNumberOfSecuritiesInDropdown = 20;

        [ImportingConstructor]
        public RootViewModel(Settings settings)
        {
            var clientFactory = settings.ClientFactory;

            var pickerViewModel = new PortfolioPickerViewModel(clientFactory);
            pickerViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.PortfolioPickerViewModel = pickerViewModel;

            var editorViewModel = new EditorViewModel(clientFactory);
            editorViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.EditorViewModel = editorViewModel;

            var securityPickerViewModel = new SecurityPickerViewModel(clientFactory, MaxNumberOfSecuritiesInDropdown);
            this.SecurityPickerViewModel = securityPickerViewModel;

            pickerViewModel.Picked += (s, e) => this.ConsiderReloading(e.BottomUpPortfolio.Id);
            pickerViewModel.Reset += (s, e) => this.ConsiderReseting();
            securityPickerViewModel.SecurityPicked += (s, e) =>
            {
                this.EditorViewModel.AddSecurity(e.Security);
                this.SecurityPickerViewModel.Clear();
            };

            editorViewModel.GotData += (s, e) => this.SaveCommand.RaiseCanExecuteChanged();
            this.SaveCommand = new DelegateCommand(this.Save, this.CanSave);
        }

        protected void ConsiderReseting()
        {
            if (this.CanGo())
            {
                this.EditorViewModel.Deactivate();
            }
        }

        protected void ConsiderReloading(String bottomUpPortfolioId)
        {
            if (this.CanGo())
            {
                this.EditorViewModel.RequestData(bottomUpPortfolioId);
            }
        }

        public DelegateCommand SaveCommand { get; private set; }
        
        public void Save()
        {
            this.EditorViewModel.RequestSaving();
        }

        public Boolean CanSave()
        {
            return this.HasUnsavedChanges;
        }

        public PortfolioPickerViewModel PortfolioPickerViewModel { get; private set; }
        public EditorViewModel EditorViewModel { get; private set; }
        public SecurityPickerViewModel SecurityPickerViewModel { get; private set; }

        public override Boolean HasUnsavedChanges
        {
            get
            {
                return this.EditorViewModel != null && this.EditorViewModel.KeptRootModel != null && this.EditorViewModel.KeptRootModel.IsModified;
            }
        }

        public override void Activate()
        {
            this.PortfolioPickerViewModel.RequestData();
        }

        public override void Deactivate()
        {
            this.PortfolioPickerViewModel.Deactivate(true);
            this.EditorViewModel.Deactivate();
        }

       
    }
}