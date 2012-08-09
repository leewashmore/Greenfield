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
using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.Views
{
    [Export]
    public partial class ViewMeetingsAgenda : UserControl
    {
        public ViewMeetingsAgenda()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelMeetingsAgenda DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
