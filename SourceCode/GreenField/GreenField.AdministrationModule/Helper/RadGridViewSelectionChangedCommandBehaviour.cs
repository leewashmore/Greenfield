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
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.AdministrationModule.Helper
{
    /// <summary>
    /// Implement RadGridViewSelectionChanged Command - 'SelectedItems' binding for FrameworkElement (RadGridView)
    /// </summary>
    public class RadGridViewSelectionChangedCommandBehaviour : CommandBehaviorBase<RadGridView>
    {
        public RadGridViewSelectionChangedCommandBehaviour(RadGridView targetObject)
            : base(targetObject)
        {
            targetObject.SelectionChanged += (se, e) => { this.ExecuteCommand(); };
        }
    }
        
}
