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

namespace TopDown.FacingServer.Backend.Targeting
{
    public partial class BgaRootModel
    {
        public BgaRootModel()
        {
        }

        public void InitializeWhenDeSerializationIsDone()
        {
            Decimal? value;
            if (this.Cash.Base.Value.HasValue)
            {
                value = 1.0m;
            }
            else
            {
                value = null;
            }
            this.GrandTotal = new NullableExpressionModel { Value = value, Issues = new System.Collections.ObjectModel.ObservableCollection<IssueModel>() };

        }

        public NullableExpressionModel GrandTotal { get; set; }
    }
}
