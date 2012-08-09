using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMSResearchDataUpdation.ServiceCaller.DimensionServiceReference;
using System.Configuration;

namespace AIMSResearchDataUpdation.ServiceCaller
{
    public class DimensionService : IDimensionService
    {   
        private Entities entity;
        private Int32 maxRecordFetchCount = 1000;
        private String[] distinctEntityTypes = new string[] { };
        
        public DimensionService()
        {
            try
            {                
                entity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionServiceUrl"]));

                distinctEntityTypes = ConfigurationManager.AppSettings["DistinctEntityTypes"].Split('|');

                if (!Int32.TryParse(ConfigurationManager.AppSettings["MaxRecordFetchCount"], out maxRecordFetchCount))
                    throw new InvalidCastException("Unable to parse MaxRecordFetchCount to Int32 primitive type");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n" + ex.GetType());
                Console.WriteLine(ex.Message + "\n" + StackTraceToString(ex)
                    + (ex.InnerException == null ? "" : "\nInnerException: " + ex.InnerException.Message + "\n" + StackTraceToString(ex.InnerException)));
                Console.ReadLine();
                Environment.Exit(1);
            }  
        }

        private string StackTraceToString(Exception exception)
        {
            StringBuilder sb = new StringBuilder(256);
            var frames = new System.Diagnostics.StackTrace(exception).GetFrames();
            for (int i = 0; i < frames.Length; i++) /* Ignore current StackTraceToString method...*/
            {
                var currFrame = frames[i];
                var method = currFrame.GetMethod();
                sb.Append(string.Format("{0}|{1} || ", method.ReflectedType != null ? method.ReflectedType.FullName : string.Empty, method.Name));
            }
            return sb.ToString().Replace(Environment.NewLine, " ");
        }

        public void RetrievePricingBaseviewData(Action<List<GF_PRICING_BASEVIEW>> callback, Action<Double> progress = null, string instrumentId = null
            , string type = null, string ticker = null, string issueName = null, DateTime? beginDate = null, DateTime? endDate = null)
        {
            List<GF_PRICING_BASEVIEW> result = new List<GF_PRICING_BASEVIEW>();
            try
            {
                
                if (entity != null)
                {
                    IQueryable<GF_PRICING_BASEVIEW> query = entity.GF_PRICING_BASEVIEW;

                    if (instrumentId != null)
                        query = query.Where(record => record.INSTRUMENT_ID == instrumentId);

                    if (type != null)
                    {
                        query = query.Where(record => record.TYPE == type);
                        distinctEntityTypes = new string[] { type };
                    }

                    if (ticker != null)
                        query = query.Where(record => record.TICKER == ticker);

                    if (issueName != null)
                        query = query.Where(record => record.ISSUE_NAME == issueName);

                    if (beginDate != null)
                        query = query.Where(record => record.FROMDATE >= beginDate);

                    if (endDate != null)
                        query = query.Where(record => record.FROMDATE <= endDate);

                    Int32 totalRecordCount = query.Count();
                    Int32 recordCounter = 0;
                    foreach (String distinctType in distinctEntityTypes)
                    {
                        Int32 recordCount = query.Where(record => record.TYPE == distinctType).Count();                        
                        
                        GF_PRICING_BASEVIEW lowestGFIdRecord = query.Where(record => record.TYPE == distinctType)
                            .OrderBy(record => record.GF_ID).FirstOrDefault();
                        GF_PRICING_BASEVIEW highestGFIdRecord = query.Where(record => record.TYPE == distinctType)
                            .OrderByDescending(record => record.GF_ID).FirstOrDefault();

                        if (lowestGFIdRecord != null && highestGFIdRecord != null)
                        {
                            Int32 iterationCounter = 0;
                            for (int i = Convert.ToInt32(lowestGFIdRecord.GF_ID); i <= Convert.ToInt32(highestGFIdRecord.GF_ID); i = i + maxRecordFetchCount)
                            {
                                result = query.Where(record => record.GF_ID >= i && record.GF_ID 
                                    < (i + maxRecordFetchCount < Convert.ToInt32(highestGFIdRecord.GF_ID) 
                                        ? i + maxRecordFetchCount : Convert.ToInt32(highestGFIdRecord.GF_ID) + 1)
                                    && record.TYPE == distinctType).ToList();

                                if (result != null)
                                {
                                    recordCounter = recordCounter + result.Count;
                                    iterationCounter = iterationCounter + result.Count;
                                    if (progress != null)
                                    {
                                        Double percProgress = ((Double)(recordCounter)
                                            / (Double)(totalRecordCount)) * (Double)100;
                                        progress(percProgress);
                                    }

                                    if (callback != null)
                                        callback(result); 
                                }
                            }
                        }
                    }
                }                
                
            }
            catch (Exception ex)
            {
                //DataServiceQueryException
                Console.WriteLine("\n" + ex.GetType());
                Console.WriteLine("\n" + ex.Message + "\n" + StackTraceToString(ex) 
                    + (ex.InnerException == null ? "" : "\nInnerException: " + ex.InnerException.Message + "\n" + StackTraceToString(ex.InnerException)));
                Console.ReadLine();
                Environment.Exit(1);
            }         
        }

        public void RetrieveSecurityBaseviewData(Action<List<GF_SECURITY_BASEVIEW>> callback, Action<Double> progress = null, string securityId = null
            , string instrumentId = null, string issuerId = null, string ticker = null, string issueName = null)
        {
            List<GF_SECURITY_BASEVIEW> result = new List<GF_SECURITY_BASEVIEW>();
            try
            {
                if (entity != null)
                {
                    if (progress != null)
                        progress(0);

                    IQueryable<GF_SECURITY_BASEVIEW> query = entity.GF_SECURITY_BASEVIEW;

                    if (securityId != null)
                    {
                        Int32 securityIdParsed;
                        if (Int32.TryParse(securityId, out securityIdParsed))
                            query = query.Where(record => record.SECURITY_ID == securityIdParsed);
                        else
                            throw new InvalidOperationException("SECURITY_ID input is invalid");
                    }

                    if (instrumentId != null)
                        query = query.Where(record => record.ASEC_SEC_SHORT_NAME == instrumentId);

                    if (issuerId != null)
                        query = query.Where(record => record.ISSUER_ID == issuerId);

                    if (ticker != null)
                        query = query.Where(record => record.TICKER == ticker);

                    if (issueName != null)
                        query = query.Where(record => record.ISSUE_NAME == issueName);

                    Int32 recordCount = query.Count();
                    Int32 recordCounter = 0;
                    GF_SECURITY_BASEVIEW lowestGFIdRecord = query.OrderBy(record => record.GF_ID).FirstOrDefault();
                    GF_SECURITY_BASEVIEW highestGFIdRecord = query.OrderByDescending(record => record.GF_ID).FirstOrDefault();
                    if (lowestGFIdRecord != null && highestGFIdRecord != null)
                    {
                        for (int i = Convert.ToInt32(lowestGFIdRecord.GF_ID); i <= Convert.ToInt32(highestGFIdRecord.GF_ID); i = i + maxRecordFetchCount)
                        {
                            result = query.Where(record => record.GF_ID >= i && record.GF_ID 
                                    < (i + maxRecordFetchCount < Convert.ToInt32(highestGFIdRecord.GF_ID) 
                                        ? i + maxRecordFetchCount : Convert.ToInt32(highestGFIdRecord.GF_ID) + 1)).ToList();
                            if (result != null)
                            {
                                recordCounter = recordCounter + result.Count;
                                if (progress != null)
                                {
                                    Double percProgress = ((Double)(recordCounter)
                                        / (Double)(recordCount)) * (Double)100;
                                    progress(percProgress);
                                }

                                if (callback != null)
                                    callback(result);
                            }

                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                //DataServiceQueryException
                Console.WriteLine("\n" + ex.GetType());
                Console.WriteLine("\n" + ex.Message + "\n" + StackTraceToString(ex)
                    + (ex.InnerException == null ? "" : "\nInnerException: " + ex.InnerException.Message + "\n" + StackTraceToString(ex.InnerException)));
                Console.ReadLine();
                Environment.Exit(1);
            }   
        }

        public void RetrieveCountryCurrencyMappingData(Action<List<GF_CTY_CUR>> callback)
        {
            List<GF_CTY_CUR> result = new List<GF_CTY_CUR>();

            try
            {
                if (entity != null)
                {
                    result = entity.GF_CTY_CUR.ToList();
                }
            }
            catch (Exception)
            { }

            if (callback != null)
                callback(result);
        }
        
    }
}
