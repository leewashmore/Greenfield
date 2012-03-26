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

namespace GreenField.Gadgets.Helpers
{
    public partial class ChildExportOptions : ChildWindow
    {
        public ChildExportOptions(List<RadExportOptions> exportOptions, string title = "Export Options")
        {
            InitializeComponent();
            ChildWindowTitle = title;            
            ExportOptions = exportOptions;            
        }

        private string _childWindowTitle;
	    public string ChildWindowTitle
	    {
		    get { return _childWindowTitle;}
		    set 
            {
                _childWindowTitle = value;
                this.Title = value;
            }
	    }

        private List<RadExportOptions> _exportOptions;
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
            this.OKButton.IsEnabled = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = ".xls",
                Filter = RadExportFilterOptionDesc.GetEnumDescription(ExportOption.ExportFilterOption)
            };

            if (dialog.ShowDialog() == true)
            {
                RadExport.ExportStream(dialog.FilterIndex, ExportOption, dialog.OpenFile());
                this.DialogResult = true;
            }
            else
            {
                this.DialogResult = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

