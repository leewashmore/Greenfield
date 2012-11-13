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

namespace GreenField.ServiceCaller
{
    public class ApplicationException : Exception
    {
        public ApplicationException() { }
        public ApplicationException(String message) : base(message) { }
        public ApplicationException(String message, Exception inner) : base(message, inner) { }
    }
}
