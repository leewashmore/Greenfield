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

namespace GreenField.IssuerShares.Controls
{

    public class EditorViewModel
    {
        private IClientFactory clientFactory;

        public EditorViewModel(IClientFactory clientFactory)
        {
            // TODO: Complete member initialization
            this.clientFactory = clientFactory;
        }


        internal void RequestData(String shortSecurityName)
        {
            // request data from backend
            // then initialize data grid

            // wrap lines into object with REMOVE command
        }


    }
}
