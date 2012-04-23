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

namespace GreenField.App
{
    [Export]
    public partial class Shell : UserControl
    {
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
            }
        }
    }
}
