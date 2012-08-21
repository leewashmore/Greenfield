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
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.CS class for RelativePerformanceUI gadget
    /// </summary>
    public partial class ViewRelativePerformanceUI : ViewBaseUserControl
    {
        #region PrivateVariables

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of type View-Model
        /// </summary>
        private ViewModelRelativePeroformanceUI _dataContextRelativePerformanceUI;
        public ViewModelRelativePeroformanceUI DataContextRelativePerformanceUI
        {
            get { return _dataContextRelativePerformanceUI; }
            set { _dataContextRelativePerformanceUI = value; }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextRelativePerformanceUI != null)
                    DataContextRelativePerformanceUI.IsActive = _isActive;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceUI(ViewModelRelativePeroformanceUI dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceUI = dataContextSource;
        }

        #endregion

        #region Printing

        #region PrivateVariablesPrinting

        double offsetY;
        double totalHeight;
        Canvas canvas;
        RadGridView grid;

        #endregion

        /// <summary>
        /// Print Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RichTextBox.Document = PDFExporter.Print(dgRelativePerformanceUI, 10);
            }));

            RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);


            //offsetY = 0d;
            //totalHeight = 0d;

            //grid = new RadGridView();
            //grid.DataContext = dgRelativePerformanceUI.DataContext;
            //grid.ItemsSource = dgRelativePerformanceUI.ItemsSource;
            //grid.RowIndicatorVisibility = Visibility.Collapsed;
            //grid.ShowGroupPanel = false;
            //grid.CanUserFreezeColumns = false;
            //grid.IsFilteringAllowed = false;
            //grid.AutoExpandGroups = true;
            //grid.AutoGenerateColumns = false;

            //foreach (GridViewDataColumn column in dgRelativePerformanceUI.Columns.OfType<GridViewDataColumn>())
            //{
            //    GridViewDataColumn newColumn = new GridViewDataColumn();
            //    newColumn.DataMemberBinding = new System.Windows.Data.Binding(column.UniqueName);
            //    grid.Columns.Add(newColumn);
            //}

            //foreach (GridViewDataColumn column in grid.Columns.OfType<GridViewDataColumn>())
            //{
            //    GridViewDataColumn currentColumn = column;

            //    GridViewDataColumn originalColumn = (from c in dgRelativePerformanceUI.Columns.OfType<GridViewDataColumn>()
            //                                         where c.UniqueName == currentColumn.UniqueName
            //                                         select c).FirstOrDefault();
            //    if (originalColumn != null)
            //    {
            //        column.Width = originalColumn.ActualWidth;
            //        column.DisplayIndex = originalColumn.DisplayIndex;
            //    }
            //}

            //StyleManager.SetTheme(grid, StyleManager.GetTheme(dgRelativePerformanceUI));

            //grid.SortDescriptors.AddRange(dgRelativePerformanceUI.SortDescriptors);
            //grid.GroupDescriptors.AddRange(dgRelativePerformanceUI.GroupDescriptors);
            //grid.FilterDescriptors.AddRange(dgRelativePerformanceUI.FilterDescriptors);

            //ScrollViewer.SetHorizontalScrollBarVisibility(grid, ScrollBarVisibility.Hidden);
            //ScrollViewer.SetVerticalScrollBarVisibility(grid, ScrollBarVisibility.Hidden);

            //PrintDocument doc = new PrintDocument();

            //canvas = new Canvas();
            //canvas.Children.Add(grid);

            //doc.PrintPage += this.doc_PrintPage;
            //doc.Print("RadGridView print");
        }

        /// <summary>
        /// Prinitng Helper Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Class Containing the Name of Exported File
        /// </summary>
        private static class ExportTypes
        {
            public const string RELATIVE_PERFORMANCE_UI = "Relative Performance UI";
        }

        /// <summary>
        /// Export Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                //{
                //    new RadExportOptions() { ElementName = ExportTypes.RELATIVE_PERFORMANCE_UI, Element = this.dgRelativePerformanceUI, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },                  
                //};
                //ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE);
                //childExportOptions.Show();

                ExportExcel.ExportGridExcel(dgRelativePerformanceUI);

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        #endregion

        #region ApplyStyles

        /// <summary>
        /// Applying Styles to Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRelativePerformanceUI_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// Disposing UserControl
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceUI.Dispose();
            this.DataContextRelativePerformanceUI = null;
            this.DataContext = null;
        }

        #endregion
    }
}
