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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Targeting.App
{
    [Export(typeof(ILoggerFacade))]
    public class Logger : ILoggerFacade
    {
        public void Log(String message, Category category, Priority priority)
        {
            // do nothing
        }
    }
}
