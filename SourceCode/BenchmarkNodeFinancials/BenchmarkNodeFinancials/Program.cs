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
        private static DimensionServiceReference.Entities _dimensionEntity;
        #endregion

        static void Main(string[] args)
        {
            try
            {
                _dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_BENCHMARK_HOLDINGS lastBusinessRecord = _dimensionEntity.GF_BENCHMARK_HOLDINGS.OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);
                if (args.Length == 1)
                {
                    bool isDatetime = IsDateTime(args[0]);
                    if (isDatetime)
                    {
                        lastBusinessDate = Convert.ToDateTime(args[0]);
                    }
                    else
                    {
                        Console.WriteLine("Invalid Arguments Passed.Pass only Datetime argument");
                        Console.ReadLine();
                        return;
                    }                   
                }
                else
                    if (args.Length > 1)
                    {
                        Console.WriteLine("Invalid Arguments Length.Pass only one Argument");
                        Console.ReadLine();
                        return;
                    }

                _log.Debug("Application initiated on " + lastBusinessDate);
                BenchmarkNodeFinancial bnf = new BenchmarkNodeFinancial();
                bnf.RetrieveData(lastBusinessDate);
                _log.Debug("Application completed successfully on " + lastBusinessDate);
            }
        
            catch (Exception ex)
            {
                _log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }

        public static bool IsDateTime(string txtDate) 
        {
            DateTime tempDate; 
            return DateTime.TryParse(txtDate, out tempDate) ? true : false;
        } 
    }    
}
