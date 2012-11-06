using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ChildViewInsertEntity
    /// </summary>
    public partial class ChildViewInsertEntity : ChildWindow
    {
        #region Fields
        /// <summary>
        /// Stores group names present on the selected benchmark
        /// </summary>
        List<String> groupNames;

        /// <summary>
        /// Service caller MEF singleton
        /// </summary>
        IDBInteractivity dBInteractivity;

        /// <summary>
        /// Logging MEF singleton
        /// </summary>
        ILoggerFacade logger;
        #endregion

        #region Properties
        /// <summary>
        /// Stores inserted market snapshot preference
        /// </summary>
        public MarketSnapshotPreference InsertedMarketSnapshotPreference { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        /// <param name="groupName">GroupName where entity is being inserted</param>
        /// <param name="groupNames">GroupNames already inserted in the snapshot</param>
        public ChildViewInsertEntity(List<EntitySelectionData> result, IDBInteractivity dBInteractivity
            , ILoggerFacade logger, string groupName = null, List<string> groupNames = null)
        {
            InitializeComponent();
            this.dBInteractivity = dBInteractivity;
            this.logger = logger;
            if (groupName != null)
            {
                this.txtGroupName.Text = groupName;
                this.txtGroupName.IsEnabled = false;
            }
            this.groupNames = groupNames;
            this.DataContext = new ChildViewModelInsertEntity(result, dBInteractivity, logger);
            this.btnOK.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
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
            InsertedMarketSnapshotPreference = (this.DataContext as ChildViewModelInsertEntity).SelectedMarketSnapshotPreference;
            InsertedMarketSnapshotPreference.GroupName = this.txtGroupName.Text;
            if (groupNames != null)
            {
                if (groupNames.Contains(InsertedMarketSnapshotPreference.GroupName))
                {
                    this.txtMessage.Text = "*Group name '" + InsertedMarketSnapshotPreference.GroupName
                        + "' is already present in this snapshot. Please input a different group name";
                    this.txtMessage.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
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
        /// txtGroupName TextChanged Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void txtGroupName_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.txtMessage.Visibility = System.Windows.Visibility.Collapsed;
            this.btnOK.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }

        /// <summary>
        /// cmbEntitySelection SelectionChanged Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cmbEntitySelection_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.btnOK.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }                
        #endregion
    }
}

