using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace ExcelWriter
{
    class Program
    {
        static void Main(string[] args)
        {
            var bankAccounts = new List<Account> {
                new Account { 
                  ID = 345678,
                  Balance = 541.27
                },
                new Account {
                  ID = 1230221,
                  Balance = -127.44
                }
            };

            DisplayInExcel(bankAccounts);

        }

        static void DisplayInExcel(IEnumerable<Account> accounts)
        {
            var excelApp = new Excel.Application();
            // Make the object visible.
            excelApp.Interactive = false;
            excelApp.DisplayAlerts = false;
            excelApp.ScreenUpdating = false;
            excelApp.Visible = false;
            excelApp.UserControl = false;

            try
            {
                Workbook workBook = excelApp.Workbooks.Add(Type.Missing);

                Excel._Worksheet workSheet = workBook.Worksheets[1];

                workSheet.Cells[1, "A"] = "ID Number";
                workSheet.Cells[1, "B"] = "Current Balance";

                var row = 1;
                foreach (var acct in accounts)
                {
                    row++;
                    workSheet.Cells[row, 1] = acct.ID;
                    workSheet.Cells[row, 2] = acct.Balance;
                }

                workSheet.Columns[1].AutoFit();
                workSheet.Columns[2].AutoFit();

                ((Excel.Range)workSheet.Columns[1]).AutoFit();
                ((Excel.Range)workSheet.Columns[2]).AutoFit();
                workSheet.Name = "Accounts";

                string fileName = GetFileName();
                // Save the Workbook and quit Excel.
                workBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, false, false,
                    Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);
                workBook.Saved = true;
                workBook.Close(false, Type.Missing, Type.Missing);
            }
            catch
            {
                Console.WriteLine("Error occurred while writing file!!!!");
            }
            finally
            {
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                excelApp = null;
            }

        }

        private static string GetFileName()
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid() + "_Model.xlsx";

            return fileName;
        }

    }

    public class Account
    {
        public int ID { get; set; }
        public double Balance { get; set; }
    }

}
