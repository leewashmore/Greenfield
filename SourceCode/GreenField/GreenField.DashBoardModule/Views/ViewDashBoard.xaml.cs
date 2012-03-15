using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using Telerik.Windows.Persistence;
using Telerik.Windows.Controls;
using System.IO;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.DashboardDefinitions;
using System.Collections.ObjectModel;
using GreenField.Gadgets;
using GreenField.Gadgets.Views;
using GreenField.Gadgets.ViewModels;
using System.Reflection;
using GreenField.DashBoardModule.Helpers;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common.Helper;


namespace GreenField.DashBoardModule.Views
{
    [Export]
    public partial class ViewDashBoard : UserControl
    {

        #region Fields
        private IManageDashboard _manageDashboard;
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private DashboardGadgetPayLoad _dashboardGadgetPayLoad;        
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ViewDashBoard(IManageDashboard manageDashboard, ILoggerFacade logger,
            IEventAggregator eventAggregator, IDBInteractivity dbInteractivity)
        {
            InitializeComponent();
            
            //Initialize MEF singletons
            _eventAggregator = eventAggregator;
            _manageDashboard = manageDashboard;
            _logger = logger;
            _dbInteractivity = dbInteractivity;

            //Subscribe to MEF Events
            _eventAggregator.GetEvent<DashboardGadgetSave>().Subscribe(HandleDashboardGadgetSave);
            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Subscribe(HandleDashBoardTileViewItemAdded);

            //Check for Empty Dashboard
            this.rtvDashBoard.LayoutUpdated += (se, e) =>
                {
                    this.txtNoGadgetMessage.Visibility = this.rtvDashBoard.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    this.rtvDashBoard.Visibility = this.rtvDashBoard.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                    for(int index = 0; index < rtvDashBoard.Items.Count; index++)
                    {
                        (rtvDashBoard.Items[index] as RadTileViewItem).Opacity =
                            (rtvDashBoard.Items[index] as RadTileViewItem).TileState == TileViewItemState.Minimized ? 0.5 : 1;
                    }                    
                };

        } 
        #endregion

        private void DashBoardTileViewItemRemoved(object sender, RoutedEventArgs e)
        {
            try
            {
                RadTileViewItem tileViewItem = (sender as Button).ParentOfType<RadTileViewItem>();
                this.rtvDashBoard.Items.Remove(tileViewItem);
                this.rtvDashBoard.Visibility = this.rtvDashBoard.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                this.txtNoGadgetMessage.Visibility = this.rtvDashBoard.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                //_logger.Log("User : " + SessionManager.SESSION.UserName +"\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }
        }
       
        /// <summary>
        /// Handle Dashboard Gadget Add event
        /// </summary>
        /// <param name="param">DashBoardTileViewItemInfo</param>
        public void HandleDashBoardTileViewItemAdded(DashBoardTileViewItemInfo param)
        {
            try
            {
                if (rtvDashBoard.Items.Count == 1)
                    rtvDashBoard.RowHeight = new GridLength(400);

                this.rtvDashBoard.Items.Add(new RadTileViewItem
                    {
                        RestoredHeight = 400,
                        Header = param.DashBoardTileHeader,
                        Content = param.DashBoardTileObject,
                    });                
            }
            catch (InvalidOperationException)
            {
                //System generates data errors that could be ignored
            }
        }

        /// <summary>
        /// Handles Dashboard Gadget Load event - reconstructs gadgets based on preference got from database.
        /// </summary>
        /// <param name="param"></param>
        public void HandleDashboardGadgetLoad(DashboardGadgetPayLoad payLoad)
        {
            try
            {
                if (SessionManager.SESSION != null)
                {
                    this.busyIndicator.IsBusy = true;

                    _dashboardGadgetPayLoad = payLoad;
                    _manageDashboard.GetDashboardPreferenceByUserName(SessionManager.SESSION.UserName, (dashboardPreference) =>
                            {
                                this.busyIndicator.IsBusy = false;

                                ObservableCollection<tblDashboardPreference> preference = new ObservableCollection<tblDashboardPreference>(dashboardPreference.OrderBy(t => t.GadgetPosition));
                                if (preference.Count > 0 && rtvDashBoard.Items.Count.Equals(0))
                                {
                                    if (preference.Count > 1)
                                        rtvDashBoard.RowHeight = new GridLength(400);
                                    foreach (tblDashboardPreference item in preference)
                                    {
                                        RadTileViewItem radTileViewItem = new RadTileViewItem
                                        {
                                            Header = item.GadgetName,
                                            RestoredHeight = 400,
                                            Content = GetDashBoardTileContent(item.GadgetViewClassName, item.GadgetViewModelClassName),
                                            TileState = GetTileState(item.GadgetState),
                                            Position = item.GadgetPosition
                                        };
                                        this.rtvDashBoard.Items.Add(radTileViewItem);

                                    }
                                }                                
                            });
                }
            }
            catch (Exception ex)
            {
                _logger.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);   
            }            
        }        

        /// <summary>
        /// Handles Dashboard Gadget Save event - records preference to database.
        /// </summary>
        /// <param name="param">null</param>
        public void HandleDashboardGadgetSave(object param)
        {
            try
            {
                ObservableCollection<tblDashboardPreference> dashboardPreference = new ObservableCollection<tblDashboardPreference>();
                if (this.rtvDashBoard.Items.Count > 0)
                {
                    string uniqueKey = SessionManager.SESSION.UserName + "-" + DateTime.Now.ToString();
                    for (int index = 0; index < rtvDashBoard.Items.Count; index++)
                    {
                        tblDashboardPreference entry = new tblDashboardPreference()
                        {
                            UserName = SessionManager.SESSION.UserName,
                            PreferenceGroupID = uniqueKey,
                            GadgetViewClassName = TypeResolution.GetTypeFullName((rtvDashBoard.Items[index] as RadTileViewItem).Content),
                            GadgetViewModelClassName = TypeResolution.GetTypeDataContextFullName((rtvDashBoard.Items[index] as RadTileViewItem).Content),
                            GadgetName = (rtvDashBoard.Items[index] as RadTileViewItem).Header.ToString(),
                            GadgetState = (rtvDashBoard.Items[index] as RadTileViewItem).TileState.ToString(),
                            GadgetPosition = (rtvDashBoard.Items[index] as RadTileViewItem).Position
                        };

                        dashboardPreference.Add(entry);                        
                    }
                }
                _manageDashboard.SetDashBoardPreference(dashboardPreference, (result) =>
                    {
                        if (result)
                            MessageBox.Show("User Preseference saved");
                        else
                            MessageBox.Show("User Preseference save failed");
                    });
            }
            catch (Exception ex)
            {
                _logger.Log("User : " + SessionManager.SESSION.UserName +"\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Converts string reference to TileViewItemState enumeration
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private TileViewItemState GetTileState(string state)
        {
            switch (state)
            {                    
                case "Maximized":
                    return TileViewItemState.Maximized;
                case "Minimized":
                    return TileViewItemState.Minimized;
                case "Restored":
                default:
                    return TileViewItemState.Restored;
            }
        }

        /// <summary>
        /// Construct content within a dashboard gadget. It is assumed that view consists of a
        /// constructor that takes viewmodel instance and view model class consists od a cons-
        /// tructor that takes IEventAggregator, IDBInteractivity and ILoggerFacade instance.
        /// </summary>
        /// <param name="gadgetViewClassName">Gadget view Type</param>
        /// <param name="gadgetViewModelClassName">Gadget View Model Type</param>
        /// <returns></returns>
        private object GetDashBoardTileContent(string gadgetViewClassName, string gadgetViewModelClassName)
        {
            object content = null;

            try
            {
                Assembly assembly = TypeResolution.GetAssembly(gadgetViewClassName);
                Type viewType = TypeResolution.GetAssemblyType(gadgetViewClassName);
                Type viewModelType = TypeResolution.GetAssemblyType(gadgetViewModelClassName);

                if (viewType.IsClass && viewModelType.IsClass)
                {
                    Type[] argumentTypes = new Type[] { typeof(DashBoardGadgetParam) };
                    object[] argumentValues = new object[] { new DashBoardGadgetParam()
                    {
                        LoggerFacade = _logger,
                        EventAggregator = _eventAggregator,
                        DBInteractivity = _dbInteractivity,
                        DashboardGadgetPayLoad = _dashboardGadgetPayLoad                        
                    }};
                    object viewModelObject = TypeResolution.GetNewTypeObject(viewModelType, argumentTypes, argumentValues);
                    content = TypeResolution.GetNewTypeObject(viewType, new Type[] { viewModelType }, new object[] { viewModelObject });
                }
            }
            catch (Exception ex)
            {
                _logger.Log("User : " + SessionManager.SESSION.UserName +"\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }

            return content;
                
        }

        
    }
}
