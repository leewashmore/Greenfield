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

namespace TopDown.FacingServer.Backend.Targeting
{
    public abstract partial class IssueModel
    {
        public abstract void Accept(IIssueModelResolver resolver);
    }

    public interface IIssueModelResolver
    {
        void Resolve(CompoundIssueModel model);
        void Resolve(WarningModel model);
        void Resolve(ErrorModel model);
    }
}
