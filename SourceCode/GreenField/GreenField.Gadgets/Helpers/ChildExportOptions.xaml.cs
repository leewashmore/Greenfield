using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Helpers
{
    public partial class ChildExportOptions : ChildWindow
    {
        #region Constructors
        /// <summary>
        /// Child Window to display export options for Radgridview and RadChart data
        /// </summary>
        /// <param name="exportOptions">List of RadExportOptions objects</param>
        /// <param name="title">(optional) Window Title; default: 'Export Options'</param>
        public ChildExportOptions(List<RadExportOptions> exportOptions, string title)
        {
            InitializeComponent();
            ChildWindowTitle = title;
            ExportOptions = exportOptions;
        }

        /// <summary>
        /// Child Window to display export options for Radgridview and RadChart data
        /// </summary>
        /// <param name="exportOptions">List of RadExportOptions objects</param>
        public ChildExportOptions(List<RadExportOptions> exportOptions)
        {
            InitializeComponent();
            ExportOptions = exportOptions;
        }

        /// <summary>
        /// Child Window to display export options for Radgridview and RadChart data
        /// </summary>
        public ChildExportOptions()
        {
            InitializeComponent();
        }
        #endregion

        /// <summary>
        /// Title
        /// </summary>
        private string _childWindowTitle = "Export Options";
        public string ChildWindowTitle
        {
            get { return _childWindowTitle; }
            set
            {
                _childWindowTitle = value;
                this.Title = value;
            }
        }

        private List<ExportElementOptions> _exportElementOptionsInfo;
        public List<ExportElementOptions> ExportElementOptionsInfo
        {
            get { return _exportElementOptionsInfo; }
            set { _exportElementOptionsInfo = value; }
        }

        private List<ExportElementType> _exportElementTypeInfo;
        public List<ExportElementType> ExportElementTypeInfo
        {
            get { return _exportElementTypeInfo; }
            set { _exportElementTypeInfo = value; }
        }

        private List<ExportFontFamily> _exportFontFamilyInfo;
        public List<ExportFontFamily> ExportFontFamilyInfo
        {
            get { return _exportFontFamilyInfo; }
            set { _exportFontFamilyInfo = value; }
        }

        private List<ExportFontWeight> _exportFontWeightInfo;
        public List<ExportFontWeight> ExportFontWeightInfo
        {
            get { return _exportFontWeightInfo; }
            set { _exportFontWeightInfo = value; }
        }

        private List<ExportTextAlignment> _exportTextAlignmentInfo;
        public List<ExportTextAlignment> ExportTextAlignmentInfo
        {
            get { return _exportTextAlignmentInfo; }
            set { _exportTextAlignmentInfo = value; }
        }

        private List<RadExportOptions> _exportOptions = new List<RadExportOptions>();
        public List<RadExportOptions> ExportOptions
        {
            get { return _exportOptions; }
            set
            {
                _exportOptions = value;
                this.cbExportOptions.ItemsSource = value;
                this.cbExportOptions.DisplayMemberPath = "ElementName";
            }
        }

        public RadExportOptions ExportOption { get; set; }

        void cbExportOptions_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            ExportOption = this.cbExportOptions.SelectedValue as RadExportOptions;
            this.brdOptions.Visibility = ExportOption.ExportFilterOption == RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER ? Visibility.Visible : Visibility.Collapsed;
            InitializeExportElementOptions();
            this.OKButton.IsEnabled = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExportOption.ExportFilterOption == RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ExportOption.RichTextBox.Document = PDFExporter.CreateDocument(ExportOption.Element as RadGridView
                        , ExportOption.SkipColumnDisplayIndex, ExportOption.CellValueOverwrite, ExportOption.ColumnAggregateOverwrite
                        , ExportOption.InitialHeaderBlock);
                }));

                ExportOption.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                ExportOption.RichTextBox.Print(ExportOption.ElementName, Telerik.Windows.Documents.UI.PrintMode.Native);                
                this.DialogResult = true;
            }
            else if (ExportOption.ExportFilterOption == RadExportFilterOption.RADCHART_PRINT_FILTER)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ExportOption.RichTextBox.Document = PDFExporter.PrintChart(ExportOption.Element as RadChart);
                }));

                ExportOption.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                ExportOption.RichTextBox.Print(ExportOption.ElementName, Telerik.Windows.Documents.UI.PrintMode.Native);
                this.DialogResult = true;
            }
            else
            {
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    DefaultExt = ".xls",
                    Filter = RadExportFilterOptionDesc.GetEnumDescription(ExportOption.ExportFilterOption)
                };

                if (dialog.ShowDialog() == true)
                {
                    RadGridView_ElementExport.ExportElementOptions = ExportElementOptionsInfo;
                    RadExport.ExportStream(dialog.FilterIndex, ExportOption, dialog.OpenFile(), ExportOption.SkipColumnDisplayIndex
                        , ExportOption.CellValueOverwrite, ExportOption.ColumnAggregateOverwrite, initialHeaderBlock: ExportOption.InitialHeaderBlock);
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void InitializeExportElementOptions()
        {
            InitializeExportElementOptionsInfo();

            InitializeExportElementTypeInfo();
            this.cbExportElementType.ItemsSource = ExportElementTypeInfo;
            this.cbExportElementType.DisplayMemberPath = "ExportElementTypeTitle";

            if (ExportFontFamilyInfo == null)
            {
                InitializeExportFontFamilyInfo();
            }
            this.cbFontFamily.ItemsSource = ExportFontFamilyInfo;
            this.cbFontFamily.DisplayMemberPath = "ExportFontFamilyTitle";


            if (ExportFontWeightInfo == null)
            {
                InitializeExportFontWeightInfo();
            }
            this.cbFontWeight.ItemsSource = ExportFontWeightInfo;
            this.cbFontWeight.DisplayMemberPath = "ExportFontWeightTitle";

            if (ExportTextAlignmentInfo == null)
            {
                InitializeExportTextAlignmentInfo();
            }
            this.cbTextAllignment.ItemsSource = ExportTextAlignmentInfo;
            this.cbTextAllignment.DisplayMemberPath = "ExportTextAlignmentTitle";   
        }

        private void InitializeExportElementTypeInfo()
        {
            ExportElementTypeInfo = new List<ExportElementType>
            {
                new ExportElementType() { ExportElementTypeTitle = "Column Header", ExportElementTypeValue = ExportElement.HeaderCell },
                new ExportElementType() { ExportElementTypeTitle = "Column Cell", ExportElementTypeValue = ExportElement.Cell },
                new ExportElementType() { ExportElementTypeTitle = "Group Header", ExportElementTypeValue = ExportElement.GroupHeaderCell },
                new ExportElementType() { ExportElementTypeTitle = "Group Indent", ExportElementTypeValue = ExportElement.GroupIndentCell },
                new ExportElementType() { ExportElementTypeTitle = "Group Footer", ExportElementTypeValue = ExportElement.GroupFooterCell },
                new ExportElementType() { ExportElementTypeTitle = "Footer", ExportElementTypeValue = ExportElement.FooterCell }                        
            };
        }

        private void InitializeExportFontFamilyInfo()
        {
            ExportFontFamilyInfo = new List<ExportFontFamily>
            {
                new ExportFontFamily() { ExportFontFamilyTitle = "Arial", ExportFontFamilyValue = new FontFamily("Arial") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Arial Black", ExportFontFamilyValue = new FontFamily("Arial Black") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Comic Sans MS", ExportFontFamilyValue = new FontFamily("Comic Sans MS") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Courier New", ExportFontFamilyValue = new FontFamily("Courier New") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Georgia", ExportFontFamilyValue = new FontFamily("Georgia") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Lucida Grande", ExportFontFamilyValue = new FontFamily("Lucida Grande") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Times New Roman", ExportFontFamilyValue = new FontFamily("Times New Roman") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Trebuchet MS", ExportFontFamilyValue = new FontFamily("Trebuchet MS") },
                new ExportFontFamily() { ExportFontFamilyTitle = "Verdana", ExportFontFamilyValue = new FontFamily("Verdana") },
            };
        }

        private void InitializeExportFontWeightInfo()
        {
            ExportFontWeightInfo = new List<ExportFontWeight>
            {
                new ExportFontWeight() { ExportFontWeightTitle = "Normal", ExportFontWeightValue = FontWeights.Normal },
                new ExportFontWeight() { ExportFontWeightTitle = "Black", ExportFontWeightValue = FontWeights.Black },
                new ExportFontWeight() { ExportFontWeightTitle = "Bold", ExportFontWeightValue = FontWeights.Bold },
                new ExportFontWeight() { ExportFontWeightTitle = "ExtraBlack", ExportFontWeightValue = FontWeights.ExtraBlack },
                new ExportFontWeight() { ExportFontWeightTitle = "ExtraBold", ExportFontWeightValue = FontWeights.ExtraBold },
                new ExportFontWeight() { ExportFontWeightTitle = "ExtraLight", ExportFontWeightValue = FontWeights.ExtraLight },
                new ExportFontWeight() { ExportFontWeightTitle = "Light", ExportFontWeightValue = FontWeights.Light },
                new ExportFontWeight() { ExportFontWeightTitle = "Medium", ExportFontWeightValue = FontWeights.Medium },
                new ExportFontWeight() { ExportFontWeightTitle = "SemiBold", ExportFontWeightValue = FontWeights.SemiBold },
                new ExportFontWeight() { ExportFontWeightTitle = "Thin", ExportFontWeightValue = FontWeights.Thin }
            };
        }

        private void InitializeExportTextAlignmentInfo()
        {
            ExportTextAlignmentInfo = new List<ExportTextAlignment>
            {
                new ExportTextAlignment() { ExportTextAlignmentTitle = "Center", ExportTextAlignmentValue = TextAlignment.Center },
                new ExportTextAlignment() { ExportTextAlignmentTitle = "Justify", ExportTextAlignmentValue = TextAlignment.Justify },
                new ExportTextAlignment() { ExportTextAlignmentTitle = "Left", ExportTextAlignmentValue = TextAlignment.Left },
                new ExportTextAlignment() { ExportTextAlignmentTitle = "Right", ExportTextAlignmentValue = TextAlignment.Right }                        
            };
        }

        private void InitializeExportElementOptionsInfo()
        {
            List<ExportElementOptions> result = new List<ExportElementOptions>();

            result.Add(new ExportElementOptions(ExportElement.HeaderRow));
            result.Add(new ExportElementOptions(ExportElement.HeaderCell, true));
            result.Add(new ExportElementOptions(ExportElement.Row));
            result.Add(new ExportElementOptions(ExportElement.Cell));
            result.Add(new ExportElementOptions(ExportElement.GroupHeaderRow));
            result.Add(new ExportElementOptions(ExportElement.GroupHeaderCell, true));
            result.Add(new ExportElementOptions(ExportElement.GroupIndentCell));
            result.Add(new ExportElementOptions(ExportElement.GroupFooterRow));
            result.Add(new ExportElementOptions(ExportElement.GroupFooterCell, true));
            result.Add(new ExportElementOptions(ExportElement.FooterRow));
            result.Add(new ExportElementOptions(ExportElement.FooterCell, true));

            ExportElementOptionsInfo = result;
        }

        private void cbExportElementType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ExportElementOptions selectedOption = ExportElementOptionsInfo.Where(t => t.ExportElementType == (e.AddedItems[0] as ExportElementType).ExportElementTypeValue).FirstOrDefault();
                this.rcpBackground.SelectedColor = selectedOption.ExportElementBackground;
                this.rcpForeground.SelectedColor = selectedOption.ExportElementForeground;
                this.rnudFontSize.Value = selectedOption.ExportElementFontSize;
                if (ExportFontFamilyInfo == null)
                {
                    InitializeExportFontFamilyInfo();
                }
                this.cbFontFamily.SelectedValue = ExportFontFamilyInfo.Where(t => t.ExportFontFamilyValue.Source == selectedOption.ExportElementFontFamily.Source).FirstOrDefault();
                if (ExportFontWeightInfo == null)
                {
                    InitializeExportFontWeightInfo();
                }
                this.cbFontWeight.SelectedValue = ExportFontWeightInfo.Where(t => t.ExportFontWeightValue == selectedOption.ExportElementFontWeight).FirstOrDefault();
                if (ExportTextAlignmentInfo == null)
                {
                    InitializeExportTextAlignmentInfo();
                }
                this.cbTextAllignment.SelectedValue = ExportTextAlignmentInfo.Where(t => t.ExportTextAlignmentValue == selectedOption.ExportElementTextAlignment).FirstOrDefault(); 
            }
        }

        private void rcpBackground_SelectedColorChanged(object sender, EventArgs e)
        {
            if (cbExportElementType != null)
            {
                ExportElementType exportElementType = this.cbExportElementType.SelectedValue as ExportElementType;
                ExportElementOptionsInfo.Where(t => t.ExportElementType == exportElementType.ExportElementTypeValue).FirstOrDefault().ExportElementBackground = this.rcpBackground.SelectedColor; 
            }
        }

        private void rcpForeground_SelectedColorChanged(object sender, EventArgs e)
        {
            if (cbExportElementType != null)
            {
                ExportElementType exportElementType = this.cbExportElementType.SelectedValue as ExportElementType;
                ExportElementOptionsInfo.Where(t => t.ExportElementType == exportElementType.ExportElementTypeValue).FirstOrDefault().ExportElementForeground = this.rcpForeground.SelectedColor; 
            }
        }

        private void rnudFontSize_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (cbExportElementType != null)
            {
                ExportElementType exportElementType = this.cbExportElementType.SelectedValue as ExportElementType;
                ExportElementOptionsInfo.Where(t => t.ExportElementType == exportElementType.ExportElementTypeValue).FirstOrDefault().ExportElementFontSize = (double)this.rnudFontSize.Value; 
            }
        }

        private void cbFontFamily_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbExportElementType != null)
            {
                ExportElementType exportElementType = this.cbExportElementType.SelectedValue as ExportElementType;
                ExportElementOptionsInfo.Where(t => t.ExportElementType == exportElementType.ExportElementTypeValue).FirstOrDefault().ExportElementFontFamily = ((ExportFontFamily)this.cbFontFamily.SelectedValue).ExportFontFamilyValue; 
            }
        } 

        private void cbFontWeight_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbExportElementType != null)
            {
                ExportElementType exportElementType = this.cbExportElementType.SelectedValue as ExportElementType;
                ExportElementOptionsInfo.Where(t => t.ExportElementType == exportElementType.ExportElementTypeValue).FirstOrDefault().ExportElementFontWeight = ((ExportFontWeight)this.cbFontWeight.SelectedValue).ExportFontWeightValue; 
            }
        }

        private void cbTextAllignment_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbExportElementType != null)
            {
                ExportElementType exportElementType = this.cbExportElementType.SelectedValue as ExportElementType;
                ExportElementOptionsInfo.Where(t => t.ExportElementType == exportElementType.ExportElementTypeValue).FirstOrDefault().ExportElementTextAlignment = ((ExportTextAlignment)this.cbTextAllignment.SelectedValue).ExportTextAlignmentValue; 
            }
        }               
    }
}

