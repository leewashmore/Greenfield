using System;
using System.Windows;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using System.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewICPresentationNew
    /// </summary>
    public partial class ViewICPresentationNew : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// YTD Abs value
        /// </summary>
        private Decimal valueYTDAbs;

        /// <summary>
        /// YTD ReltoLoc value
        /// </summary>
        private Decimal valueYTDReltoLoc;

        /// <summary>
        /// YTD ReltoEM value
        /// </summary>
        private Decimal valueYTDReltoEM;

        /// <summary>
        /// Value FV Buy
        /// </summary>
        private Decimal valueFVBuy;

        /// <summary>
        /// Value FV Sell
        /// </summary>
        private Decimal valueFVSell;


        /// <summary>
        /// ppt template
        /// </summary>
        private string pptTemplate; 


        #endregion

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelICPresentationNew dataContextViewModelICPresentationNew;
        public ViewModelICPresentationNew DataContextViewModelICPresentationNew
        {
            get { return dataContextViewModelICPresentationNew; }
            set { dataContextViewModelICPresentationNew = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextViewModelICPresentationNew != null)
                {
                    DataContextViewModelICPresentationNew.IsActive = isActive;
                }
                if (value)
                {
                    this.txtbYTDAbsolute.Text = "0.00";
                    this.txtbYTDReltoLoc.Text = "0.00";
                    this.txtbYTDReltoEM.Text = "0.00";
                    DataContextViewModelICPresentationNew.PowerpointTemplate = "Full";
                }
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelICPresentationNew</param>
        public ViewICPresentationNew(ViewModelICPresentationNew dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelICPresentationNew = dataContextSource;
        }
        #endregion        

        #region Event Handlers
        /// <summary>
        /// txtbYTDAbsolute LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbYTDAbsolute_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        /// <summary>
        /// txtbYTDReltoLoc LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbYTDReltoLoc_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        /// <summary>
        /// txtbYTDReltoEM LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtbYTDReltoEM_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        /// <summary>
        /// txtFVBuy LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFVBuy_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }


        /// <summary>
        /// txtFVSell LostFocus event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFVSell_LostFocus(object sender, RoutedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }

        /// <summary>
        /// PPT Template SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void PPTTemplate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }


        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "PowerPoint Presentation (*.pptx)|*.pptx" };
            if (dialog.ShowDialog() == true)
            {
              //  DataContextViewModelCreateUpdatePresentations.DownloadStream = dialog.OpenFile();
                DataContextViewModelICPresentationNew.DownloadStream = dialog.OpenFile();
            }
        } 

        #endregion        

        #region Helper Methods
        /// <summary>
        /// Validates and raises YTD input parameter changes
        /// </summary>
        private void RaiseICPresentationOverviewInfoChanged()
        {

            #region valueFVBuy
            Decimal valueFVBuy;
            if (!Decimal.TryParse(this.txtFVBuy.Text, out valueFVBuy))
            {
                this.txtFVBuy.Text = valueFVBuy.ToString();
                return;
            }
           // valueYTDAbs = Convert.ToDecimal((int)(valueYTDAbs * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtFVBuy.Text = valueFVBuy.ToString();
            this.valueFVBuy = valueFVBuy;
            #endregion


            #region valueFVSell
            Decimal valueFVSell;
            if (!Decimal.TryParse(this.txtFVSell.Text, out valueFVSell))
            {
                this.txtFVSell.Text = valueFVSell.ToString();
                return;
            }
            // valueYTDAbs = Convert.ToDecimal((int)(valueYTDAbs * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtFVSell.Text = valueFVSell.ToString();
            this.valueFVSell = valueFVSell;
            #endregion

            #region FVMeasure
            String valFVMeasure = this.FVMeasure.SelectedValue as String;
            #endregion

            #region valueYTDAbs
            Decimal valueYTDAbs;
            if (!Decimal.TryParse(this.txtbYTDAbsolute.Text, out valueYTDAbs))
            {
                this.txtbYTDAbsolute.Text = valueYTDAbs.ToString();
                return;
            }
            valueYTDAbs = Convert.ToDecimal((int)(valueYTDAbs * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtbYTDAbsolute.Text = valueYTDAbs.ToString();
            this.valueYTDAbs = valueYTDAbs; 
            #endregion

            #region valueYTDReltoLoc
            Decimal valueYTDReltoLoc;
            if (!Decimal.TryParse(this.txtbYTDReltoLoc.Text, out valueYTDReltoLoc))
            {
                this.txtbYTDReltoLoc.Text = valueYTDReltoLoc.ToString();
                return;
            }
            valueYTDReltoLoc = Convert.ToDecimal((int)(valueYTDReltoLoc * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtbYTDReltoLoc.Text = valueYTDReltoLoc.ToString();
            this.valueYTDReltoLoc = valueYTDReltoLoc; 
            #endregion

            #region valueYTDReltoEM
            Decimal valueYTDReltoEM;
            if (!Decimal.TryParse(this.txtbYTDReltoEM.Text, out valueYTDReltoEM))
            {
                this.txtbYTDReltoEM.Text = valueYTDReltoEM.ToString();
                return;
            }
            valueYTDReltoEM = Convert.ToDecimal((int)(valueYTDReltoEM * Convert.ToDecimal(100))) / Convert.ToDecimal(100);
            this.txtbYTDReltoEM.Text = valueYTDReltoEM.ToString();
            this.valueYTDReltoEM = valueYTDReltoEM;


            this.pptTemplate = (string)PPTTemplate.SelectedItem;

            #endregion

            DataContextViewModelICPresentationNew.RaiseICPresentationOverviewInfoChanged(valueYTDAbs, valueYTDReltoLoc, valueYTDReltoEM, valueFVBuy, valueFVSell, valFVMeasure,pptTemplate);
        }


        /// <summary>
        /// cbVoteType SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void FVMeasure_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            RaiseICPresentationOverviewInfoChanged();
        }


        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelICPresentationNew.Dispose();
            this.DataContextViewModelICPresentationNew = null;
            this.DataContext = null;
        }
        #endregion        
    }
}
