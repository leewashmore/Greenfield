using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenField.Labs.DimensionEntitiesService;
using System.Configuration;

namespace GreenField.Labs
{
    public static class EntitiesHelper
    {
        private static Entities dimensionEntity;
        public static Entities DimensionEntity
        {
            get
            {
                var urlFromConfig = ConfigurationManager.AppSettings["DimensionWebService"];
                if (null == dimensionEntity)
                {
                    dimensionEntity = new Entities(new Uri(urlFromConfig));
                }

                return dimensionEntity;
            }
        }
    }
}
