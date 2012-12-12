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
        void Accept(ISecurityResolver resolver);
    }

    public interface ISecurityResolver
    {
        void Resolve(CompanySecurity stock);
        void Resolve(Fund fund);
    }
}
