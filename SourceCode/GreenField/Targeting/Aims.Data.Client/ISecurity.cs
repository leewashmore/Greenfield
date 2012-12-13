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

namespace Aims.Data.Client
{
    public interface ISecurity
    {
        String Id { get; }
        String Name { get; }
        void Accept(ISecurityResolver resolver);
    }

    public interface ISecurityResolver
    {
        void Resolve(IFund fund);
        void Resolve(ICompanySecurity security);
    }


}
