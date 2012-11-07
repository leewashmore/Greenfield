using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;

namespace GreenField.DashboardModule.Views
{
    /// <summary>
    /// Code behind for ViewDashboardCompanyValuationDiscountedCashFlow
    /// </summary>
    [Export]
    public partial class ViewDashboardCompanyValuationDiscountedCashFlow : UserControl, INavigationAware
    {
        #region Fields
        /// <summary>
        /// MEF event aggreagator instance
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Logging instance
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// Service caller instance
        /// </summary>
        private IDBInteractivity dBInteractivity;

        private List<string> EPS_BVPSp;
        public List<string> EPS_BVPS
        {
            get
            {
                if (EPS_BVPSp == null)
                {
                    EPS_BVPSp = new List<string>();
                }
                return EPS_BVPSp;
            }
            set { EPS_BVPSp = value; }
        }

        /// <summary>
        /// Collection of Tables to create DCF PDF Report
        /// </summary>
        private List<Table> dcfReport;
        public List<Table> DCFReport
        {
            get
            {
                if (dcfReport == null)
                {
                    dcfReport = new List<Table>();
                }
                return dcfReport;
            }
            set
            {
                dcfReport = value;
            }
        }

        /// <summary>
        /// Selected Security
        /// </summary>
        private string securityName;
        public string SecurityName
        {
            get { return securityName; }
            set { securityName = value; }
        }

        /// <summary>
        /// Country of the Selected Security
        /// </summary>
        private string countryName;
        public string CountryName
        {
            get { return countryName; }
            set { countryName = value; }
        }

        /// <summary>
        /// DCF-Report Created BY
        /// </summary>
        private string createdBy;
        public string CreatedBy
        {
            get { return createdBy; }
            set { createdBy = value; }
        }

        /// <summary>
        /// Creation Date
        /// </summary>
        private string creationDate;
        public string CreationDate
        {
            get { return creationDate; }
            set { creationDate = value; }
        }

        /// <summary>
        /// Final Report
        /// </summary>
        private RadDocument finalReport;
        public RadDocument FinalReport
        {
            get
            {
                if (finalReport == null)
                {
                    finalReport = new RadDocument();
                }
                return finalReport;
            }
            set { finalReport = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="eventAggregator"></param>
        /// <param name="dbInteractivity"></param>
        [ImportingConstructor]
        public ViewDashboardCompanyValuationDiscountedCashFlow(ILoggerFacade logger, IEventAggregator eventAggregator, 
            IDBInteractivity dbInteractivity)
        {
            InitializeComponent();

            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.dBInteractivity = dbInteractivity;

            eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// DashboardGadgetLoad Event Handler
        /// </summary>
        /// <param name="payload">DashboardGadgetPayload</param>
        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.rtvDashboard.Items.Count > 0)
            {
                return;
            }
            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = dBInteractivity,
                EventAggregator = eventAggregator,
                LoggerFacade = logger
            };

            ViewModelDCF _viewModel = new ViewModelDCF(param);

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_ASSUMPTIONS,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewAnalysisSummary(_viewModel)
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_FREE_CASH_FLOW,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewFreeCashFlows(new ViewModelFreeCashFlows(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_TERMINAL_VALUE_CALCULATIONS,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewTerminalValueCalculations(_viewModel)
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SUMMARY,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewDCFSummary(_viewModel)
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SENSIVITY,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewSensitivity(_viewModel)
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_EPS,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewSensitivityEPS(_viewModel)
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_BVPS,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewSensitivityBVPS(_viewModel)
            });
        }
        #endregion

        #region INavigationAware methods
        /// <summary>
        /// Returns true if satisfies requisite condition
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        /// <returns>True/False</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// Executed on navigation from this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(false);
        }

        /// <summary>
        /// Executed on navigation to this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(true);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Set IsActive property on Dashboard content
        /// </summary>
        /// <param name="value">IsActive value</param>
        private void SetIsActiveOnDahsboardItems(bool value)
        {
            int a = rtvDashboard.Items.Count;
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)item.Content;
                if (control != null)
                {
                    control.IsActive = value;
                }
            }
        }
     
        /// <summary>
        /// Generate DCF Report PDF
        /// </summary>
        /// <param name="sender">Sender of Event</param>
        /// <param name="e"></param>
        private void btnPDF_Click(object sender, RoutedEventArgs e)
        {
            DCFReport = new List<Table>();
            RadDocument mergedDocument = new RadDocument();

            RadDocument finalReport = new RadDocument();
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)item.Content;
                DCFPDFExport pdfData = control.CreateDocument();
                Table table = null;
                if (pdfData != null)
                {
                    table = pdfData.DataTable;
                    if (pdfData.CreatedBy != null && pdfData.SecurityName != null && pdfData.CountryName != null)
                    {
                        SecurityName = pdfData.SecurityName;
                        CountryName = pdfData.CountryName;
                        CreatedBy = pdfData.CreatedBy;
                        CreationDate = DateTime.Now.ToShortDateString();
                    }
                }
                if (table != null)
                {
                    DCFReport.Add(table);
                }
            }
            if (DCFReport != null)
            {
                finalReport = MergeDocuments(DCFReport);
                finalReport.SectionDefaultPageMargin = new Telerik.Windows.Documents.Layout.Padding() { All = 10 };
                PDFExporter.ExportPDF_RadDocument(finalReport, 10);
            }
        }

        /// <summary>
        /// Method to Merge Multiple RadDocuments
        /// </summary>
        /// <param name="tables">Array of type RadDocuments</param>
        /// <returns>Merged Documents</returns>
        private RadDocument MergeDocuments(List<Table> tables)
        {
            RadDocument mergedDocument = new RadDocument();
            mergedDocument.Sections.Add(GetSection(tables, 0, 4));
            mergedDocument.Sections.Add(GetSection(tables, 4, tables.Count));
            return mergedDocument;
        }

        /// <summary>
        /// Returns name of the gadget according to the order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        private string ReturnGadgetName(int order)
        {
            if (EPS_BVPS.Count == 0)
            {
                EPS_BVPS.Add(" ");
                EPS_BVPS.Add(" ");
            }

            switch (order)
            {
                case 0:
                    return "ASSUMPTIONS";
                case 1:
                    return "FREE CASH FLOWS";
                case 2:
                    return "TERMINAL VALUE CALCULATIONS";
                case 3:
                    return "SUMMARY";
                case 4:
                    return "SENSITIVITY";
                case 5:
                    return "SENSITIVITY EPS ";// + "EPS= " + Convert.ToString(EPS_BVPS[0]);
                case 6:
                    return "SENSITIVITY BVPS ";// + "BVPS= " + Convert.ToString(EPS_BVPS[1]);
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Create a Section
        /// </summary>
        /// <param name="tables"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        private Telerik.Windows.Documents.Model.Section GetSection(List<Table> tables, int startIndex, int endIndex)
        {
            Table headerTable = new Table(1, 4);
            TableRow headerRow = new TableRow();

            TableCell securityCell = new TableCell();
            Telerik.Windows.Documents.Model.Paragraph securityParagraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span securitySpan = new Telerik.Windows.Documents.Model.Span("Security: " + SecurityName);
            securityParagraph.Inlines.Add(securitySpan);
            securityCell.Children.Add(securityParagraph);

            TableCell countryCell = new TableCell();
            Telerik.Windows.Documents.Model.Paragraph countryParagraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span countrySpan = new Telerik.Windows.Documents.Model.Span("Country: " + CountryName);
            countryParagraph.Inlines.Add(countrySpan);
            countryCell.Children.Add(countryParagraph);

            TableCell createdByCell = new TableCell();
            Telerik.Windows.Documents.Model.Paragraph createdByParagraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span createdBySpan = new Telerik.Windows.Documents.Model.Span("Created by: " + CreatedBy);
            createdByParagraph.Inlines.Add(createdBySpan);
            createdByCell.Children.Add(createdByParagraph);

            TableCell createdOn = new TableCell();
            Telerik.Windows.Documents.Model.Paragraph createdOnParagraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span createdOnSpan = new Telerik.Windows.Documents.Model.Span("Created on: " + CreationDate);
            createdOnParagraph.Inlines.Add(createdOnSpan);
            createdOn.Children.Add(createdOnParagraph);

            headerRow.Cells.Add(securityCell);
            headerRow.Cells.AddAfter(securityCell, countryCell);
            headerRow.Cells.AddAfter(countryCell, createdByCell);
            headerRow.Cells.AddAfter(createdByCell, createdOn);
            headerTable.Rows.Add(headerRow);


            Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
            Table documentTable = new Table();
            List<Table> tablesToAddToPage = new List<Table>();
            for (int i = startIndex; i < endIndex; i++)
            {
                if (tables.Count >= i)
                {
                    tablesToAddToPage.Add(tables[i]);
                }
            }

            documentTable = GenerateCombinedTable(tablesToAddToPage);
            section.Blocks.Add(headerTable);
            section.Blocks.Add(documentTable);

            return section;
        }

        /// <summary>
        /// Generate Combined DCF tables
        /// </summary>
        /// <param name="tables">List of Tables</param>
        /// <returns>Combined Table</returns>
        private Table GenerateCombinedTable(List<Table> tables)
        {
            Table documentTable = new Table(tables.Count(), 3);
            for (int i = 0; i < tables.Count; i++)
            {
                if (i == 2 || i == 3)
                {
                    if (i == 2)
                    {
                        TableRow row = GetTableRowForSingleTable(tables[i], 1);
                        TableCell cellEmptyParagraph = new TableCell();
                        cellEmptyParagraph.PreferredWidth = new TableWidthUnit(20);
                        Telerik.Windows.Documents.Model.Paragraph p1 = new Telerik.Windows.Documents.Model.Paragraph();
                        cellEmptyParagraph.Blocks.Add(p1);
                        row.Cells.Add(cellEmptyParagraph);
                        documentTable.Rows.Add(row);
                    }
                    else
                    {
                        TableRow row = documentTable.Rows.Last;
                        GetTableRowForMultipleTables(tables[i], ref row);
                    }
                }
                else
                {
                    TableRow row = GetTableRowForSingleTable(tables[i], 3);
                    documentTable.Rows.Add(row);
                }
            }
            documentTable.SetGridColumnWidth(0, new TableWidthUnit(10));
            documentTable.SetGridColumnWidth(1, new TableWidthUnit(1));
            documentTable.SetGridColumnWidth(2, new TableWidthUnit(10));
            return documentTable;
        }

        /// <summary>
        /// Creates table Row when only one table is in the Row
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnSpan"></param>
        /// <returns></returns>
        private TableRow GetTableRowForSingleTable(Table table, int columnSpan)
        {
            TableRow row = new TableRow();
            TableCell cell1 = new TableCell();
            cell1.ColumnSpan = columnSpan;
            cell1.Blocks.Add(table);
            row.Cells.Add(cell1);
            return row;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row"></param>
        private void GetTableRowForMultipleTables(Table table, ref TableRow row)
        {
            TableCell cell1 = new TableCell();
            cell1.Blocks.Add(table);
            row.Cells.Add(cell1);
        }

        /// <summary>
        /// Create Emoty Row
        /// </summary>
        /// <param name="colSpan"></param>
        /// <returns></returns>
        private TableRow GetEmptyRow(int colSpan)
        {
            TableRow emptyRow = new TableRow();
            TableCell cellEmptyParagraph = new TableCell();
            cellEmptyParagraph.ColumnSpan = colSpan;
            Telerik.Windows.Documents.Model.Paragraph p1 = new Telerik.Windows.Documents.Model.Paragraph();
            cellEmptyParagraph.Blocks.Add(p1);

            return emptyRow;
        }

        /// <summary>
        /// Fair Value Store Off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFairValue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (eventAggregator != null)
                {
                    eventAggregator.GetEvent<DCFFairValueSetEvent>().Publish(true);
                }
            }
            catch (Exception ex)
            {
                Logging.LogException(logger, ex);
            }
        }
        #endregion
    }
}