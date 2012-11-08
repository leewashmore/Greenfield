﻿using System;
using System.Windows;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewICPresentationNew
    /// </summary>
    public partial class ViewICPresentationNew : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// YTD Abs value
        /// </summary>
        private Decimal valueYTDAbs;

        /// <summary>
        /// YTD ReltoLoc value
        /// </summary>
        private Decimal valueYTDReltoLoc;

        /// <summary>
        /// YTD ReltoEM value
        /// </summary>
        private Decimal valueYTDReltoEM; 
        #endregion

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelICPresentationNew dataContextViewModelICPresentationNew;
        public ViewModelICPresentationNew DataContextViewModelICPresentationNew
        {
            get { return dataContextViewModelICPresentationNew; }
            set { dataContextViewModelICPresentationNew = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextViewModelICPresentationNew != null)
                {
                    DataContextViewModelICPresentationNew.IsActive = isActive;
                }
                if (value)
                {
                    this.txtbYTDAbsolute.Text = "0.00";
                    this.txtbYTDReltoLoc.Text = "0.00";
                    this.txtbYTDReltoEM.Text = "0.00";
                }
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelICPresentationNew</param>
        public ViewICPresentationNew(ViewModelICPresentationNew dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelICPresentationNew = dataContextSource;
        }
        #endregion        

        #region Event Handlers
        /// <summary>
        /// txtbYTDAbsolute LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbYTDAbsolute_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        /// <summary>
        /// txtbYTDReltoLoc LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbYTDReltoLoc_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        /// <summary>
        /// txtbYTDReltoEM LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbYTDReltoEM_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }  
        #endregion        

        #region Helper Methods
        /// <summary>
        /// Validates and raises YTD input parameter changes
        /// </summary>
        private void RaiseICPresentationOverviewInfoChanged()
        {
            #region valueYTDAbs
            Decimal valueYTDAbs;
            if (!Decimal.TryParse(this.txtbYTDAbsolute.Text, out valueYTDAbs))
            {
                this.txtbYTDAbsolute.Text = valueYTDAbs.ToString();
                return;
            }
            valueYTDAbs = Convert.ToDecimal((int)(valueYTDAbs * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtbYTDAbsolute.Text = valueYTDAbs.ToString();
            this.valueYTDAbs = valueYTDAbs; 
            #endregion

            #region valueYTDReltoLoc
            Decimal valueYTDReltoLoc;
            if (!Decimal.TryParse(this.txtbYTDReltoLoc.Text, out valueYTDReltoLoc))
            {
                this.txtbYTDReltoLoc.Text = valueYTDReltoLoc.ToString();
                return;
            }
            valueYTDReltoLoc = Convert.ToDecimal((int)(valueYTDReltoLoc * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtbYTDReltoLoc.Text = valueYTDReltoLoc.ToString();
            this.valueYTDReltoLoc = valueYTDReltoLoc; 
            #endregion

            #region valueYTDReltoEM
            Decimal valueYTDReltoEM;
            if (!Decimal.TryParse(this.txtbYTDReltoEM.Text, out valueYTDReltoEM))
            {
                this.txtbYTDReltoEM.Text = valueYTDReltoEM.ToString();
                return;
            }
            valueYTDReltoEM = Convert.ToDecimal((int)(valueYTDReltoEM * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtbYTDReltoEM.Text = valueYTDReltoEM.ToString();
            this.valueYTDReltoEM = valueYTDReltoEM; 
            #endregion

            DataContextViewModelICPresentationNew.RaiseICPresentationOverviewInfoChanged(valueYTDAbs, valueYTDReltoLoc, valueYTDReltoEM);
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
    }
}