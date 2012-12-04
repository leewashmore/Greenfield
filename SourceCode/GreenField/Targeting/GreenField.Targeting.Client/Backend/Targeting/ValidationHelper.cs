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
using System.Linq;
using System.Collections.Generic;

namespace TopDown.FacingServer.Backend.Targeting
{
    internal static class ValidationHelper
    {
        public static IEnumerable<String> GetErrors(String propertyName, String validatedPropertyName, IExpressionModel expressionModel)
        {
            if (String.IsNullOrWhiteSpace(propertyName)) return No.Strings;
            if (propertyName != validatedPropertyName) return No.Strings;

            var result = expressionModel.Issues.Select(x => x.Message).ToList();
            return result;
        }
    }
}
