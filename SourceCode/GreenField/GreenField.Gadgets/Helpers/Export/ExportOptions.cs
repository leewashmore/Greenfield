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
using System.Linq;
using Telerik.Windows.Data;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public enum RadExportFilterOption
    {
        [DescriptionAttribute("Excel Workbook (*.xls)|*.xls|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp")]
        RADCHART_EXPORT_FILTER = 0,
        [DescriptionAttribute("Excel Workbook (*.xls)|*.xls|XML (*.xml)|*.xml|CSV (Comma delimited)|*.csv|Word Document (*.doc)|*.doc")]
        RADGRIDVIEW_EXPORT_FILTER = 1      
    }

    public class ExportElementOptions
    {
        private Color _defaultBackground = Colors.White;
        private Color _highlightedBackground = Colors.DarkGray;

        private Color _defaultForeground = Colors.Black;
        private Color _highlightedForeground = Colors.White;

        private FontFamily _defaultFontFamily = new FontFamily("Arial");
        private FontFamily _highlightedFontFamily = new FontFamily("Trebuchet MS");

        private double _defaultFontSize = 10;
        private double _highlightedFontSize = 12;

        private FontWeight _defaultFontWeight = FontWeights.Normal;
        private FontWeight _highlightedFontWeight = FontWeights.Bold;

        private TextAlignment _defaultTextAlignment = TextAlignment.Left;
        private TextAlignment _highlightedTextAlignment = TextAlignment.Center;

        public ExportElement ExportElementType { get; set; }
        public Color ExportElementBackground { get; set; }
        public Color ExportElementForeground { get; set; }
        public FontFamily ExportElementFontFamily { get; set; }
        public double ExportElementFontSize { get; set; }
        public FontWeight ExportElementFontWeight { get; set; }
        public TextAlignment ExportElementTextAlignment { get; set; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="Type"><value = "True">Highlighted elements</value><value = "False">Default elements</param>
        public ExportElementOptions(ExportElement exportElement, bool Type = false)
        {
            ExportElementType = exportElement;
            if (Type)
            {
                ExportElementBackground = _highlightedBackground;
                ExportElementForeground = _highlightedForeground;
                ExportElementFontFamily = _highlightedFontFamily;
                ExportElementFontSize = _highlightedFontSize;
                ExportElementFontWeight = _highlightedFontWeight;
                ExportElementTextAlignment = _highlightedTextAlignment;
            }
            else
            {
                ExportElementBackground = _defaultBackground;
                ExportElementForeground = _defaultForeground;
                ExportElementFontFamily = _defaultFontFamily;
                ExportElementFontSize = _defaultFontSize;
                ExportElementFontWeight = _defaultFontWeight;
                ExportElementTextAlignment = _defaultTextAlignment;
            }
        }
    }

    public class ExportElementType
    {
        public ExportElement ExportElementTypeValue { get; set; }
        public string ExportElementTypeTitle { get; set; }
    }

    public class ExportFontFamily
    {
        public FontFamily ExportFontFamilyValue { get; set; }
        public string ExportFontFamilyTitle { get; set; }
    }    

    public class ExportFontWeight
    {
        public FontWeight ExportFontWeightValue { get; set; }
        public string ExportFontWeightTitle { get; set; }
    }

    public class ExportTextAlignment
    {
        public TextAlignment ExportTextAlignmentValue { get; set; }
        public string ExportTextAlignmentTitle { get; set; }
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
        public RadExportFilterOption ExportFilterOption { get; set; }        
    }

    public static class RadExport
    {
        public static void ExportStream(int filterIndex, RadExportOptions exportOption, Stream stream)
        {
            switch (exportOption.ExportFilterOption)
            {
                case RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER:
                    {
                        switch (filterIndex)
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
                case RadExportFilterOption.RADCHART_EXPORT_FILTER:
                    {
                        switch (filterIndex)
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

    public static class RadGridView_ElementExport
    {

        public static List<ExportElementOptions> ExportElementOptions;

        /// <summary>
        /// Handles RadGridView Element Export in predefined prameterized details
        /// </summary>
        /// <param name="exportElement"></param>
        /// <param name="cellValueConverter"></param>
        public static void ElementExporting(GridViewElementExportingEventArgs exportElement, Func<object> cellValueConverter = null)
        {
            ExportElementOptions element = ExportElementOptions.Where(t => t.ExportElementType == exportElement.Element).FirstOrDefault();
            if (element != null)
            {
                exportElement.Background = element.ExportElementBackground;
                exportElement.Foreground = element.ExportElementForeground;
                exportElement.FontFamily = element.ExportElementFontFamily;
                exportElement.FontSize = element.ExportElementFontSize;
                exportElement.Height = element.ExportElementFontSize + 5;
                exportElement.Width = 100;
                exportElement.VerticalAlignment = VerticalAlignment.Center;
                exportElement.FontWeight = element.ExportElementFontWeight;
                exportElement.TextAlignment = element.ExportElementTextAlignment;   
            }          

            if (exportElement.Element == ExportElement.Cell)
            {
                if (cellValueConverter != null)
                {
                    exportElement.Value = cellValueConverter();                    
                }
            }
            
            else if (exportElement.Element == ExportElement.GroupFooterCell)
            {
                GridViewDataColumn column = exportElement.Context as GridViewDataColumn;
                QueryableCollectionViewGroup qcvGroup = exportElement.Value as QueryableCollectionViewGroup;

                if (column != null && qcvGroup != null && column.AggregateFunctions.Count > 0)
                {
                    exportElement.Value = GetAggregates(qcvGroup, column);
                }
            }
            
        }

        private static string GetAggregates(QueryableCollectionViewGroup group, GridViewDataColumn column)
        {
            List<string> aggregates = new List<string>();

            foreach (AggregateFunction f in column.AggregateFunctions)
            {
                foreach (AggregateResult r in group.AggregateResults)
                {
                    if (f.FunctionName == r.FunctionName && r.FormattedValue != null)
                    {
                        aggregates.Add(r.FormattedValue.ToString());
                    }
                }
            }

            return String.Join(",", aggregates.ToArray());
        }
    }
}
