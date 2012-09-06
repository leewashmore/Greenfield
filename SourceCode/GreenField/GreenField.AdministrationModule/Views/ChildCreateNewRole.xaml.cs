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
using System.Collections.ObjectModel;
using GreenField.AdministrationModule.Controls;

namespace GreenField.AdministrationModule.Views
{
    public partial class ChildCreateNewRole : ChildWindow
    {
        private const string invalidRoleError = "*Invalid Role Name. Ideal Role Name should not start with space";
        private const string recurrentRoleError = "*RoleName Already exists";
        private static Brush ValidBrush = new SolidColorBrush(Colors.Black);
        private static Brush InvalidBrush = new SolidColorBrush(Colors.Red);

        private bool _invalidRoleValidation;
        public bool InvalidRoleValidation
        {
            get { return _invalidRoleValidation; }
            set 
            {
                _invalidRoleValidation = value;
                txtblkRoleName.Foreground = value ? ValidBrush : InvalidBrush;
                txtEnterValue.BorderBrush = value ? ValidBrush : InvalidBrush;
                invalidFieldPopup.IsOpen = !(value);
            }
        }

        private bool _recurrentRoleValidation;
        public bool RecurrentRoleValidation
        {
            get { return _recurrentRoleValidation; }
            set
            {
                _recurrentRoleValidation = value;
                txtblkRoleName.Foreground = value ? ValidBrush : InvalidBrush;
                txtEnterValue.BorderBrush = value ? ValidBrush : InvalidBrush;
                recurrentFieldPopup.IsOpen = !(value);
            }
        }
        
        

        public ObservableCollection<string> Roles { get; set; }

        public ChildCreateNewRole()
        {
            InitializeComponent();

            invalidFieldPopup.Child = new ErrorMessage(invalidRoleError);
            recurrentFieldPopup.Child = new ErrorMessage(recurrentRoleError);

            invalidFieldPopup.Opened += (se, e) => { invalidFieldNotificationTransition.Begin(); };
            recurrentFieldPopup.Opened += (se, e) => { recurrentFieldNotificationTransition.Begin(); };
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Roles == null)
            {
                Roles = new ObservableCollection<string>();
            }

            RecurrentRoleValidation = !(Roles.Contains(txtEnterValue.Text.ToLower()));
            if (RecurrentRoleValidation)
                this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtEnterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            RecurrentRoleValidation = true;
            InvalidRoleValidation = !((txtEnterValue.Text.ToLower().StartsWith(" ")) || (txtEnterValue.Text.ToLower().Contains(",")));
            OKButton.IsEnabled = InvalidRoleValidation && txtEnterValue.Text.ToLower() != String.Empty;
            
        }

        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RecurrentRoleValidation = !(Roles.Contains(txtEnterValue.Text.ToLower()));
                if (RecurrentRoleValidation)
                    this.DialogResult = true;
            }
        }
    }
}

