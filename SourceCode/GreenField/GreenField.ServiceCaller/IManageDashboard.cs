﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using GreenField.ServiceCaller.DashboardDefinitions;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;

namespace GreenField.ServiceCaller
{
    public interface IManageDashboard
    {
        void GetDashboardPreferenceByUserName(String userName, Action<List<tblDashboardPreference>> callback);

        void SetDashboardPreference(ObservableCollection<tblDashboardPreference> dashBoardPreference,string userName, Action<bool?> callback);
    }
}
