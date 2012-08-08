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
using GreenField.LoginModule.ViewModel;

namespace GreenField.LoginModule.Views
{
    [Export]
    public partial class ViewPasswordChangeForm : UserControl
    {
        public ViewPasswordChangeForm()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelPasswordChangeForm DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
