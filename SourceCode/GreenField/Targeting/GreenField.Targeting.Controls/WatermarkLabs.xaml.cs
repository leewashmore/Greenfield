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
using System.ComponentModel.Composition;
using TopDown.FacingServer.Backend.Targeting;

namespace GreenField.Targeting.Controls
{
    [Export]
    public partial class WatermarkLabs : UserControl
    {
        public WatermarkLabs()
        {
            InitializeComponent();
            this.DataContext = new EditableExpressionModel { DisplayValue = 0.0m, EditedValue = 0.10m };
        }
    }
}
