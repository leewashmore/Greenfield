using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    public partial class ViewSecurityOverview : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSecurityOverview dataContextSecurityOverview;
        public ViewModelSecurityOverview DataContextSecurityOverview
        {
            get { return dataContextSecurityOverview; }
            set { dataContextSecurityOverview = value; }
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
                if (DataContextSecurityOverview != null)
                { DataContextSecurityOverview.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSecurityOverview(ViewModelSecurityOverview dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSecurityOverview = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextSecurityOverview.Dispose();
            this.DataContextSecurityOverview = null;
            this.DataContext = null;
        }
        #endregion
    }
}
