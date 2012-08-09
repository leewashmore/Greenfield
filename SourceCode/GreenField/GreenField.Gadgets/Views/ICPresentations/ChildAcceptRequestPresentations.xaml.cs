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
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;
using GreenField.Gadgets.Models;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingServiceReference;
using System.Collections.ObjectModel;
using GreenField.Gadgets.ViewModels;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Gadgets.Views
{
    public partial class ChildAcceptRequestPresentations : ChildWindow
    {
        
        #region Fields
         //private ManageMeetings _manageMeetings;
        private ObservableCollection<MeetingInfo> MeetingList { get; set; }
        private DateTime _presentationDate { get; set; }        
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


        public ChildAcceptRequestPresentations(IDBInteractivity dBInteractivity, ILoggerFacade logger,DateTime presentationDate)
        {
            InitializeComponent();

            _dBInteractivity = dBInteractivity;
            _logger = logger;
            _presentationDate = presentationDate;

            _dBInteractivity.GetMeetings(GetMeetingsCallBackMethod);
        }

        
        #endregion

        #region Properties
       // public MarketSnapshotPreference InsertedMarketSnapshotPreference { get; set; } 

        public MeetingInfo SelectedMeeting { get; set; }
        #endregion

        #region Events

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmbBoxMeetingDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            btnAccept.IsEnabled = true;
            SelectedMeeting = cmbBoxMeetingDate.SelectedItem as MeetingInfo;
            txtBoxMeetingClosingDate.Text = String.Format(SelectedMeeting.MeetingClosedDateTime.ToString(), "g");
        }

        #endregion

        #region CallBack Method(s)

        private void GetMeetingsCallBackMethod(List<MeetingInfo> val)
        {
            MeetingList = new ObservableCollection<MeetingInfo>(val.Where(mi => mi.MeetingClosedDateTime >= DateTime.Now).OrderBy(mi => mi.MeetingDateTime).ToList());
            
            try { SelectedMeeting = MeetingList.Where(mi => mi.MeetingDateTime == _presentationDate).FirstOrDefault(); }
            catch (InvalidOperationException) { }

            cmbBoxMeetingDate.ItemsSource = MeetingList;
            cmbBoxMeetingDate.SelectedItem = SelectedMeeting;
        }

        #endregion

    }
}

