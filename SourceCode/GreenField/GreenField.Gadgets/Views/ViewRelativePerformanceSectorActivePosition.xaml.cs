using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View for RelativePerformanceSectorActivePosition class
    /// </summary>
    public partial class ViewRelativePerformanceSectorActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceSectorActivePosition dataContextRelativePerformanceSectorActivePosition;
        public ViewModelRelativePerformanceSectorActivePosition DataContextRelativePerformanceSectorActivePosition
        {
            get { return dataContextRelativePerformanceSectorActivePosition; }
            set { dataContextRelativePerformanceSectorActivePosition = value; }
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
                if (DataContextRelativePerformanceSectorActivePosition != null)
                {
                    DataContextRelativePerformanceSectorActivePosition.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceSectorActivePosition(ViewModelRelativePerformanceSectorActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceSectorActivePosition = dataContextSource;
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceSectorActivePosition.Dispose();
            this.DataContextRelativePerformanceSectorActivePosition = null;
            this.DataContext = null;
        }
        #endregion
    }
}
