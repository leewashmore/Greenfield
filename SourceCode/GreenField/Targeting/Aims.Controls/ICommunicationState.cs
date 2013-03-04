using System;

namespace Aims.Controls
{
    public interface ICommunicationState
    {
        Boolean IsLoading { get; }
        void StartLoading();
        void FinishLoading();
        void FinishLoading(Exception exception);
    }
}
