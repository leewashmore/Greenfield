using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Persisting;

namespace Aims.Core
{
    public class IssuerRepository
    {
        private Dictionary<String, Issuer> byId;

        public IssuerRepository(IMonitor monitor, IEnumerable<SecurityInfo> securities)
        {
            var grouppedByIssuerId = securities.GroupBy(x => x.IssuerId);
            this.byId = new Dictionary<String, Issuer>();
            foreach (var group in grouppedByIssuerId)
            {
                var issuer = monitor.DefaultIfFails("Creating an issuer (ID: " + group.Key + ").", delegate
                {
                    var id = group.Key;
                    if (String.IsNullOrWhiteSpace(id)) throw new ApplicationException("Issuer ID cannot be empty.");

                    var grouppedByName = group.GroupBy(x => x.IssuerName);
                    if (grouppedByName.Count() > 1) throw new ApplicationException("There is more than one issuer name sharing the same ID: \"" + String.Join("\", \"", grouppedByName.Select(x => x.Key).ToArray()) + "\"");

                    var firstOpt = grouppedByName.FirstOrDefault();
                    if (firstOpt == null) throw new ApplicationException("There is no issuers associated with this ID.");

                    var name = firstOpt.Key;
                    if (String.IsNullOrWhiteSpace(name)) throw new ApplicationException("Issuer name cannot be empty.");

                    return new Issuer(id, name);
                });
             
                this.byId.Add(issuer.Id, issuer);
            }
        }

        public Issuer FindIssuer(String issuerId)
        {
            Issuer found;
            if (this.byId.TryGetValue(issuerId, out found))
            {
                return found;
            }
            return null;
        }

        public Issuer GetIssuer(String issuerId)
        {
            var found = this.FindIssuer(issuerId);
            if (found == null) throw new ApplicationException("There is no issuer with the \"" + issuerId + "\" ID.");
            return found;
        }
    }
}
