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
using System.ComponentModel;
using System.Reflection;
using Telerik.Windows.Controls;
using System.IO;

namespace GreenField.Gadgets.Helpers
{
    public enum RadExportFilterOptions
    {
        [DescriptionAttribute("Excel Workbook (*.xls)|*.xls|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp")]
        RADCHART_EXPORT_FILTER = 0,
        [DescriptionAttribute("Excel Workbook (*.xls)|*.xls|XML (*.xml)|*.xml|CSV (Comma delimited)|*.csv|Word Document (*.doc)|*.doc")]
        RADGRIDVIEW_EXPORT_FILTER = 1      
    }

    public static class RadExportFilterOptionDesc
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes!= null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }        
    }

    public class RadExportOptions
    {
        public string ElementName { get; set; }
        public UIElement Element { get; set; }
        public RadExportFilterOptions ExportFilterOption { get; set; }        
    }

    public static class RadExport
    {
        public static void ExportStream(int FilterIndex, RadExportOptions exportOption, Stream stream)
        {
            switch (exportOption.ExportFilterOption)
            {
                case RadExportFilterOptions.RADGRIDVIEW_EXPORT_FILTER:
                    {
                        switch (FilterIndex)
                        {
                            case 1:
                                RadExport.ExportRadGridViewXLS(exportOption.Element, stream);
                                break;
                            case 2:
                                RadExport.ExportRadGridViewXML(exportOption.Element, stream);
                                break;
                            case 3:
                                RadExport.ExportRadGridViewCSV(exportOption.Element, stream);
                                break;
                            case 4:
                                RadExport.ExportRadGridViewDOC(exportOption.Element, stream);
                                break;
                            default:
                                RadExport.ExportRadGridViewXLS(exportOption.Element, stream);
                                break;
                        }
                    }
                    break;
                case RadExportFilterOptions.RADCHART_EXPORT_FILTER:
                    {
                        switch (FilterIndex)
                        {
                            case 1:
                                RadExport.ExportRadChartXLS(exportOption.Element, stream);
                                break;
                            case 2:
                                RadExport.ExportRadChartPNG(exportOption.Element, stream);
                                break;
                            case 3:
                                RadExport.ExportRadChartBMP(exportOption.Element, stream);
                                break;
                            default:
                                RadExport.ExportRadChartXLS(exportOption.Element, stream);
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            stream.Close();
        }

        private static void ExportRadChartXLS(UIElement element, Stream stream)
        {
            (element as RadChart).ExportToExcelML(stream);
        }

        private static void ExportRadChartPNG(UIElement element, Stream stream)
        {
            (element as RadChart).ExportToImage(stream, new Telerik.Windows.Media.Imaging.PngBitmapEncoder());
        }

        private static void ExportRadChartBMP(UIElement element, Stream stream)
        {
            (element as RadChart).ExportToImage(stream, new Telerik.Windows.Media.Imaging.BmpBitmapEncoder());
        }

        private static void ExportRadGridViewXLS(UIElement element, Stream stream)
        {
            (element as RadGridView).Export(stream, new GridViewExportOptions()
                            {
                                Format = ExportFormat.Html,
                                ShowColumnFooters = true,
                                ShowColumnHeaders = true,
                                ShowGroupFooters = true
                            });
        }

        private static void ExportRadGridViewXML(UIElement element, Stream stream)
        {
            (element as RadGridView).Export(stream, new GridViewExportOptions()
            {
                Format = ExportFormat.ExcelML,
                ShowColumnFooters = true,
                ShowColumnHeaders = true,
                ShowGroupFooters = true
            });
        }

        private static void ExportRadGridViewCSV(UIElement element, Stream stream)
        {
            (element as RadGridView).Export(stream, new GridViewExportOptions()
            {
                Format = ExportFormat.Csv,
                ShowColumnFooters = true,
                ShowColumnHeaders = true,
                ShowGroupFooters = true
            });
        }

        private static void ExportRadGridViewDOC(UIElement element, Stream stream)
        {
            (element as RadGridView).Export(stream, new GridViewExportOptions()
            {
                Format = ExportFormat.Html,
                ShowColumnFooters = true,
                ShowColumnHeaders = true,
                ShowGroupFooters = true
            });
        }

        

    }

}
