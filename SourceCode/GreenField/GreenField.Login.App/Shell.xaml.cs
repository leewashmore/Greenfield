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
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using System.Windows.Interop;


namespace GreenField.Login.App
{
    [Export]
    public partial class Shell : UserControl
    {
        IRegionManager _regionManager;       

        [ImportingConstructor]
        public Shell(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeComponent();
            Settings settings = new Settings();
            settings.EnableAutoZoom = false;
        }
    }
}
