using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace AIMS.Composites.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CompositesOperations" in both code and config file together.
    public class CompositesOperations : ICompositesOperations
    {
        public void DoWork()
        {
            AIMS.Composites.DAL.AIMS_MainEntities aAIMS_MainEntities = new AIMS.Composites.DAL.AIMS_MainEntities();
            List<AIMS.Composites.DAL.GetComposites_Result> x = new List<DAL.GetComposites_Result>();
            x = aAIMS_MainEntities.GetComposites().ToList();

            //aAIMS_MainEntities.GetComposites();

            //AIMS.Composites.DAL. .CompositesDataModel compositesDataModel = new DAL.CompositesDataModel();

            //AIMS.Composites.DAL.GetComposites_Result result = new DAL.GetComposites_Result();
            //compositesDataModel.GetComposites();
        }
    }
}
