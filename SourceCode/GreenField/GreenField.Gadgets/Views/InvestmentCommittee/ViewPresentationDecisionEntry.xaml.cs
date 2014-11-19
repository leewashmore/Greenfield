using System;
using System.Windows;
using Telerik.Windows.Controls;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.Common;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Class behind for ViewPresentationDecisionEntry
    /// </summary>
    public partial class ViewPresentationDecisionEntry : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// User input committee buy range
        /// </summary>
        private Decimal committeeBuyRange = 0;

        /// <summary>
        /// User input committee sell range
        /// </summary>
        private Decimal committeeSellRange = 0;

        /// <summary>
        /// User input committee pfv measure
        /// </summary>
        private String committeePFVMeasure = null; 
        #endregion

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentationDecisionEntry dataContextViewModelPresentationDecisionEntry;
        public ViewModelPresentationDecisionEntry DataContextViewModelPresentationDecisionEntry
        {
            get { return dataContextViewModelPresentationDecisionEntry; }
            set { dataContextViewModelPresentationDecisionEntry = value; }
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
                if (value)
                {
                    committeeBuyRange = 0;
                    committeeSellRange = 0;
                    committeePFVMeasure = null;
                }
                if (DataContextViewModelPresentationDecisionEntry != null)
                {
                    DataContextViewModelPresentationDecisionEntry.IsActive = isActive;
                    ICPresentationOverviewData selectedPresentationOverviewInfo = ICNavigation.Fetch(ICNavigationInfo.PresentationOverviewInfo) as ICPresentationOverviewData;
                    if (selectedPresentationOverviewInfo != null && (selectedPresentationOverviewInfo.StatusType == StatusType.CLOSED_FOR_VOTING || selectedPresentationOverviewInfo.StatusType == StatusType.DECISION_ENTERED))
                    {
                        this.btnSubmit.IsEnabled = true;
                    }
                    else
                    {
                        this.btnSubmit.IsEnabled = false;
                    }

                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelPresentationDecisionEntry</param>
        public ViewPresentationDecisionEntry(ViewModelPresentationDecisionEntry dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentationDecisionEntry = dataContextSource;
           
        }
        #endregion        

        #region Event Handlers
        /// <summary>
        /// cbPFVICDecision SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbPFVICDecision_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.cbPFVICDecision.SelectedValue as string != committeePFVMeasure)
            {
                RaiseUpdateICDecisionRecommendation();
            }
        }

        /// <summary>
        /// txtbPFVICDecisionBuy LostFocus EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void txtbPFVICDecisionBuy_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtbPFVICDecisionBuy.Text != committeeBuyRange.ToString())
            {
                RaiseUpdateICDecisionRecommendation();
            }
        }

        /// <summary>
        /// txtbPFVICDecisionSell LostFocus EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void txtbPFVICDecisionSell_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtbPFVICDecisionSell.Text != committeeSellRange.ToString())
            {
                RaiseUpdateICDecisionRecommendation();
            }
        }

        /// <summary>
        /// cbFinalVoteType SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbFinalVoteType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            RadComboBox voteComboBox = sender as RadComboBox;
            if (voteComboBox == null)
            {
                return;
            }
            VoterInfo voterInfo = voteComboBox.DataContext as VoterInfo;
            DataContextViewModelPresentationDecisionEntry.RaiseUpdateFinalVoteType(voterInfo);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Raise IC Decision Recommendation update
        /// </summary>
        private void RaiseUpdateICDecisionRecommendation()
        {
            Boolean pfvChanged = false;
            if (this.cbPFVICDecision.SelectedItem == null)
            {
                return;
            }
            if (DataContextViewModelPresentationDecisionEntry.SecurityPFVMeasureCurrentPrices != null)
            {
                if (DataContextViewModelPresentationDecisionEntry
                        .SecurityPFVMeasureCurrentPrices[this.cbPFVICDecision.SelectedItem as String] == null)
                {
                    Prompt.ShowDialog("Error: missing current value of P/FV measure: " + this.cbPFVICDecision.SelectedItem as String);
                    this.cbPFVICDecision.SelectedValue = committeePFVMeasure;
                    this.txtbPFVICDecisionBuy.Text = this.committeeBuyRange.ToString();
                    this.txtbPFVICDecisionSell.Text = this.committeeSellRange.ToString();
                    return;
                }
            }

            if (this.cbPFVICDecision.SelectedItem as String != committeePFVMeasure)
            {
                pfvChanged = true;                
            }

            committeePFVMeasure = this.cbPFVICDecision.SelectedItem as String;

            Decimal committeeBuyRange;
            if (!Decimal.TryParse(this.txtbPFVICDecisionBuy.Text, out committeeBuyRange))
            {
                this.txtbPFVICDecisionBuy.Text = committeeBuyRange.ToString();
                return;
            }

            Boolean buyRangeChanged = true;
            if (this.committeeBuyRange == committeeBuyRange)
            {
                buyRangeChanged = false;
            }
            this.committeeBuyRange = committeeBuyRange;

            Decimal committeeSellRange;
            if (!Decimal.TryParse(this.txtbPFVICDecisionSell.Text, out committeeSellRange))
            {
                this.txtbPFVICDecisionSell.Text = committeeSellRange.ToString();
                return;
            }

            Boolean sellRangeChanged = true;
            if (this.committeeSellRange == committeeSellRange)
            {
                sellRangeChanged = false;
            }
            this.committeeSellRange = committeeSellRange;

            if (pfvChanged || buyRangeChanged || sellRangeChanged)
            {
                DataContextViewModelPresentationDecisionEntry.UpdateICDecisionRecommendation(committeePFVMeasure, committeeBuyRange, committeeSellRange); 
            }
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
    }
}

