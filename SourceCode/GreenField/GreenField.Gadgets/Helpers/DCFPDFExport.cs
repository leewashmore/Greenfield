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

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Model Class for DCF PDF Export
    /// </summary>
    public class DCFPDFExport
    {
        /// <summary>
        /// Data table for DCF grids
        /// </summary>
        public Table DataTable { get; set; }

        /// <summary>
        /// Security name
        /// </summary>
        private string _securityName;
        public string SecurityName
        {
            get { return _securityName; }
            set { _securityName = value; }
        }

        /// <summary>
        /// name of the country for the Security
        /// </summary>
        private string  _countryName;
        public string  CountryName
        {
            get { return _countryName; }
            set { _countryName = value; }
        }

        /// <summary>
        /// Created By
        /// </summary>
        private string  _createdBy;
        public string  CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }
        

    }
}
