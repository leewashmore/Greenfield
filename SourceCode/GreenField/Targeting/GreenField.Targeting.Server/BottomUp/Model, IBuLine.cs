using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.Targeting.Server.BottomUp
{
    public interface IBuLineModel
    {
        void Accept(IBuLineModelResolver resolver);
    }

    public interface IBuLineModelResolver
    {
        void Resolve(ItemModel model);
        void Resolve(CashLineModel model);
        void Resolve(TotalLineModel model);
    }
}
