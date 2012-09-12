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
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.Common;
using GreenField.DataContracts;
using Telerik.Windows.Controls.Charting;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewScatterGraph : ViewBaseUserControl
    {
        public ViewModelScatterGraph DataContextSource { get; set; }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextSource != null) //DataContext instance
                    DataContextSource.IsActive = _isActive;
            }
        }

        public ViewScatterGraph(ViewModelScatterGraph dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextSource = dataContextSource;
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (this.chScatter.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Chart",
                        Element = this.chScatter
                        ,
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER
                    });

                if (this.dgScatterGraph.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Data",
                        Element = this.dgScatterGraph
                        ,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Scatter Graph");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgScatterGraph.Visibility == Visibility.Visible)
            {
                Flipper.FlipItem(this.dgScatterGraph, this.chScatter);
            }
            else
            {
                Flipper.FlipItem(this.chScatter, this.dgScatterGraph);
            }
        }

        private void dgScatterGraph_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }


        private void ChartArea_ItemToolTipOpening(ItemToolTip2D tooltip, ItemToolTipEventArgs e)
        {
            RatioComparisonData dataPointContext = e.DataPoint.DataItem as RatioComparisonData;
            if (dataPointContext == null)
            {
                tooltip.Content = null;
                return;
            }

            tooltip.Content = dataPointContext.ISSUE_NAME + " ("
                + EnumUtils.GetDescriptionFromEnumValue<ScatterGraphValuationRatio>(DataContextSource.SelectedValuationRatio) + ":" + dataPointContext.VALUATION + ", "
                + EnumUtils.GetDescriptionFromEnumValue<ScatterGraphFinancialRatio>(DataContextSource.SelectedFinancialRatio) + ":" + dataPointContext.FINANCIAL + ")";

        }

        private void chScatter_Loaded(object sender, RoutedEventArgs e)
        {
            //this.chScatter.DefaultView.ChartArea.AxisX.la = EnumUtils.GetDescriptionFromEnumValue<ScatterGraphValuationRatio>(_dataContextSource.SelectedValuationRatio);
            //this.chScatter.DefaultView.ChartArea.AxisY.AxisName = EnumUtils.GetDescriptionFromEnumValue<ScatterGraphFinancialRatio>(_dataContextSource.SelectedFinancialRatio);
        }

        private void chScatter_DataBound(object sender, ChartDataBoundEventArgs e)
        {
            if (DataContextSource.RatioComparisonInfo != null)
            {
                this.chScatter.DefaultView.ChartArea.Annotations.Clear();

                if (DataContextSource.RatioComparisonInfo.Count() != 0)
                {
                    Decimal? financialRatioTotal = DataContextSource.RatioComparisonInfo.Sum(record => record.FINANCIAL);
                    Decimal? financialRatioAverage = financialRatioTotal / DataContextSource.RatioComparisonInfo.Count();

                    Decimal? valuationRatioTotal = DataContextSource.RatioComparisonInfo.Sum(record => record.VALUATION);
                    Decimal? valuationRatioAverage = valuationRatioTotal / DataContextSource.RatioComparisonInfo.Count();

                    this.chaScatter.Annotations.Add(new CustomGridLine() { XIntercept = Convert.ToDouble(valuationRatioAverage), StrokeThickness = 1 });
                    this.chaScatter.Annotations.Add(new CustomGridLine() { YIntercept = Convert.ToDouble(financialRatioAverage), StrokeThickness = 1 });

                    //RatioComparisonData issuerRecord = _dataContextSource.RatioComparisonInfo.Where(record => record.ISSUE_NAME == _dataContextSource.EntitySelectionInfo.LongName
                    //    && record.ISSUER_ID == _dataContextSource.IssuerReferenceInfo.IssuerId).FirstOrDefault();


                }
            }



            //RatioComparisonData issuerRecord = _dataContextSource.RatioComparisonInfo.Where(record => record.ISSUE_NAME == "SampleIssueName15").FirstOrDefault();
            //DataPoint point = new DataPoint() 
            //{
            //    XValue = Convert.ToDouble(issuerRecord.VALUATION), 
            //    YValue = Convert.ToDouble(issuerRecord.FINANCIAL),                
            //};

            //this.chScatter.DefaultView.ChartArea.Annotations.Add(new CustomLine()
            //{
            //    MinX = Convert.ToDouble(issuerRecord.VALUATION) - 0.5,
            //    MinY = Convert.ToDouble(issuerRecord.FINANCIAL) - 0.5,
            //    MaxX = Convert.ToDouble(issuerRecord.VALUATION) + 0.5,
            //    MaxY = Convert.ToDouble(issuerRecord.FINANCIAL) + 0.5,
            //    Stroke = new SolidColorBrush(Color.FromArgb(255, 159, 29, 33)),
            //    StrokeThickness = 1,
            //    Slope = Convert.ToDouble(issuerRecord.FINANCIAL) / Convert.ToDouble(issuerRecord.VALUATION),
            //    YIntercept = 0
            //});


            //ScatterSeriesDefinition scatterSeriesDefinition = new ScatterSeriesDefinition() { ShowItemLabels = false, ShowItemToolTips = false, LegendDisplayMode = LegendDisplayMode.None };
            //ItemMapping xItemMapping = new ItemMapping() { FieldName = "FINANCIAL", DataPointMember = DataPointMember.XValue };
            //ItemMapping yItemMapping = new ItemMapping() { FieldName = "VALUATION", DataPointMember = DataPointMember.YValue };
            //SeriesMapping selectedItemSeriesMapping = new SeriesMapping();
            //selectedItemSeriesMapping.SeriesDefinition = scatterSeriesDefinition;
            //selectedItemSeriesMapping.ItemMappings.Add(xItemMapping);
            //selectedItemSeriesMapping.ItemMappings.Add(yItemMapping);
            //selectedItemSeriesMapping.ItemsSource = issuerRecord;

            //this.chScatter.SeriesMappings.Add(selectedItemSeriesMapping);

        }




    }
}
