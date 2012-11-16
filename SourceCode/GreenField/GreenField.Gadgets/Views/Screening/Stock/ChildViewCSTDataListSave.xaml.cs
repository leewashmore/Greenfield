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
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.ObjectModel;
using GreenField.Gadgets.ViewModels;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.cs class for ChildViewCSTDataListSave
    /// </summary>
    public partial class ChildViewCSTDataListSave : ChildWindow
    {
        #region Fields
        /// <summary>
        /// Enum for list accessibility
        /// </summary>
        public enum AccessbitlityMode
        {
            Private,
            Public
        }
        #endregion

        #region Properties

        /// <summary>
        /// FetchedFlag
        /// </summary>
        public string FetchedFlag { get; set; }

        /// <summary>
        /// FetchedAccessibility
        /// </summary>
        public string FetchedAccessibility { get; set; }

        /// <summary>
        /// FetchedListName
        /// </summary>
        public string FetchedListName { get; set; }

        /// <summary>
        /// Accessbitlity
        /// </summary>
        public AccessbitlityMode Accessbitlity { get; set; }

        /// <summary>
        /// SelectedAccessibility
        /// </summary>
        public string SelectedAccessibility { get; set; }

        /// <summary>
        /// FetchedCSTUserPreference
        /// </summary>
        public List<CSTUserPreferenceInfo> FetchedCSTUserPreference { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ChildViewCSTDataListSave()
        {
            InitializeComponent();
            // fetch flag set before navigation
            FetchedFlag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;

            // fetch the accessibillity of the data list that was selected before navigating here
            FetchedAccessibility = CSTNavigation.FetchString(CSTNavigationInfo.Accessibility) as string;

            // fetch the name of the data list that was selected before navigating here
            FetchedListName = CSTNavigation.FetchString(CSTNavigationInfo.ListName) as string;

            // fetch the user preference of the data list that was selected before navigating here
            FetchedCSTUserPreference = CSTNavigation.Fetch(CSTNavigationInfo.CSTUserPreference) as List<CSTUserPreferenceInfo>;

            if (FetchedFlag == "Edit")
            {
                txtDataListName.Text = FetchedListName;
                if (FetchedAccessibility.ToLower() == "public")
                {
                    rbtnPublic.IsChecked = true;
                    rbtnPrivate.IsChecked = false;
                }
                else
                {
                    rbtnPublic.IsChecked = false;
                    rbtnPrivate.IsChecked = true;
                }
            }
        }
        #endregion

        #region Events

        /// <summary>
        /// Method to check which radio button was selected
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null)
            {
                SelectedAccessibility = rb.Content as String; 
            }
            //if (Convert.ToBoolean(rbtnPublic.IsChecked))
            //{
            //    SelectedAccessibility = "Public";
            //}
            //else if (Convert.ToBoolean(rbtnPrivate.IsChecked))
            //{
            //    SelectedAccessibility = "Private";
            //}
        }

        /// <summary>
        /// Method to save data list
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAccessibility == null || string.IsNullOrWhiteSpace(txtDataListName.Text))
            {
                this.txtMessage.Text = "*Neither ListName nor Accessibility can be left empty.";
                this.txtMessage.Visibility = System.Windows.Visibility.Visible;
                return;
            }
            if (FetchedFlag == "Create")
            {
                if (FetchedCSTUserPreference.Select(r => r.ListName).Distinct().Contains(txtDataListName.Text))
                {
                    this.txtMessage.Text = "*List Name entered already exits.Choose a different Name.";
                    this.txtMessage.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
            }
            else if (FetchedFlag == "Edit")
            {
                List<CSTUserPreferenceInfo> temp = new List<CSTUserPreferenceInfo>();
                temp = this.FetchedCSTUserPreference;
                List<CSTUserPreferenceInfo> t = new List<CSTUserPreferenceInfo>();
                t = temp.Where(r => r.ListName == FetchedListName).Distinct().ToList();
                foreach (CSTUserPreferenceInfo item in t)
                {
                    temp.Remove(item);
                }
                if (temp.Select(r => r.ListName).Distinct().Contains(txtDataListName.Text))
                {
                    this.txtMessage.Text = "*List Name entered already exits.Choose a different Name.";
                    this.txtMessage.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
            }
            this.DialogResult = true;
        }

        /// <summary>
        /// btnCancel_Click
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion
    }
}

