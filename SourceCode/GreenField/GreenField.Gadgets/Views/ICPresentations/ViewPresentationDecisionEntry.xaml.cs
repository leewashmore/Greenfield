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
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewPresentationDecisionEntry : ViewBaseUserControl
    {
        private Decimal _committeeBuyRange;
        private Decimal _committeeSellRange;
        private String _committeePFVMeasure;

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentationDecisionEntry _dataContextViewModelPresentationDecisionEntry;
        public ViewModelPresentationDecisionEntry DataContextViewModelPresentationDecisionEntry
        {
            get { return _dataContextViewModelPresentationDecisionEntry; }
            set { _dataContextViewModelPresentationDecisionEntry = value; }
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
                if (DataContextViewModelPresentationDecisionEntry != null) //DataContext instance
                    DataContextViewModelPresentationDecisionEntry.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPresentationDecisionEntry(ViewModelPresentationDecisionEntry dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentationDecisionEntry = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelPresentationDecisionEntry.Dispose();
            this.DataContextViewModelPresentationDecisionEntry = null;
            this.DataContext = null;
        }
        #endregion

        #region Event Handlers
        private void chkbAcceptWithoutDiscussion_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chkbAcceptWithoutDiscussion == null)
                return;

            if (this.chkbAcceptWithoutDiscussion.IsChecked != null)
            {
                DataContextViewModelPresentationDecisionEntry.UpdateICDecisionAsPresented(false);
            }
        }

        private void chkbAcceptWithoutDiscussion_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.chkbAcceptWithoutDiscussion == null)
                return;

            if (this.chkbAcceptWithoutDiscussion.IsChecked != null)
            {
                DataContextViewModelPresentationDecisionEntry.UpdateICDecisionAsPresented(true);
            }
        }

        private void cbPFVICDecision_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            RaiseUpdateICDecisionRecommendation();
        }

        private void txtbPFVICDecisionBuy_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseUpdateICDecisionRecommendation();
        }

        private void txtbPFVICDecisionSell_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseUpdateICDecisionRecommendation();
        }

        private void cbFinalVoteType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            RadComboBox voteComboBox = sender as RadComboBox;
            if (voteComboBox == null)
                return;

            VoterInfo voterInfo = voteComboBox.DataContext as VoterInfo;
            DataContextViewModelPresentationDecisionEntry.RaiseUpdateFinalVoteType(voterInfo);
        }
        #endregion

        #region Private Methods
        private void RaiseUpdateICDecisionRecommendation()
        {
            Boolean pfvChanged = false;
            if (this.cbPFVICDecision.SelectedItem == null)
                return;

            if (this.cbPFVICDecision.SelectedItem as String != _committeePFVMeasure)
            {
                pfvChanged = true;
                if (DataContextViewModelPresentationDecisionEntry.SecurityPFVMeasureCurrentPrices != null)
                {
                    if (DataContextViewModelPresentationDecisionEntry
                            .SecurityPFVMeasureCurrentPrices[this.cbPFVICDecision.SelectedItem as String] == null)
                    {
                        this.cbPFVICDecision.SelectedValue = _committeePFVMeasure;
                        Prompt.ShowDialog("Error: missing current value of P/FV measure: " + this.cbPFVICDecision.SelectedItem as String);
                        return;
                    }
                }
            }

            _committeePFVMeasure = this.cbPFVICDecision.SelectedItem as String;

            Decimal committeeBuyRange;
            if (!Decimal.TryParse(this.txtbPFVICDecisionBuy.Text, out committeeBuyRange))
            {
                this.txtbPFVICDecisionBuy.Text = _committeeBuyRange.ToString();
                return;
            }

            Boolean buyRangeChanged = true;
            if (_committeeBuyRange == committeeBuyRange)
                buyRangeChanged = false;

            if (DataContextViewModelPresentationDecisionEntry.SecurityPFVMeasureCurrentPrices != null)
            {
                if (DataContextViewModelPresentationDecisionEntry
                        .SecurityPFVMeasureCurrentPrices[this.cbPFVICDecision.SelectedItem as String] == null)
                {
                    this.cbPFVICDecision.SelectedValue = _committeePFVMeasure;
                    Prompt.ShowDialog("Error: missing current value of P/FV measure: " + this.cbPFVICDecision.SelectedItem as String);
                    return;
                }
            }

            _committeeBuyRange = committeeBuyRange;

            Decimal committeeSellRange;
            if (!Decimal.TryParse(this.txtbPFVICDecisionSell.Text, out committeeSellRange))
            {
                this.txtbPFVICDecisionSell.Text = _committeeSellRange.ToString();
                return;
            }

            Boolean sellRangeChanged = true;
            if (_committeeSellRange == committeeSellRange)
                sellRangeChanged = false;
            _committeeSellRange = committeeSellRange;

            //String committeeRecommendation = this.cbPFVICDecision.SelectedItem as String;

            if (pfvChanged || buyRangeChanged || sellRangeChanged)
            {
                DataContextViewModelPresentationDecisionEntry.UpdateICDecisionRecommendation(_committeePFVMeasure, _committeeBuyRange, _committeeSellRange); 
            }
        } 
        #endregion
    }
}

