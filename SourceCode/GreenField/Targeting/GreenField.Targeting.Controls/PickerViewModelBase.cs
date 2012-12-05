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

namespace GreenField.Targeting.Controls
{
    public class PickerViewModelBase : CommunicatingViewModelBase
    {
        public event EventHandler Reset;
        protected virtual void OnReset()
        {
            if (!this.IsSilent)
            {
                var handler = this.Reset;
                if (handler != null)
                {
                    handler(this, new EventArgs());
                }
            }
        }

        protected bool IsSilent { get; set; }
    }
}
