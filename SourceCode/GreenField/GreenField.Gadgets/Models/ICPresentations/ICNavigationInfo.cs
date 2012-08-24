using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace GreenField.Gadgets.Models
{
    public enum ICNavigationInfo
    {
        PresentationOverviewInfo,
        ViewPluginFlagEnumerationInfo
    }

    public static class ICNavigation
    {
        public static Dictionary<ICNavigationInfo, Object>  NavigationInfo { get; set; }
        public static void Update(ICNavigationInfo key, Object value)
        {
            if (NavigationInfo == null)
                NavigationInfo = new Dictionary<ICNavigationInfo, object>();

            if (NavigationInfo.Keys.Contains(key))
            {
                NavigationInfo[key] = value;
                return;
            }

            NavigationInfo.Add(key, value);
        }
        public static Object Fetch(ICNavigationInfo key)
        {
            if (NavigationInfo == null)
                NavigationInfo = new Dictionary<ICNavigationInfo, object>();

            if (NavigationInfo.Keys.Contains(key))
            {
                return NavigationInfo[key];
            }

            return null;
        }


    }
}
