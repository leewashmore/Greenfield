using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View for ContributorDetractor class
    /// </summary>
    public partial class ViewContributorDetractor : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelContributorDetractor _dataContextContributorDetractor;
        public ViewModelContributorDetractor DataContextContributorDetractor
        {
            get { return _dataContextContributorDetractor; }
            set { _dataContextContributorDetractor = value; }
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
                if (DataContextContributorDetractor != null)
                {
                    DataContextContributorDetractor.IsActive = _isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewContributorDetractor(ViewModelContributorDetractor dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextContributorDetractor = dataContextSource;
        } 
        #endregion       

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextContributorDetractor.Dispose();
            this.DataContextContributorDetractor = null;
            this.DataContext = null;
        }
        #endregion
    }
}
