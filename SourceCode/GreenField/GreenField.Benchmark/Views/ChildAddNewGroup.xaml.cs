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
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Benchmark.Views
{
    public partial class ChildAddNewGroup : ChildWindow
    {
        private List<string> _groupNames;

        public ChildAddNewGroup(string title, List<string> groupNames)
        {
            InitializeComponent();
            _groupNames = groupNames;
            this.Title = title;
        }

        
        public string GroupName { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            GroupName = txtEnterValue.Text;
            bool duplicateGroupValidation = _groupNames!=null ? _groupNames.Contains(GroupName) : false;
            if (duplicateGroupValidation)
            {
                MessageBox.Show("Group Name already exists. Please enter another group identifier");
                return;
            }

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtEnterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            OKButton.IsEnabled = txtEnterValue.Text != "";
        }
    }
}

