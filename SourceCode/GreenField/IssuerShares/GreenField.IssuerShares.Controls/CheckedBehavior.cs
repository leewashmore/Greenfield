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
using Microsoft.Practices.Prism.Commands;

namespace GreenField.IssuerShares.Controls
{
    public class CheckBoxCommandBehavior : CommandBehaviorBase<CheckBox>
    {
        public CheckBoxCommandBehavior(CheckBox checkableObj)
            : base(checkableObj)
        {
            checkableObj.Checked += new RoutedEventHandler(checkableObj_Checked);
            checkableObj.Unchecked += new RoutedEventHandler(checkableObj_Checked);
        }

        private void checkableObj_Checked(object s, RoutedEventArgs e)
        {
            ExecuteCommand();
        }
    }

    public static class CheckBoxChecked
    {
        private static readonly DependencyProperty CheckBoxCommandBehaviorProperty = DependencyProperty.RegisterAttached(
            "CheckBoxCommandBehavior",
            typeof(CheckBoxCommandBehavior),
            typeof(CheckBoxChecked),
            null);

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(CheckBoxChecked),
            new PropertyMetadata(OnSetCommandCallback));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(CheckBoxChecked),
            new PropertyMetadata(OnSetCommandParameterCallback));


        public static void SetCommand(CheckBox toggleBtn, ICommand cmd)
        {
            toggleBtn.SetValue(CommandProperty, cmd);
        }

        public static ICommand GetCommand(CheckBox toggleBtn)
        {
            return toggleBtn.GetValue(CommandProperty) as ICommand;
        }

        public static void SetCommandParameter(CheckBox selector, object parameter)
        {
            selector.SetValue(CommandParameterProperty, parameter);
        }

        public static object GetCommandParameter(CheckBox selector)
        {
            return selector.GetValue(CommandParameterProperty);
        }

        public static CheckBoxCommandBehavior GetOrCreateBehavior(CheckBox toggleBtn)
        {
            var behavior = toggleBtn.GetValue(CheckBoxCommandBehaviorProperty) as CheckBoxCommandBehavior;

            if (behavior == null)
            {
                behavior = new CheckBoxCommandBehavior(toggleBtn);
                toggleBtn.SetValue(CheckBoxCommandBehaviorProperty, behavior);
            }

            return behavior;
        }

        public static void OnSetCommandCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var toggleBtn = depObj as CheckBox;
            if (toggleBtn != null)
            {
                CheckBoxCommandBehavior behavior = GetOrCreateBehavior(toggleBtn);
                behavior.Command = e.NewValue as ICommand;
            }
        }

        private static void OnSetCommandParameterCallback(DependencyObject depObject, DependencyPropertyChangedEventArgs e)
        {
            var toggleBtn = depObject as CheckBox;
            if (toggleBtn != null)
            {
                CheckBoxCommandBehavior behavior = GetOrCreateBehavior(toggleBtn);
                behavior.CommandParameter = e.NewValue;
            }
        }
    }
}
