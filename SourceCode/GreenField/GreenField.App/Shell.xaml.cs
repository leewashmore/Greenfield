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
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.App.ViewModel;
using Telerik.Windows.Controls;
using Microsoft.Practices.Prism.Events;

namespace GreenField.App
{
    [Export]
    public partial class Shell : UserControl
    {
        [ImportingConstructor]
        public Shell()
        {
            InitializeComponent();            
        }

        [Import]
        public ViewModelShell DataContextSource
        {
            set
            {
                this.DataContext = value;
                //value.ShellDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(dataContextSource_ShellDataLoadEvent);
                //value.ShellFilterDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(dataContextSource_ShellFilterDataLoadEvent);
                //value.ShellSnapshotDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(dataContextSource_ShellSnapshotDataLoadEvent);                
            }
        }

        //void dataContextSource_ShellSnapshotDataLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        //{
        //    this.ctrToolBox.snapshotBusyIndicator.IsBusy = e.ShowBusy;
        //}

        //void dataContextSource_ShellFilterDataLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        //{
        //    this.ctrToolBox.filterBusyIndicator.IsBusy = e.ShowBusy;
        //}       

        //void dataContextSource_ShellDataLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        //{
        //    this.gridBusyIndicator.IsBusy = e.ShowBusy;
        //}        
    }
}
