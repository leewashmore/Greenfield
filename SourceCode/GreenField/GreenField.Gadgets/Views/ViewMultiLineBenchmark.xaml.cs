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
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMultiLineBenchmark : ViewBaseUserControl
    {
        #region PropertyDeclaration

        private ViewModelMultiLineBenchmark _dataContextMultilineBenchmark;
        public ViewModelMultiLineBenchmark DataContextMultilineBenchmark
        {
            get
            {
                return _dataContextMultilineBenchmark;
            }
            set
            {
                _dataContextMultilineBenchmark = value;
            }
        }


        #endregion

        public ViewMultiLineBenchmark(ViewModelMultiLineBenchmark dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextMultilineBenchmark = dataContextSource;
        }

        public override void Dispose()
        {

        }
    }
}
