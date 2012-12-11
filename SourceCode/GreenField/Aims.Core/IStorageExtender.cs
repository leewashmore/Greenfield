using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Core
{
    public static class IStorageExtender
    {
        public static TValue Claim<TValue>(this IStorage<TValue> storage, String key, Func<TValue> creator)
            where TValue: class
        {
            var valueOpt = storage[key];
            if (valueOpt == null)
            {
                valueOpt = creator();
                storage[key] = valueOpt;
            }
            return valueOpt;
        }
    }
}
