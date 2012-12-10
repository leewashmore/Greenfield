using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
