using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Persisting;

namespace Aims.Core
{
    public class SecurityRepository
    {
        private static readonly Char[] splitters = new Char[] { ' ', ',', '/', '#', '_', '-' };
        private static readonly String[] skipWords = new string[] { "plc", "pcl", "hldgs", "a", "-", "&", "lt", "e", "ltd", "co", "corp", "sa", "inc", "bhd", "of", "as", "pt", "tbk" };

        private String[] keys;
        private Dictionary<String, List<ISecurity>> map;
        private Dictionary<String, ISecurity> all;
        private Dictionary<String, ISecurity> byShortName;

        /// <summary>
        /// The key is the look-through-fund field, the value is the fund (which is still a security) that holds that field.
        /// </summary>
        private Dictionary<String, Fund> fundsByLookThroughFund;

        public SecurityRepository(
            IMonitor monitor,
            IEnumerable<SecurityInfo> securities,
            CountryRepository countryRepository
        )
            : this(
                monitor,
                securities,
                splitters,
                new HashSet<String>(skipWords),
                countryRepository
            )
        {
        }

        public SecurityRepository(
            IMonitor monitor,
            IEnumerable<SecurityInfo> securityInfos,
            IEnumerable<Char> splitters,
            HashSet<String> skipWords,
            CountryRepository countryRepository
        )
        {
			if (!securityInfos.Any()) throw new ApplicationException("There are no securities.");

            this.all = new Dictionary<String, ISecurity>();
            this.byShortName = new Dictionary<String, ISecurity>();
            this.map = new Dictionary<String, List<ISecurity>>();
            var securities = this.CreateSecurities(securityInfos, countryRepository, monitor);
            foreach (var security in securities)
            {
                monitor.SwallowIfFails("Registering a security.", delegate
                {
                    try
                    {
                        ISecurity existing;
                        if (this.all.TryGetValue(security.Id, out existing))
                        {
                            throw new ApplicationException("There is a security with the same ID already registered.");
                        }
                        this.all.Add(security.Id, security);
                        this.byShortName.Add(security.ShortName, security);
                    }
                    catch (Exception exception)
                    {
                        throw new ApplicationException("Unable to register a security with the \"" + security.Id + "\" ID.", exception);
                    }
                });
                var tobeIndexed = security.Ticker + " " + security.ShortName + " " + security.Name;
                var keys = tobeIndexed.Split(splitters.ToArray()).Select(x => x.Trim().ToLower()).Where(x => !String.IsNullOrWhiteSpace(x));
                foreach (var key in keys)
                {
                    if (skipWords.Contains(key)) continue;
                    this.ClaimList(key).Add(security);
                }
            }

            // look through fund 
			this.fundsByLookThroughFund = this.CreateFundsMapByLookupThroughFund(securityInfos, monitor);

            this.keys = map.Keys.ToArray();
            Array.Sort<String>(this.keys);
        }

		protected Dictionary<String, Fund> CreateFundsMapByLookupThroughFund(
			IEnumerable<SecurityInfo> securityInfos,
			IMonitor monitor
		)
		{
			var result = new Dictionary<String, Fund>();
			foreach (var securityInfo in securityInfos)
			{
				var count = securityInfos.Count(x => !String.IsNullOrWhiteSpace(x.LookThruFund));
				if (!String.IsNullOrWhiteSpace(securityInfo.LookThruFund))
				{
					var fund = this.GetFund(securityInfo.Id);
                    monitor.SwallowIfFails("Trying to add " + securityInfo.LookThruFund + " for \"" + fund + "\".", delegate
                    {
                        result.Add(securityInfo.LookThruFund, fund);
                    });
				}
			}
			return result;
		}


        private IEnumerable<ISecurity> CreateSecurities(
            IEnumerable<SecurityInfo> securityInfos,
            CountryRepository countryRepository,
            IMonitor monitor
        )
        {
            var result = new List<ISecurity>();
            foreach (var securityInfo in securityInfos)
            {
                var securityOpt = monitor.DefaultIfFails("Validating security", delegate
                {
                    if (String.IsNullOrEmpty(securityInfo.Name)) throw new ApplicationException("Name is not specified.");
                    if (String.IsNullOrEmpty(securityInfo.ShortName)) throw new ApplicationException("Short name is not specified.");

                    ISecurity security;

                    if (String.IsNullOrWhiteSpace(securityInfo.LookThruFund))
                    {
                        Country country = null;
                        if (String.IsNullOrEmpty(securityInfo.IsoCountryCode)) throw new ApplicationException("Country code is not specified.");
                        try
                        {
                            country = countryRepository.GetCountry(securityInfo.IsoCountryCode);
                        }
                        catch (CountryNotFoundException)
                        {
                            country = new Country(securityInfo.IsoCountryCode, securityInfo.AsecCountryName);
                        }
                        var stock = new CompanySecurity(
                            securityInfo.Id,
                            securityInfo.Ticker,
                            securityInfo.ShortName,
                            securityInfo.Name,
                            country,
                            securityInfo.IssuerId,
                            securityInfo.SecurityType,
                            securityInfo.Currency,
                            securityInfo.Isin,
                            securityInfo.IsoCountryCode
                        );
                        security = stock;
                        
                    }
                    else
                    {
                        var fund = new Fund(
                            securityInfo.Id,
                            securityInfo.Name,
                            securityInfo.ShortName,
                            securityInfo.Ticker,
                            securityInfo.IssuerId,
                            securityInfo.SecurityType,
                            securityInfo.Currency,
                            securityInfo.Isin,
                            securityInfo.IsoCountryCode
                        );
                        security = fund;
                    }
                    return security;
                });



                if (securityOpt == null) continue;
                result.Add(securityOpt);
            }
            return result;

        }

        public IEnumerable<ISecurity> FindByIssuer(String issuerId)
        {
            IEnumerable<ISecurity> found;
            found = all.Where(x => x.Value.IssuerId == issuerId).Select(x => x.Value);
            return found;

        }

        public IEnumerable<ISecurity> FindSomeUsingPattern(
            String pattern,
            Int32 atMost,
            Predicate<ISecurity> predicate
        )
        {
            if (String.IsNullOrWhiteSpace(pattern)) return No.Securities;

            pattern = pattern.Trim().ToLower();
            var result = new List<ISecurity>();
            var index = Array.BinarySearch<String>(this.keys, pattern);
            if (index < 0)
            {
                index = ~index;
                if (index < this.keys.Length)
                {
                    var key = this.keys[index];
                    result.AddRange(this.map[key].Where(x => predicate(x)));
                    if (result.Count >= atMost)
                    {
                        // straigh hit we take averything no matter how many were requested
                        return result;
                    }
                    else
                    {
                        do
                        {
                            index++;
                            if (index >= this.keys.Length) break;
                            key = this.keys[index];
                            if (!key.StartsWith(pattern)) break;
                            var tobeAddedMaybe = this.map[key].Where(x => predicate(x)).ToArray();
                            foreach (var tobeAdded in tobeAddedMaybe)
                            {
                                if (result.Select(x => x.Id).Contains(tobeAdded.Id)) continue;
                                result.Add(tobeAdded);
                            }
                        }
                        while (result.Count < atMost);
                    }
                }
                else
                {
                    // do nothing
                }
            }
            else
            {
                var key = this.keys[index];
                var filtered = this.map[key].Where(x => predicate(x));
                result.AddRange(filtered);
                while (result.Count < atMost)
                {
                    index++;
                    if (index >= this.keys.Length) break;
                    key = this.keys[index];
                    if (!key.StartsWith(pattern)) break;

                    var tobeAddedMaybe = this.map[key].Where(x => predicate(x)).ToArray();
                    foreach (var tobeAdded in tobeAddedMaybe)
                    {
                        if (result.Select(x => x.Id).Contains(tobeAdded.Id)) continue;
                        result.Add(tobeAdded);
                    }
                }
            }
            return result.Take(atMost);
        }

        protected List<ISecurity> ClaimList(String key)
        {
            List<ISecurity> found;
            if (this.map.TryGetValue(key, out found)) return found;
            found = new List<ISecurity>();
            this.map.Add(key, found);
            return found;
        }

		public ISecurity FindSecurity(String securityId)
		{
			ISecurity found;
			if (this.all.TryGetValue(securityId, out found))
			{
				return found;
			}
			else
			{
				return null;
			}
		}

        public ISecurity GetSecurity(String securityId)
        {
            var foundOpt = this.FindSecurity(securityId);
            if (foundOpt == null) throw new ApplicationException("There is no security with the \"" + securityId + "\" ID.");
			return foundOpt;
        }

        public Fund GetFund(String fundId)
        {
			var securityOpt = this.FindSecurity(fundId);
			if (securityOpt == null) throw new ApplicationException("There is no security with the ID \"" + fundId + "\" to be resolved as a fund.");
            var fundOpt = securityOpt.TryAsFund();
            if (fundOpt == null) throw new ApplicationException("Security \"" + fundId + "\" isn't a fund.");
            return fundOpt;
        }


        public Fund GetFundByPorfolioId(String portfolioId)
        {
            Fund fund;
            if (this.fundsByLookThroughFund.TryGetValue(portfolioId, out fund))
            {
                return fund;
            }
            else
            {
                throw new ApplicationException("Unable to get a fund using the \"" + portfolioId + "\" portfolio ID as it's not a fund ID.");
            }
        }

        public Boolean IsPortfolioAFund(String portfolioId)
        {
            return this.fundsByLookThroughFund.ContainsKey(portfolioId);
        }

        public ISecurity FindSecurityByShortName(string securityShortName)
        {
            ISecurity found;
            if (this.byShortName.TryGetValue(securityShortName, out found))
            {
                return found;
            }
            else
            {
                return null;
            }
        }
    }
}
