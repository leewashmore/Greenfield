using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpst.ChangingTtgbsbv
{
    public interface IChange
    {
        void Accept(IChangeResolver resolver);
    }

    public interface IChangeResolver
    {
        void Resolve(InsertChange change);
        void Resolve(DeleteChange change);
        void Resolve(UpdateChange change);
    }
}
