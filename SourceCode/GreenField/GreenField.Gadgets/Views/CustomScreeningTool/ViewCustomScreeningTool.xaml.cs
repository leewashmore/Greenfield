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

namespace GreenField.Gadgets.Views
{
    public partial class ViewCustomScreeningTool : ViewBaseUserControl
    {
        ObservableCollection<MyDataRow> _data = new ObservableCollection<MyDataRow>();

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
            dataContextSource.RetrieveCustomXmlDataCompletedEvent +=new Common.RetrieveCustomXmlDataCompleteEventHandler(dataContextSource_RetrieveCustomXmlDataCompletedEvent);
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


            this.dgCustomSecurity.ItemsSource = null;
            this.dgCustomSecurity.Columns.Clear();

            // grab the xml into a XDocument
            XDocument xmlDoc = XDocument.Parse(e.XmlInfo);

            // find the columns
            List<string> columnNames = xmlDoc.Descendants("column")
                                             .Attributes("name")
                                             .Select(a => a.Value)
                                             .ToList();

            // add them to the grid
            foreach (string columnName in columnNames)
            {
                GridViewDataColumn column = new GridViewDataColumn();
                //  column.DataMemberBinding = new Binding(columnName);

                column.Header = columnName;
                column.UniqueName = columnName;
                this.dgCustomSecurity.Columns.Add(column);

            }



            // add the rows
            var rows = xmlDoc.Descendants("row");
            foreach (var row in rows)
            {
                MyDataRow rowData = new MyDataRow();

                foreach (string colName in columnNames)
                {
                    var cells = row.Descendants("Element").Where(a=>a.Attribute("name").Value == colName).ToList();
                    foreach (var cell in cells)
                    {
                        rowData[colName] = cell.Value;
                    }
                }
                _data.Add(rowData);
            }

            this.dgCustomSecurity.ItemsSource = _data;
            //this.dgCustomSecurity.on;
            // (this.dgCustomSecurity.Items[0])
        }
    

    }
}
