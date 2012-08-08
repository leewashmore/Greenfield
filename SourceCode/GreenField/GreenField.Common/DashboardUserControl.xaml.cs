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
using GreenField.Common.Helper;
using GreenField.Common.DragDockPanelControls;
using System.Collections;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.DashboardServiceReference;

namespace GreenField.Common
{
    public partial class DashboardUserControl : UserControl
    {
        Dashboard _dashboard = new Dashboard();
        string strSecurityType; 
        string userSettings="";
        PersistPreference persistDDPanel;
        List<PersistPreference> userPreference = new List<PersistPreference>();
        List<PersistPreference> userPreferenceDeserialize = new List<PersistPreference>();
        bool isPopulatingPref = false;
        DataGrid dgContent;

        public DashboardUserControl()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DashboardUserControl_Loaded);
        }

        void DashboardUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _dashboard.GetUserPreference(1, GetUserPreferenceCompleted);
        }

        public void GetUserPreferenceCompleted(List<StoredUserPreference> result)
        {
            userSettings = result.FirstOrDefault().UserPreference.ToString();
            SetPersonalizedLayout();
        }

        public List<PersistPreference> UserPreferenceList
        {
            get { return (List<PersistPreference>)(GetValue(UserPreferenceListProperty)); }
            set { SetValue(UserPreferenceListProperty, value); }
        }

        public static readonly DependencyProperty UserPreferenceListProperty =
        DependencyProperty.Register(
        "UserPreferenceList",
        typeof(List<PersistPreference>),
        typeof(DashboardUserControl),
        new PropertyMetadata(new PropertyChangedCallback(OnUserPreferenceListChanged))
        );

        public IList DataSource
        {
            get { return (IList)(GetValue(DataSourceProperty)); }
            set { SetValue(DataSourceProperty, value); }
        }

        public static readonly DependencyProperty DataSourceProperty =
        DependencyProperty.Register(
        "DataSource",
        typeof(IList),
        typeof(DashboardUserControl),
        new PropertyMetadata(new PropertyChangedCallback(OnUserPreferenceListChanged))
        );

        private static void OnUserPreferenceListChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DashboardUserControl controlObject = (DashboardUserControl)(o);
           // controlObject.SetPersonalizedLayout();
        }

        private void SetPersonalizedLayout()
        {
            if (null != UserPreferenceList && null != DataSource)
            {
                userPreference= Serializer.Deserialize<List<PersistPreference>>(userSettings);
                isPopulatingPref = true;
                foreach (PersistPreference item in userPreference)
                {
                    Button btnPopulate = new Button();
                    //btnPopulate.Tag = new KeyValuePair<int, string>(item.Id, item.Content as string);
                    btnPopulate.Tag = item.Content as string;
                    this.AddButton_Click(btnPopulate, new RoutedEventArgs());
                    SetItemIndex(item.Position);
                }
                isPopulatingPref = false;
                userPreference.Clear();
                userPreference = userPreferenceDeserialize;
                //LayoutGrid.Deserialize<PersistingDragGridPanel>(SerializedLayout, personalizedDGPanel);
            }
        }

        private void SetItemIndex(int position)
        {
            int count = LayoutGrid.Children.Count;
            LayoutGrid.SetIndex(LayoutGrid.Children[count - 1], position);
            LayoutGrid.PreparePersistedPanel(LayoutGrid.Children[count - 1]);
        }

        private void RegisterEvents()
        {
            AddButtonSec.Click += new RoutedEventHandler(AddButton_Click);
            AddButtonCur.Click += new RoutedEventHandler(AddButton_Click);
            AddButtonCom.Click += new RoutedEventHandler(AddButton_Click);
            //RemoveButton.Click += new RoutedEventHandler(RemoveButton_Click);
            //LayoutGrid.Deserialized += new EventHandler(LayoutGrid_Deserialized);
            //LayoutGrid.Serialized += new EventHandler(LayoutGrid_Serialized);
        }

        private object GetListSecurity(string securityType)
        {
            dgContent = new DataGrid();
            dgContent.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            dgContent.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            dgContent.ItemsSource = DataSource;
            return dgContent;
        }

        void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (LayoutGrid.Children.Count > 1)
                LayoutGrid.Children.RemoveAt(LayoutGrid.Children.Count - 1);
        }

        void AddButton_Click(object sender, RoutedEventArgs e)
        {
            strSecurityType = (sender as Button).Tag as string;
            PopulatePanels(strSecurityType);
        }

        private void PopulatePanels(string strSecType)
        {
            DragDockPanel childDDPanel;
            int panelId = new Random((int)((DateTime.Now.Ticks) % (Int32.MaxValue))).Next();
            switch (strSecType)
            {
                case "Security":
                    childDDPanel = new DragDockPanel { Content = GetListSecurity("Share"), Header = new string[] { "Security Basket", panelId.ToString() } };
                    childDDPanel.Closed += childDDPanel_Closed;
                    PopulateUserPreferences(panelId, childDDPanel, "Security Basket");
                    LayoutGrid.Children.Add(childDDPanel);
                    return;
                case "Currency":
                    childDDPanel = new DragDockPanel { Content = GetListSecurity("Currency"), Header = new string[] { "Currency Basket", panelId.ToString() } };
                    childDDPanel.Closed += childDDPanel_Closed;
                    PopulateUserPreferences(panelId, childDDPanel, "Currency Basket");
                    LayoutGrid.Children.Add(childDDPanel);

                    return;
                case "Commodity":
                    childDDPanel = new DragDockPanel { Content = GetListSecurity("Commodity"), Header = new string[] { "Commodity Basket", panelId.ToString() } };
                    childDDPanel.Closed += childDDPanel_Closed;
                    PopulateUserPreferences(panelId, childDDPanel, "Commodity Basket");
                    LayoutGrid.Children.Add(childDDPanel);
                    return;
                default:
                    childDDPanel = new DragDockPanel { Content = GetListSecurity("Share"), Header = new string[] { "Security Basket", panelId.ToString() } };
                    childDDPanel.Closed += childDDPanel_Closed;
                    PopulateUserPreferences(panelId, childDDPanel, "Security Basket");
                    LayoutGrid.Children.Add(childDDPanel);
                    return;
            }
        }

        private void PopulateUserPreferences(int id, DragDockPanel child, string header)
        {
            if (!isPopulatingPref)
            {
                persistDDPanel = new PersistPreference();
                persistDDPanel.Id = id;
                persistDDPanel.Content = child;
                persistDDPanel.Header = header;
                userPreference.Add(persistDDPanel);
            }
            else
            {
                persistDDPanel = new PersistPreference();
                persistDDPanel.Id = id;
                persistDDPanel.Content = child;
                persistDDPanel.Header = header;
                userPreferenceDeserialize.Add(persistDDPanel);
            }
        }

        void childDDPanel_Closed(object sender, EventArgs e)
        {
            List<PersistPreference> dummyUserPreference = new List<PersistPreference>(userPreference);

            foreach (PersistPreference item in dummyUserPreference)
            {
                int id = Convert.ToInt32(((sender as DragDockPanel).Header as string[])[1]);
                if (item.Id == id)
                {
                    userPreference.Remove(item);
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement child in LayoutGrid.Children)
            {
                if (child is DragDockPanel)
                {
                    //ddPanelProperty.Header=((DragDockPanel)child).Header as string;
                    foreach (PersistPreference item in userPreference)
                    {
                        if (item.Content.Equals(child))
                        {
                            item.Position = LayoutGrid.GetIndex(child);
                            item.Content = ((string)item.Header).Split(' ')[0];
                        }
                    }
                }
            }
            string strSer = Serializer.Serialize<List<PersistPreference>>(userPreference);
            UpdateUserPref(1, strSer);
        }

        private void UpdateUserPref(int userID, string strSer)
        {
            _dashboard.StoreUserPreference(userID, strSer, (result) => { });
        }
    }
}