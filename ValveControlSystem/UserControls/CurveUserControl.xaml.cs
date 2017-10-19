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
        private const int _headerLength = 7;
        private DataPoint[] _dataPointsTemp;
        private DataPoint[] _dataPointsPres;

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

        private void changeXAxis(int max)
        {
            if (int.Parse(this.axisX.AxisMaximum.ToString()) != max)
            {
                this.axisX.AxisMaximum = max;
                this.axisX.Interval = max > 200 ? (max / 200 * 10) : 10;
            }
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

        public void HandleData(byte[] dataArray)
        {
            try
            {
                GetTempFromVoltage getDoubleTemp = new GetTempFromVoltage();
                if (dataArray.Length == 237)
                {
                    _dataPointsTemp = new DataPoint[4];
                    _dataPointsPres = new DataPoint[80];
                    changeXAxis(((dataArray[_headerLength] << 8) + dataArray[_headerLength + 1]) * 80);
                    int packageNo = (dataArray[_headerLength + 2] << 8) + dataArray[_headerLength + 3];
                    for (int i = 0; i < 4; i++)
                    {
                        DataPoint dataPointTemperature;
                        dataPointTemperature = new DataPoint();
                        dataPointTemperature.XValue = packageNo * 80 + i * 20;
                        dataPointTemperature.YValue = getDoubleTemp.GetTemperature((dataArray[_headerLength + i * 56 + 44] << 8) + dataArray[_headerLength + i * 56 + 45]);
                        dataPointTemperature.MarkerEnabled = false;
                        _dataPointsTemp[i] = dataPointTemperature;
                        for (int j = 0; j < 40; j++, j++)
                        {
                            DataPoint dataPointPressure;
                            dataPointPressure = new DataPoint();
                            dataPointPressure.XValue = packageNo * 80 + i * 20 + j / 2;
                            dataPointPressure.YValue = (dataArray[_headerLength + i * 56 + j + 4] << 8) + dataArray[_headerLength + i * 56 + j + 5];
                            dataPointPressure.MarkerEnabled = false;
                            _dataPointsPres[i * 20 + j / 2] = dataPointPressure;
                        }
                    }
                    addData(_dataPointsTemp, _dataPointsPres);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("处理回放数据异常：" + ee.Message);
            }
        }

        private void addData(DataPoint[] dataPointsTemp, DataPoint[] dataPointsPres)
        {
            if (dataPointsTemp != null && dataPointsTemp.Length == 4)
            {
                foreach (var tempPoint in dataPointsTemp)
                {
                    _dataSeries2.DataPoints.Add(tempPoint);
                }
            }
            if (dataPointsPres != null && dataPointsPres.Length == 80)
            {
                foreach (var presPoint in dataPointsPres)
                {
                    _dataSeries1.DataPoints.Add(presPoint);
                }
            }
        }

        public void ClearCurve()
        {
            try
            {
                if (_dataSeries1 != null && _dataSeries2 != null)
                {
                    _dataSeries1.DataPoints.Clear();
                    _dataSeries2.DataPoints.Clear();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("清空曲线异常：" + ee.Message);
            }
        }
    }
}
