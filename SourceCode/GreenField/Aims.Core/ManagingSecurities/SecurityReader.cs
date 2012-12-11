using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Aims.Core.Persisting;

namespace Aims.Core
{
	public class SecurityReader
	{
		public IEnumerable<SecurityInfo> ReadSecuritiesFormFile(String filepath)
		{
			var securities = new List<SecurityInfo>();
			var lines = File.ReadAllLines(filepath);
			foreach (var line in lines)
			{
				if (String.IsNullOrWhiteSpace(line)) continue;
				var security = this.ReadSecurityFromTextLine(line);
				securities.Add(security);
			}
			return securities;
		}

		public SecurityInfo ReadSecurityFromTextLine(String line)
		{
			if (!(line.StartsWith("(") && line.EndsWith(")"))) throw new ApplicationException();
			line = line.Substring(1, line.Length - 2);
			var tokens = line.Split(',').Select(x => x.Trim()).ToArray();
			var result = new SecurityInfo
			{
				Id = this.StripQuotesIfAny(tokens[2]),
				ShortName = this.StripQuotesIfAny(tokens[3]),
				Name = this.StripQuotesIfAny(tokens[4]),
				IsoCountryCode = this.StripQuotesIfAny(tokens[33])
			};
			return result;
		}

		protected String StripQuotesIfAny(String value)
		{
			if (value.StartsWith("\"") && value.EndsWith("\""))
			{
				return value.Substring(1, value.Length - 2);
			}
			else
			{
				return value;
			}
		}
	}
}
