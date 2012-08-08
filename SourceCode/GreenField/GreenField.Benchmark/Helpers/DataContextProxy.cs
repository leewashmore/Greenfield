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
using System.Windows.Data;

namespace GreenField.Benchmark.Helpers
{
    public class DataContextProxy : FrameworkElement
    {
        public DataContextProxy()
        {
            this.Loaded += new RoutedEventHandler(DataContextProxy_Loaded);
        }

        void DataContextProxy_Loaded(object sender, RoutedEventArgs e)
        {
            Binding binding = new Binding();
            if (!String.IsNullOrEmpty(BindingPropertyName))
            {
                binding.Path = new PropertyPath(BindingPropertyName);
            }
            binding.Source = this.DataContext;
            binding.Mode = BindingMode;
            this.SetBinding(DataContextProxy.DataSourceProperty, binding);
        }

        public Object DataSource
        {
            get { return (Object)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(Object), typeof(DataContextProxy), null);


        public string BindingPropertyName { get; set; }

        public BindingMode BindingMode { get; set; }

    }
}
