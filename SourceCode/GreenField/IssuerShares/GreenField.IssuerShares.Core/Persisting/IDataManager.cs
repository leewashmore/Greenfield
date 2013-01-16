using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.IssuerShares.Core.Persisting
{
    public interface IDataManager : Aims.Core.Persisting.IDataManager
    {
        IEnumerable<IssuerSharesCompositionInfo> GetIssuerSharesComposition(String issuerId);
        IEnumerable<Aims.Data.Server.SecurityModel> GetIssuerSecurities(String securityShortName);
        Int32 UpdateIssuerSharesComposition(String issuerId, List<ItemModel> items);

    }
}
