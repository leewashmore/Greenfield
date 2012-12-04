using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Net.Mail;
using System.Resources;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Web;
using TopDown.Core;
using System.Web.Caching;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingCalculations;
using TopDown.Core.Sql;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingBpt;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingBpt.ChangingTtbpt;
using TopDown.Core.ManagingPst;
using TopDown.Core.Gadgets.PortfolioPicker;
using GreenField.Targeting.Backend.Helpers;

namespace GreenField.Targeting.Backend
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TargetingOperations : GreenField.Targeting.Server.Facade
    {
        public TargetingOperations(GreenField.Targeting.Server.FacadeSettings settings)
            : base(settings)
        {
        }

        
    }
}
