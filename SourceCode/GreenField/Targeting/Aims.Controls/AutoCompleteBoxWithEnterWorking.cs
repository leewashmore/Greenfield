using System;
using System.Windows.Controls;
using System.Windows.Input;

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
