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
using GreenField.Common;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewEMSummaryMarketData : ViewBaseUserControl
    {
        List<EMSummaryMarketData> data = new List<EMSummaryMarketData>();
        public ViewEMSummaryMarketData(ViewModelEMSummaryMarketData dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.RetrieveEMSummaryDataCompletedEvent += new
                RetrieveEMSummaryDataCompleteEventHandler(dataContextSource_RetrieveEMSummaryDataCompletedEvent);
        }
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        private void dataContextSource_RetrieveEMSummaryDataCompletedEvent(Common.RetrieveEMSummaryDataCompleteEventArgs e)
        {
            data = e.EMSummaryInfo;

            if (data != null)
            {
                for (int i = 4; i < this.dgEMSummaryMarketData.Columns.Count; i++)
                {
                    String propertyName = ((GridViewDataColumn)this.dgEMSummaryMarketData.Columns[i])
                        .DataMemberBinding.Path.Path.ToString();
                    this.dgEMSummaryMarketData.Columns[i].AggregateFunctions.Add(new AggregateFunctionEMDataSummary() { SourceField = propertyName });
                }
                this.dgEMSummaryMarketData.ItemsSource = data;

                InitializeGridHeaders();
            }
        }
        private void InitializeGridHeaders()
        {
            //this.dgEMSummaryMarketData.ColumnGroups[1].Header = data.First().BenchmarkId;
            //DateTime portfolioDate = data.First().PortfolioDate;
            //this.dgEMSummaryMarketData.Columns[1].Header = portfolioDate.ToString("M/dd/yyyy");

            this.dgEMSummaryMarketData.Columns[5].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[6].Header = String.Format("{0} C", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[7].Header = String.Format("{0}", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketData.Columns[8].Header = String.Format("{0} C", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketData.Columns[9].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[10].Header = String.Format("{0} C", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[11].Header = String.Format("{0}", DateTime.Now.Year - 1999);
            this.dgEMSummaryMarketData.Columns[12].Header = String.Format("{0} C", DateTime.Now.Year - 1999);

            this.dgEMSummaryMarketData.Columns[13].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[14].Header = String.Format("{0}", DateTime.Now.Year - 2000);
            this.dgEMSummaryMarketData.Columns[15].Header = String.Format("{0}", DateTime.Now.Year - 2000);

            foreach (GridViewColumnGroup item in this.dgEMSummaryMarketData.ColumnGroups)
            {

            }
        }
    }
}
