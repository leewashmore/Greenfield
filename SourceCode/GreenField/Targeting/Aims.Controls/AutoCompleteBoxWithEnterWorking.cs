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

namespace Aims.Controls
{
    public class AutoCompleteBoxWithEnterWorking : AutoCompleteBox
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                this.OnEnter();
            }
        }

        public event EventHandler Enter;
        protected virtual void OnEnter()
        {
            var handler = this.Enter;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
