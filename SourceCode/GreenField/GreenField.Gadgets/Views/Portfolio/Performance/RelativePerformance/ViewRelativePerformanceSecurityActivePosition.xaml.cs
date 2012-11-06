using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// class for view for RelativePerformanceSecurityActivePosition
    /// </summary>
    public partial class ViewRelativePerformanceSecurityActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceSecurityActivePosition dataContextRelativePerformanceSecurityActivePosition;
        public ViewModelRelativePerformanceSecurityActivePosition DataContextRelativePerformanceSecurityActivePosition
        {
            get { return dataContextRelativePerformanceSecurityActivePosition; }
            set { dataContextRelativePerformanceSecurityActivePosition = value; }
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
                if (DataContextRelativePerformanceSecurityActivePosition != null)
                {
                    DataContextRelativePerformanceSecurityActivePosition.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceSecurityActivePosition(ViewModelRelativePerformanceSecurityActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceSecurityActivePosition = dataContextSource;
        } 
        #endregion
       
        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceSecurityActivePosition.Dispose();
            this.DataContextRelativePerformanceSecurityActivePosition = null;
            this.DataContext = null;
        }
        #endregion
    }
}