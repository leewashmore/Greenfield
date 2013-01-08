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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using Module = GreenField.IssuerShares.Controls;
using GreenField.App.Helpers;
namespace GreenField.App.ViewModel
{
    // this piece of the ViewModelShell class is related solely to issuer shares
    public partial class ViewModelShell
    {
        // navigation commands that bound to the main menu and lead to issuer shares screen

        public DelegateCommand IssuerSharesCommand { get; set; }

        // this method is called from the ViewModelSheel constructor
        private void InitializeIssuerSharesCommands()
        {
            this.IssuerSharesCommand = new DelegateCommand(this.NavigateToIssuerSharesCommand);
        }

        // navigate to targeting views respectively

        public void NavigateToIssuerSharesCommand()
        {
            ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ISSUERSHARES_ISSUER_SHARES_COMPOSITION);
            UpdateToolBoxSelectorVisibility();
            this.regionManager.RequestNavigate(RegionNames.MAIN_REGION, typeof(Module.RootView).FullName);
        }

    }
}
