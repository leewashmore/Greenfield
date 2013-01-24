using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core
{
    public interface ISecurity
    {
        String Id { get; }
        String Name { get; }
        String ShortName { get; }
        String Ticker { get;}
        String IssuerId { get;  }
        String SecurityType { get; }
        String Currency { get; }
        String Isin { get; }
        String IsoCountryCode { get; }
        void Accept(ISecurityResolver resolver);
    }

    public interface ISecurityResolver
    {
        void Resolve(CompanySecurity stock);
        void Resolve(Fund fund);
    }
}
