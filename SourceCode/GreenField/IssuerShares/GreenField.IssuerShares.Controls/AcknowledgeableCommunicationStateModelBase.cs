using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.IssuerShares.Controls
{
    public class AcknowledgeableCommunicationStateModelBase
    {
        public AcknowledgeableCommunicationStateModelBase()
        {
            this.AcknowledgeCommand = new DelegateCommand(this.Acknowledge);
        }

        public ICommand AcknowledgeCommand { get; private set; }

        public void Acknowledge()
        {
            this.OnAcknowledged();
        }

        public event EventHandler Acknowledged;
        protected virtual void OnAcknowledged()
        {
            var handler = this.Acknowledged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
