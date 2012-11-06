using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewMeetingConfigurationSchedule
    /// </summary>
    public partial class ViewMeetingConfigurationSchedule : ViewBaseUserControl
    {        
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelMeetingConfigSchedule dataContextViewModelMeetingConfigSchedule;
        public ViewModelMeetingConfigSchedule DataContextViewModelMeetingConfigSchedule
        {
            get { return dataContextViewModelMeetingConfigSchedule; }
            set { dataContextViewModelMeetingConfigSchedule = value; }
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
                if (DataContextViewModelMeetingConfigSchedule != null)
                {
                    DataContextViewModelMeetingConfigSchedule.IsActive = isActive;
                }
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelMeetingConfigSchedule</param>
        public ViewMeetingConfigurationSchedule(ViewModelMeetingConfigSchedule dataContextSource)
        {
            InitializeComponent();            
            this.DataContext = dataContextSource;
            this.DataContextViewModelMeetingConfigSchedule = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelMeetingConfigSchedule.Dispose();
            this.DataContextViewModelMeetingConfigSchedule = null;
            this.DataContext = null;
        }
        #endregion
    }
}