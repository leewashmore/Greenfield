using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBaskets
{
    public interface IBasket
    {
        Int32 Id { get; }
        void Accept(IBasketResolver resolver);
    }

    public interface IBasketResolver
    {
        void Resolve(CountryBasket basket);
        void Resolve(RegionBasket basket);
    }
}
