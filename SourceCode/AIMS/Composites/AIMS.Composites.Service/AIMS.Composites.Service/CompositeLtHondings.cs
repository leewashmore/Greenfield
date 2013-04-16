using System.Collections.Generic;
using System.Configuration;

namespace AIMS.Composites.Service
{
    public class CompositeLtHondings
    {
        private const int RecordsPerBulk = 500;
        private readonly string connectionString;

        public CompositeLtHondings()
        {
            connectionString = ConfigurationManager.ConnectionStrings["Aims_Main"].ConnectionString;
        }

        public void Save(List<GF_COMPOSITE_LTHOLDINGS> compositeLtholdings, IDumper dumper)
        {
            var pusher = new CompositeLTHoldingsManager(connectionString, RecordsPerBulk, dumper);
            pusher.Save(compositeLtholdings);
        }
    }
}