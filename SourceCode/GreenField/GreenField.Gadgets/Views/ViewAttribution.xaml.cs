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
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Class for the Attribution View
    /// </summary>
    public partial class ViewAttribution : UserControl
    {
        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DataContextSource">ViewModelAttribution as Data context for this View</param>
        public ViewAttribution(ViewModelAttribution DataContextSource )
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
        }
        #endregion
    }
}
