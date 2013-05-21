using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketingCalc.DAL;
using MarketingCalc.DataContract;
using MarketingCalc.ServiceCaller;
using System.Runtime.InteropServices;
namespace MarketingCalc.Runner
{
    class Program
    {
        private static bool isclosing = false;

        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            Console.WriteLine("CTRL+C,CTRL+BREAK to exit");

 

            MarketingEntities entity = new MarketingEntities();
            MarketingCalcServiceCaller mcsc = new MarketingCalcServiceCaller();
            
            DateTime dt ;
            String portfolioId = null;
            if (args != null && args.Length > 0)
            {
                if (args[0] != null)
                {
                    portfolioId = args[0];
                }
                if (DateTime.TryParse(args[1], out dt))
                {
                    //do nothing
                }
                else
                {
                    Console.WriteLine("Bad date format");
                    //return ;
                }


            }
            else
            {
               dt= DateTime.Parse(getPreviousMonthEndDate());
            }

            int holdingCount = entity.GF_PORTFOLIO_LTHOLDINGS_HISTORY.Where(g => g.PORTFOLIO_DATE == dt).Count();
            if (holdingCount == 0)
            {
                Console.WriteLine(DateTime.Now+ " There are  no holdings in  GF_PORTFOLIO_LTHOLDINGS_HISTORY table for the date  " + dt.ToString());
               // return;
                Environment.Exit(-1);
            }

            int securityCount = entity.GF_SECURITY_BASEVIEW_HISTORY.Where(g => g.EFFECTIVE_DATE == dt).Count();
            if (securityCount == 0)
            {
                Console.WriteLine(DateTime.Now + " There are  no securities in  GF_SECURITY_BASEVIEW_HISTORY table for the date  " + dt.ToString());
                //return ;
                Environment.Exit(-1);
            }

            int pfCount = entity.PERIOD_FINANCIALS_HISTORY.Where(g => g.EFFECTIVE_DATE == dt).Count();
            if (pfCount == 0)
            {
                Console.WriteLine(DateTime.Now + " There are  no data in  PERIOD_FINANCIALS_HISTORY table for the date  " + dt.ToString());
                //return ;
                Environment.Exit(-1);
            }
          
          
            if (portfolioId == null)
            {
                List<GetPortfolioList_Result> pf = entity.GetPortfolioList().ToList();
                
                    foreach (var portfolio in pf)
                    {
                        mcsc.CallService(portfolio.portfolio_id, dt);
                        if (isclosing)
                        {
                            Console.WriteLine("External intervention stopped gracefull ");
                            //break;
                            Environment.Exit(-2);
                        }
                    }
                
            }
            else
            {
                if (portfolioId.Equals("ALL"))
                {
                    List<GetPortfolioList_Result> pf = entity.GetPortfolioList().ToList();
                    foreach (var portfolio in pf)
                    {
                        mcsc.CallService(portfolio.portfolio_id, dt);
                        if (isclosing)
                        {
                            Console.WriteLine("External intervention stopped gracefully ");
                            //break;
                            Environment.Exit(-2);
                        }
                    }
                }
                else
                {
                    mcsc.CallService(portfolioId, dt);
                }

                
            }
         }

        private static string getPreviousMonthEndDate()
        {
            int year = DateTime.Today.Year;
            int currentMonth = DateTime.Today.Month;
            int priorMonth = currentMonth - 1;
            if (currentMonth == 1) // If this runs in january then prior month will be december and the year also has to be set to prior year.
            {
                year = year - 1;
                priorMonth = 12;
            }

            int lastDayOfMonth = DateTime.DaysInMonth(year, priorMonth);
            DateTime dt = new DateTime(year, priorMonth, lastDayOfMonth);
            if (!(dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday))
            {
                //do Nothing
            }
            else
            { // do until to get last week day of the month.
                for (; ; )
                {
                    lastDayOfMonth = lastDayOfMonth - 1;
                    dt = new DateTime(year, priorMonth, lastDayOfMonth);
                    if (!(dt.DayOfWeek == DayOfWeek.Sunday || dt.DayOfWeek == DayOfWeek.Saturday))
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

            }

            string monthendDate = (DateTime.Today.Month - 1) + "/" + lastDayOfMonth + "/" + DateTime.Today.Year;
            return monthendDate;
        }


        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isclosing = true;
                    Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    isclosing = true;
                    Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isclosing = true;
                    Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isclosing = true;
                    Console.WriteLine("User is logging off!");
                    break;

            }
            return true;
        }



        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion


    }
}
