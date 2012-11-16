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
using Microsoft.Practices.Prism.Commands;

namespace GreenField.ServiceCaller.TargetingDefinitions
{
    public partial class OtherModel : IExpandableModel
    {
        public OtherModel()
        {
            this.ToggleExpandedCommand = new DelegateCommand<Object>(whatever => this.IsExpanded = !this.IsExpanded);
        }

        public ICommand ToggleExpandedCommand { get; private set; }


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
