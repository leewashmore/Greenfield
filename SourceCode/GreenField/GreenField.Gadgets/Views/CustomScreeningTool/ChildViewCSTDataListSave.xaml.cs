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
    public partial class ChildViewCSTDataListSave : ChildWindow
    {
        #region Fields

        public enum AccessbitlityMode
        {
            Private,
            Public
        }

        #endregion

        #region Constructor
        
        public ChildViewCSTDataListSave()
        {
            InitializeComponent();
            FetchedFlag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            FetchedAccessibility = CSTNavigation.FetchString(CSTNavigationInfo.Accessibility) as string;
            FetchedListName = CSTNavigation.FetchString(CSTNavigationInfo.ListName) as string;
            FetchedCSTUserPreference = CSTNavigation.Fetch(CSTNavigationInfo.CSTUserPreference) as List<CSTUserPreferenceInfo>;
            if (FetchedFlag == "Edit")
            {
                //ListName = FetchedListName;
                txtDataListName.Text = FetchedListName;
                //ListName = txtDataListName.Text;
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

        #region Properties

        public string FetchedFlag { get; set; }

        public string FetchedAccessibility { get; set; }

        public string FetchedListName { get; set; }

        public AccessbitlityMode Accessbitlity {get;  set; }

        public string SelectedAccessibility { get; set; }

        public List<CSTUserPreferenceInfo> FetchedCSTUserPreference { get; set; }

        //public string ListName { get; set; }
        

        //private bool _isSelectedRbtnPublic = true;
        //public bool IsSelectedRbtnPublic
        //{
        //    get { return _isSelectedRbtnPublic; }
        //    set
        //    {
        //        if (_isSelectedRbtnPublic != value)
        //        {
        //            _isSelectedRbtnPublic = value;
        //        }
        //    }
        //}

        //private bool _isSelectedRbtnPrivate = false;
        //public bool IsSelectedRbtnPrivate
        //{
        //    get { return _isSelectedRbtnPrivate; }
        //    set
        //    {
        //        if (_isSelectedRbtnPrivate != value)
        //        {
        //            _isSelectedRbtnPrivate = value;
        //        }              
        //    }
        //}
        #endregion

        #region Events

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            //choiceTextBlock.Text = "You chose: " + rb.GroupName + ": " + rb.Name;
            if (Convert.ToBoolean(rbtnPublic.IsChecked))
            {
                SelectedAccessibility = "Public";
            }
            else if (Convert.ToBoolean(rbtnPrivate.IsChecked))
            {
                SelectedAccessibility = "Private";
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAccessibility == null || txtDataListName.Text.Count() < 0 || txtDataListName.Text == "")
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
                //if (FetchedListName.Equals(txtDataListName.Text))
                //{
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
                //}
            }
            this.DialogResult = true;            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// tbSnapshotName TextChanged Event Handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">TextChangedEventArgs</param>
        private void tbListName_TextChanged(object sender, TextChangedEventArgs e)
        {
           // this.btnSave.IsEnabled = this.txtDataListName.Text.Count() > 0;
        } 

        #endregion

    }
}

