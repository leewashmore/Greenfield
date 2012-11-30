using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.Labs
{
    internal class TestDimnesionEntitiesFactory : IDimensionEntitiesFactory
    {
        public DimensionEntitiesService.Entities CreateEntities()
        {
            return EntitiesHelper.DimensionEntity;
        }
    }
}
