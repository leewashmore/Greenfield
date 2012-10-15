using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Excel = Microsoft.Office.Interop.Excel;

namespace ReutersPlugIn
{
    public partial class ReutersRibbon
    {
        protected Excel.Worksheet reutersWorksheet;

        private void ReutersRibbon_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void btnImport_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = true;
            Globals.ThisAddIn.GetData();
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.TaskPane.Visible = true;
        }
    }
}
