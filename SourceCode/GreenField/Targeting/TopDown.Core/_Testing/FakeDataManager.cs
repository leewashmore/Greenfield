using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.Persisting;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TopDown.Core._Testing
{
	public class FakeDataManager : DataManager
	{
		[DebuggerStepThrough]
		public FakeDataManager(GetPortfolioSecurityTargetsSettings getPortfolioSecurityTargetsSettings)
			: base(null, null)
		{
			this.getPortfolioSecurityTargetsSettings = getPortfolioSecurityTargetsSettings;
		}

		private readonly static String[] AllCountries = new String[] { "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AO", "AQ", "AR", "AS", "AT", "AU", "AW", "AX", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BL", "BM", "BN", "BO", "BQ", "BQ", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CC", "CD", "CF", "CG", "CH", "CI", "CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CW", "CX", "CY", "CZ", "DE", "DJ", "DK", "DM", "DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "FI", "FJ", "FK", "FM", "FO", "FR", "GA", "GB", "GD", "GE", "GF", "GG", "GH", "GI", "GL", "GM", "GN", "GP", "GQ", "GR", "GS", "GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID", "IE", "IL", "IM", "IN", "IO", "IQ", "IR", "IS", "IT", "JE", "JM", "JO", "JP", "KE", "KG", "KH", "Pr", "KI", "KM", "KN", "KP", "KR", "KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT", "LU", "LV", "LY", "MA", "MC", "MD", "ME", "MF", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ", "MR", "MS", "MT", "MU", "MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI", "NL", "NO", "NP", "NR", "NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM", "PN", "PR", "PS", "PT", "PW", "PY", "QA", "RE", "RO", "RS", "RU", "RW", "SA", "SB", "SC", "SD", "SE", "SG", "SH", "SI", "SJ", "SK", "SL", "SM", "SN", "SO", "SR", "SS", "ST", "SV", "SX", "SY", "SZ", "TC", "TD", "TF", "TG", "TH", "TJ", "TK", "TL", "TM", "TN", "TO", "TR", "TT", "TV", "TW", "IS", "TZ", "UA", "UG", "UM", "US", "UY", "UZ", "VA", "Pr", "VC", "VE", "VG", "VI", "VN", "VU", "WF", "WS", "YE", "YT", "ZA", "ZM", "ZW" };

		public const Int32 Seed = 12345;

		public class GetPortfolioSecurityTargetsSettings
		{
			public Int32 NumberOfPortfolios { get; set; }
			public Decimal DistriburionVariation { get; set; }
		}

		private GetPortfolioSecurityTargetsSettings getPortfolioSecurityTargetsSettings;
		public IEnumerable<BuPortfolioSecurityTargetInfo> GetPortfolioSecurityTargets()
		{
			var settings = this.getPortfolioSecurityTargetsSettings;
			var random = new RandomGenerator(Seed);
			var result = new List<BuPortfolioSecurityTargetInfo>();

			var securityIdList = this.GetSecurityIdList(200).GetEnumerator();
			var portfolioIdList = this.GetPortfolioIdList(this.getPortfolioSecurityTargetsSettings.NumberOfPortfolios);
			var countries = random.Pick(AllCountries, 10);
			foreach (var portfolioId in portfolioIdList)
			{
				var securityCount = random.Number(3, 5);
				var targets = random.DecimalDistribution(securityCount, 1.0m - settings.DistriburionVariation + random.Decimal(settings.DistriburionVariation)).GetEnumerator();
				for (var index = 0; index < securityCount; index++)
				{
					result.Add(new BuPortfolioSecurityTargetInfo
					{
						BottomUpPortfolioId = portfolioId,
						SecurityId = securityIdList.Next(),
						Target = targets.Next()
					});
				}
			}

			return result;
		}

		private ICollection<String> GetPortfolioIdList(Int32 count)
		{
			var result = new List<String>();
			for (var index = 0; index < count; index++)
			{
				result.Add("P" + index.ToString("00"));
			}
			return result;
		}

		private ICollection<String> GetSecurityIdList(Int32 count)
		{
			var result = new List<String>();
			for (var index = 0; index < count; index++)
			{
				result.Add("S" + index.ToString("000"));
			}
			return result;
		}
	}
}
