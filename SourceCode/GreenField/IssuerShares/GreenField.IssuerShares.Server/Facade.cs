using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.IssuerShares.Server
{
    public class Facade : IFacade
    {
        private Serializer serializer;
        
        public Facade(
            Serializer serializer
        )
        {
            this.serializer = serializer;
        }


        public RootModel GetRootModel(String issuerId)
        {
            
        }
    }
}
