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
using GreenField.DataContracts;

namespace GreenField.Gadgets.Models
{
    public enum CSTNavigationInfo
    {
        SelectedDataList,
        Flag,
        ListName,
        Accessibility,
        CSTUserPreference
    }

    public static class CSTNavigation
    {
        public static Dictionary<CSTNavigationInfo, List<CSTUserPreferenceInfo>> NavigationInfo { get; set; }
        public static void Update(CSTNavigationInfo key, List<CSTUserPreferenceInfo> value)
        {
            if (NavigationInfo == null)
                NavigationInfo = new Dictionary<CSTNavigationInfo, List<CSTUserPreferenceInfo>>();

            if (NavigationInfo.Keys.Contains(key))
            {
                NavigationInfo[key] = value;
                return;
            }

            NavigationInfo.Add(key, value);
        }

        public static Dictionary<CSTNavigationInfo, Object> NavigationInfoString { get; set; }
        public static void UpdateString(CSTNavigationInfo key, Object value)
        {
            if (NavigationInfoString == null)
                NavigationInfoString = new Dictionary<CSTNavigationInfo, object>();

            if (NavigationInfoString.Keys.Contains(key))
            {
                NavigationInfoString[key] = value;
                return;
            }

            NavigationInfoString.Add(key, value);
        }

        public static Object Fetch(CSTNavigationInfo key)
        {
            if (NavigationInfo == null)
                NavigationInfo = new Dictionary<CSTNavigationInfo, List<CSTUserPreferenceInfo>>();

            if (NavigationInfo.Keys.Contains(key))
            {
                return NavigationInfo[key];
            }

            return null;
        }

        public static Object FetchString(CSTNavigationInfo key)
        {
            if (NavigationInfoString == null)
                NavigationInfoString = new Dictionary<CSTNavigationInfo, object>();

            if (NavigationInfoString.Keys.Contains(key))
            {
                return NavigationInfoString[key];
            }

            return null;
        }


    }
}