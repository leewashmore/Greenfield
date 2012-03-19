using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using GreenField.Benchmark.ViewModel;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Benchmark.Views
{
    [Export(typeof(ViewMorningSnapshot))]
    public partial class ViewMorningSnapshot : UserControl
    {
        //private RadGridView grid = null;
        /// <summary>
        /// Constructor
        /// </summary>        
        public ViewMorningSnapshot()
        {
            InitializeComponent();
            this.radGridBenchmark.Columns[2].Header = DateTime.Today.AddDays(-1).ToShortDateString();
            this.radGridBenchmark.Columns[7].Header = DateTime.Today.AddYears(-1).Year;
            this.radGridBenchmark.Columns[8].Header = DateTime.Today.AddYears(-2).Year;
            this.radGridBenchmark.Columns[9].Header = DateTime.Today.AddYears(-3).Year;
            
        }

        //public ViewMorningSnapshot(RadGridView grid)
        //{
        //    this.radGridBenchmark = grid;
        //}

        [Import]
        public ViewModelMorningSnapshot DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }       

        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            RadContextMenu menu = (RadContextMenu)sender;
            GridViewRow row = menu.GetClickedElement<GridViewRow>();

            if (row != null)
            {
                row.IsSelected = row.IsCurrent = true;
                GridViewCell cell = menu.GetClickedElement<GridViewCell>();
                if (cell != null)
                {
                    cell.IsCurrent = true;
                }
            }
            else
            {
                menu.IsOpen = false;
            }
        }

        //public static readonly DependencyProperty IsEnabledProperty
        //    = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ViewMorningSnapshot),
        //        new Telerik.Windows.PropertyMetadata(new PropertyChangedCallback(OnIsEnabledPropertyChanged)));

        //public static void SetIsEnabled(DependencyObject dependencyObject, bool enabled)
        //{
        //    dependencyObject.SetValue(IsEnabledProperty, enabled);
        //}

        //public static bool GetIsEnabled(DependencyObject dependencyObject)
        //{
        //    return (bool)dependencyObject.GetValue(IsEnabledProperty);
        //}

        //private static void OnIsEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        //{
        //    RadGridView grid = dependencyObject as RadGridView;
        //    if (grid != null)
        //    {
        //        if ((bool)e.NewValue)
        //        {
        //            // Create new GridViewHeaderMenu and attach RowLoaded event.
        //            //GridViewHeaderMenu menu = new GridViewHeaderMenu(grid);
        //            //menu.Attach();
        //        }
        //    }
        //}

    }
}