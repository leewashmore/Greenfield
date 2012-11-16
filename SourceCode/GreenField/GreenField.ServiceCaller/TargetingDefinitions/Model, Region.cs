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
using System.Diagnostics;

namespace GreenField.ServiceCaller.TargetingDefinitions
{
    public partial class RegionModel : IExpandableModel
    {
        private class _ToggleExpandedCommand : ICommand
        {
            private RegionModel model;

            public _ToggleExpandedCommand(RegionModel regionModel)
            {
                this.model = regionModel;
            }

            public Boolean CanExecute(Object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(Object parameter)
            {
                this.model.IsExpanded = !this.model.IsExpanded;
            }
        }

        public RegionModel()
        {
            this.ToggleExpandedCommand = new _ToggleExpandedCommand(this);
        }
        
        public ICommand ToggleExpandedCommand { get; private set; }
        public void ToggleExpanded()
        {
            var value = this.IsExpanded;
            this.IsExpanded = !value;
        }

        private Boolean isExpanded;
        public Boolean IsExpanded
        {
            get { return this.Parent.IsExpanded && this.isExpanded; }
            set { this.isExpanded = value; this.RaisePropertyChanged("IsExpanded"); }
        }
        
        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
