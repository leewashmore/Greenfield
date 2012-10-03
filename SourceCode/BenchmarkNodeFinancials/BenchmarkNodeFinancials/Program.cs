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
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        static void Main(string[] args)
        {
            _log.Debug("Application initiated");
            BenchmarkNodeFinancial bnf = new BenchmarkNodeFinancial();
            bnf.RetrieveData(args);
            _log.Debug("Application completed successfully");
        }        
    }    
}
