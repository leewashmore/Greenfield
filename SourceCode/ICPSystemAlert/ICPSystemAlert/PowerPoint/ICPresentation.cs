using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICPSystemAlert
{
    public class ICPresentation
    {
        public CompanyOverview CompanyOverviewInstance { get; set; }

        public InvestmentThesis InvestmentThesisInstance { get; set; }

        public KeyOperatingAssumpations KeyOperatingAssumpationsInstance { get; set; }

        public VQG VQGInstance { get; set; }

        public SWOTAnalysis SWOTAnalysisInstance { get; set; }
    }
}
