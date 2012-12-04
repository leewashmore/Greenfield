using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.FacingServer.Backend.Targeting
{
    public interface IValueChangeWatcher
    {
        void GetNotifiedAboutChangedValue(EditableExpressionModel model);
    }
}
