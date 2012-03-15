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
using System.Windows.Controls.Primitives;

namespace GreenField.LoginModule.Controls
{
    public partial class ErrorMessage : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message"> Error Message</param>
        public ErrorMessage(string Message)
        {
            InitializeComponent();
            this.MessageBlock.Text = Message;
        }
    }
}
