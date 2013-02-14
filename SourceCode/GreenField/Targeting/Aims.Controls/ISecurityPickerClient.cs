using System;
using Aims.Data.Client;
using System.Collections.Generic;

namespace Aims.Controls
{
    public interface ISecurityPickerClient
    {
        void RequestSecurities(String pattern, Action<IEnumerable<ISecurity>> callback, Action<Exception> errorHandler);
    }
}
