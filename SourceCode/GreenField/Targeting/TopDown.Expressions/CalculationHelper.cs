﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class CalculationException : ApplicationException
    {
        public CalculationException(String message, Exception e) : base(message, e) { }
    }

    public static class CalculationHelper
    {
		public const Decimal InsignificantDifference = 0.0000001m;
		public static Boolean NoDifference(Decimal one, Decimal another)
		{
			return Math.Abs(one - another) < InsignificantDifference;
		}

    }
}
