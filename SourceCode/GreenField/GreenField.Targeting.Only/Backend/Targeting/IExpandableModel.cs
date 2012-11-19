using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace GreenField.Targeting.Only.Backend.Targeting
{
    public interface IParentModel : INotifyPropertyChanged
    {
        Int32 Level { get; }
    }
    public interface IExpandableModel : IParentModel
    {
        Boolean IsExpanded { get; set; }
    }
}
