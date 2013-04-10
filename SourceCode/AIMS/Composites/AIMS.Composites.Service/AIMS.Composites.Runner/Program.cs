using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIMS.Composites.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //AIMS.Composites.DAL.Class1 class1 = new DAL.Class1();
            //class1.Somefunction();

            AIMS.Composites.Service.CompositesOperations compositesOperations = new AIMS.Composites.Service.CompositesOperations();
            compositesOperations.PopulateCompositeLTHoldings();
        }
    }
}
