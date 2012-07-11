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
    /// Code-Behind for ConsensusEstimates-Valuations Gadget
    /// </summary>
    public partial class ViewValuations : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Property of View-Model
        /// </summary>
        private ViewModelValuations _dataContextValuations;
        public ViewModelValuations DataContextValuations
        {
            get { return _dataContextValuations; }
            set { _dataContextValuations = value; }
        }



        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewValuations(ViewModelValuations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextValuations = dataContextSource;
        }

        #endregion
    }
}
