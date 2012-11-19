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

namespace GreenField.Targeting.Only.Backend.Targeting
{
    public partial class GlobeModel : IExpandableModel
    {
        private Boolean isExpanded;
        public Boolean IsExpanded
        {
            get { return this.isExpanded; }
            set { this.isExpanded = value; this.RaisePropertyChanged("IsExpanded"); }
        }

        public Int32 Level
        {
            get { return -1; }
        }
    }
}
