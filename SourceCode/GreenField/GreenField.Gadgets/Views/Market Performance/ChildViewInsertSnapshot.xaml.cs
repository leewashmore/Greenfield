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
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace GreenField.Gadgets.Views
{
    public partial class ChildViewInsertSnapshot : ChildWindow
    {
        #region Fields
        private List<MarketSnapshotSelectionData> _marketSnapshotSelectionInfo = new List<MarketSnapshotSelectionData>(); 
        #endregion

        #region Contructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="marketSnapshotSelectionInfo">List of MarketSnapshotSelectionData objects already present in the snapshot</param>
        public ChildViewInsertSnapshot(List<MarketSnapshotSelectionData> marketSnapshotSelectionInfo)
        {
            InitializeComponent();
            _marketSnapshotSelectionInfo = marketSnapshotSelectionInfo;
        } 
        #endregion

        #region Event Handlers
        /// <summary>
        /// OK button Click Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_marketSnapshotSelectionInfo.Where(record => record.SnapshotName == this.tbSnapshotName.Text).Count().Equals(0))
            {
                this.txtMessage.Text = "*Snapshot by the name of " + this.tbSnapshotName.Text + " already exists. Provide an alternate name";
                this.txtMessage.Visibility = System.Windows.Visibility.Visible;
                return;
            }

            this.DialogResult = true;
        }

        /// <summary>
        /// Cancel button Click Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// tbSnapshotName TextChanged Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void tbSnapshotName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.txtMessage.Visibility = System.Windows.Visibility.Collapsed;
            this.OKButton.IsEnabled = this.tbSnapshotName.Text.Count() > 0;
        } 
        #endregion
    }
}

