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

namespace GreenField.IssuerShares.Client.Backend.IssuerShares
{
    public partial class ItemModel
    {
        public ICommand RemoveCommand { get; private set; }
        public void InitializeRemoveCommand(ICommand command)
        {
            this.RemoveCommand = command;
        }

        public ICommand ChangedPreferredCommand { get; private set; }
        public void InitializeChangedPreferredCommand(ICommand command)
        {
            this.ChangedPreferredCommand = command;
        }
    }
}
