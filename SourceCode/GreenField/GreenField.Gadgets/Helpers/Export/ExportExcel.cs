using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Export to Excel class
    /// </summary>
    public static class ExportExcel
    {
        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="dgGeneric">DataGrid to be Exported</param>
        public static void ExportGridExcel(RadGridView dgGeneric)
        {
            try
            {
                string extension = "";
                ExportFormat format = ExportFormat.Html;
                extension = "xls";
                format = ExportFormat.Html;

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = extension;
                dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "Excel");
                dialog.FilterIndex = 1;

                if (dialog.ShowDialog() == true)
                {
                    using (Stream stream = dialog.OpenFile())
                    {
                        GridViewExportOptions exportOptions = new GridViewExportOptions();
                        exportOptions.Format = format;
                        exportOptions.ShowColumnFooters = true;
                        exportOptions.ShowColumnHeaders = true;
                        exportOptions.ShowGroupFooters = true;
                        dgGeneric.Export(stream, exportOptions);
                    }
                }

                
            }
            catch (Exception)
            {                
                throw;                
            }


        }
    }
}
