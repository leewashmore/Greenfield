using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.IssuerShares.Server
{
    public class Facade : IFacade
    {
        private Serializer serializer;
        private Core.ModelManager modelManager;
        
        public Facade(
            Core.ModelManager modelManager,
            Serializer serializer
        )
        {
            this.modelManager = modelManager;
            this.serializer = serializer;
        }

        public RootModel GetRootModel(String issuerId)
        {
            var model = this.modelManager.GetRootModel(issuerId);
            var serializedModel = this.serializer.SerializeRoot(model);
            return serializedModel;
        }
    }
}
