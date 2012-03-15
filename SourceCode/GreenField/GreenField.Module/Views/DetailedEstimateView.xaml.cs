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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using GreenField.Module.ViewModels;

namespace GreenField.Module.Views
{
    [Export] 
    public partial class DetailedEstimateView : UserControl
    {
        public DetailedEstimateView()
        {
            InitializeComponent();
        }

        [Import]
        public DetailedEstimateViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
