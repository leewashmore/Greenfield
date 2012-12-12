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
using Aims.Data.Client;

namespace TopDown.FacingServer.Backend.Targeting
{
    public abstract partial class SecurityModel : ISecurity
    {
        public SecurityModel()
        {
        }

        public SecurityModel(String id, String name) : this()
        {
            this.Id = id;
            this.Name = name;
        }

        public abstract void Accept(ISecurityResolver resolver);
    }
}
