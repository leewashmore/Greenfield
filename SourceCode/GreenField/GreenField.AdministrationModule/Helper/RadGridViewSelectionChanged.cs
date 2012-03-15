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
using Telerik.Windows.Controls;

namespace GreenField.AdministrationModule.Helper
{
    /// <summary>
    /// Implement RadGridViewSelectionChanged Command - 'SelectedItems' binding for FrameworkElement (RadGridView)
    /// </summary>
    public static class RadGridViewSelectionChanged
    {
        #region Command Dependency Property
        #region Property
        /// <summary>
        /// Get 'Command' Dependency Property
        /// </summary>
        /// <param name="dataGrid">RadGridView</param>
        /// <returns>ICommand</returns>
        public static ICommand GetCommand(RadGridView dataGrid)
        {
            return dataGrid.GetValue(CommandProperty) as ICommand;
        }

        /// <summary>
        /// Set 'Command' Dependency Property
        /// </summary>
        /// <param name="dataGrid">RadGridView</param>
        /// <param name="command">ICommand</param>
        public static void SetCommand(RadGridView dataGrid, ICommand command)
        {
            dataGrid.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// 'Command' Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(RadGridViewSelectionChanged), new PropertyMetadata(OnSetCommandCallback)); 
        #endregion

        #region Event Handling
        /// <summary>
        /// Assign Selection Changed Behavior to Command Dependency Property
        /// </summary>
        /// <param name="dependencyObject">RadGridView</param>
        /// <param name="e"></param>
        private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var dataGrid = dependencyObject as RadGridView;
                if (dataGrid != null)
                {
                    RadGridViewSelectionChangedCommandBehaviour behaviour = GetOrCreateBehavior(dataGrid);
                    behaviour.Command = e.NewValue as ICommand;
                }
            }
            catch (Exception) { }
        } 
        #endregion
        #endregion

        #region CommandParameter Dependency Property
        #region Property
        /// <summary>
        /// Get 'CommandParameter' Dependency Property
        /// </summary>
        /// <param name="dataGrid">RadGridView</param>
        /// <returns>ICommand</returns>
        public static object GetCommandParameter(RadGridView dataGrid)
        {
            return dataGrid.GetValue(CommandParameterProperty) as object;
        }

        /// <summary>
        /// Set 'CommandParameter' Dependency Property
        /// </summary>
        /// <param name="dataGrid">RadGridView</param>
        /// <param name="command">ICommand</param>
        public static void SetCommandParameter(RadGridView dataGrid, object parameter)
        {
            dataGrid.SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// 'CommandParameter' Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(RadGridViewSelectionChanged), new PropertyMetadata(OnSetCommandParameterCallback)); 
        #endregion 

        #region Event Handling
        /// <summary>
        /// Assign Selection Changed Behavior to CommandParameter Dependency Property
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="e"></param>
        private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = dependencyObject as RadGridView;
            if (dataGrid != null)
            {
                RadGridViewSelectionChangedCommandBehaviour behavior = GetOrCreateBehavior(dataGrid);
                behavior.CommandParameter = e.NewValue;
            }
        } 
        #endregion       
        #endregion

        #region Command Behavior Dependency Property
        #region Property
        private static RadGridViewSelectionChangedCommandBehaviour GetOrCreateBehavior(RadGridView dataGrid)
        {
            var behavior = dataGrid.GetValue(ClickCommandBehaviorProperty) as RadGridViewSelectionChangedCommandBehaviour;
            if (behavior == null)
            {
                behavior = new RadGridViewSelectionChangedCommandBehaviour(dataGrid);
                dataGrid.SetValue(ClickCommandBehaviorProperty, behavior);
            }
            return behavior;
        }

        /// <summary>
        /// 'ClickCommandBehavior' Dependency Property
        /// </summary>
        private static readonly DependencyProperty ClickCommandBehaviorProperty =
            DependencyProperty.RegisterAttached("ClickCommandBehavior", typeof(RadGridViewSelectionChangedCommandBehaviour), typeof(RadGridViewSelectionChanged), null); 
        #endregion  
        #endregion
    }
}
