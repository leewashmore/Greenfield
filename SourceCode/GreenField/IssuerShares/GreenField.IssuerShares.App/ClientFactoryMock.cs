using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.IssuerShares.Controls;
using System.ComponentModel.Composition;

namespace GreenField.IssuerShares.App
{
    [Export(typeof(IClientFactory))]
    public class ClientFactoryMock : IClientFactory
    {
        public IClient CreateClient()
        {
            return new ClientMock();
        }
    }
}
