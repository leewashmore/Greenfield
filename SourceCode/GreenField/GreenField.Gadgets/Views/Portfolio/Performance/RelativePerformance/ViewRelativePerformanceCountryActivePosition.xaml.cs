using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View model for ViewContributorDetractor class
    /// </summary>
    public partial class ViewRelativePerformanceCountryActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceCountryActivePosition dataContextRelativePerformanceCountryActivePosition;
        public ViewModelRelativePerformanceCountryActivePosition DataContextRelativePerformanceCountryActivePosition
        {
            get { return dataContextRelativePerformanceCountryActivePosition; }
            set { dataContextRelativePerformanceCountryActivePosition = value; }
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
                if (DataContextRelativePerformanceCountryActivePosition != null) 
                {
                    DataContextRelativePerformanceCountryActivePosition.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceCountryActivePosition(ViewModelRelativePerformanceCountryActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceCountryActivePosition = dataContextSource;
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceCountryActivePosition.Dispose();
            this.DataContextRelativePerformanceCountryActivePosition = null;
            this.DataContext = null;
        }
        #endregion
    }
}
