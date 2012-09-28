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
using Telerik.Windows.Controls;
using System.Xml.Linq;
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller;
using GreenField.Common;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Views
{
    public partial class ViewCustomScreeningTool : ViewBaseUserControl
    {


        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCustomScreeningTool _dataContextViewModelCustomScreeningTool;
        public ViewModelCustomScreeningTool DataContextViewModelCustomScreeningTool
        {
            get { return _dataContextViewModelCustomScreeningTool; }
            set { _dataContextViewModelCustomScreeningTool = value; }
        }



        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextViewModelCustomScreeningTool != null) //DataContext instance
                    DataContextViewModelCustomScreeningTool.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCustomScreeningTool(ViewModelCustomScreeningTool dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCustomScreeningTool = dataContextSource;
            dataContextSource.RetrieveCustomXmlDataCompletedEvent += new Common.RetrieveCustomXmlDataCompleteEventHandler(dataContextSource_RetrieveCustomXmlDataCompletedEvent);
        }


        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCustomScreeningTool.Dispose();
            this.DataContextViewModelCustomScreeningTool = null;
            this.DataContext = null;
        }
        #endregion

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        private void dataContextSource_RetrieveCustomXmlDataCompletedEvent(Common.RetrieveCustomXmlDataCompleteEventArgs e)
        {

            ObservableCollection<MyDataRow> _data = new ObservableCollection<MyDataRow>();
            Dictionary<string, string> columnMapping = new Dictionary<string, string>();
            Dictionary<string, string> columnMappingForAggregates = new Dictionary<string, string>();

            this.dgCustomSecurity.ItemsSource = null;
            this.dgCustomSecurity.Columns.Clear();

            // grab the xml into a XDocument
            XDocument xmlDoc = XDocument.Parse(e.XmlInfo);

            // find the columns
            List<String> columnNames = xmlDoc.Descendants("column")
                .Where(a => a.Attribute("isaggregate").Value == "false")
                .Attributes("name")                                             
                .Select(a => a.Value)
                .ToList();

            foreach (string colName in columnNames)
            {
                string displayName = xmlDoc.Descendants("column")
                                             .Attributes("displayname")
                                             .Where(a => a.PreviousAttribute.Value == colName)
                                             .FirstOrDefault().Value;

                columnMapping.Add(colName, displayName);
            }

            foreach (KeyValuePair<string, string> kvp in columnMapping)
            {
                GridViewDataColumn column = new GridViewDataColumn();
                column.Header = kvp.Value;
                column.UniqueName = kvp.Value;
                column.DataMemberBinding = new System.Windows.Data.Binding(kvp.Key);
                column.IsFilterable = true;
                column.IsGroupable = true;
                column.HeaderCellStyle = this.Resources["GridViewHeaderCellStyle"] as Style;
                column.CellStyle = this.Resources["GridViewCellStyle"] as Style;
                column.Width = new GridViewLength(1, GridViewLengthUnitType.Auto);               
                this.dgCustomSecurity.Columns.Add(column);
            }

            List<String> columnNamesWithAggregation = xmlDoc.Descendants("column")
                .Where(a => a.Attribute("isaggregate").Value == "true")
                .Attributes("name")
                .Select(a => a.Value)
                .ToList();

            foreach (string colName in columnNamesWithAggregation)
            {
                string displayName = xmlDoc.Descendants("column")
                                             .Attributes("displayname")
                                             .Where(a => a.PreviousAttribute.Value == colName)
                                             .FirstOrDefault().Value;

                columnMappingForAggregates.Add(colName, displayName);
            }

            foreach (KeyValuePair<string, string> kvp in columnMappingForAggregates)
            {
                GridViewDataColumn column = new GridViewDataColumn();
                column.Header = kvp.Value;
                column.UniqueName = kvp.Value;
                column.DataMemberBinding = new System.Windows.Data.Binding(kvp.Key);
                column.IsFilterable = true;
                column.IsGroupable = true;
                column.HeaderCellStyle = this.Resources["GridViewHeaderCellStyle"] as Style;
                column.CellStyle = this.Resources["GridViewCellStyle"] as Style;
                column.Width = new GridViewLength(1, GridViewLengthUnitType.Auto);
                //column.AggregateFunctions.Add(new HarmonicMeanCalculation { SourceField = kvp.Key });
                //column.AggregateFunctions.Add(new HarmonicMeanCalculation ());
                this.dgCustomSecurity.Columns.Add(column);             
            }

            this.dgCustomSecurity.ShowColumnFooters = true;

            foreach (KeyValuePair<string, string> kvp in columnMappingForAggregates)
            {
                columnMapping.Add(kvp.Key, kvp.Value);
            }

            // add the rows
            var rows = xmlDoc.Descendants("row");
            foreach (var row in rows)
            {
                MyDataRow rowData = new MyDataRow();

                foreach (KeyValuePair<string, string> kvp in columnMapping)
                {
                    var cells = row.Descendants("Element").Where(a => a.Attribute("name").Value == kvp.Key).ToList();
                    foreach (var cell in cells)
                    {
                        rowData[kvp.Key] = cell.Value;
                    }                 
                }
                _data.Add(rowData);
            }

            this.dgCustomSecurity.ItemsSource = _data;
            this.dgCustomSecurity.IsFilteringAllowed = true;
            this.dgCustomSecurity.GroupRowStyle = this.Resources["GridViewGroupRowStyle"] as Style;
            this.dgCustomSecurity.ShowGroupFooters = true;
            this.dgCustomSecurity.Columns["Market Capitalization"].IsVisible = false;           
            this.DataContextViewModelCustomScreeningTool.BusyIndicatorIsBusy = false;
            this.DataContextViewModelCustomScreeningTool.FlagBusyIndicator = 1;
        }

        #region Excel Export
        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgCustomSecurity.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                    {                  
                        new RadExportOptions() { ElementName = "Custom Screening Tool", Element = this.dgCustomSecurity, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.CUSTOM_SCREENING_TOOL);
                    childExportOptions.Show();
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        } 
        #endregion

        #region PDFExport
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            PDFExporter.btnExportPDF_Click(this.dgCustomSecurity);
        }
        #endregion

        #region Print grid

        #region Printing the DataGrid

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RichTextBox.Document = PDFExporter.Print(dgCustomSecurity, 6);
            }));

            this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
            RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        }

        #endregion
        #endregion
    }
}
