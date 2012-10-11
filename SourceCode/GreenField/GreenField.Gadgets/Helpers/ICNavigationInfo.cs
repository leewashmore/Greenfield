using System;
using System.Collections.Generic;
using System.Linq;

namespace GreenField.Gadgets.Models
{
    /// <summary>
    /// Enumeration of navigation content types
    /// </summary>
    public enum ICNavigationInfo
    {
        MeetingInfo,
        PresentationOverviewInfo,
        ViewPluginFlagEnumerationInfo
    }

    /// <summary>
    /// IC Presentation Navigation Implementation for data transfer between views
    /// </summary>
    public static class ICNavigation
    {
        /// <summary>
        /// Stores values for navigation content types
        /// </summary>
        public static Dictionary<ICNavigationInfo, Object> NavigationInfo { get; set; }

        /// <summary>
        /// Updates navigation content type with value if exists or creates one
        /// </summary>
        /// <param name="key">navigation content type</param>
        /// <param name="value">value</param>
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

        /// <summary>
        /// Fetches navigation content type value if exists else returns null
        /// </summary>
        /// <param name="key">navigation content type</param>
        /// <returns>navigation content type value</returns>
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
