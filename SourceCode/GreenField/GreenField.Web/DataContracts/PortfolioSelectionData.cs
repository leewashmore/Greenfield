using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class PortfolioSelectionData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string PortfolioId { get; set; }

        [DataMember]
        public string PortfolioThemeSubGroupName { get; set; }

        [DataMember]
        public string PortfolioThemeSubGroupId { get; set; }

    }
}