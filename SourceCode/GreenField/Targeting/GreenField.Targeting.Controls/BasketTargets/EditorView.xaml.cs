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
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            this.InitializeComponent();
        }

        private void WhenResetToBase(object sender, RoutedEventArgs e)
        {
            var index = (Int32)((sender as MenuItem).CommandParameter);
            var lines = this.grid.ItemsSource as List<IBtLineModel>;
            foreach (var line in lines)
            {
                var casted = line as BtSecurityModel;
                if (casted == null) continue;
                var target = casted.PortfolioTargets[index].PortfolioTarget;
                if (target.EditedValue.HasValue)
                {
                    target.EditedValue = null;
                }
            }
        }

        private void WhenSecurityResetToBase(object sender, RoutedEventArgs e)
        {
            var security = (BtSecurityModel)((sender as MenuItem).CommandParameter);
            foreach (var pt in security.PortfolioTargets)
            {
                var target = pt.PortfolioTarget;
                if (target.EditedValue.HasValue)
                {
                    target.EditedValue = null;
                }
            }

        }

        private void WhenSecurityRestoreToUnedited(object sender, RoutedEventArgs e)
        {
            var security = (BtSecurityModel)((sender as MenuItem).CommandParameter);
            foreach (var pt in security.PortfolioTargets)
            {
                var target = pt.PortfolioTarget;
                if (target.EditedValue != target.InitialValue)
                {
                    target.EditedValue = target.InitialValue;
                }
            }
        }

        private void WhenRestoreToUnedited(object sender, RoutedEventArgs e)
        {
            var index = (Int32)((sender as MenuItem).CommandParameter);
            var lines = this.grid.ItemsSource as List<IBtLineModel>;
            foreach (var line in lines)
            {
                var casted = line as BtSecurityModel;
                if (casted == null) continue;
                var target = casted.PortfolioTargets[index].PortfolioTarget;
                if (target.EditedValue != target.InitialValue)
                {
                    target.EditedValue = target.InitialValue;
                }
            }
        }
    }
}
