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
    public partial class ViewPresentationVote : ViewBaseUserControl
    {
        private Decimal _committeeBuyRange;
        private Decimal _committeeSellRange;        

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentationVote _dataContextViewModelPresentationVote;
        public ViewModelPresentationVote DataContextViewModelPresentationVote
        {
            get { return _dataContextViewModelPresentationVote; }
            set { _dataContextViewModelPresentationVote = value; }
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
                if (DataContextViewModelPresentationVote != null) //DataContext instance
                    DataContextViewModelPresentationVote.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPresentationVote(ViewModelPresentationVote dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentationVote = dataContextSource;
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

        #region Event Handlers
        private void txtbBuyRange_LostFocus(object sender, RoutedEventArgs e)
        {
            Decimal committeeBuyRange;
            if (!Decimal.TryParse(this.txtbBuyRange.Text, out committeeBuyRange))
            {
                this.txtbBuyRange.Text = _committeeBuyRange.ToString();
                return;
            }

            _committeeBuyRange = committeeBuyRange;
        }

        private void txtbSellRange_LostFocus(object sender, RoutedEventArgs e)
        {
            Decimal committeeSellRange;
            if (!Decimal.TryParse(this.txtbSellRange.Text, out committeeSellRange))
            {
                this.txtbSellRange.Text = _committeeSellRange.ToString();
                return;
            }

            _committeeSellRange = committeeSellRange;
        }

        private void cbVoteType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            DataContextViewModelPresentationVote.RaiseUpdateVoteType(this.cbVoteType.SelectedValue as String);
        }
        
        #endregion

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "PDF (*.pdf) |*.pdf" };
            if (dialog.ShowDialog() == true)
            {
                DataContextViewModelPresentationVote.DownloadStream = dialog.OpenFile();
            }
        }

        

       

        

        
    }
}

