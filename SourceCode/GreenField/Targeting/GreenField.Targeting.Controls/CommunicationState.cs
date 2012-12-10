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
using Aims.Controls;
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class CommunicationState : ICommunicationState
    {
        private RootViewModelBase viewModel;

        [DebuggerStepThrough]
        public CommunicationState(RootViewModelBase viewModel)
        {
            this.viewModel = viewModel;
        }

        public Boolean IsLoading { get; private set; }

        public void StartLoading()
        {
            this.IsLoading = true;
            this.viewModel.CommunicationStateModel = new LoadingCommunicationStateModel();
        }

        public void FinishLoading()
        {
            this.viewModel.CommunicationStateModel = new LoadedCommunicationStateModel();
            this.IsLoading = false;
        }

        public void FinishLoading(Exception exception)
        {
            this.viewModel.CommunicationStateModel = new ErrorCommunicationStateModel(exception);
            this.IsLoading = false;
        }
    }
}
