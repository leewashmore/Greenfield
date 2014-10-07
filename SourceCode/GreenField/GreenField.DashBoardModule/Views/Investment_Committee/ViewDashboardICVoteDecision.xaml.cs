using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using System;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardICVoteDecision : UserControl, INavigationAware
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        private IRegionManager _regionManager;
        private RadTileView _radTileview;
        #endregion

        [ImportingConstructor]
        public ViewDashboardICVoteDecision(ILoggerFacade logger, IEventAggregator eventAggregator,
             IDBInteractivity dbInteractivity, IRegionManager regionManager)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
            _logger = logger;
            _dBInteractivity = dbInteractivity;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Subscribe(HandleDashboardTileViewItemAdded);
           // this.tbHeader.Text = GadgetNames.ICPRESENTATION_VOTE;
        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.rtvDashboard.Items.Count > 0)
                return;


            this.rtvDashboard.Items.Clear();
            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = _dBInteractivity,
                EventAggregator = _eventAggregator,
                LoggerFacade = _logger,
                RegionManager = _regionManager
            };

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = GadgetNames.ICPRESENTATION_PRESENTATIONS,
                Content = new ViewPresentations(new ViewModelPresentations(param), DashboardCategoryType.INVESTMENT_COMMITTEE_IC_VOTE_DECISION),
                RestoredHeight = 300
            });

            

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = GadgetNames.ICPRESENTATION_VOTE,
                Content = new ViewPresentationVote(new ViewModelPresentationVote(param)),
                RestoredHeight = 300
            });



            if (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
            {


                this.rtvDashboard.Items.Add(new RadTileViewItem
                {

                    Header = GadgetNames.ICPRESENTATION_PRESENTATIONS_DECISION_ENTRY,
                    Content = new ViewPresentationDecisionEntry(new ViewModelPresentationDecisionEntry(param)),
                    RestoredHeight = 600
                });

            }
            

    

     
            
        


        }


        #region INavigationAware methods
        /// <summary>
        /// Returns true if satisfies requisite condition
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        /// <returns>True/False</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// Executed on navigation from this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(false);
        }

        /// <summary>
        /// Executed on navigation to this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(true);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Set IsActive property on Dashboard content
        /// </summary>
        /// <param name="value">IsActive value</param>
        private void SetIsActiveOnDahsboardItems(bool value)
        {
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)item.Content;
                if (control != null)
                {
                    control.IsActive = value;
                    
                }
            }
        }



        public void HandleDashboardTileViewItemAdded(DashboardTileViewItemInfo param)
        {
            try
            {
                foreach (RadTileViewItem rtvitem in this.rtvDashboard.Items)
                {
                    if( rtvitem.Header as string ==  param.DashboardTileHeader)
                    {
                        ViewBaseUserControl control = (ViewBaseUserControl)(rtvitem.Content);
                        if (control != null)
                        {
                            control.IsActive = true;
                        }

                    }
                }
             
            }
            catch (InvalidOperationException)
            {
                //System generates data errors that could be ignored
            }
        }

        #endregion
    }
}
