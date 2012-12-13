using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Data.Client
{
    public interface IFund : ISecurity
    {
        String ShortName { get; }
        String Ticker { get; }
    }
}
