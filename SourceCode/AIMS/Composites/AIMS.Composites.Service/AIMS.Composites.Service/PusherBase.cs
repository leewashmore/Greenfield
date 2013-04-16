using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;

namespace AIMS.Composites.Service
{
    public abstract class PusherBase<TRecord>
    {
        protected abstract SqlConnection CreateConnection();

        protected virtual void PushOneBulk(Int32 bulkNumber, DataTable table, String destinationTableName, SqlConnection conn = null)
        {
            SqlConnection connection = null;
            try
            {
                if (conn == null)
                {
                    connection = this.CreateConnection();
                    connection.Open();
                }
                else
                {
                    connection = conn;
                }
                var bulker = new SqlBulkCopy(connection)
                    {
                        DestinationTableName = destinationTableName,
                        BulkCopyTimeout = 0
                    };

                try
                {
                    bulker.WriteToServer(table);
                }
                catch (Exception exception)
                {
                    throw new ApplicationException("Unable to do bulk insert.", exception);
                }
            }
            finally
            {
                if (conn == null)
                {
                    if (connection != null) connection.Dispose();
                }
            }
        }

        public IEnumerable<DataTable> SplitIntoBulks(IEnumerable<TRecord> gfCompositeLtholdingss, Int32 recordsPerBulk)
        {
            var totalNumber = gfCompositeLtholdingss.Count();
            var numberOfTables = totalNumber / recordsPerBulk;
            var numberOfTailRecords = totalNumber % recordsPerBulk;

            var result = new List<DataTable>();

            for (var tableIndex = 0; tableIndex < numberOfTables; tableIndex++)
            {
                var bulkOfRecords = gfCompositeLtholdingss.Skip(tableIndex * recordsPerBulk).Take(recordsPerBulk);
                var table = this.CreateBulk(bulkOfRecords);
                result.Add(table);
            }

            if (numberOfTailRecords > 0)
            {
                var bulkOfRecord = gfCompositeLtholdingss.Skip(numberOfTables * recordsPerBulk);
                var table = this.CreateBulk(bulkOfRecord);
                result.Add(table);
            }

            return result;
        }

        protected abstract DataTable CreateBulk(IEnumerable<TRecord> GF_COMPOSITE_LTHOLDINGS);
    }
}
