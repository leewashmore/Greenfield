using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Targeting.Only
{
    public class ViewModelBase : NotificationObject
    {
        private Boolean isLoading;
        public Boolean IsLoading
        {
            get { return this.isLoading; }
            set
            {
                this.isLoading = value;
                if (value)
                {
                    this.OnLoading();
                }
                else
                {
                    this.OnLoaded();
                }
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }

        public event EventHandler Loading;
        protected virtual void OnLoading()
        {
            var handler = this.Loading;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public event EventHandler Loaded;
        protected virtual void OnLoaded()
        {
            var handler = this.Loaded;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
