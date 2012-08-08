using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using GreenField.LoginModule.ViewModel;
using System.Runtime.InteropServices;

namespace GreenField.LoginModule.Views
{
    [Export]
    public partial class ViewLoginForm : UserControl
    {
        private int _lastKey;
        private ModifierKeys _modifiers;
        private int _lastPasswordLength = 0;

        private bool _capsLockOn = false;
        public bool CapsLockOn
        {
            get { return _capsLockOn; }
            set
            {
                _capsLockOn = value;
                tbCapsLockNotification.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public ViewLoginForm()
        {
            InitializeComponent();

            this.pbPassword.KeyDown += (se, e) =>
            {
                _lastKey = e.PlatformKeyCode;
                _modifiers = Keyboard.Modifiers;

            };

            this.pbPassword.PasswordChanged += (se, e) =>
                {
                    if (_lastKey >= 0x41 && _lastKey <= 0x5a && pbPassword.Password.Length > _lastPasswordLength)
                    {
                        var lastChar = pbPassword.Password.Last();
                        if (_modifiers != ModifierKeys.Shift)
                            CapsLockOn = char.ToLower(lastChar) != lastChar;
                        else
                            CapsLockOn = char.ToUpper(lastChar) != lastChar;
                    }
                    _lastPasswordLength = pbPassword.Password.Length;
                };
        }

        [Import]
        public ViewModelLoginForm DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }

    }
}
