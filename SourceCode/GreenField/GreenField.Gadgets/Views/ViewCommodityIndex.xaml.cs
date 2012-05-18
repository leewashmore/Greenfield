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

namespace GreenField.Gadgets.Views
{
    public partial class ViewCommodityIndex : ViewBaseUserControl
    {
        public ViewCommodityIndex(ViewModelCommodityIndex dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            //if(dataContextSource.CommodityDataLoadEvent != null)
            dataContextSource.CommodityDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceCommodityLoadEvent);

        }
        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceCommodityLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }
        #endregion
    }
}
