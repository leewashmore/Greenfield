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
using Telerik.Windows.Controls;
using System.IO;
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Data;
using GreenField.Gadgets.Helpers;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewIndexConstituents : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelIndexConstituents _dataContextIndexConstituents;
        public ViewModelIndexConstituents DataContextIndexConstituents
        {
            get { return _dataContextIndexConstituents; }
            set { _dataContextIndexConstituents = value; }
        }    
        
        #endregion     

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewIndexConstituents(ViewModelIndexConstituents dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.IndexConstituentDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceIndexConstituentLoadEvent);
            this.DataContextIndexConstituents = dataContextSource;
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceIndexConstituentLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        } 
        #endregion

        #region Export To Excel Methods
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = "xls",
                Filter = "Excel Workbook (*.xls)|*.xls|CSV (Comma delimited)|*.csv|Word Document (*.doc)|*.doc",
            };

            if (dialog.ShowDialog() == true)
            {
                ExportFormat format = dialog.FilterIndex == 1 ? ExportFormat.Html :
                    dialog.FilterIndex == 2 ? ExportFormat.Csv : ExportFormat.Html;
                using (Stream stream = dialog.OpenFile())
                {
                    GridViewExportOptions exportOptions = new GridViewExportOptions()
                    {
                        Format = format,
                        ShowColumnFooters = true,
                        ShowColumnHeaders = true,
                        ShowGroupFooters = true
                    };
                    this.dgIndexConstituents.Export(stream, exportOptions);
                }
            }

        }

        private void dgIndexConstituents_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            if (e.Element == ExportElement.HeaderRow || e.Element == ExportElement.FooterRow
                || e.Element == ExportElement.GroupFooterRow)
            {
                e.Background = Colors.Gray;
                e.Foreground = Colors.Black;
                e.FontSize = 20;
                e.FontWeight = FontWeights.Bold;
            }
            else if (e.Element == ExportElement.Row)
            {
                //e.Background = RowBackgroundPicker.SelectedColor;
                //e.Foreground = RowForegroundPicker.SelectedColor;
            }
            else if (e.Element == ExportElement.Cell &&
                e.Value != null && e.Value.Equals("Chocolade"))
            {
                e.FontFamily = new FontFamily("Verdana");
                e.Background = Colors.LightGray;
                e.Foreground = Colors.Blue;
            }
            else if (e.Element == ExportElement.GroupHeaderRow)
            {
                e.FontFamily = new FontFamily("Verdana");
                e.Background = Colors.LightGray;
                e.Height = 30;
            }
            else if (e.Element == ExportElement.GroupHeaderCell &&
                e.Value != null && e.Value.Equals("Chocolade"))
            {
                e.Value = "MyNewValue";
            }
            else if (e.Element == ExportElement.GroupFooterCell)
            {
                GridViewDataColumn column = e.Context as GridViewDataColumn;
                QueryableCollectionViewGroup qcvGroup = e.Value as QueryableCollectionViewGroup;

                if (column != null && qcvGroup != null && column.AggregateFunctions.Count() > 0)
                {
                    e.Value = GetAggregates(qcvGroup, column);
                }
            }
        } 
        #endregion

        private string GetAggregates(QueryableCollectionViewGroup group, GridViewDataColumn column)
        {
            List<string> aggregates = new List<string>();

            foreach (AggregateFunction f in column.AggregateFunctions)
            {
                foreach (AggregateResult r in group.AggregateResults)
                {
                    if (f.FunctionName == r.FunctionName && r.FormattedValue != null)
                    {
                        aggregates.Add(r.FormattedValue.ToString());
                    }
                }
            }

            return String.Join(",", aggregates.ToArray());
        }
                
        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextIndexConstituents.Dispose();
            this.DataContextIndexConstituents.IndexConstituentDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceIndexConstituentLoadEvent);
            this.DataContextIndexConstituents = null;
            this.DataContext = null;
        } 
        #endregion
    }
}
