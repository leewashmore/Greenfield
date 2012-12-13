using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core.Persisting
{
    public interface IDataManager
    {
        IEnumerable<SecurityInfo> GetAllSecurities();
        IEnumerable<CountryInfo> GetAllCountries();
		IEnumerable<PortfolioInfo> GetAllPortfolios();
    }
}