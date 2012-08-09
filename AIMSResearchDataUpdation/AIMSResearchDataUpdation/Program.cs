using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIMSResearchDataUpdation
{
    class Program
    {
        static void Main(string[] args)
        {
            Arguments CommandLine = new Arguments(args);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            if (CommandLine["PRICES"] != null)
            {
                String instrumentId = CommandLine["INSTRUMENT_ID"];
                String type = CommandLine["TYPE"];
                String ticker = CommandLine["TICKER"];
                String issueName = CommandLine["ISSUE_NAME"];
                
                DateTime? beginDate = Convert.ToDateTime("01/01/2012");
                if (CommandLine["BEGIN_DATE"] != null)
                {
                    beginDate = Convert.ToDateTime(CommandLine["BEGIN_DATE"]);
                }

                DateTime? endDate = null;
                if (CommandLine["END_DATE"] != null)
                {
                    endDate = Convert.ToDateTime(CommandLine["END_DATE"]);
                }
                Console.WriteLine("\nInput Filters: INSTRUMENT_ID [" + (instrumentId == null ? "N/A" : instrumentId) + "]"
                    + " TYPE [" + (type == null ? "N/A" : type) + "]"
                    + " TICKER [" + (ticker == null ? "N/A" : ticker) + "]"
                    + " ISSUE_NAME [" + (issueName == null ? "N/A" : issueName) + "]"
                    + " BEGIN_DATE [" + (beginDate == null ? "N/A" : beginDate.ToString()) + "]"
                    + " END_DATE [" + (endDate == null ? "N/A" : endDate.ToString()) + "]");

                Task_Pricing_Baseview pricingExecution = new Task_Pricing_Baseview(instrumentId, type, ticker, issueName,
                    beginDate, endDate);
                return;
            }

            if (CommandLine["SECURITY"] != null)
            {
                String securityId = CommandLine["SECURITY_ID"];
                String instrumentId = CommandLine["INSTRUMENT_ID"];
                String issuerId = CommandLine["ISSUER_ID"];
                String ticker = CommandLine["TICKER"];
                String issueName = CommandLine["ISSUE_NAME"];

                Console.WriteLine("\nInput Filters: SECURITY_ID [" + (securityId == null ? "N/A" : securityId) + "]"
                    + " INSTRUMENT_ID [" + (instrumentId == null ? "N/A" : instrumentId) + "]"
                    + " ISSUER_ID [" + (issuerId == null ? "N/A" : issuerId) + "]"
                    + " TICKER [" + (ticker == null ? "N/A" : ticker) + "]"
                    + " ISSUE_NAME [" + (issueName == null ? "N/A" : issueName) + "]");

                Task_Security_Baseview securityExecution = new Task_Security_Baseview(securityId, instrumentId, issuerId
                    , ticker, issueName);
                return;
            }
            
            Console.WriteLine("\nNo parameters for execution");            
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e) 
        {
            Console.WriteLine("\n\nExiting Application"); 
            Console.ReadLine(); 
        } 
    }
}
