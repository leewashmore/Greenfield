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
        
        private DateTime _presentationDate { get; set; }        
        IDBInteractivity _dBInteractivity;
        ILoggerFacade _logger;

        public enum AccessbitlityMode
        {
            Private,
            Public
        }

        #endregion

        #region Constructor
        
        public ChildViewCSTDataListSave(IDBInteractivity dBInteractivity, ILoggerFacade logger, DateTime presentationDate)
        {
            InitializeComponent();

            _dBInteractivity = dBInteractivity;
            _logger = logger;
            _presentationDate = presentationDate;
        }

        
        #endregion

        #region Properties

        public AccessbitlityMode SelectedAccessbitlity {get;  set; }

        private bool? _rbtnPublic = true;
        public bool? RbtnPublic
        {
            get { return _rbtnPublic; }
            set
            {
                if (_rbtnPublic != value)
                {
                    _rbtnPublic = value;
                    //RaisePropertyChanged(() => RbtnPublic);
                }
            }
        }

        private bool? _rbtnPrivate = false;
        public bool? RbtnPrivate
        {
            get { return _rbtnPrivate; }
            set
            {
                if (_rbtnPrivate != value)
                {
                    _rbtnPrivate = value;
                    //RaisePropertyChanged(() => RbtnPrivate);
                }              
            }
        }
        #endregion

        #region Events

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = true;
            //Validate data fields and save list name and accessibility flag
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

