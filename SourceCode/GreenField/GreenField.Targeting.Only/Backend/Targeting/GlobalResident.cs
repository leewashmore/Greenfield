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
using System.ComponentModel;

namespace GreenField.Targeting.Only.Backend.Targeting
{
    public abstract partial class GlobeResident : IGlobeResident
    {
        public Int32 Level { get { return this.Parent != null ? this.Parent.Level + 1 : 0; } }

        public Boolean IsVisible
        {
            get
            {
                return this.Parent.IsExpanded;
            }
        }

        private IExpandableModel parent;
        public IExpandableModel Parent
        {
            get { return this.parent; }
            set
            {
                if (this.parent != null)
                {
                    this.parent.PropertyChanged -= this.ParentChangedProperty;
                }
                if (value != null)
                {
                    value.PropertyChanged += this.ParentChangedProperty;
                }
                this.parent = value;
            }
        }

        protected void ParentChangedProperty(object sender, PropertyChangedEventArgs e)
        {
            // parent has its IsExpanded property that can be changed at any moment
            // we want to catch that moment and notify that the IsVisible property (which is based on the parent's IsExpanded property has also changed)
            // but it's still not enough as the filtering is triggered by the CollectionChanged event of the collection that is bound to the gird!
            this.RaisePropertyChanged("IsVisible");
        }

        public abstract void Accept(IGlobeResidentResolver resolver);
    }
}
