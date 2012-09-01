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
using GreenField.DataContracts;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using Microsoft.Practices.Prism.Logging;
using System.IO;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views.Documents
{
    public partial class ChildViewDocumentsEditDelete : ChildWindow
    {
        public ChildViewDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger)
        {
            InitializeComponent();
            this.DataContext = new ChildViewModelDocumentsEditDelete(dBInteractivity, logger);
        }
    }
}

