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
    public class DataAndIndexWrap
    {
        [DebuggerStepThrough]
        public DataAndIndexWrap(Int32 index, Object data)
        {
            this.Index = index;
            this.Data = data;
        }
        public Int32 Index { get; private set; }
        public Object Data { get; private set; }
    }
}
