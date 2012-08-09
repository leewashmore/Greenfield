using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;

namespace GreenField.Gadgets.Models
{
    public class ICPNavigationInfo
    {
        public ViewPluginFlagEnumeration ViewPluginFlagEnumerationObject { get; set; }
        public ICPMeetingInfo MeetingInfoObject { get; set; }
        public ICPPresentationInfo PresentationInfoObject { get; set; }
    }
}
