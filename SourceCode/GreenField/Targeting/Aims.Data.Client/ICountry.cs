using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Data.Client
{
    public interface ICountry
    {
        String IsoCode { get; }
        String Name { get; }
    }
}
