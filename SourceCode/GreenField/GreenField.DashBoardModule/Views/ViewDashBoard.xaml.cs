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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.UserSession;
using Microsoft.Practices.Prism.Regions;
using GreenField.DataContracts;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboard : UserControl, INavigationAware
    {
        #region Fields
        private IManageDashboard manageDashboard;
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private DashboardGadgetPayload dashboardGadgetPayLoad;
        private IRegionManager regionManager;
        private object dcfViewModelObject;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ViewDashboard(IManageDashboard manageDashboard, ILoggerFacade logger,
            IEventAggregator eventAggregator, IDBInteractivity dbInteractivity, IRegionManager regionManager)
        {
            InitializeComponent();

            //Initialize MEF singletons
            this.eventAggregator = eventAggregator;
            this.manageDashboard = manageDashboard;
            this.logger = logger;
            this.dbInteractivity = dbInteractivity;
            this.regionManager = regionManager;
            //Subscribe to MEF Events
            eventAggregator.GetEvent<DashboardGadgetSave>().Subscribe(HandleDashboardGadgetSave);
            eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            eventAggregator.GetEvent<DashboardTileViewItemAdded>().Subscribe(HandleDashboardTileViewItemAdded);

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

            this.rtvDashboard.TileStateChanged += (se, e) =>
            {
                if (this.rtvDashboard.Items.Count == 1)
                {
                    if ((e.Source as RadTileViewItem).TileState != TileViewItemState.Maximized)
                    {
                        (e.Source as RadTileViewItem).TileState = TileViewItemState.Maximized;
                    }
                }
            };
        }
        #endregion

        private void DashboardTileViewItemRemoved(object sender, RoutedEventArgs e)
        {
            try
            {
                RadTileViewItem tileViewItem = (sender as Button).ParentOfType<RadTileViewItem>();
                string gadgetName = (tileViewItem.Header as Telerik.Windows.Controls.HeaderedContentControl).Content as string;

                switch (gadgetName)
                {
                    case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_ASSUMPTIONS:
                    case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_TERMINAL_VALUE_CALCULATIONS:
                    case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SUMMARY:
                    case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SENSIVITY:
                    case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_EPS:
                    case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_BVPS:
                    case GadgetNames.HOLDINGS_FREE_CASH_FLOW:
                        {
                            for (int i = 0; i < this.rtvDashboard.Items.Count; i++)
                            {
                                string gName = ((this.rtvDashboard.Items[i] as RadTileViewItem).Header as 
                                    Telerik.Windows.Controls.HeaderedContentControl).Content as string;
                                if (gName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_ASSUMPTIONS ||
                                    gName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_TERMINAL_VALUE_CALCULATIONS ||
                                    gName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SUMMARY ||
                                    gName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SENSIVITY ||
                                    gName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_EPS ||
                                    gName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_BVPS || 
                                    gName == GadgetNames.HOLDINGS_FREE_CASH_FLOW)
                                {
                                    this.rtvDashboard.Items.RemoveAt(i);
                                    i--;
                                }                                
                            }
                        }
                        break;
                    default:
                        {
                            this.rtvDashboard.Items.Remove(tileViewItem);
                            (tileViewItem.Content as ViewBaseUserControl).Dispose();
                            tileViewItem.Content = null;                            
                        }
                        break;
                }

                this.rtvDashboard.Visibility = this.rtvDashboard.Items.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                this.txtNoGadgetMessage.Visibility = this.rtvDashboard.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                //_logger.Log("User : " + SessionManager.SESSION.UserName +"\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
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
                RadTileViewItem item = new RadTileViewItem();
                item.RestoredHeight = 300;                
                item.Header = new Telerik.Windows.Controls.HeaderedContentControl 
                {
                    Content = param.DashboardTileHeader, 
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize=8,
                    FontFamily= new FontFamily("Arial") 
                };
                item.Content = param.DashboardTileObject;
                this.rtvDashboard.Items.Add(item);

                ViewBaseUserControl control = (ViewBaseUserControl)(item.Content);
                if (control != null)
                {
                    control.IsActive = true;
                }

                if (this.rtvDashboard.Items.Count == 1)
                {
                    this.rtvDashboard.RowHeight = new GridLength(400);
                    (this.rtvDashboard.Items[0] as RadTileViewItem).TileState = TileViewItemState.Maximized;                                        
                }
                if (this.rtvDashboard.Items.Count == 2)
                {
                    (this.rtvDashboard.Items[0] as RadTileViewItem).TileState = TileViewItemState.Restored;
                    (this.rtvDashboard.Items[1] as RadTileViewItem).TileState = TileViewItemState.Restored;
                }
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
                    if (dashboardGadgetPayLoad != null)
                    {
                        this.busyIndicator.IsBusy = false;
                        return;
                    }
                    dashboardGadgetPayLoad = payLoad;
                    manageDashboard.GetDashboardPreferenceByUserName(SessionManager.SESSION.UserName, (dashboardPreference) =>
                            {
                                this.busyIndicator.IsBusy = false;
                                
                                ObservableCollection<tblDashboardPreference> preference = new ObservableCollection<tblDashboardPreference>(dashboardPreference.OrderBy(t => t.GadgetPosition));
                                if (preference.Count > 0 && rtvDashboard.Items.Count.Equals(0))
                                {
                                    foreach (tblDashboardPreference item in preference)
                                    {
                                        switch (item.GadgetName)
                                        {
                                            case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_ASSUMPTIONS:
                                            case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_TERMINAL_VALUE_CALCULATIONS:
                                            case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SUMMARY:
                                            case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SENSIVITY:
                                            case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_EPS:
                                            case GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_BVPS:
                                                {
                                                    if (dcfViewModelObject == null)
                                                    {
                                                        Type[] argumentTypes = new Type[] { typeof(DashboardGadgetParam) };
                                                        object[] argumentValues = new object[] { GetDashboardGadgetParam() };
                                                        dcfViewModelObject = TypeResolution.GetNewTypeObject(typeof(ViewModelDCF), argumentTypes, argumentValues);
                                                    }

                                                    InsertGadget(item.GadgetName, item.GadgetViewClassName, item.GadgetViewModelClassName, item.GadgetPosition,
                                                        dcfViewModelObject, preference.Count == 1 ? "Maximized" : item.GadgetState);
                                                }
                                                break;
                                            default:
                                                {
                                                    InsertGadget(item.GadgetName, item.GadgetViewClassName, item.GadgetViewModelClassName, item.GadgetPosition,
                                                        null, preference.Count == 1 ? "Maximized" : item.GadgetState);
                                                }
                                                break;
                                        }
                                    }                            
                                }
                            });
                }
            }
            catch (Exception ex)
            {
                logger.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }
        }

        private void InsertGadget(string displayName, string gadgetViewClassName, string gadgetViewModelClassName, int gadgetPosition, object viewModelObject = null, String tileState = "Restored")
        {
            RadTileViewItem radTileViewItem = new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl 
                { 
                    Content = displayName,
                    Foreground = new SolidColorBrush(Colors.Black)
                },
                Foreground = new SolidColorBrush(Colors.Black),
                RestoredHeight = 300,
                Content = GetDashboardTileContent(displayName, gadgetViewClassName, gadgetViewModelClassName, viewModelObject),
                Position = gadgetPosition,
                TileState = GetTileState(tileState)                
            };
            this.rtvDashboard.Items.Add(radTileViewItem);
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
                            GadgetName = ((rtvDashboard.Items[index] as RadTileViewItem).Header as Telerik.Windows.Controls.HeaderedContentControl).Content.ToString(),
                            GadgetState = (rtvDashboard.Items[index] as RadTileViewItem).TileState.ToString(),
                            GadgetPosition = (rtvDashboard.Items[index] as RadTileViewItem).Position
                        };

                        dashboardPreference.Add(entry);
                    }
                }
                manageDashboard.SetDashboardPreference(dashboardPreference, SessionManager.SESSION.UserName, (result) =>
                    {
                        if (result != null)
                        {
                            if ((bool)result)
                                Prompt.ShowDialog("User Preference saved");
                            else
                                Prompt.ShowDialog("User Preference save failed");
                        }
                        else
                        {
                            Prompt.ShowDialog("User Preference save failed");
                        }
                    });
            }
            catch (Exception ex)
            {
                logger.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
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
        /// Construct content within propertyName dashboard gadget. It is assumed that view consists of propertyName
        /// constructor that takes viewmodel instance and view model class consists od propertyName cons-
        /// tructor that takes IEventAggregator, IDBInteractivity and ILoggerFacade instance.
        /// </summary>
        /// <param name="gadgetViewClassName">Gadget view Type</param>
        /// <param name="gadgetViewModelClassName">Gadget View Model Type</param>
        /// <returns></returns>
        private object GetDashboardTileContent(string displayName, string gadgetViewClassName, string gadgetViewModelClassName, object viewModelObject = null)
        {
            object content = null;

            try
            {
                Assembly assembly = TypeResolution.GetAssembly(gadgetViewClassName);
                Type viewType =  TypeResolution.GetAssemblyType(gadgetViewClassName);
                Type viewModelType = viewModelObject == null ? TypeResolution.GetAssemblyType(gadgetViewModelClassName) : viewModelObject.GetType();

                if (viewType.IsClass && viewModelType.IsClass)
                {
                    Type[] argumentTypes = new Type[] { typeof(DashboardGadgetParam) };
                    object[] argumentValues = new object[] { GetDashboardGadgetParam(displayName) };
                    if (viewModelObject == null)
                    {
                        viewModelObject = TypeResolution.GetNewTypeObject(viewModelType, argumentTypes, argumentValues);
                    }
                    content = TypeResolution.GetNewTypeObject(viewType, new Type[] { viewModelType }, new object[] { viewModelObject });
                    ViewBaseUserControl control = (ViewBaseUserControl)(content);
                    if (control != null)
                    {
                        control.IsActive = true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Log("User : " + SessionManager.SESSION.UserName + "\nMessage: " + ex.Message + "\nStackTrace: " + ex.StackTrace, Category.Exception, Priority.Medium);
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + ex.StackTrace, "Exception", MessageBoxButton.OK);
            }

            return content;

        }

        private DashboardGadgetParam GetDashboardGadgetParam(String gadgetName = null)
        {
            DashboardGadgetParam param;
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                Object additionalInfo = null;

                switch (gadgetName)
                {
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_BANK:
                        additionalInfo = ScatterChartDefaults.BANK;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INDUSTRIAL:
                        additionalInfo = ScatterChartDefaults.INDUSTRIAL;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INSURANCE:
                        additionalInfo = ScatterChartDefaults.INSURANCE;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_UTILITY:
                        additionalInfo = ScatterChartDefaults.UTILITY;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_BALANCE_SHEET:
                        additionalInfo = FinancialStatementType.BALANCE_SHEET;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_INCOME_STATEMENT:
                        additionalInfo = FinancialStatementType.INCOME_STATEMENT;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_CASH_FLOW:
                        additionalInfo = FinancialStatementType.CASH_FLOW_STATEMENT;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_FUNDAMENTALS_SUMMARY:
                        additionalInfo = FinancialStatementType.FUNDAMENTAL_SUMMARY;
                        break;
                    default:
                        break;
                }
                param = new DashboardGadgetParam()
                {
                    DBInteractivity = dbInteractivity,
                    EventAggregator = eventAggregator,
                    AdditionalInfo = additionalInfo,
                    LoggerFacade = logger,
                    DashboardGadgetPayload = dashboardGadgetPayLoad,
                    RegionManager = regionManager
                };
            }
            catch (Exception ex)
            {
                param = null;
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }

            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            return param;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)(item.Content);
                if (control != null)
                {
                    control.IsActive = false;
                }
            }    
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)(item.Content);
                if (control != null)
                {
                    control.IsActive = true;
                }
            }            
        }
    }
}
