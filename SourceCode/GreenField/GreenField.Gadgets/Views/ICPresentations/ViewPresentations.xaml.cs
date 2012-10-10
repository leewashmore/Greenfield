using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// code behind for ViewPresentations
    /// </summary>
    public partial class ViewPresentations : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentations _dataContextViewModelPresentations;
        public ViewModelPresentations DataContextViewModelPresentations
        {
            get { return _dataContextViewModelPresentations; }
            set { _dataContextViewModelPresentations = value; }
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
                if (DataContextViewModelPresentations != null)
                {
                    DataContextViewModelPresentations.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPresentations(ViewModelPresentations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentations = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelPresentations.Dispose();
            this.DataContextViewModelPresentations = null;
            this.DataContext = null;
        }
        #endregion
    }
}
