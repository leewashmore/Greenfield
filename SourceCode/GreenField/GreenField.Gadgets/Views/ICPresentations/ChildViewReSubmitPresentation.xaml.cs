using System;
using System.Windows;
using System.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ChildViewReSubmitPresentation
    /// </summary>
    public partial class ChildViewReSubmitPresentation : ChildWindow
    {
        #region Properties
        /// <summary>
        /// Stores true if Alert check box is checked
        /// </summary>
        public Boolean IsAlertChecked { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ChildViewReSubmitPresentation()
        {
            InitializeComponent();
        } 
        #endregion        

        #region Event Handlers
        /// <summary>
        /// OKButton Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// CancelButton Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// chkbAlert Checked EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void chkbAlert_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chkbAlert.IsChecked != null)
            {
                IsAlertChecked = Convert.ToBoolean(this.chkbAlert.IsChecked);
            }
        } 
        #endregion
    }
}