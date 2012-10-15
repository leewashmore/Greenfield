namespace ICPSystemAlert
{
    /// <summary>
    /// Powerpoint presentation file details
    /// </summary>
    public class ICPresentation
    {
        /// <summary>
        /// Stores company overview information in powerpoint presentation file
        /// </summary>
        public CompanyOverview CompanyOverviewInstance { get; set; }

        /// <summary>
        /// Stores investment thesis information in powerpoint presentation file
        /// </summary>
        public InvestmentThesis InvestmentThesisInstance { get; set; }

        /// <summary>
        /// Stores key operating assumption information in powerpoint presentation file
        /// </summary>
        public KeyOperatingAssumpations KeyOperatingAssumpationsInstance { get; set; }

        /// <summary>
        /// Stores value, quality and growth information in powerpoint presentation file
        /// </summary>
        public VQG VQGInstance { get; set; }

        /// <summary>
        /// Stores strength, weakness, opportunity and threat information in powerpoint presentation file
        /// </summary>
        public SWOTAnalysis SWOTAnalysisInstance { get; set; }
    }
}
