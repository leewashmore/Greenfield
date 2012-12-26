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
using Microsoft.Practices.Prism.Commands;

namespace GreenField.IssuerShares.App
{
    [Export]
    public partial class ShellView : UserControl
    {
        [ImportingConstructor]
        public ShellView(ShellViewModel viewModel)
        {
            this.DataContext = viewModel;
            this.InitializeComponent();
            
        }

    }
}
