using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.Labs.DimensionEntitiesService;

namespace GreenField.Labs
{
    public interface IDimensionEntitiesFactory
    {
        Entities CreateEntities();
    }
}
