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
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;

namespace GreenField.IssuerShares.Controls
{
    public class CompositionChangedEvent : CompositePresentationEvent<CompositionChangedEventInfo>
    {

    }

    public class CompositionChangedEventInfo
    {
        public List<Int32> Securities { get; set; }
    }
}
