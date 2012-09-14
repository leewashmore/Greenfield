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
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Helpers
{
    public partial class ViewBaseUserControl : UserControl, IDisposable
    {
        public virtual bool IsActive { get; set; }
        public virtual void Dispose(){}
        public virtual RadDocument CreateDocument() 
        {
            return null;
        }
    }
}