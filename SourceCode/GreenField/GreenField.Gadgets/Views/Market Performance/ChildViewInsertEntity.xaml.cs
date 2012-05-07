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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace GreenField.Gadgets.Views
{
    public partial class ChildViewInsertEntity : ChildWindow
    {
        private List<string> _groupNames;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        /// <param name="groupName">GroupName where entity is being inserted</param>
        /// <param name="groupNames">GroupNames already inserted in the snapsot</param>
        public ChildViewInsertEntity(List<EntitySelectionData> result, string groupName = null, List<string> groupNames = null)
        {
            InitializeComponent();
            if (groupName != null)
            {
                this.txtGroupName.Text = groupName;
                this.txtGroupName.IsEnabled = false;
            }

            this._groupNames = groupNames;
            this.DataContext = new ChildViewModelInsertEntity(result);
            this.OKButton.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }

        public MarketSnapshotPreference InsertedMarketSnapshotPreference { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            InsertedMarketSnapshotPreference = (this.DataContext as ChildViewModelInsertEntity).SelectedMarketSnapshotPreference;
            InsertedMarketSnapshotPreference.GroupName = this.txtGroupName.Text;
            if (_groupNames != null)
            {
                if (_groupNames.Contains(InsertedMarketSnapshotPreference.GroupName))
                {
                    MessageBox.Show("Group name '" + InsertedMarketSnapshotPreference.GroupName
                        + "' is already present in this snapshot. Please input a different group name");
                    return;
                } 
            }
            this.DialogResult = true;
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;            
        }

        private void txtGroupName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.OKButton.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }

        private void cmbEntitySelection_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.OKButton.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }       


        
    }
}

