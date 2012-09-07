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
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    public partial class ViewICPresentationNew : ViewBaseUserControl
    {        
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelICPresentationNew _dataContextViewModelICPresentationNew;
        public ViewModelICPresentationNew DataContextViewModelICPresentationNew
        {
            get { return _dataContextViewModelICPresentationNew; }
            set { _dataContextViewModelICPresentationNew = value; }
        }



        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextViewModelICPresentationNew != null) //DataContext instance
                    DataContextViewModelICPresentationNew.IsActive = _isActive;
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewICPresentationNew(ViewModelICPresentationNew dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelICPresentationNew = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelICPresentationNew.Dispose();
            this.DataContextViewModelICPresentationNew = null;
            this.DataContext = null;
        }
        #endregion


        private Decimal _valueYTDAbs;
        private Decimal _valueYTDReltoLoc;
        private Decimal _valueYTDReltoEM;

        private void RaiseICPresentationOverviewInfoChanged()
        {
            Decimal valueYTDAbs;
            if (!Decimal.TryParse(this.txtbYTDAbsolute.Text, out valueYTDAbs))
            {
                this.txtbYTDAbsolute.Text = _valueYTDAbs.ToString();
                return;
            }
            _valueYTDAbs = valueYTDAbs;

            Decimal valueYTDReltoLoc;
            if (!Decimal.TryParse(this.txtbYTDReltoLoc.Text, out valueYTDReltoLoc))
            {
                this.txtbYTDAbsolute.Text = _valueYTDReltoLoc.ToString();
                return;
            }
            _valueYTDReltoLoc = valueYTDReltoLoc;

            Decimal valueYTDReltoEM;
            if (!Decimal.TryParse(this.txtbYTDReltoEM.Text, out valueYTDReltoEM))
            {
                this.txtbYTDAbsolute.Text = _valueYTDReltoEM.ToString();
                return;
            }
            _valueYTDReltoEM = valueYTDReltoEM;

            DataContextViewModelICPresentationNew.RaiseICPresentationOverviewInfoChanged(_valueYTDAbs, _valueYTDReltoLoc, _valueYTDReltoEM);
        }

        private void txtbYTDAbsolute_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        private void txtbYTDReltoLoc_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        private void txtbYTDReltoEM_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        } 
    }
}
