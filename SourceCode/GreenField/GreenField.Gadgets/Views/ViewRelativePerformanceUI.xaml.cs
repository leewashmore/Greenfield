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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using Telerik.Windows.Controls;
using System.Windows.Printing;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRelativePerformanceUI : ViewBaseUserControl
    {
        #region PrivateVariables

        

        #endregion

        #region Constructor

        public ViewRelativePerformanceUI(ViewModelRelativePeroformanceUI dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.RelativePerformanceReturnDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(dataContextSource_RelativePerformanceReturnDataLoadedEvent);
        }
        
        #endregion

        #region DataProgressIndicator

        void dataContextSource_RelativePerformanceReturnDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }
        
        #endregion

        #region EventsUnsubscribe

        public override void Dispose()
        {

        }

        #endregion

        #region Printing

        #region PrivateVariablesPrinting

        double offsetY;
        double totalHeight;
        Canvas canvas;
        RadGridView grid;
        
        #endregion

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            offsetY = 0d;
            totalHeight = 0d;

            grid = new RadGridView();
            grid.DataContext = dgRelativePerformanceUI.DataContext;
            grid.ItemsSource = dgRelativePerformanceUI.ItemsSource;
            grid.RowIndicatorVisibility = Visibility.Collapsed;
            grid.ShowGroupPanel = false;
            grid.CanUserFreezeColumns = false;
            grid.IsFilteringAllowed = false;
            grid.AutoExpandGroups = true;
            grid.AutoGenerateColumns = false;

            foreach (GridViewDataColumn column in dgRelativePerformanceUI.Columns.OfType<GridViewDataColumn>())
            {
                GridViewDataColumn newColumn = new GridViewDataColumn();
                newColumn.DataMemberBinding = new System.Windows.Data.Binding(column.UniqueName);
                grid.Columns.Add(newColumn);
            }

            foreach (GridViewDataColumn column in grid.Columns.OfType<GridViewDataColumn>())
            {
                GridViewDataColumn currentColumn = column;

                GridViewDataColumn originalColumn = (from c in dgRelativePerformanceUI.Columns.OfType<GridViewDataColumn>()
                                                     where c.UniqueName == currentColumn.UniqueName
                                                     select c).FirstOrDefault();
                if (originalColumn != null)
                {
                    column.Width = originalColumn.ActualWidth;
                    column.DisplayIndex = originalColumn.DisplayIndex;
                }
            }

            StyleManager.SetTheme(grid, StyleManager.GetTheme(dgRelativePerformanceUI));

            grid.SortDescriptors.AddRange(dgRelativePerformanceUI.SortDescriptors);
            grid.GroupDescriptors.AddRange(dgRelativePerformanceUI.GroupDescriptors);
            grid.FilterDescriptors.AddRange(dgRelativePerformanceUI.FilterDescriptors);

            ScrollViewer.SetHorizontalScrollBarVisibility(grid, ScrollBarVisibility.Hidden);
            ScrollViewer.SetVerticalScrollBarVisibility(grid, ScrollBarVisibility.Hidden);

            PrintDocument doc = new PrintDocument();

            canvas = new Canvas();
            canvas.Children.Add(grid);

            doc.PrintPage += this.doc_PrintPage;
            doc.Print("RadGridView print");
        }

        void doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = canvas;

            if (totalHeight == 0)
            {
                totalHeight = grid.DesiredSize.Height;
            }

            Canvas.SetTop(grid, -offsetY);

            offsetY += e.PrintableArea.Height;
            e.HasMorePages = offsetY <= totalHeight;
        }

        #endregion

        #region ExportToExcel

        private static class ExportTypes
        {
            public const string RELATIVE_PERFORMANCE_UI = "Relative Performance UI";
        }
        
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.RELATIVE_PERFORMANCE_UI, Element = this.dgRelativePerformanceUI, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },                  
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

    }
}
