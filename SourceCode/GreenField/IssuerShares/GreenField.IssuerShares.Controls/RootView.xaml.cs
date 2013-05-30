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
using System.Diagnostics;

namespace GreenField.IssuerShares.Controls
{
    [Export]
    public partial class RootView : UserControl
    {
        [ImportingConstructor]
        public RootView(RootViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            viewModel.SecurityPickerViewModel.SecurityPickerLabelName = "Add Security to Composition";
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
