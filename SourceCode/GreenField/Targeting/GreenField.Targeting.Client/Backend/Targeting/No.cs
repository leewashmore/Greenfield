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
using System.Collections;
using System.Collections.Generic;

namespace TopDown.FacingServer.Backend.Targeting
{
    public static class No
    {
        private static IEnumerable<IssueModel> issues = new IssueModel[] { };
        public static IEnumerable<IssueModel> Issues
        {
            get { return issues; }
        }

        private static IEnumerable<String> strings = new String[] {};
        public static IEnumerable<String> Strings
        {
            get { return strings; }
        }
    }
}
