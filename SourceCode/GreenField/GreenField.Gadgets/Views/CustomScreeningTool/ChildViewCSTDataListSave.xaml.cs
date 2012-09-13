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
        }
        
        #endregion

        #region Properties

        public AccessbitlityMode Accessbitlity {get;  set; }

        public string SelectedAccessibility { get; set; }
        

        private bool _isSelectedRbtnPublic = true;
        public bool IsSelectedRbtnPublic
        {
            get { return _isSelectedRbtnPublic; }
            set
            {
                if (_isSelectedRbtnPublic != value)
                {
                    _isSelectedRbtnPublic = value;
                }
            }
        }

        private bool _isSelectedRbtnPrivate = false;
        public bool IsSelectedRbtnPrivate
        {
            get { return _isSelectedRbtnPrivate; }
            set
            {
                if (_isSelectedRbtnPrivate != value)
                {
                    _isSelectedRbtnPrivate = value;
                }              
            }
        }
        #endregion

        #region Events

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            //choiceTextBlock.Text = "You chose: " + rb.GroupName + ": " + rb.Name;
            if (IsSelectedRbtnPublic)
            {
                SelectedAccessibility = "Public";
            }
            else if (IsSelectedRbtnPrivate)
            {
                SelectedAccessibility = "Private";
            }
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

       

        #endregion

        #region CallBack Method(s)

      

        #endregion

    }
}

