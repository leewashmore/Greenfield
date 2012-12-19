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
using System.Windows.Threading;

namespace GreenField.Targeting.Controls
{
    public class PickerViewModelBase : CommunicatingViewModelBase
    {
        public PickerViewModelBase()
        {
        }

        public event CancellableEventHandler Reseting;
        protected virtual void OnReseting(CancellableEventArgs args)
        {
            if (!this.IsSilent)
            {
                var handler = this.Reseting;
                if (handler != null)
                {
                    handler(this, args);
                }
            }
        }

        protected bool IsSilent { get; set; }
    }
}
