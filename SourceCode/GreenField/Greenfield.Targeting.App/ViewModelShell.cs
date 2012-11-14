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
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;

namespace GreenField.Targeting.App
{
    [Export]
    public class ViewModelShell : NotificationObject
    {
        [ImportingConstructor]
        public ViewModelShell(
            IRegionManager regionManager,
            IManageSessions manageSessions,
            ILoggerFacade logger,
            IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity
        )
        {
        }
    }
}
