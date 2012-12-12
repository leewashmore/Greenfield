using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Data.Client
{
    public interface ICompanySecurity : ISecurity
    {
        String ShortName { get; }
        String Ticker { get; }
        ICountry Country { get; }
    }
}
