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
using GreenField.DashboardModule.Helpers;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;


namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboard : UserControl
    {

        #region Fields
        private IManageDashboard _manageDashboard;
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private DashboardGadgetPayload _dashboardGadgetPayLoad;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ViewDashboard(IManageDashboard manageDashboard, ILoggerFacade logger,
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
            _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Subscribe(HandleDashboardTileViewItemAdded);

            //Check for Empty Dashboard
            this.rtvDashboard.LayoutUpdated += (se, e) =>
                {
                    this.txtNoGadgetMessage.Visibility = this.rtvDashboard.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                    this.rtvDashboard.Visibility = this.rtvDashboard.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                    for (int index = 0; index < rtvDashboard.Items.Count; index++)
                    {
                        (rtvDashboard.Items[index] as RadTileViewItem).Opacity =
                            (rtvDashboard.Items[index] as RadTileViewItem).TileState == TileViewItemState.Minimized ? 0.5 : 1;
                    }
                };

        }
        #endregion

        private void DashboardTileViewItemRemoved(object sender, RoutedEventArgs e)
        {
            try
            {
                RadTileViewItem tileViewItem = (sender as Button).ParentOfType<RadTileViewItem>();
                this.rtvDashboard.Items.Remove(tileViewItem);
                this.rtvDashboard.Visibility = this.rtvDashboard.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                this.txtNoGadgetMessage.Visibility = this.rtvDashboard.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

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
        /// <param name="param">DashboardTileViewItemInfo</param>
        public void HandleDashboardTileViewItemAdded(DashboardTileViewItemInfo param)
        {
            try
            {
                if (rtvDashboard.Items.Count == 1)
                    rtvDashboard.RowHeight = new GridLength(400);

                this.rtvDashboard.Items.Add(new RadTileViewItem
                    {
                        RestoredHeight = 400,
                        Header = param.DashboardTileHeader,
                        Content = param.DashboardTileObject,
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
        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payLoad)
        {
            try
            {
                if (SessionManager.SESSION != null)
                {
                    this.busyIndicator.IsBusy = true;
                    if (_dashboardGadgetPayLoad != null)
                    {
                        this.busyIndicator.IsBusy = false;
                        return;
                    }
                    _dashboardGadgetPayLoad = payLoad;
                    _manageDashboard.GetDashboardPreferenceByUserName(SessionManager.SESSION.UserName, (dashboardPreference) =>
                            {
                                this.busyIndicator.IsBusy = false;

                                ObservableCollection<tblDashboardPreference> preference = new ObservableCollection<tblDashboardPreference>(dashboardPreference.OrderBy(t => t.GadgetPosition));
                                if (preference.Count > 0 && rtvDashboard.Items.Count.Equals(0))
                                {
                                    if (preference.Count > 1)
                                    {
                                        rtvDashboard.RowHeight = new GridLength(400);
                                        foreach (tblDashboardPreference item in preference)
                                        {
                                            RadTileViewItem radTileViewItem = new RadTileViewItem
                                            {
                                                Header = item.GadgetName,
                                                RestoredHeight = 400,
                                                Content = GetDashboardTileContent(item.GadgetViewClassName, item.GadgetViewModelClassName),
                                                TileState = GetTileState(item.GadgetState),
                                                Position = item.GadgetPosition
                                            };
                                            this.rtvDashboard.Items.Add(radTileViewItem);

                                        }
                                    }
                                    if (preference.Count == 1)
                                    {
                                        foreach (tblDashboardPreference item in preference)
                                        {
                                            RadTileViewItem radTileViewItem = new RadTileViewItem
                                            {
                                                Header = item.GadgetName,
                                                RestoredHeight = 400,
                                                Content = GetDashboardTileContent(item.GadgetViewClassName, item.GadgetViewModelClassName),
                                                TileState = GetTileState("Restored"),
                                                Position = item.GadgetPosition
                                            };
                                            this.rtvDashboard.Items.Add(radTileViewItem);
                                        }
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
                if (this.rtvDashboard.Items.Count > 0)
                {
                    string uniqueKey = SessionManager.SESSION.UserName + "-" + DateTime.Now.ToString();
                    for (int index = 0; index < rtvDashboard.Items.Count; index++)
                    {
                        tblDashboardPreference entry = new tblDashboardPreference()
                        {
                            UserName = SessionManager.SESSION.UserName,
                            PreferenceGroupID = uniqueKey,
                            GadgetViewClassName = TypeResolution.GetTypeFullName((rtvDashboard.Items[index] as RadTileViewItem).Content),
                            GadgetViewModelClassName = TypeResolution.GetTypeDataContextFullName((rtvDashboard.Items[index] as RadTileViewItem).Content),
                            GadgetName = (rtvDashboard.Items[index] as RadTileViewItem).Header.ToString(),
                            GadgetState = (rtvDashboard.Items[index] as RadTileViewItem).TileState.ToString(),
                            GadgetPosition = (rtvDashboard.Items[index] as RadTileViewItem).Position
                        };

                        dashboardPreference.Add(entry);
                    }
                }
                _manageDashboard.SetDashboardPreference(dashboardPreference,SessionManager.SESSION.UserName, (result) =>
                    {
                        if (result)
                            MessageBox.Show("User Preference saved");
                        else
                            MessageBox.Show("User Preference save failed");
                    });
            }
            catch (Exception ex)
            {
                _logger.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
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
        private object GetDashboardTileContent(string gadgetViewClassName, string gadgetViewModelClassName)
        {
            object content = null;

            try
            {
                Assembly assembly = TypeResolution.GetAssembly(gadgetViewClassName);
                Type viewType = TypeResolution.GetAssemblyType(gadgetViewClassName);
                Type viewModelType = TypeResolution.GetAssemblyType(gadgetViewModelClassName);

                if (viewType.IsClass && viewModelType.IsClass)
                {
                    Type[] argumentTypes = new Type[] { typeof(DashboardGadgetParam) };
                    object[] argumentValues = new object[] { new DashboardGadgetParam()
                    {
                        LoggerFacade = _logger,
                        EventAggregator = _eventAggregator,
                        DBInteractivity = _dbInteractivity,
                        DashboardGadgetPayload = _dashboardGadgetPayLoad                        
                    }};
                    object viewModelObject = TypeResolution.GetNewTypeObject(viewModelType, argumentTypes, argumentValues);
                    content = TypeResolution.GetNewTypeObject(viewType, new Type[] { viewModelType }, new object[] { viewModelObject });
                }
            }
            catch (Exception ex)
            {
                _logger.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }

            return content;

        }


    }
}
