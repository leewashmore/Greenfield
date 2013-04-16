using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using Telerik.Windows.Documents.Model;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Diagnostics;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Layout;
using System.Globalization;
using System.Windows.Media;
using GreenField.DataContracts;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for ViewInvestmentContext
    /// </summary>
    public partial class ViewInvestmentContext : ViewBaseUserControl
    {
        /// <summary>
        /// Instance of View-Model
        /// </summary>
        ///
        private ViewModelInvestmentContext dataContextSource;
        public ViewModelInvestmentContext DataContextSource
        {
            get { return dataContextSource; }
            set { dataContextSource = value; }
        }


        private static int fontSizePDF = 8;

        

        private static string[] headerData = { "Name","", "Mkt. Value", "$ Mkt. Cap", "Fwd PE", "Fwd PB", "PE " + DateTime.Now.Year, "PE " + (DateTime.Now.Year + 1), "PB " + (DateTime.Now.Year), "PB "+(DateTime.Now.Year+1), "EV/ EBITDA "+(DateTime.Now.Year), "EV/ EBITDA "+(DateTime.Now.Year+1), "DY "+(DateTime.Now.Year), "ROE "+(DateTime.Now.Year) };
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewInvestmentContext(ViewModelInvestmentContext dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
        }

        #endregion

        /// <summary>
        /// Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownloadPDF_Click(object sender, RoutedEventArgs e)
        {
            fontSizePDF = 10;
          //  this.dataContextSource.DownloadInvestmentContext();
            RadDocument document = GenerateRadDocument();
            PDFExporter.ExportPDF_RadDocument(document, fontSizePDF);
            
            
            
        }
        
        private RadDocument GenerateRadDocument()
        {
            
            RadDocument document = new RadDocument();
            document.LayoutMode = DocumentLayoutMode.Paged;
            Telerik.Windows.Documents.Layout.Padding padding = new Telerik.Windows.Documents.Layout.Padding(0);
            document.SectionDefaultPageMargin = padding;
 
         
            //document.


            string context = "Country";
          
            for (int i = 0; i < dataContextSource.InvestmentContextDataInfo.Count; i++)
            {

                Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
                string headerText = "Investment Context Summary By "+ dataContextSource.Context;
                if ( i == 0)
                {
                    context = "Country";
                    headerText = "Investment Context Summary By " + context;

                }
                else if ( i == 1)
                {
                    context = "Industry";
                    headerText = "Investment Context Summary By " + context;
                }
          
                #region SetHeaders
                section.Headers.Default = generateHeader(headerText);
                #endregion

                #region SetFooters
                section.Footers.Default = generateFooter();
                #endregion


                Table table = new Table();


             
                InvestmentContextDetailsData icdData = dataContextSource.InvestmentContextDataInfo[i];
                
                string issuerId = dataContextSource.SelectedSecurity.IssuerId;
                GenerateDataRow(table, icdData,issuerId , context);

                section.Blocks.Add(table);
                document.Sections.Add(section);
            }
                

                return document;
            
        }

        private void GenerateDataRow(Table table, InvestmentContextDetailsData icdData,string issuerId, string context)
        {
            List<InvestmentContextDetailsData> sortedData = icdData.children.OrderBy(x => x.MarketCap).Reverse().ToList();
            Telerik.Windows.Documents.Model.Border leftcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border topcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border bottomcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border rightcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.Single);
             foreach (InvestmentContextDetailsData icd in sortedData)
             {

                    TableRow row = new TableRow();
                    ///icd = dataContextSource.InvestmentContextDataInfo[i];
                    TableCell cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Left;
                    //cell.Style = "nowrap";
                    //cell.Style[""] = "nowrap";
                    // cell.T
                    cell.ColumnSpan = 2;
                    cell.PreferredWidth = new TableWidthUnit(200);
                    if (context == "Country")
                    {
                        AddCellValue(cell, icd.GicsSectorName);
                    }
                    else
                    {
                        if (icd.IssuerId == issuerId)
                        {
                            AddCellValue(cell, icd.IssuerName);
                        }
                        else
                        {
                            AddChildCellValue(cell, icd.IssuerName);

                        }
                    }


                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.MarketValue != null)
                    {
                        CultureInfo usa = new CultureInfo("en-US");
                        //usa.NumberFormat.CurrencyDecimalDigits = 0;
                        decimal val = Math.Round((decimal)icd.MarketValue);
                        //decimal val = (decimal)icd.mktVal;
                        string output = string.Empty;
                        output = val.ToString("C0", usa.NumberFormat);
                        if (context == "Country")
                        {
                            AddCellValue(cell, output);
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, output);
                            }
                            else
                            {
                                AddChildCellValue(cell, output);

                            }
                           
                        }
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.MarketValue);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                    if (icd.MarketCap != null)
                    {
                        CultureInfo usa = new CultureInfo("en-US");
                        //usa.NumberFormat.NumberDecimalDigits = 0;
                        decimal val = Math.Round((decimal)icd.MarketCap);

                        //decimal val = (decimal)icd.mktcap;
                        string output = string.Empty;
                        output = val.ToString("C0", usa.NumberFormat);
                        if (context == "Country")
                        {
                            AddCellValue(cell, output);
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, output);
                            }
                            else
                            {
                                AddChildCellValue(cell, output);

                            }
                        }
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.MarketCap);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.ForwardPE != null)
                    {

                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPE, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPE, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.ForwardPE, 1));

                            }
                            //AddChildCellValue(cell, "" + Math.Round((decimal)icd.ForwardPE, 1));
                        }
                        //AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPE, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.ForwardPE);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                    if (icd.ForwardPBV != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPBV, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPBV, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.ForwardPBV, 1));

                            }
                            //AddChildCellValue(cell, "" + Math.Round((decimal)icd.ForwardPBV, 1));
                        }
                        // AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPBV, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.ForwardPBV);
                    }

                    row.Cells.Add(cell);


                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.PECurrentYear != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.PECurrentYear, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.PECurrentYear, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.PECurrentYear, 1));

                            }

                           // AddChildCellValue(cell, "" + Math.Round((decimal)icd.PECurrentYear, 1));
                        }


                        //AddCellValue(cell, "" + Math.Round((decimal)icd.PECurrentYear, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.PECurrentYear);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.PENextYear != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.PENextYear, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.PENextYear, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.PENextYear, 1));

                            }
                            //AddChildCellValue(cell, "" + Math.Round((decimal)icd.PENextYear, 1));
                        }

                        //AddCellValue(cell, "" + Math.Round((decimal)icd.PENextYear, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.PENextYear);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.PBVCurrentYear != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.PBVCurrentYear, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.PBVCurrentYear, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.PBVCurrentYear, 1));

                            }
                           // AddChildCellValue(cell, "" + Math.Round((decimal)icd.PBVCurrentYear, 1));
                        }

                        // AddCellValue(cell, "" + Math.Round((decimal)icd.PBVCurrentYear, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.PBVCurrentYear);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                    if (icd.PBVNextYear != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.PBVNextYear, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.PBVNextYear, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.PBVNextYear, 1));

                            }
                            //AddChildCellValue(cell, "" + Math.Round((decimal)icd.PBVNextYear, 1));
                        }

                        // AddCellValue(cell, "" + Math.Round((decimal)icd.PBVNextYear, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.PBVNextYear);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.EB_EBITDA_CurrentYear != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_CurrentYear, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_CurrentYear, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_CurrentYear, 1));

                            }
                           // AddChildCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_CurrentYear, 1));
                        }

                        //AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_CurrentYear, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.EB_EBITDA_CurrentYear);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                    if (icd.EB_EBITDA_NextYear != null)
                    {


                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_NextYear, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_NextYear, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_NextYear, 1));

                            }
                           // AddChildCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_NextYear, 1));
                        }
                        //  AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_NextYear, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.EB_EBITDA_NextYear);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.DividendYield != null)
                    {
                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.DividendYield * 100, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.DividendYield * 100, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.DividendYield * 100, 1));

                            }
                           // AddChildCellValue(cell, "" + Math.Round((decimal)icd.DividendYield *100, 1));
                        }
                        // AddCellValue(cell, "" + Math.Round((decimal)icd.DividendYield *100, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.DividendYield);
                    }
                    row.Cells.Add(cell);

                    cell = new TableCell();
                    cell.TextAlignment = RadTextAlignment.Right;
                    if (icd.ROE != null)
                    {

                        if (context == "Country")
                        {
                            AddCellValue(cell, "" + Math.Round((decimal)icd.ROE * 100, 1));
                        }
                        else
                        {
                            if (icd.IssuerId == issuerId)
                            {
                                AddCellValue(cell, "" + Math.Round((decimal)icd.ROE * 100, 1));
                            }
                            else
                            {
                                AddChildCellValue(cell, "" + Math.Round((decimal)icd.ROE * 100, 1));

                            }
                         //   AddChildCellValue(cell, "" + Math.Round((decimal)icd.ROE * 100, 1));
                        }

                        // AddCellValue(cell, "" + Math.Round((decimal)icd.ROE * 100, 1));
                    }
                    else
                    {
                        AddCellValue(cell, "" + icd.ROE);
                    }
                    row.Cells.Add(cell);
                    table.AddRow(row);
                    //Debug.WriteLine(icd.GicsSectorName + "--->" + icd.children.Count);
                    if (icd.children != null)
                    {

                        GenerateChildRows(table, icd.children, issuerId);
                    }
                    //cell = new TableCell() { ColumnSpan = 5 };
                    //row.Cells.Add(cell);
                    //table.AddRow(row);



                }

             if (context == "Industry")
             {
                 generateBlankRow(table);
             }

                GenerateTotalLine(table, icdData);
        }


        private void GenerateTotalLine(Table table,InvestmentContextDetailsData icd)
        {
            Telerik.Windows.Documents.Model.Border leftcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border topcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border bottomcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border rightcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.Single);
            //InvestmentContextDetailsData icd = dataContextSource.InvestmentContextDataInfo;
            TableRow row = new TableRow();
            ///icd = dataContextSource.InvestmentContextDataInfo[i];
            TableCell cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Left;
            //cell.Style = "nowrap";
            //cell.Style[""] = "nowrap";
            // cell.T
            cell.ColumnSpan = 2;
            cell.PreferredWidth = new TableWidthUnit(150);
            AddCellValue(cell, icd.GicsSectorName);
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.MarketValue != null)
            {
                CultureInfo usa = new CultureInfo("en-US");
                //usa.NumberFormat.CurrencyDecimalDigits = 0;
                decimal val = Math.Round((decimal)icd.MarketValue);
                //decimal val = (decimal)icd.mktVal;
                string output = string.Empty;
                output = val.ToString("C0", usa.NumberFormat);

                AddCellValue(cell, output);
            }
            else
            {
                AddCellValue(cell, "" + icd.MarketValue);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            if (icd.MarketCap != null)
            {
                CultureInfo usa = new CultureInfo("en-US");
                //usa.NumberFormat.NumberDecimalDigits = 0;
                decimal val = Math.Round((decimal)icd.MarketCap);

                //decimal val = (decimal)icd.mktcap;
                string output = string.Empty;
                output = val.ToString("C0", usa.NumberFormat);
                AddCellValue(cell, output);
            }
            else
            {
                AddCellValue(cell, "" + icd.MarketCap);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.ForwardPE != null)
            {

                AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPE, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.ForwardPE);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            if (icd.ForwardPBV != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.ForwardPBV, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.ForwardPBV);
            }

            row.Cells.Add(cell);


            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.PECurrentYear != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.PECurrentYear, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.PECurrentYear);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.PENextYear != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.PENextYear, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.PENextYear);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.PBVCurrentYear != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.PBVCurrentYear, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.PBVCurrentYear);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            if (icd.PBVNextYear != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.PBVNextYear, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.PBVNextYear);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.EB_EBITDA_CurrentYear != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_CurrentYear, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.EB_EBITDA_CurrentYear);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            if (icd.EB_EBITDA_NextYear != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.EB_EBITDA_NextYear, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.EB_EBITDA_NextYear);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.DividendYield != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.DividendYield * 100, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.DividendYield);
            }
            row.Cells.Add(cell);

            cell = new TableCell();
            cell.TextAlignment = RadTextAlignment.Right;
            if (icd.ROE != null)
            {
                AddCellValue(cell, "" + Math.Round((decimal)icd.ROE * 100, 1));
            }
            else
            {
                AddCellValue(cell, "" + icd.ROE);
            }
            row.Cells.Add(cell);
            table.AddRow(row);
        }


        private void GenerateChildRows(Table table, List<InvestmentContextDetailsData> children, string issuerId)
        {

            Telerik.Windows.Documents.Model.Border leftcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border topcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border bottomcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border rightcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.Single);
            int i = 0;
            List<InvestmentContextDetailsData> sortedData = children.OrderBy(x => x.MarketCap).Reverse().ToList();
            
            foreach (var child in sortedData)
            {
                TableRow row = new TableRow();
               
                TableCell cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Left;
                //cell.Style = "nowrap";
                //cell.Style[""] = "nowrap";
                // cell.T
                cell.ColumnSpan = 2;
                cell.PreferredWidth = new TableWidthUnit(200);
                if (child.IssuerId == issuerId)
                {
                    AddCellValue(cell, child.IssuerName);
                    
                }
                else
                {
                    AddChildCellValue(cell, child.IssuerName);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.MarketValue != null)
                {
                    CultureInfo usa = new CultureInfo("en-US");
                    //usa.NumberFormat.CurrencyDecimalDigits = 0;
                    decimal val = Math.Round((decimal)child.MarketValue);
                    //decimal val = (decimal)icd.mktVal;
                    string output = string.Empty;
                    output = val.ToString("C0", usa.NumberFormat);
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, output);
                    }
                    else
                    {
                        AddChildCellValue(cell, output);
                    }
                }
                else
                {
                    AddChildCellValue(cell, "" + child.MarketValue);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                if (child.MarketCap != null)
                {
                    CultureInfo usa = new CultureInfo("en-US");
                    //usa.NumberFormat.NumberDecimalDigits = 0;
                    decimal val = Math.Round((decimal)child.MarketCap);
                    
                    //decimal val = (decimal)icd.mktcap;
                    string output = string.Empty;
                    output = val.ToString("C0", usa.NumberFormat);
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, output);
                    }
                    else
                    {
                        AddChildCellValue(cell, output);
                    }
                }
                else
                {
                    AddChildCellValue(cell, "" + child.MarketCap);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.ForwardPE != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.ForwardPE, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.ForwardPE, 1));
                    }
                }
                else
                {
                    AddChildCellValue(cell, "" + child.ForwardPE);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                if (child.ForwardPBV != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.ForwardPBV, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.ForwardPBV, 1));
                    }
                }
                else
                {
                    AddChildCellValue(cell, "" + child.ForwardPBV);
                }

                row.Cells.Add(cell);


                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.PECurrentYear != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.PECurrentYear, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.PECurrentYear, 1));
                    }
                }
                else
                {
                    AddChildCellValue(cell, "" + child.PECurrentYear);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.PENextYear != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.PENextYear, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.PENextYear, 1));
                    }
                }
                else
                {
                    AddChildCellValue(cell, "" + child.PENextYear);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.PBVCurrentYear != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.PBVCurrentYear, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.PBVCurrentYear, 1));
                    }
                   // AddChildCellValue(cell, "" + Math.Round((decimal)child.PBVCurrentYear, 1));
                }
                else
                {
                    AddChildCellValue(cell, "" + child.PBVCurrentYear);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                if (child.PBVNextYear != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.PBVNextYear, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.PBVNextYear, 1));
                    }
                   // AddChildCellValue(cell, "" + Math.Round((decimal)child.PBVNextYear, 1));
                }
                else
                {
                    AddChildCellValue(cell, "" + child.PBVNextYear);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.EB_EBITDA_CurrentYear != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.EB_EBITDA_CurrentYear, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.EB_EBITDA_CurrentYear, 1));
                    }
                  //  AddChildCellValue(cell, "" + Math.Round((decimal)child.EB_EBITDA_CurrentYear, 1));
                }
                else
                {
                    AddChildCellValue(cell, "" + child.EB_EBITDA_CurrentYear);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                cell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
                if (child.EB_EBITDA_NextYear != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.EB_EBITDA_NextYear, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.EB_EBITDA_NextYear, 1));
                    }
                    //AddChildCellValue(cell, "" + Math.Round((decimal)child.EB_EBITDA_NextYear, 1));
                }
                else
                {
                    AddChildCellValue(cell, "" + child.EB_EBITDA_NextYear);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.DividendYield != null)
                {
                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.DividendYield * 100, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.DividendYield * 100, 1));
                    }
                    //AddChildCellValue(cell, "" + Math.Round((decimal)child.DividendYield * 100, 1));
                }
                else
                {
                    AddChildCellValue(cell, "" + child.DividendYield);
                }
                row.Cells.Add(cell);

                cell = new TableCell();
                cell.TextAlignment = RadTextAlignment.Right;
                if (child.ROE != null)
                {

                    if (child.IssuerId == issuerId)
                    {
                        AddCellValue(cell, "" + Math.Round((decimal)child.ROE * 100, 1));
                    }
                    else
                    {
                        AddChildCellValue(cell, "" + Math.Round((decimal)child.ROE * 100, 1));
                    }
                    //AddChildCellValue(cell, "" + Math.Round((decimal)child.ROE * 100, 1));
                }
                else
                {
                    AddChildCellValue(cell, "" + child.ROE);
                }
                row.Cells.Add(cell);
                table.AddRow(row);
                i = i + 1;
              //  if (i > 15) break;

            }
            generateBlankRow(table);


        }
        private void generateBlankRow(Table table)
        {

            Telerik.Windows.Documents.Model.Border leftcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border topcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border bottomcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border rightcellBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.Single);
            TableRow blankrow = new TableRow();

            TableCell blankcell = new TableCell();
            blankcell.TextAlignment = RadTextAlignment.Left;
            blankcell.ColumnSpan = 4;
            blankcell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            AddCellValue(blankcell, "");
            blankrow.Cells.Add(blankcell);

            blankcell = new TableCell();
            blankcell.TextAlignment = RadTextAlignment.Left;
            blankcell.ColumnSpan = 2;
            blankcell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            AddCellValue(blankcell, "");
            blankrow.Cells.Add(blankcell);

            blankcell = new TableCell();
            blankcell.TextAlignment = RadTextAlignment.Left;
            blankcell.ColumnSpan = 4;
            blankcell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            AddCellValue(blankcell, "");
            blankrow.Cells.Add(blankcell);

            blankcell = new TableCell();
            blankcell.TextAlignment = RadTextAlignment.Left;
            blankcell.ColumnSpan = 2;
            blankcell.Borders = new TableCellBorders(leftcellBorder, topcellBorder, rightcellBorder, bottomcellBorder);
            AddCellValue(blankcell, "");
            blankrow.Cells.Add(blankcell);


            table.AddRow(blankrow);

        }

        private void generateHeaderRow(Table table)
        {
            Telerik.Windows.Documents.Model.Border leftBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border topBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border rightBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.None);
            Telerik.Windows.Documents.Model.Border bottomBorder = new Telerik.Windows.Documents.Model.Border(BorderStyle.Single);
            TableRow headerRow = new TableRow();
            for (int i = 0; i < headerData.Length; i++)
            {
                TableCell headercell = new TableCell() { VerticalAlignment = RadVerticalAlignment.Center };
                ;
                headercell.Borders = new TableCellBorders(leftBorder, topBorder, rightBorder, bottomBorder);
                AddCellValue(headercell, headerData[i]);
                if (i == 0)
                {
                    headercell.TextAlignment = RadTextAlignment.Left;
                    headercell.VerticalAlignment = RadVerticalAlignment.Center;
                    headercell.PreferredWidth = new TableWidthUnit(177);
                    headercell.ColumnSpan = 2;
                }
                else
                {
                    headercell.VerticalAlignment = RadVerticalAlignment.Center;
                    headercell.TextAlignment = RadTextAlignment.Center;
                    headercell.PreferredWidth = new TableWidthUnit(TableWidthUnitType.Auto);
                }
                headerRow.Cells.Add(headercell);
            }


            table.Rows.Add(headerRow);
               
        }

        private Header generateHeader(string headerText)
        {
           
            RadDocument headerDoc = new RadDocument();
            Section hSection = new Section();
            headerDoc.Sections.Add(hSection);
            headerDoc.SectionDefaultPageMargin = new Telerik.Windows.Documents.Layout.Padding(0);
            Paragraph hparagraph = new Paragraph();
            hSection.Blocks.Add(hparagraph);

            Span hspan = new Span(headerText);
            hspan.FontSize = 10;
            hspan.FontWeight = FontWeights.Bold;
            hparagraph.TextAlignment = RadTextAlignment.Center;
            hparagraph.Inlines.Add(hspan);
            hparagraph.SpacingBefore = 0;
            Table headerTable = new Table();
            generateHeaderRow(headerTable);
            hSection.Blocks.Add(headerTable);
            
            Header header = new Header();
            header.Body = headerDoc;

            return header;
            
           

        }

        private Footer generateFooter()
        {
            RadDocument footerdoc = new RadDocument();
            Section fsection = new Section();
            footerdoc.Sections.Add(fsection);
            footerdoc.SectionDefaultPageMargin = new Telerik.Windows.Documents.Layout.Padding(0);

            Paragraph fparagraph = new Paragraph();
            fparagraph.FontSize = 1;
            fsection.Blocks.Add(fparagraph);
            fparagraph.TextAlignment = RadTextAlignment.Right;
            footerdoc.MeasureAndArrangeInDefaultSize();
            PageField pf = new PageField();
            footerdoc.InsertField(pf, FieldDisplayMode.Result);
            Footer footer = new Footer();
            footer.Body = footerdoc;
            return footer;

        }

        /*
        private RadDocument CreateDocumentPDF()
        {
            RadDocument mainDocument = new RadDocument();
            mainDocument.LayoutMode = DocumentLayoutMode.Paged;
            Section mainSection = new Section();
            mainDocument.Sections.Add(mainSection);
            Table table = new Table(4, 4);
            table.Borders = new TableBorders(new Telerik.Windows.Documents.Model.Border(2.5f, BorderStyle.Dotted, Colors.Black));
            mainSection.Blocks.Add(table);
            var lastPageParagraph = CreateParagraph();
            lastPageParagraph.Inlines.Add(CreateSpan(string.Format("Number of Rows: {0}", 4)));
            mainSection.Blocks.Add(lastPageParagraph);
            mainDocument.MeasureAndArrangeInDefaultSize();
            mainSection.HasDifferentFirstPageHeaderFooter = true;
            mainSection.Headers.Default = new Header() { Body = this.GenerateRadDocument() };
            mainSection.Headers.First = new Header()
            {
                Body = this.CreateDocumentWithContent(tableParameter =>
                {
                    TableRow additionalRow = new TableRow();
                    TableCell additionalCell = new TableCell();
                    additionalCell.ColumnSpan = 3;
                    Paragraph additionalParagraph = CreateParagraph();
                    Span additionalSpan = this.CreateSpan("text");
                    additionalParagraph.Inlines.Add(additionalSpan);
                    additionalCell.Blocks.Add(additionalParagraph);
                    additionalRow.Cells.Add(additionalCell);
                    tableParameter.AddRow(additionalRow);
               })
            };
            return mainDocument;

        }

    

        private RadDocument CreateDocumentWithContent(Action<Table> additionalContent = null)
        {
            RadDocument document = new RadDocument();
            Section section = new Section();
            section.PageMargin = new Padding(0);
            Table table = new Table();
            table.Borders = new TableBorders(new Telerik.Windows.Documents.Model.Border(2.5f, BorderStyle.Dashed, Colors.Black));
            TableRow row = new TableRow();
            TableCell first = new TableCell();
            TableCell second = new TableCell() { TextAlignment = RadTextAlignment.Center };
            TableCell third = new TableCell() { TextAlignment = RadTextAlignment.Right };
            row.Cells.Add(first);
            row.Cells.Add(second);
            row.Cells.Add(third);
            table.AddRow(row);
            if (additionalContent != null)
            {
                additionalContent.Invoke(table);
            }
            section.Blocks.Add(table);
            Paragraph lastParagraph = CreateParagraph();
            lastParagraph.FontSize = 2;
            lastParagraph.SpacingAfter = 0;
            section.Blocks.Add(lastParagraph);
            document.Sections.Add(section);
            return document;
        }



        private Span CreateSpan(string text)
        {

            return new Span(text) { FontSize = 14 };

        }



        private static Paragraph CreateParagraph()
        {

            return new Paragraph() { SpacingAfter = 10 };

        }


        */

        private static void AddCellValue(TableCell cell, string value)
        {
            Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
            //parag
            if (value != null && value != "")
            {
                span.Text = value;
                span.FontFamily = new System.Windows.Media.FontFamily("Arial");
                span.FontSize = fontSizePDF;
                span.FontWeight = FontWeights.Bold;
                paragraph.Inlines.Add(span);
            }
            cell.Blocks.Add(paragraph);
        }

        private static void AddChildCellValue(TableCell cell, string value)
        {
            Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
            //parag
            if (value != null && value != "")
            {
                span.Text = value;
                span.FontFamily = new System.Windows.Media.FontFamily("Arial");
                span.FontSize = fontSizePDF;
                
                paragraph.Inlines.Add(span);
            }
            cell.Blocks.Add(paragraph);
        }




    }
}