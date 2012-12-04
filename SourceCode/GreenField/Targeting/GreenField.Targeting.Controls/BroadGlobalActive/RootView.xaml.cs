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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    [Export]
    public partial class RootView : UserControl
    {
        /// <summary>
        /// This constructor is not supposed to be called explicitly (only via MEF).
        /// </summary>
        /// <param name="viewModel">This view model is going to be constructed by MEF, so don't try to set in manually.</param>
        [ImportingConstructor]
        [Obsolete("Don't call me.")]
        public RootView(RootViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
