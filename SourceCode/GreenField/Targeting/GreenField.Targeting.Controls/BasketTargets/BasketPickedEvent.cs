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
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class BasketPickedEventArgs : CancellableEventArgs
    {
        [DebuggerStepThrough]
        public BasketPickedEventArgs(Int32 targetingTypeGroupId, Int32 basketId, Boolean isCancelled)
            : base(isCancelled)
        {
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.BasketId = basketId;
        }

        public Int32 TargetingTypeGroupId { get; private set; }

        public Int32 BasketId { get; private set; }
    }

    public delegate void BasketPickedEventHandler(Object sender, BasketPickedEventArgs args);
}
