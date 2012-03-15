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
using Telerik.Windows.Controls;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    public class ItemStyleSelector : StyleSelector
    {
        public Style GroupStyle { get; set; }
        public Style ItemStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return (item is CollectionViewGroup) ? this.GroupStyle : this.ItemStyle;
        }
    }
}
