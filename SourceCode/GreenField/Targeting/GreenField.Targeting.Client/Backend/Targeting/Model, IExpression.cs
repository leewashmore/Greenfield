using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TopDown.FacingServer.Backend.Targeting
{
    public interface IExpressionModel
    {
        ObservableCollection<IssueModel> Issues { get; }
    }
}
