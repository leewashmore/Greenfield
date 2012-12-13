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
using Microsoft.Practices.Prism.Logging;
using System.ComponentModel.Composition;

namespace GreenField.IssuerShares.App
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
