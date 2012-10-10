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
        /// <summary>
        /// log4net required for logging
        /// </summary>
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// DimensionServiceReference.Entities object for connection to the views
        /// </summary>
        private static DimensionServiceReference.Entities dimensionEntity;
        #endregion

        static void Main(string[] args)
        {
            try
            {
                dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                DateTime lastBusinessDate = DateTime.Today.AddDays(-1);
                GF_BENCHMARK_HOLDINGS lastBusinessRecord = dimensionEntity.GF_BENCHMARK_HOLDINGS.
                    OrderByDescending(record => record.PORTFOLIO_DATE).FirstOrDefault();
                if (lastBusinessRecord != null)
                {
                    if (lastBusinessRecord.PORTFOLIO_DATE != null)
                    {
                        lastBusinessDate = Convert.ToDateTime(lastBusinessRecord.PORTFOLIO_DATE);
                    }
                }
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

                log.Debug("Application initiated on " + lastBusinessDate);
                BenchmarkNodeFinancial bnf = new BenchmarkNodeFinancial();
                bnf.RetrieveData(lastBusinessDate);
                log.Debug("Application completed successfully on " + lastBusinessDate);
            }
        
            catch (Exception ex)
            {
                log.Error(System.Reflection.MethodBase.GetCurrentMethod(), ex);
            }
        }

        /// <summary>
        /// To check if the entered date is in correct format
        /// </summary>
        /// <param name="txtDate"></param>
        /// <returns></returns>
        public static bool IsDateTime(string txtDate) 
        {
            DateTime tempDate; 
            return DateTime.TryParse(txtDate, out tempDate) ? true : false;
        } 
    }    
}
