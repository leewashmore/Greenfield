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
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class ValueTraverser
    {
        public IEnumerable<EditableExpressionModel> TraverseValues(BtRootModel model)
        {
            var result = new List<EditableExpressionModel>();

            result.AddRange(model.Securities.Select(x => x.Base));
            result.AddRange(model.Securities.SelectMany(x => x.PortfolioTargets.Select(y => y.PortfolioTarget)));

            return result;
        }
    }
}
