using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingPst
{
    public interface IPstChange
    {
        void Accept(IPstChangeResolver resolver);
    }
    public interface IPstChangeResolver
    {
        void Resolve(PstUpdateChange change);
        void Resolve(PstDeleteChange change);
        void Resolve(PstInsertChange change);
    }
}
