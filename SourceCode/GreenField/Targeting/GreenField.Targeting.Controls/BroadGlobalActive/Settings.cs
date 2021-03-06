﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    [Export]
    public class Settings
    {
        [DebuggerStepThrough]
        public Settings(
            IClientFactory clientFactory,
            ModelTraverser modelTraverser,
            DefaultExpandCollapseStateSetter defaultExpandCollapseStateSetter
        )
        {
            this.ClientFactory = clientFactory;
            this.ModelTraverser = modelTraverser;
            this.DefaultExpandCollapseStateSetter = defaultExpandCollapseStateSetter;
        }

        public IClientFactory ClientFactory { get; private set; }
        public ModelTraverser ModelTraverser { get; private set; }
        public DefaultExpandCollapseStateSetter DefaultExpandCollapseStateSetter { get; private set; }
    }
}