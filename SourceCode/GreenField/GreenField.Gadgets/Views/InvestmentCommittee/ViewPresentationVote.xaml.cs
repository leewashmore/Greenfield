using System;
using System.Windows;
using System.Windows.Controls;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewPresentationVote
    /// </summary>
    public partial class ViewPresentationVote : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// User input committee buy range
        /// </summary>
        private Decimal committeeBuyRange;

        /// <summary>
        /// User input committee sell range
        /// </summary>
        private Decimal committeeSellRange; 
        #endregion        

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentationVote dataContextViewModelPresentationVote;
        public ViewModelPresentationVote DataContextViewModelPresentationVote
        {
            get { return dataContextViewModelPresentationVote; }
            set { dataContextViewModelPresentationVote = value; }
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
                if (DataContextViewModelPresentationVote != null)
                {
                    DataContextViewModelPresentationVote.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelPresentationVote</param>
        public ViewPresentationVote(ViewModelPresentationVote dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentationVote = dataContextSource;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// txtbBuyRange LostFocus EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void txtbBuyRange_LostFocus(object sender, RoutedEventArgs e)
        {
            Decimal committeeBuyRange;
            if (!Decimal.TryParse(this.txtbBuyRange.Text, out committeeBuyRange))
            {
                this.txtbBuyRange.Text = committeeBuyRange.ToString();
                return;
            }
            this.committeeBuyRange = committeeBuyRange;
        }

        /// <summary>
        /// txtbSellRange LostFocus EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void txtbSellRange_LostFocus(object sender, RoutedEventArgs e)
        {
            Decimal committeeSellRange;
            if (!Decimal.TryParse(this.txtbSellRange.Text, out committeeSellRange))
            {
                this.txtbSellRange.Text = committeeSellRange.ToString();
                return;
            }
            this.committeeSellRange = committeeSellRange;
        }

        /// <summary>
        /// cbVoteType SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbVoteType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataContextViewModelPresentationVote.RaiseUpdateVoteType(this.cbVoteType.SelectedValue as String);
        }

        /// <summary>
        /// btnPreview Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "PDF (*.pdf) |*.pdf" };
            if (dialog.ShowDialog() == true)
            {
                DataContextViewModelPresentationVote.DownloadStream = dialog.OpenFile();
            }
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelPresentationVote.Dispose();
            this.DataContextViewModelPresentationVote = null;
            this.DataContext = null;
        }
        #endregion
    }
}

