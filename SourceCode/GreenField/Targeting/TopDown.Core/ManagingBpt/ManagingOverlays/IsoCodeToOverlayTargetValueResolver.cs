using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using Aims.Core;

namespace TopDown.Core.Overlaying
{
    public class IsoCodeToOverlayTargetValueResolver
    {
        private Dictionary<String, Decimal> map;
        
        public IsoCodeToOverlayTargetValueResolver(
            SecurityRepository securityRepository,
            IEnumerable<BuPortfolioSecurityTargetInfo> flatTargets
        )
        {
            this.map = new Dictionary<String, Decimal>();
            var grouppedByCountry = flatTargets.
                GroupBy(x => securityRepository
                    .GetSecurity(x.SecurityId)
                    .AsCompanySecurity().Country.IsoCode
                )
                .Select(x => new
                {
                    IsoCode = x.Key,
                    TargetValue = x.Sum(y => y.Target)
                });

            foreach (var group in grouppedByCountry)
            {
                this.map.Add(group.IsoCode, group.TargetValue);
            }
        }

        public Decimal ResolveOverlayTargetValue(String isoCode, Decimal defaultValue)
        {
            Decimal resolved;
            if (this.map.TryGetValue(isoCode, out resolved))
            {
                return resolved;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
