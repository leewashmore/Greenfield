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
using GreenField.LoginModule.Controls;
using System.Windows.Controls.Primitives;

namespace GreenField.LoginModule.Views
{
    [Export]
    public partial class ViewRegisterForm : UserControl
    {
        public ViewRegisterForm()
        {
            InitializeComponent();
            
        }

        [Import]
        public ViewModelRegisterForm DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
