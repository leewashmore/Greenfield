using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using AIMS.CalcEngine.Properties;

namespace AIMS.CalcEngine
{
    public class AIMS_CalcEngine
    {
        private static ManualResetEvent[] resetEvents;
        private static String[] issuerId;
        private static int _runId;
        private static String runMode = "F";

        private static void Main(string[] args)
        {
            int numberOfThreads = Settings.Default.NumberOfThreads;
            if (args != null && args[0] != null)
            {
                runMode = args[0];
            }
            _runId = Get_Data_Process("START");
            RunCalcEngine(numberOfThreads);
        }

        private static void RunCalcEngine(int numberOfThreads)
        {
            try
            {
                print("Start Getting list of all issuers to process ");
                //int runId = Get_Data_Process();
                List<String> issuerList = GetAllIssuers();
                print("End Getting list of all issuers to process ");
                String[] issuerListArr = issuerList.ToArray();
                int startIndex = 0;
                print("Begin processing all issuers ");
                while (startIndex < issuerListArr.Length)
                {
                    int threadCount;
                    if (issuerListArr.Length - startIndex > numberOfThreads)
                    {
                        threadCount = numberOfThreads;
                    }
                    else
                    {
                        threadCount = issuerListArr.Length - startIndex;
                    }

                    resetEvents = new ManualResetEvent[threadCount];
                    issuerId = new String[threadCount];
                    for (int i = 0; i < threadCount; i++,startIndex++)
                    {
                        issuerId[i] = issuerListArr[startIndex];
                    }

                    for (int i = 0; i < issuerId.Length; i++)
                    {
                        resetEvents[i] = new ManualResetEvent(false);
                        ThreadPool.QueueUserWorkItem(DoWork, i);
                    }
                    
                    WaitHandle.WaitAll(resetEvents);
                    print("Total issuers processed = " + startIndex);
                }
                DoFinalUpdateStatus();
                print("Get_Data Completed Successfully ");
            }
            catch (Exception e)
            {
                print(e.Message);
                print(e.StackTrace);
            }
        }

        private static void print(String message)
        {
            Console.WriteLine(DateTime.Now + " : " + message);
        }

        private static List<String> GetAllIssuers()
        {
            SqlConnection conn = null;
            var issuerList = new List<string>();

            try
            {
                print("Begin Get list of all the issuers to process");

                conn = new SqlConnection {ConnectionString = Settings.Default.ConnectionString};
                //conn.ConnectionString = "integrated security=SSPI;data source=lonweb1t.ashmore.local;" + "persist security info=False;initial catalog=UAT_AIMS";
                conn.Open();
                var cmd = new SqlCommand
                    {
                        CommandText =
                            "Select issuer_id from Get_Data_issuer_List where status_txt ='READY' and RUN_ID = " +
                            _runId,
                        CommandType = CommandType.Text,
                        CommandTimeout = 0,
                        Connection = conn
                    };
                // Insert code to process data.

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    issuerList.Add(reader["ISSUER_ID"].ToString());
                }

                print("End Get list of all the issuers to process");

                return issuerList;
            }
            catch (Exception e)
            {
                print(e.InnerException.ToString());
                print(e.StackTrace);


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
            var index = (int) o;
            SqlConnection conn = null;
            try
            {
                print("Begin processing " + issuerId[index]);
                conn = new SqlConnection {ConnectionString = Settings.Default.ConnectionString};
                conn.Open();
                DoUpdateProcessStatus(conn, index, "Running");
                var cmd = new SqlCommand("Get_Data", conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 0
                    };
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
                var cmd = new SqlCommand();
                conn = new SqlConnection {ConnectionString = Settings.Default.ConnectionString};
                conn.Open();
                String strQuery = "		update GET_DATA_RUN " +
                                  " set STATUS_TXT = 'Done' " +
                                  ",   END_TIME = GETDATE() " +
                                  ",   ISSUER_COUNT = (select COUNT(*) from GET_DATA_ISSUER_LIST where RUN_ID = " +
                                  _runId + " ) " +
                                  "  where RUN_ID = " + _runId;
                Console.WriteLine(strQuery);
                cmd.CommandText = strQuery;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 0;
                cmd.Connection = conn;
                cmd.ExecuteScalar();
            }
            catch (Exception e)
            {
                print(e.Source);
                print(e.StackTrace);
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

        private static void DoUpdateProcessStatus(SqlConnection conn, int index, String runStatus)
        {
            String strQuery = "";
            if (runStatus.Equals("Running"))
            {
                strQuery = "update GET_DATA_ISSUER_LIST " +
                           "set START_TIME = GETDATE() " +
                           ",  PROCESS_ID = @@SPID" +
                           ",  STATUS_TXT = 'Active'" +
                           " where RUN_ID =" + _runId +
                           "  and ISSUER_ID = '" + issuerId[index] + "'";
            }
            else if (runStatus.Equals("Finished"))
            {
                strQuery = "update GET_DATA_ISSUER_LIST " +
                           "set END_TIME = GETDATE() " +
                           ",  STATUS_TXT = 'Complete'" +
                           " where RUN_ID =" + _runId +
                           "  and ISSUER_ID = '" + issuerId[index] + "'";
            }
            else if (runStatus.Equals("Failed"))
            {
                strQuery = "update GET_DATA_ISSUER_LIST " +
                           "set END_TIME = GETDATE() " +
                           ",  STATUS_TXT = 'Failed'" +
                           " where RUN_ID =" + _runId +
                           "  and ISSUER_ID = '" + issuerId[index] + "'";
            }

            var cmd = new SqlCommand
                {
                    CommandText = strQuery,
                    CommandType = CommandType.Text,
                    CommandTimeout = 0,
                    Connection = conn
                };
            cmd.ExecuteScalar();
        }


        private static int Get_Data_Process(String command)
        {
            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection {ConnectionString = Settings.Default.ConnectionString};
                //conn.ConnectionString = "integrated security=SSPI;data source=lonweb1t.ashmore.local;" + "persist security info=False;initial catalog=UAT_AIMS";
                conn.Open();
                var cmd = new SqlCommand("Get_Data_Process_Thread", conn)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 0
                    };
                cmd.Parameters.Add(new SqlParameter("@COMMAND", SqlDbType.VarChar));
                cmd.Parameters["@COMMAND"].Value = command;
                cmd.Parameters.Add(new SqlParameter("@RUN_MODE", SqlDbType.VarChar));
                cmd.Parameters["@RUN_MODE"].Value = runMode;
                var sqlParam = new SqlParameter("@RUN_ID_OUT", SqlDbType.Int) {Direction = ParameterDirection.Output};
                cmd.Parameters.Add(sqlParam);
                cmd.ExecuteScalar();
                int runId = Convert.ToInt32(cmd.Parameters["@RUN_ID_OUT"].Value);
                return runId;
            }
            catch (Exception e)
            {
                print(e.InnerException.ToString());
                print(e.StackTrace);
                
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