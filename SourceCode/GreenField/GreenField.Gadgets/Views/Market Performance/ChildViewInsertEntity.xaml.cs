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
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Gadgets.Views
{
    public partial class ChildViewInsertEntity : ChildWindow
    {
        #region Fields
        List<string> _groupNames;
        IDBInteractivity _dBInteractivity;
        ILoggerFacade _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        /// <param name="groupName">GroupName where entity is being inserted</param>
        /// <param name="groupNames">GroupNames already inserted in the snapsot</param>
        public ChildViewInsertEntity(List<EntitySelectionData> result, IDBInteractivity dBInteractivity, ILoggerFacade logger, string groupName = null, List<string> groupNames = null)
        {
            InitializeComponent();
            _dBInteractivity = dBInteractivity;
            _logger = logger;
            if (groupName != null)
            {
                this.txtGroupName.Text = groupName;
                this.txtGroupName.IsEnabled = false;
            }

            this._groupNames = groupNames;
            this.DataContext = new ChildViewModelInsertEntity(result, _dBInteractivity, _logger);
            this.OKButton.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        } 
        #endregion

        #region Properties
        public MarketSnapshotPreference InsertedMarketSnapshotPreference { get; set; } 
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
            if (_groupNames != null)
            {
                if (_groupNames.Contains(InsertedMarketSnapshotPreference.GroupName))
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
            this.OKButton.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }

        /// <summary>
        /// cmbEntitySelection SelectionChanged Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cmbEntitySelection_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.OKButton.IsEnabled = this.txtGroupName.Text.Count() > 0 && this.cmbEntitySelection.SelectedItem != null;
        }        
        #endregion

        private void cmbBenchmarkFilterSelection_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}

