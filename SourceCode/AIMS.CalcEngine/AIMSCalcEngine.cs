using System;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace AIMS.CalcEngine
{
    public class AIMS_CalcEngine
    {
        private static ManualResetEvent[] resetEvents;
        private static String[] issuerId = null;
        private static int runId = 0;
        private static String runMode = "F";
        private static void Main(string[] args)
        {
            int numberOfThreads = Properties.Settings.Default.NumberOfThreads;
            if (args!=null && args[0] != null)
            {
                runMode = args[0];
            }
            runId = Get_Data_Process("START");
            RunCalcEngine(numberOfThreads);
        }
        static void RunCalcEngine(int numberOfThreads)
        {
            try
            {   
                int threadCount = 0;
                
               print("Start Getting list of all issuers to process ");
                //int runId = Get_Data_Process();
                List<String> issuerList = GetAllIssuers();
                print("End Getting list of all issuers to process ");
                String[] issuerListArr = issuerList.ToArray();
                int startIndex = 0;
                print("Begin processing all issuers " );
                while(startIndex < issuerListArr.Length)
                {
                    if (issuerListArr.Length - startIndex > numberOfThreads)
                    {
                        threadCount = numberOfThreads;
                    }
                    else
                    {
                        threadCount= issuerListArr.Length-startIndex;
                    }

                     resetEvents = new ManualResetEvent[threadCount];
                    issuerId = new String[threadCount];
                    for(int i = 0 ; i<threadCount;i++,startIndex++)
                    {
                        issuerId[i]=issuerListArr[startIndex];
                     
                    }

                    for (int i = 0; i < issuerId.Length; i++)
                    {
                        resetEvents[i] = new ManualResetEvent(false);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(DoWork), (object)i);
                    }
                    

                    WaitHandle.WaitAll(resetEvents);
                    print("Total issuers processed = " + startIndex);

                }
                DoFinalUpdateStatus();
               print("Get_Data Completed Successfully " );

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        static void print(String message)
        {
            Console.WriteLine(DateTime.Now + " : " + message);
        }
        static List<String> GetAllIssuers()
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<string> issuerList = new List<string>();

            try
            {
               

                conn = new System.Data.SqlClient.SqlConnection();
                //conn.ConnectionString = "integrated security=SSPI;data source=lonweb1t.ashmore.local;" + "persist security info=False;initial catalog=UAT_AIMS";
                conn.ConnectionString =  Properties.Settings.Default.ConnectionString;
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select issuer_id from Get_Data_issuer_List where status_txt ='READY' and RUN_ID = " + runId;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.Connection = conn;
                                // Insert code to process data.
                
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    
                    issuerList.Add(reader["ISSUER_ID"].ToString());
                }

                
                return issuerList;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);


                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
                return null;
                
            }
            finally
            {
               
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
               
                               
            }
        }



        private static void DoWork(object o)
        {
            int index = (int)o;
            SqlConnection conn = null;
            try
            {
                print("Begin processing " + issuerId[index] );
                conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = Properties.Settings.Default.ConnectionString;
                conn.Open();
                DoUpdateProcessStatus(conn, index,"Running");
                SqlCommand cmd = new SqlCommand("Get_Data", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@ISSUER_ID", SqlDbType.VarChar));
                cmd.Parameters["@Issuer_Id"].Value = issuerId[index];
                cmd.Parameters.Add(new SqlParameter("@RUN_MODE", SqlDbType.VarChar));
                cmd.Parameters["@RUN_MODE"].Value = runMode;

                 // Insert code to process data.
                cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                DoUpdateProcessStatus(conn, index, "Finished");
                print("End process issuer " + issuerId[index]);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                DoUpdateProcessStatus(conn, index, "Failed");
                print("Failed to process issuer " + issuerId[index]);
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
                resetEvents[index].Set();
            }

        }
        private static void DoFinalUpdateStatus()
        {
            SqlConnection conn = null;
            try
            {
                SqlCommand cmd = new SqlCommand();
                conn = new System.Data.SqlClient.SqlConnection();
                conn.ConnectionString = Properties.Settings.Default.ConnectionString;
                conn.Open();
                String strQuery = "		update GET_DATA_RUN " +
                                    " set STATUS_TXT = 'Done' " +
                                    ",   END_TIME = GETDATE() " +
                                    ",   ISSUER_COUNT = (select COUNT(*) from GET_DATA_ISSUER_LIST where RUN_ID = " + runId + " ) " +
                                    "  where RUN_ID = " + runId;
              
                cmd.CommandText = strQuery;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.Connection = conn;
                cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Source);
                Console.WriteLine(e.StackTrace);
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
            }
            finally
            {
              
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
                
            }
        }

        private static void DoUpdateProcessStatus(SqlConnection conn,int index, String runStatus)
        {
            String strQuery = "";
            if (runStatus.Equals("Running"))
            {
                strQuery = "update GET_DATA_ISSUER_LIST " +
                                       "set START_TIME = GETDATE() " +
                                        ",  PROCESS_ID = @@SPID" +
                                        ",  STATUS_TXT = 'Active'" +
                                        " where RUN_ID =" + runId +
                                        "  and ISSUER_ID = '" + issuerId[index]+"'";
            }
            else if(runStatus.Equals("Finished"))
            {
                strQuery = "update GET_DATA_ISSUER_LIST " +
                                    "set END_TIME = GETDATE() " +
                                     ",  STATUS_TXT = 'Complete'" +
                                     " where RUN_ID =" + runId +
                                     "  and ISSUER_ID = '" + issuerId[index]+"'";
            }
            else if (runStatus.Equals("Failed"))
            {
                strQuery = "update GET_DATA_ISSUER_LIST " +
                                    "set END_TIME = GETDATE() " +
                                     ",  STATUS_TXT = 'Failed'" +
                                     " where RUN_ID =" + runId +
                                     "  and ISSUER_ID = '" + issuerId[index] + "'";
            }
            
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = strQuery;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 0;
            cmd.Connection = conn;
            cmd.ExecuteScalar();


        }


        private static int Get_Data_Process(String command)
        {
            SqlConnection conn = null;
            int runId=0;

            try
            {
                conn = new System.Data.SqlClient.SqlConnection();
                //conn.ConnectionString = "integrated security=SSPI;data source=lonweb1t.ashmore.local;" + "persist security info=False;initial catalog=UAT_AIMS";
                conn.ConnectionString = Properties.Settings.Default.ConnectionString;
                conn.Open();
                SqlCommand cmd = new SqlCommand("Get_Data_Process_Thread", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@COMMAND",SqlDbType.VarChar));
                cmd.Parameters["@COMMAND"].Value = command;
                cmd.Parameters.Add(new SqlParameter("@RUN_MODE", SqlDbType.VarChar));
                cmd.Parameters["@RUN_MODE"].Value = runMode;
                SqlParameter sqlParam = new SqlParameter("@RUN_ID_OUT", SqlDbType.Int);
                sqlParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(sqlParam);
                cmd.ExecuteScalar();
                runId = Convert.ToInt32(cmd.Parameters["@RUN_ID_OUT"].Value);
                return runId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }
                return 0;

            }
            finally
            {
              
                if (conn != null)
                {
                    conn.Close();
                    conn = null;
                }


            }
        }


    }
}
