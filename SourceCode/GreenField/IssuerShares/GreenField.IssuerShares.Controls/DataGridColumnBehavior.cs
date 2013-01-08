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
using System.Linq;
using System.Collections.ObjectModel;
using GreenField.IssuerShares.Client.Backend.IssuerShares;

namespace GreenField.IssuerShares.Controls
{
    public class DataGridDynamicColumnsBehavior : Behavior<DataGrid>, IValueConverter
    {
        public static readonly DependencyProperty AreEmptyColumnShownProperty = DependencyProperty.Register("AreEmptyColumnShown", typeof(Boolean), typeof(DataGridDynamicColumnsBehavior), new PropertyMetadata(WhenAreEmptyColumnShownChanges));
        public Boolean AreEmptyColumnShown
        {
            get { return (Boolean)this.GetValue(AreEmptyColumnShownProperty); }
            set { this.SetValue(AreEmptyColumnShownProperty, value); }
        }

        private static void WhenAreEmptyColumnShownChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as DataGridDynamicColumnsBehavior;
            var before = (Boolean)e.OldValue;
            var after = (Boolean)e.NewValue;

            if (before != after)
            {
                self.SetEmptyColumnsVisibility(after);
            }
        }

        protected void SetEmptyColumnsVisibility(Boolean isVisible)
        {
            var grid = this.AssociatedObject;
            foreach (var column in grid.Columns)
            {
                if (this.IsEmptyColumn(column))
                {
                    column.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private Boolean IsEmptyColumn(DataGridColumn column)
        {
            var grid = this.AssociatedObject;
            var castedColumn = column as Column;
            if (castedColumn == null) return false;

            var badColumnsCount = grid.Columns.Count(x => x is Column);
            var goodColumnsCount = grid.Columns.Count - badColumnsCount;

            foreach (var something in grid.ItemsSource)
            {
                var data = something as IssuerSecurityShareRecordModel;
                if (data == null) continue;
                var index = column.DisplayIndex - goodColumnsCount;
                //if (data.PortfolioTargets[index].PortfolioTarget.EditedValue.HasValue)
                //{
                //    return false;
                //}
            }
            return true;
        }



        public static readonly DependencyProperty ColumnsDataProperty = DependencyProperty.Register("ColumnsData", typeof(Object), typeof(DataGridDynamicColumnsBehavior), new PropertyMetadata(OnColumnsDataChanged));
        public Int32 ColumnsData
        {
            get { return (Int32)this.GetValue(ColumnsDataProperty); }
            set { this.SetValue(ColumnsDataProperty, value); }
        }

        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridDynamicColumnsBehavior), new PropertyMetadata(null));
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)this.GetValue(CellTemplateProperty); }
            set { this.SetValue(CellTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(DataGridDynamicColumnsBehavior), new PropertyMetadata(null));
        public Style HeaderStyle
        {
            get { return (Style)this.GetValue(HeaderStyleProperty); }
            set { this.SetValue(HeaderStyleProperty, value); }
        }

        protected static void OnColumnsDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var self = dependencyObject as DataGridDynamicColumnsBehavior;
            var grid = self.AssociatedObject;

            for (var index = grid.Columns.Count - 1; index >= 0; index--)
            {
                var column = grid.Columns[index];
                if (column is Column)
                {
                    grid.Columns.RemoveAt(index);
                }
            }

            var portfolios = e.NewValue as ObservableCollection<IssuerSecurityShareRecordModel>;
            if (portfolios != null)
            {
                var indexOffset = grid.Columns.Count;
                foreach (var portfolio in portfolios)
                {
                    var column = new Column();
                    var index = grid.Columns.Count - indexOffset;
                    column.Binding = new Binding(String.Empty)
                    {
                        ConverterParameter = portfolio.SecurityId,
                        Converter = self
                    };
                    column.CellTemplate = self.CellTemplate;
                    column.HeaderStyle = self.HeaderStyle;
                    column.Header = new HeaderInfo { Name = portfolio.SecurityTicker, Index = index };
                    grid.Columns.Add(column);
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        // this class is also a value converter which we use to wrap whatever data object is given into something that has the index of the column too
        // we get the index as a converter parameter
        // this converter is used for binding defined few lines above in the code
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var line = value as HistoryLineModel;
            var ticker = (int?)parameter;
            //var result = new DataAndIndexWrap(index, line);
            var result = line.Values[ticker];
            return result;
            //return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // do no modifications
            return value;
        }


    }
}
