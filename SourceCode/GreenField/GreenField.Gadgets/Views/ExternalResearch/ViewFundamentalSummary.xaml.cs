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
    public partial class ViewFundamentalSummary : ViewBaseUserControl
    {

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewFundamentalSummary(ViewModelFundamentalSummary dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
        }
        
        #endregion

        #region PropertyDeclaration

        private ViewModelFundamentalSummary _dataContextSouce;
        public ViewModelFundamentalSummary DataContextSource
        {
            get { return _dataContextSouce; }
            set { _dataContextSouce = value; }
        }
        


        #endregion

    }
}
