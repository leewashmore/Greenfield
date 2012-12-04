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

namespace GreenField.Targeting.Controls
{
    /// <summary>
    /// The view model that is capable of telling if it has unsaved changes.
    /// </summary>
    public interface IDirtyViewModel
    {
        /// <summary>
        /// Gets true if the view has unsaved changes.
        /// </summary>
        Boolean HasUnsavedChanges { get; }
    }
}
