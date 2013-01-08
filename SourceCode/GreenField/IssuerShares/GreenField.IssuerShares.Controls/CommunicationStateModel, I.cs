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

namespace GreenField.IssuerShares.Controls
{
    public interface ICommunicationStateModel
    {
        void Accept(ICommunicationStateModelResolver resolver);
    }

    public interface ICommunicationStateModelResolver
    {
        void Resolve(ErrorCommunicationStateModel content);
        void Resolve(LoadingCommunicationStateModel content);
        void Resolve(LoadedCommunicationStateModel content);
    }
}
