using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core
{
    public class Helper
    {
            public static String RenderPercent(Double number, Double total)
            {
                if (total > 0)
                {
                    var ratio = number / total;
                    return ratio.ToString("p");
                }
                else
                {
                    return String.Empty;
                }
            }
    }
}
