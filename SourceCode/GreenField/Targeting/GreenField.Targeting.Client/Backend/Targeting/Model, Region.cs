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

namespace TopDown.FacingServer.Backend.Targeting
{
    public partial class RegionModel : IExpandableModel
    {
        private Boolean isExpanded;
        public Boolean IsExpanded
        {
            get { return this.Parent.IsExpanded && this.isExpanded; }
            set { this.isExpanded = value; this.RaisePropertyChanged("IsExpanded"); this.RaisePropertyChanged("ExpandedIcon"); }
        }

        public String ExpandedIcon
        {
            get { return this.isExpanded ? "-" : "+"; }
        }
        
        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
