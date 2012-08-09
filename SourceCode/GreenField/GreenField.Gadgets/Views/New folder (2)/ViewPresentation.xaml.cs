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
using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;
using System.ComponentModel.Composition;

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.Views
{
    [Export]
    public partial class ViewPresentation : UserControl
    {
        public ViewPresentation()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelICPresenterOverview DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
