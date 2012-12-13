using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBpt;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingSecurities;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core
{
    public class No : Aims.Core.No
    {
        private readonly static IEnumerable<IModelDifference> modelDifference = new IModelDifference[] { };
        public static IEnumerable<IModelDifference> ModelDifference
        {
            [DebuggerStepThrough]
            get { return modelDifference; }
        }

        private readonly static IEnumerable<BuPortfolioSecurityTargetInfo> portfolioSecurityTargets = new BuPortfolioSecurityTargetInfo[] { };
        public static IEnumerable<BuPortfolioSecurityTargetInfo> PortfolioSecurityTargets
        {
            [DebuggerStepThrough]
            get { return portfolioSecurityTargets; }
        }

        private readonly static IEnumerable<Country> countries = new Country[] { };
        public static IEnumerable<Country> Countries
        {
            [DebuggerStepThrough]
            get { return countries; }
        }

        private readonly static IEnumerable<IValidationIssue> validationIssues = new IValidationIssue[] { };
        public static IEnumerable<IValidationIssue> ValidationIssues
        {
            [DebuggerStepThrough]
            get { return validationIssues; }
        }

        private class _DumpWriter : IDumpWriter
        {
            public void WriteLine(String line) { }
            public void Indent() { }
            public void Unindent() { }
        }

        private readonly static IDumpWriter dumpWriter = new _DumpWriter();
        public static IDumpWriter DumpWriter { get { return dumpWriter; } }


        private readonly static String[] isoCodes = new String[] { };
        public static IEnumerable<String> IsoCodes { [DebuggerStepThrough]get { return isoCodes; } }
    }
}
