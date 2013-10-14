using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AIMS_MarketCap
{
    class Program
    {
        static void Main(string[] args)
        {
            AIMS_Entities entity;
            FileStream fs1;
            StreamWriter writer;
            string filePath;
            string fileName=null;
            try
            {
                 if (args != null && args.Length > 0)
                 { 
                     filePath =args[0];
                     fileName =filePath+"\\Aims_MarketCap_"+DateTime.Now.Year+""+DateTime.Now.Month+""+DateTime.Now.Day+".csv";

                 } else 
                 {
                     Console.WriteLine("Invalid number of arguments");
                     Console.WriteLine("AIMS_MarketCap.exe  <filepath>");
                     Environment.Exit(-1);
                 }

                entity = new AIMS_Entities();
                
                int count = entity.expAimsMktCap().Count();
                if (count == 0)
                {
                    Console.WriteLine(DateTime.Now + ": Get data process did not create the market cap ");
                    Environment.Exit(-1);
                }
                if (fileName != null)
                {
                    fs1 = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    writer = new StreamWriter(fs1);
                    List<expAimsMktCap_Result> result = entity.expAimsMktCap().ToList();
                    foreach (var record in result)
                    {
                        writer.Write(record.Root_Source_Date + "," + record.ASEC_SEC_SHORT_NAME + "," + record.amount + "\n");
                    }
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Error generating the AIMS Market Cap file " + e.Message) ;
                Environment.Exit(-1);

            }
        }
    }
}
