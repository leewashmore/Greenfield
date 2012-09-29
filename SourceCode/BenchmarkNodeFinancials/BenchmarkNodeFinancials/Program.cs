using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using BenchmarkNodeFinancials.DimensionServiceReference;
using System.Configuration;

namespace BenchmarkNodeFinancials
{
    class Program
    {
        #region Fields
        private static AIMSDataEntity _entity; 
        private static DimensionServiceReference.Entities _dimensionEntity;
        #endregion

        static void Main(string[] args)
        {
            BenchmarkNodeFinancial bnf = new BenchmarkNodeFinancial();
            bnf.RetrieveData();           
        }
        
    }    
}
