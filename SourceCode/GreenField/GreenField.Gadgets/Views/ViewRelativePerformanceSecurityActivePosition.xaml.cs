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
        private ViewModelRelativePerformanceSecurityActivePosition _dataContextRelativePerformanceSecurityActivePosition;
        public ViewModelRelativePerformanceSecurityActivePosition DataContextRelativePerformanceSecurityActivePosition
        {
            get { return _dataContextRelativePerformanceSecurityActivePosition; }
            set { _dataContextRelativePerformanceSecurityActivePosition = value; }
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
                if (DataContextRelativePerformanceSecurityActivePosition != null) //DataContext instance
                    DataContextRelativePerformanceSecurityActivePosition.IsActive = _isActive;
            }
        }

        #endregion

        #region Constructor
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

        private void dgRelativePerformance_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}