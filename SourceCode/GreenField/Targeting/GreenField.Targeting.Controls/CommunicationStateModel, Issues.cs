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
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Targeting.Controls
{
    public class IssuesCommunicationStateModel : AcknowledgeableCommunicationStateModelBase, ICommunicationStateModel
    {
        [DebuggerStepThrough]
        public IssuesCommunicationStateModel(ObservableCollection<IssueModel> issues)
        {
            this.Issues = issues;
        }

        public ObservableCollection<IssueModel> Issues { get; private set; }

        [DebuggerStepThrough]
        public void Accept(ICommunicationStateModelResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
