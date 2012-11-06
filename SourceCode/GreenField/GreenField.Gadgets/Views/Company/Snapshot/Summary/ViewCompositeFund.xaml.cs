using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// class for view for ConsensusEstimatesDetails
    /// </summary>
    public partial class ViewCompositeFund : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCompositeFund dataContextCompositeFund;
        public ViewModelCompositeFund DataContextCompositeFund
        {
            get { return dataContextCompositeFund; }
            set { dataContextCompositeFund = value; }
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
                if (DataContextCompositeFund != null)
                { DataContextCompositeFund.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCompositeFund(ViewModelCompositeFund dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextCompositeFund = dataContextSource;
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextCompositeFund.Dispose();
            this.DataContextCompositeFund = null;
            this.DataContext = null;
        }
        #endregion
    }
}
