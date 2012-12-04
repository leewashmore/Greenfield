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
using System.Collections.ObjectModel;
using TopDown.FacingServer.Backend.Targeting;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Targeting.Controls
{
    public class EditorViewModelBase : CommunicatingViewModelBase
    {
        public event EventHandler GotData;
        protected virtual void OnGotData()
        {
            var handler = this.GotData;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }


       
    }
}
