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

namespace GreenField.LoginModule.Resources
{
    /// <summary>
    /// Not currently utilized - is required if resource directly integrated in xaml
    /// </summary>
    public class ResourceWrapper
    {
        private ErrorMessages appErrorMessages;
        private SecurityQuestions appSecurityQuestions;

        public ResourceWrapper()
        {
            appErrorMessages = new ErrorMessages();
            appSecurityQuestions = new SecurityQuestions();
        }

        public ErrorMessages AppErrorMessages { get { return appErrorMessages; } }
        public SecurityQuestions AppSecurityQuestions { get { return appSecurityQuestions; } }

    }
}
