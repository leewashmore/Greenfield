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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code- behind for Target Price
    /// </summary>
    public partial class ViewTargetPrice : ViewBaseUserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewTargetPrice(ViewModelTargetPrice dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextTargetPrice = dataContextSource;
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of type View-Model
        /// </summary>
        private ViewModelTargetPrice _dataContextTargetPrice;
        public ViewModelTargetPrice DataContextTargetPrice
        {
            get
            {
                return _dataContextTargetPrice;
            }
            set
            {
                _dataContextTargetPrice = value;
            }
        }


        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// Unsubscribe the Event Handlers
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

    }
}
