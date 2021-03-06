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
using GreenField.DataContracts;

namespace GreenField.UserSession
{
    public static class SessionManager
    {
        public static Session SESSION { get; set; }
        public static CookieContainer CookieContainer { get; set; }
    }
}
