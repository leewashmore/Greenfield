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
using System.Windows.Interactivity;
using System.Windows.Data;

namespace GreenField.Targeting.Controls
{
	public class DataGridRowVisibilityBehavior : Behavior<DataGrid>, IValueConverter
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			this.AssociatedObject.LoadingRow += new EventHandler<DataGridRowEventArgs>(AssociatedObject_LoadingRow);
		}
		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.AssociatedObject.LoadingRow -= new EventHandler<DataGridRowEventArgs>(AssociatedObject_LoadingRow);
		}

		void AssociatedObject_LoadingRow(object sender, DataGridRowEventArgs e)
		{
			var binding = new Binding("IsVisible");
			binding.Converter = this;
			e.Row.SetBinding(DataGridRow.VisibilityProperty, binding);
		}

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((Boolean)value) ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
