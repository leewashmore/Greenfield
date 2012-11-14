using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Filter options for RadGridView UIElement and RadChart UIElement 
    /// </summary>
    public enum RadExportFilterOption
    {
        [DescriptionAttribute("Excel Workbook (*.xls)|*.xls|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp")]
        RADCHART_EXCEL_EXPORT_FILTER = 0,
        [DescriptionAttribute("Excel Workbook (*.xls)|*.xls|XML (*.xml)|*.xml|CSV (Comma delimited)|*.csv|Word Document (*.doc)|*.doc")]
        RADGRIDVIEW_EXCEL_EXPORT_FILTER = 1,
        [DescriptionAttribute("PDF (*.pdf)|*.pdf")]
        RADCHART_PDF_EXPORT_FILTER = 2,
        [DescriptionAttribute("PDF (*.pdf)|*.pdf")]
        RADGRIDVIEW_PDF_EXPORT_FILTER = 3,
        [DescriptionAttribute("")]
        RADCHART_PRINT_FILTER = 4,
        [DescriptionAttribute("")]
        RADGRIDVIEW_PRINT_FILTER = 5
    }

    public class ExportElementOptions
    {
        #region Fields
        /// <summary>
        /// default background
        /// </summary>
        private Color defaultBackground = Colors.White;

        /// <summary>
        /// highlighted background
        /// </summary>
        private Color highlightedBackground = Colors.DarkGray;

        /// <summary>
        /// default foreground
        /// </summary>
        private Color defaultForeground = Colors.Black;

        /// <summary>
        /// highlighted foreground
        /// </summary>
        private Color highlightedForeground = Colors.White;

        /// <summary>
        /// default font family
        /// </summary>
        private FontFamily defaultFontFamily = new FontFamily("Arial");

        /// <summary>
        /// highlighted font family
        /// </summary>
        private FontFamily highlightedFontFamily = new FontFamily("Trebuchet MS");

        /// <summary>
        /// default font size
        /// </summary>
        private double defaultFontSize = 10;

        /// <summary>
        /// highlighted font size
        /// </summary>
        private double highlightedFontSize = 12;

        /// <summary>
        /// default font weight
        /// </summary>
        private FontWeight defaultFontWeight = FontWeights.Normal;

        /// <summary>
        /// highlighted font weight
        /// </summary>
        private FontWeight highlightedFontWeight = FontWeights.Bold;

        /// <summary>
        /// default text alignment
        /// </summary>
        private TextAlignment defaultTextAlignment = TextAlignment.Left;

        /// <summary>
        /// highlighted text alignment
        /// </summary>
        private TextAlignment highlightedTextAlignment = TextAlignment.Center;
        #endregion

        #region Properties
        /// <summary>
        /// Export Element Type
        /// </summary>
        public ExportElement ExportElementType { get; set; }

        /// <summary>
        /// Export Element Background
        /// </summary>
        public Color ExportElementBackground { get; set; }

        /// <summary>
        /// Export Element Foreground
        /// </summary>
        public Color ExportElementForeground { get; set; }

        /// <summary>
        /// Export Element Font Family
        /// </summary>
        public FontFamily ExportElementFontFamily { get; set; }

        /// <summary>
        /// Export Element Font Size
        /// </summary>
        public double ExportElementFontSize { get; set; }

        /// <summary>
        /// Export Element Font Weight
        /// </summary>
        public FontWeight ExportElementFontWeight { get; set; }

        /// <summary>
        /// Export Element Text Alignment
        /// </summary>
        public TextAlignment ExportElementTextAlignment { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exportElement">ExportElement</param>
        /// <param name="Type"><value = "True">Highlighted elements</value><value = "False">Default elements</param>
        public ExportElementOptions(ExportElement exportElement, bool Type = false)
        {
            ExportElementType = exportElement;
            if (Type)
            {
                ExportElementBackground = highlightedBackground;
                ExportElementForeground = highlightedForeground;
                ExportElementFontFamily = highlightedFontFamily;
                ExportElementFontSize = highlightedFontSize;
                ExportElementFontWeight = highlightedFontWeight;
                ExportElementTextAlignment = highlightedTextAlignment;
            }
            else
            {
                ExportElementBackground = defaultBackground;
                ExportElementForeground = defaultForeground;
                ExportElementFontFamily = defaultFontFamily;
                ExportElementFontSize = defaultFontSize;
                ExportElementFontWeight = defaultFontWeight;
                ExportElementTextAlignment = defaultTextAlignment;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="exportElement">ExportElement</param>
        public ExportElementOptions(ExportElement exportElement)
        {
            ExportElementType = exportElement;
            ExportElementBackground = defaultBackground;
            ExportElementForeground = defaultForeground;
            ExportElementFontFamily = defaultFontFamily;
            ExportElementFontSize = defaultFontSize;
            ExportElementFontWeight = defaultFontWeight;
            ExportElementTextAlignment = defaultTextAlignment;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public ExportElementOptions()
        {
            ExportElementBackground = defaultBackground;
            ExportElementForeground = defaultForeground;
            ExportElementFontFamily = defaultFontFamily;
            ExportElementFontSize = defaultFontSize;
            ExportElementFontWeight = defaultFontWeight;
            ExportElementTextAlignment = defaultTextAlignment;
        }
        #endregion
    }

    /// <summary>
    /// Export Element Type
    /// </summary>
    public class ExportElementType
    {
        /// <summary>
        /// Export Element Type Value
        /// </summary>
        public ExportElement ExportElementTypeValue { get; set; }

        /// <summary>
        /// Export Element Type Title
        /// </summary>
        public string ExportElementTypeTitle { get; set; }
    }

    /// <summary>
    /// Export Font Family
    /// </summary>
    public class ExportFontFamily
    {
        /// <summary>
        /// Export Font Family Value
        /// </summary>
        public FontFamily ExportFontFamilyValue { get; set; }

        /// <summary>
        /// Export Font Family Title
        /// </summary>
        public string ExportFontFamilyTitle { get; set; }
    }

    /// <summary>
    /// Export Font Weight
    /// </summary>
    public class ExportFontWeight
    {
        /// <summary>
        /// Export Font Weight Value
        /// </summary>
        public FontWeight ExportFontWeightValue { get; set; }

        /// <summary>
        /// Export Font Weight Title
        /// </summary>
        public string ExportFontWeightTitle { get; set; }
    }

    /// <summary>
    /// Export Text Alignment
    /// </summary>
    public class ExportTextAlignment
    {
        /// <summary>
        /// Export Text Alignment Value
        /// </summary>
        public TextAlignment ExportTextAlignmentValue { get; set; }

        /// <summary>
        /// Export Text Alignment Title
        /// </summary>
        public string ExportTextAlignmentTitle { get; set; }
    }

    /// <summary>
    /// RadExport Filter Option Description
    /// </summary>
    public static class RadExportFilterOptionDesc
    {
        /// <summary>
        /// Retrieve enumeration description attribute string from enum value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }

    /// <summary>
    /// Export Items to be displayed in the option list
    /// </summary>
    public class RadExportOptions
    {
        /// <summary>
        /// Option name to be displayed in the dropdown list
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// UI element to be exported - RadGridView or RadChart
        /// </summary>
        public UIElement Element { get; set; }

        /// <summary>
        /// Filter option based on the UIElement being exported
        /// </summary>
        public RadExportFilterOption ExportFilterOption { get; set; }

        /// <summary>
        /// RadRichTextBox from visual tree for printing
        /// </summary>
        public RadRichTextBox RichTextBox { get; set; }

        /// <summary>
        /// Indexes in columns to skip in export
        /// </summary>
        public List<int> SkipColumnDisplayIndex { get; set; }
    }

    /// <summary>
    /// Supports export of RadGrid and RadChart export to stream
    /// </summary>
    public static class RadExport
    {
        /// <summary>
        /// Exports RadGrid and RadChart to stream
        /// </summary>
        /// <param name="filterIndex">defines the index of filter specified for extension of file exported to</param>
        /// <param name="exportOption">RadExportOptions</param>
        /// <param name="stream">Stream</param>
        public static void ExportStream(int filterIndex, RadExportOptions exportOption, Stream stream, List<int> skipColumnDisplayIndex = null)
        {
            switch (exportOption.ExportFilterOption)
            {
                case RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER:
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
                case RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER:
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
                case RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER:
                    RadExport.ExportRadGridViewPDF(exportOption.Element, stream, skipColumnDisplayIndex);
                    break;
                case RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER:
                    RadExport.ExportRadChartPDF(exportOption.Element, stream);
                    break;
                default:
                    break;
            }
            stream.Close();
        }

        /// <summary>
        /// Exports RadChart image to .xls extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
        private static void ExportRadChartXLS(UIElement element, Stream stream)
        {
            (element as RadChart).ExportToExcelML(stream);
        }

        /// <summary>
        /// Exports RadChart image to .png extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
        private static void ExportRadChartPNG(UIElement element, Stream stream)
        {
            (element as RadChart).ExportToImage(stream, new Telerik.Windows.Media.Imaging.PngBitmapEncoder());
        }

        /// <summary>
        /// Exports RadChart image to .bmp extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
        private static void ExportRadChartBMP(UIElement element, Stream stream)
        {
            (element as RadChart).ExportToImage(stream, new Telerik.Windows.Media.Imaging.BmpBitmapEncoder());
        }

        /// <summary>
        /// Exports RadChart image to .pdf extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
        private static void ExportRadChartPDF(UIElement element, Stream stream)
        {
            PDFExporter.btnExportChartPDF_Click(element as RadChart, stream: stream);
        }

        /// <summary>
        /// Exports RadGrid data to .xls extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
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

        /// <summary>
        /// Exports RadGrid data to .xml extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
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

        /// <summary>
        /// Exports RadGrid data to .csv extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
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

        /// <summary>
        /// Exports RadGrid data to .doc extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
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
                
        /// <summary>
        /// Exports RadGridView data to .pdf extension file
        /// </summary>
        /// <param name="element">UIElement</param>
        /// <param name="stream">Stream</param>
        private static void ExportRadGridViewPDF(UIElement element, Stream stream, List<int> skipColumnDisplayIndex = null)
        {
            PDFExporter.btnExportPDF_Click(element as RadGridView, stream: stream, skipColumnDisplayIndex: skipColumnDisplayIndex); 
        }
    }

    /// <summary>
    /// Wrapper for RadGridView Element Exporting event
    /// </summary>
    public static class RadGridView_ElementExport
    {
        /// <summary>
        /// Export Element Options
        /// </summary>
        public static List<ExportElementOptions> ExportElementOptions = new List<ExportElementOptions>() { };

        /// <summary>
        /// Handles RadGridView Element Export in predefined prameterized details
        /// </summary>
        /// <param name="exportElement">GridViewElementExportingEventArgs - custom argument received in RadGridView Element_Exporting event</param>
        /// <param name="cellValueConverter">Function to convert cell values to customed values</param>
        /// <param name="headerCellValueConverter">Function to convert header cell values to customed values</param>
        /// <param name="isGroupFootersVisible">True to display group footers in export</param>
        /// <param name="hideColumnIndex">List of column indexes to be hide in export</param>
        /// <param name="aggregatedColumnIndex">aggregated column indexes</param>
        public static void ElementExporting(GridViewElementExportingEventArgs exportElement, Func<object> cellValueConverter = null, 
            Func<object> headerCellValueConverter = null, bool isGroupFootersVisible = true
            , List<int> hideColumnIndex = null, List<int> aggregatedColumnIndex = null)
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


            if (hideColumnIndex != null)
            {
                GridViewDataColumn column = exportElement.Context as GridViewDataColumn;

                if (column != null)
                {
                    if (exportElement.Element == ExportElement.Cell || exportElement.Element == ExportElement.FooterCell ||
                                exportElement.Element == ExportElement.GroupFooterCell || exportElement.Element == ExportElement.GroupHeaderCell ||
                                exportElement.Element == ExportElement.GroupIndentCell || exportElement.Element == ExportElement.HeaderCell)
                    {
                        if (hideColumnIndex.Contains(column.DisplayIndex))
                        {
                            exportElement.Cancel = true;
                        }
                    }
                }
            }

            if (exportElement.Element == ExportElement.HeaderCell)
            {
                if (headerCellValueConverter != null)
                {
                    exportElement.Value = headerCellValueConverter();
                }
            }

            if (exportElement.Element == ExportElement.GroupFooterRow || exportElement.Element == ExportElement.GroupFooterCell)
            {
                if (isGroupFootersVisible == false)
                {
                    exportElement.Cancel = true;
                    return;
                }
            }

            if (exportElement.Element == ExportElement.Cell)
            {
                if (cellValueConverter != null)
                {
                    exportElement.Value = cellValueConverter();
                }
            }

            if (exportElement.Element == ExportElement.Row)
            {
                if (cellValueConverter != null)
                {
                    exportElement.Value = cellValueConverter();
                }
            }

            if (exportElement.Element == ExportElement.FooterCell)
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
                else
                {
                    exportElement.Value = "";
                }
            }

        }

        /// <summary>
        /// Gets aggregate values to be posted in footer cells
        /// </summary>
        /// <param name="group">QueryableCollectionViewGroup</param>
        /// <param name="column">GridViewDataColumn</param>
        /// <returns>aggreagte value</returns>
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
