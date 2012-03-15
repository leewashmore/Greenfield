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
using System.Windows.Data;

namespace GreenField.LoginModule.Support_Classes
{
    /// <summary>
    /// Implement KeyDown Command - 'Enter key' binding for FrameworkElement (TextBox/PasswordBox)
    /// </summary>
    public sealed class EnterKeyDown
    {
        #region Properties

        #region Command 
        /// <summary>
        /// Get 'Command' Dependency Property
        /// </summary>
        /// <param name="obj">FrameworkElement</param>
        /// <returns>ICommand</returns>
        public static ICommand GetCommand(DependencyObject obj) { return (ICommand)obj.GetValue(CommandProperty); }

        /// <summary>
        /// Set 'Command' Dependency Property
        /// </summary>
        /// <param name="obj">FrameworkElement</param>
        /// <param name="value">FrameworkElement Property Value</param>
        public static void SetCommand(DependencyObject obj, ICommand value) {obj.SetValue(CommandProperty, value); }

        /// <summary>
        /// 'Command' Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EnterKeyDown), new PropertyMetadata(null, OnCommandChanged));
        #endregion Command   
        
        #region CommandArgument   
        /// <summary>
        /// Get 'CommandArgument' Dependency Property
        /// </summary>
        /// <param name="obj">FrameworkElement Property</param>
        /// <returns></returns>
        public static object GetCommandArgument(DependencyObject obj) { return (object)obj.GetValue(CommandArgumentProperty); }

        /// <summary>
        /// Set 'CommandArgument' Dependency Property
        /// </summary>
        /// <param name="obj">FrameworkElement</param>
        /// <param name="value">FrameworkElement Property Value</param>
        public static void SetCommandArgument(DependencyObject obj, object value) { obj.SetValue(CommandArgumentProperty, value); }

        /// <summary>
        /// 'CommandArgument' Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandArgumentProperty =   
            DependencyProperty.RegisterAttached("CommandArgument", typeof(object), typeof(EnterKeyDown), new PropertyMetadata(null, OnCommandArgumentChanged)); 
        #endregion CommandArgument    
        
        #region HasCommandArgument  
        /// <summary>
        /// Get 'HasCommandArgument' Dependency Property
        /// </summary>
        /// <param name="obj">FrameworkElement</param>
        /// <returns></returns>
        private static bool GetHasCommandArgument(DependencyObject obj) { return (bool)obj.GetValue(HasCommandArgumentProperty); }

        /// <summary>
        /// Set 'HasCommandArgument' Dependency Property
        /// </summary>
        /// <param name="obj">FrameworkElement</param>
        /// <param name="value">True/False</param>
        private static void SetHasCommandArgument(DependencyObject obj, bool value) { obj.SetValue(HasCommandArgumentProperty, value); }

        /// <summary>
        /// 'HasCommandArgument' Dependency Property; default value false
        /// </summary>
        private static readonly DependencyProperty HasCommandArgumentProperty =    
            DependencyProperty.RegisterAttached("HasCommandArgument", typeof(bool), typeof(EnterKeyDown), new PropertyMetadata(false)); 
        #endregion HasCommandArgument  

        #endregion Properties  
        
        #region Event Handling
        /// <summary>
        /// Capture ICommand Property - 'CommandArgument' Change event
        /// </summary>
        /// <param name="o">FrameworkElement</param>
        /// <param name="e">FrameworkElement Property Value</param>
        private static void OnCommandArgumentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) { SetHasCommandArgument(o, true); }   
        
        /// <summary>
        /// Capture ICommand Property - 'Command' Change event
        /// </summary>
        /// <param name="o">FrameworkElement</param>
        /// <param name="e">FrameworkElement Property Value</param>
        private static void OnCommandChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                FrameworkElement element = o as FrameworkElement;
                if (element != null)
                {
                    if (e.NewValue == null)
                    {
                        element.KeyDown -= new KeyEventHandler(FrameworkElement_KeyDown);
                    }
                    else if (e.OldValue == null)
                    {
                        element.KeyDown += new KeyEventHandler(FrameworkElement_KeyDown);
                    }
                }
            }
            catch (Exception) { }
        }      

        /// <summary>
        /// Manage FrameworkElement key down event
        /// </summary>
        /// <param name="sender">FrameworkElement</param>
        /// <param name="e">FrameworkElement Property value</param>
        private static void FrameworkElement_KeyDown(object sender, KeyEventArgs e) 
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    DependencyObject o = sender as DependencyObject;
                    ICommand command = GetCommand(sender as DependencyObject);
                    FrameworkElement element = e.OriginalSource as FrameworkElement;

                    if (element != null)
                    {
                        // If the command argument has been explicitly set (even to NULL)  
                        if (GetHasCommandArgument(o))
                        {
                            object commandArgument = GetCommandArgument(o);

                            //update binding expression
                            try { ((PasswordBox)element).GetBindingExpression(PasswordBox.PasswordProperty).UpdateSource(); }
                            catch { try { ((TextBox)element).GetBindingExpression(TextBox.TextProperty).UpdateSource(); } catch { } }

                            if (command.CanExecute(commandArgument))
                            {
                                command.Execute(commandArgument);
                            }
                        }
                        else if (command.CanExecute(element.DataContext))
                        {
                            command.Execute(element.DataContext);
                        }
                    }
                }
            }
            catch (Exception) { }
        }    
        #endregion
    }
}
