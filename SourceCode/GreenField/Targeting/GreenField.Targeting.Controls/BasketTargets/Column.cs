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

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class Column : DataGridBoundColumn
    {
        public static DependencyProperty CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(Column), new PropertyMetadata(null));
        public DataTemplate CellTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(Column.CellTemplateProperty);
            }
            set
            {
                base.SetValue(Column.CellTemplateProperty, value);
            }
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return this.GenerateElement(cell, dataItem);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var element = this.CellTemplate.LoadContent() as ContentControl;
            element.SetBinding(ContentControl.ContentProperty, this.Binding);
            return element;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            return null;
        }
    }
}
