using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Composites.DAL
{
    public class Class1
    {
        public void Somefunction()
        {
            AIMS.Composites.DAL.AIMS_MainEntities aAIMS_MainEntities = new AIMS.Composites.DAL.AIMS_MainEntities();
            var x = aAIMS_MainEntities.GetComposites();
        }
    }
}
