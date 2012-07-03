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
using Telerik.Windows.Controls.Charting;

namespace GreenField.Gadgets.Views
{
    public partial class ViewPRevenue : ViewBaseUserControl
    {
        public ViewPRevenue(ViewModelPRevenue datacontextSource)
        {
        }
        public ViewPRevenue()
        {
            InitializeComponent();
        }
        private void dgPRevenue_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
        private void chPRevenue_Loaded(object sender, RoutedEventArgs e)
        {
            if (chPRevenue.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPRevenue.DefaultView.ChartLegend.Items[0];
                this.chPRevenue.DefaultView.ChartLegend.Items.Remove(var);
            }
        }

        private void chPRevenue_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelPRevenue != null)
            {
                if ((this.DataContext as ViewModelPRevenue).PRevenuePlottedData != null)
                {
                    //assigning std dev minus as min val
                    decimal _axisYMinVal = (this.DataContext as ViewModelPRevenue).PRevenuePlottedData.Select(a => a.StdDevMinus).FirstOrDefault();
                    decimal _minValPrevenue = Convert.ToInt32((this.DataContext as ViewModelPRevenue).PRevenuePlottedData.OrderBy(a => a.PRevenueVal).Select(a => a.PRevenueVal).FirstOrDefault());
                    decimal _incrementVal = 5.0M;
                    if (_minValPrevenue < _axisYMinVal && _minValPrevenue % 5 != 0 )
                        _axisYMinVal = Convert.ToDecimal(Math.Floor(Convert.ToDouble(_minValPrevenue / 5))) * _incrementVal - _incrementVal;

                    //assigning std dev plus as max val
                    decimal _axisYMaxVal = (this.DataContext as ViewModelPRevenue).PRevenuePlottedData.Select(a => a.StdDevPlus).FirstOrDefault();
                    decimal _maxValPrevenue = Convert.ToInt32((this.DataContext as ViewModelPRevenue).PRevenuePlottedData.OrderByDescending(a => a.PRevenueVal).Select(a => a.PRevenueVal).FirstOrDefault());
                    if (_maxValPrevenue > _axisYMaxVal && _maxValPrevenue % 5 != 0)
                        _axisYMaxVal = Convert.ToDecimal(Math.Floor(Convert.ToDouble(_minValPrevenue / 5))) * _incrementVal + _incrementVal;

                    (this.DataContext as ViewModelPRevenue).AxisYMinValue = _axisYMinVal;
                    (this.DataContext as ViewModelPRevenue).AxisYMaxValue = _axisYMaxVal;

                    //(this.DataContext as ViewModelPRevenue).AxisYMinValue = Convert.ToDecimal((this.DataContext as ViewModelPRevenue).PRevenuePlottedData.OrderBy(a => a.PRevenueVal).Select(a => a.PRevenueVal).FirstOrDefault());
                    //(this.DataContext as ViewModelPRevenue).AxisYMaxValue = Convert.ToDecimal((this.DataContext as ViewModelPRevenue).PRevenuePlottedData.OrderByDescending(record => record.PRevenueVal).Select(a => a.PRevenueVal).FirstOrDefault());
                        
                    int dataCount = (this.DataContext as ViewModelPRevenue).PRevenuePlottedData.Count;
                    if (dataCount != 0)
                    {
                        this.chPRevenue.DefaultView.ChartArea.AxisY.Step = 5.0;
                    }

                }
            }

        }
    }
}
