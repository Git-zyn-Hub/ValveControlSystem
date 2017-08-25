using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ValveControlSystem.Classes;
using Visifire.Charts;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// Interaction logic for CurveUserControl.xaml
    /// </summary>
    public partial class CurveUserControl : UserControl
    {
        //压力曲线
        private DataSeries _dataSeries1;
        //温度曲线
        private DataSeries _dataSeries2;
        private bool _showChartGrid;
        private CurveSetXmlHelper _curveSetXmlHelper = new CurveSetXmlHelper();

        public CurveUserControl()
        {
            InitializeComponent();
            _curveSetXmlHelper.XmlPath = System.Environment.CurrentDirectory + @"\Config.xml";
            _curveSetXmlHelper.CurveSettingXmlInitial();
            if (chartCurve.Series.Count == 0)
            {
                _dataSeries1 = new DataSeries();
                chartCurve.Series.Add(_dataSeries1);
                _dataSeries1.RenderAs = RenderAs.Line;
                _dataSeries1.XValueType = ChartValueTypes.Numeric;
                _dataSeries1.AxisYType = AxisTypes.Primary;
                _dataSeries1.MarkerEnabled = false;
                _dataSeries1.LegendText = "压力";
            }
            if (chartCurve.Series.Count == 1)
            {
                _dataSeries2 = new DataSeries();
                chartCurve.Series.Add(_dataSeries2);
                _dataSeries2.RenderAs = RenderAs.Line;
                _dataSeries2.XValueType = ChartValueTypes.Numeric;
                _dataSeries2.AxisYType = AxisTypes.Secondary;
                _dataSeries2.MarkerEnabled = false;
                _dataSeries2.LegendText = "温度";
            }
        }
        private void lineChart_Rendered(object sender, EventArgs e)
        {
            try
            {
                var c = sender as Chart;
                var legend = c.Legends[0];
                var root = legend.Parent as Grid;
                //移除水印
                if (root != null)
                {
                    root.Children.RemoveAt(14);
                }

                //lineChart.HideIndicator();
                //root.Children.RemoveAt(10);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }


        public void ChangeFontSize(double fontSize)
        {
            this.axisYPressure.TitleFontSize = fontSize;
            this.axisYPressure.AxisLabels.FontSize = fontSize;
            this.axisYTemperature.TitleFontSize = fontSize;
            this.axisYTemperature.AxisLabels.FontSize = fontSize;
            //this._axisXTime.TitleFontSize = fontSize;
            this.axisX.AxisLabels.FontSize = fontSize;
            foreach (var item in this.chartCurve.Legends)
            {
                item.FontSize = fontSize;
            }
        }
        public void ChangeFontFamily(FontFamily fontFamily)
        {
            this.axisYPressure.TitleFontFamily = fontFamily;
            this.axisYPressure.AxisLabels.FontFamily = fontFamily;
            this.axisYTemperature.TitleFontFamily = fontFamily;
            this.axisYTemperature.AxisLabels.FontFamily = fontFamily;
            //this._axisXTime.TitleFontSize = fontSize;
            this.axisX.AxisLabels.FontFamily = fontFamily;
            foreach (var item in this.chartCurve.Legends)
            {
                item.FontFamily = fontFamily;
            }
        }

        public void ChangePressureAxis(int range)
        {
            this.axisYPressure.AxisMaximum = range;
            this.axisYPressure.Interval = range / 8;
        }

        public void ChangeTemperatureAxis(int range)
        {
            this.axisYTemperature.AxisMaximum = range;
        }

        public void ChangeTrendLine(int value)
        {
            if (value == 0)
            {
                this.chartCurve.TrendLines.Clear();
            }
            else
            {
                TrendLine newTrendLine = new TrendLine();
                newTrendLine.Value = value;
                this.chartCurve.TrendLines.Clear();
                this.chartCurve.TrendLines.Add(newTrendLine);
            }
        }

        public void ChangeChartGrid(bool show)
        {
            this.chartGrid.Enabled = show;
            _showChartGrid = show;
        }

        public void ChangeBackground(Color color)
        {
            this.chartCurve.Background = new SolidColorBrush(color);
        }

        private void setCurveColorAndLineThickness(DataSeries dataSeries, CurveSetting curveSet)
        {
            dataSeries.Color = new SolidColorBrush(curveSet.LineColor);
            dataSeries.LineThickness = curveSet.LineThickness;
            dataSeries.Enabled = curveSet.Show;
        }

        public void SetCurveColorAndLineThickness()
        {
            setCurveColorAndLineThickness(_dataSeries1, _curveSetXmlHelper.GetCurveSetting("PressureCurve"));
            setCurveColorAndLineThickness(_dataSeries2, _curveSetXmlHelper.GetCurveSetting("TemperatureCurve"));
        }

        public void CurveGeneralSet()
        {
            CurveGeneralSetting cgs = _curveSetXmlHelper.GetCurveGeneralSetting();
            ChangeFontSize(cgs.FontSize);
            ChangeFontFamily(new FontFamily(cgs.FontFamily));
            ChangePressureAxis(cgs.PressureRange);
            ChangeTemperatureAxis(cgs.TemperatureRange);
            ChangeTrendLine(cgs.PressureThreshold);
            ChangeChartGrid(cgs.DisplayGrid);
            ChangeBackground(cgs.BackgroundColor);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SetCurveColorAndLineThickness();
            CurveGeneralSet();
        }
    }
}
