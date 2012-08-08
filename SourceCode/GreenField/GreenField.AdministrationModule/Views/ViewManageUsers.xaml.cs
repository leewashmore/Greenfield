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
using GreenField.AdministrationModule.ViewModels;

namespace GreenField.AdministrationModule.Views
{
    [Export]
    public partial class ViewManageUsers : UserControl
    {
        public ViewManageUsers()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelManageUsers DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
