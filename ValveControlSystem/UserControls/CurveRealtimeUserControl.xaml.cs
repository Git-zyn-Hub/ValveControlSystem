using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ValveControlSystem.Classes;
using Visifire.Charts;
using FastLinkSystem.Classes;
using System.Reflection;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// Interaction logic for CurveUserControl.xaml
    /// </summary>
    public partial class CurveRealtimeUserControl : UserControl, INotifyPropertyChanged
    {
        //压力曲线
        private DataSeries _dataSeries1;
        private bool _showChartGrid;
        private CurveSetXmlHelper _curveSetXmlHelper = new CurveSetXmlHelper();
        private const int _headerLength = 7;
        private string _pressureUnit;
        private bool _moveLeft;
        private int _retainMinutes = 2000;
        //private int _hitCount = 0;


        public string PressureUnit4Binding
        {
            get
            {
                return _pressureUnit;
            }

            set
            {
                if (_pressureUnit != value)
                {
                    _pressureUnit = value;
                    OnPropertyChanged("PressureUnit4Binding");
                    changeDataAfterUnitChanged();
                }
            }
        }


        public CurveRealtimeUserControl()
        {
            try
            {
                InitializeComponent();
                _curveSetXmlHelper.XmlPath = System.Environment.CurrentDirectory + @"\Config.xml";
                _curveSetXmlHelper.CurveSettingXmlInitial();
                if (chartCurve.Series.Count == 0)
                {
                    _dataSeries1 = new DataSeries();
                    chartCurve.Series.Add(_dataSeries1);
                    _dataSeries1.RenderAs = RenderAs.Line;
                    _dataSeries1.XValueType = ChartValueTypes.DateTime;
                    _dataSeries1.AxisYType = AxisTypes.Primary;
                    _dataSeries1.MarkerEnabled = true;
                    _dataSeries1.LegendText = "压力";
                }
                //setXAxisMinMaxValue();
            }
            catch (Exception ee)
            {
                MessageBox.Show("曲线用户控件初始化异常：" + ee.Message);
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
                if (root != null && root.Children.Count > 14)
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
            try
            {

                this.axisYPressure.TitleFontSize = fontSize;
                this.axisYPressure.AxisLabels.FontSize = fontSize;
                //this._axisXTime.TitleFontSize = fontSize;
                this.axisX.AxisLabels.FontSize = fontSize;
                foreach (var item in this.chartCurve.Legends)
                {
                    item.FontSize = fontSize;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("改变字体大小异常：" + ee.Message);
            }
        }
        public void ChangeFontFamily(FontFamily fontFamily)
        {
            try
            {
                this.axisYPressure.TitleFontFamily = fontFamily;
                this.axisYPressure.AxisLabels.FontFamily = fontFamily;
                //this._axisXTime.TitleFontSize = fontSize;
                this.axisX.AxisLabels.FontFamily = fontFamily;
                foreach (var item in this.chartCurve.Legends)
                {
                    item.FontFamily = fontFamily;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改字体异常：" + ee.Message);
            }
        }

        public void ChangePressureAxis(double range)
        {
            try
            {
                this.axisYPressure.AxisMaximum = range;
                this.axisYPressure.Interval = (int)(range / 8);
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改压力纵坐标范围异常：" + ee.Message);
            }
        }

        private void changeXAxis(int max)
        {
            try
            {
                if (int.Parse(this.axisX.AxisMaximum.ToString()) != max)
                {
                    this.axisX.AxisMaximum = max;
                    this.axisX.Interval = max > 200 ? (max / 200 * 10) : 10;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改横轴范围异常：" + ee.Message);
            }
        }

        public void ChangeTemperatureAxis(int range)
        {
            try
            {
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改温度纵坐标范围异常：" + ee.Message);
            }
        }

        public void ChangeTrendLine(double value)
        {
            try
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
            catch (Exception ee)
            {
                MessageBox.Show("修改趋势线异常：" + ee.Message);
            }
        }
        public void ChangeMoveLeft(bool moveLeft)
        {
            this._moveLeft = moveLeft;
        }

        private void moveLeftControl()
        {
            if (_moveLeft)
            {
                while (_dataSeries1.DataPoints.Count > _retainMinutes)
                {
                    _dataSeries1.DataPoints.RemoveAt(0);
                }
            }
        }

        public void ChangeChartGrid(bool show)
        {
            try
            {
                this.chartGrid.Enabled = show;
                _showChartGrid = show;
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改网格线异常：" + ee.Message);
            }
        }

        public void ChangeBackground(Color color)
        {
            try
            {
                this.chartCurve.Background = new SolidColorBrush(color);
            }
            catch (Exception ee)
            {
                MessageBox.Show("修改背景异常：" + ee.Message);
            }
        }
        public void ChangeRetainMinutes(int minutes)
        {
            _retainMinutes = minutes;
        }

        private void setCurveColorAndLineThickness(DataSeries dataSeries, CurveSetting curveSet)
        {
            try
            {
                dataSeries.Color = new SolidColorBrush(curveSet.LineColor);
                dataSeries.LineThickness = curveSet.LineThickness;
                dataSeries.Enabled = curveSet.Show;
            }
            catch (Exception ee)
            {
                MessageBox.Show("设置颜色和线宽异常：" + ee.Message);
            }
        }

        public void SetCurveColorAndLineThickness()
        {
            try
            {
                CurveSetting preCurveSetting = _curveSetXmlHelper.GetCurveSetting("PressureCurveRealtime");
                setCurveColorAndLineThickness(_dataSeries1, preCurveSetting);

                PressureUnit4Binding = preCurveSetting.Unit;
            }
            catch (Exception ee)
            {
                MessageBox.Show("设置颜色和线宽异常：" + ee.Message);
            }
        }

        public void CurveGeneralSet()
        {
            try
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
            catch (Exception ee)
            {
                MessageBox.Show("曲线通用设置异常：" + ee.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetCurveColorAndLineThickness();
                CurveGeneralSet();
            }
            catch (Exception ee)
            {
                MessageBox.Show("控件加载异常：" + ee.Message);
            }
        }

        public void HandleData(byte[] dataArray)
        {
            try
            {
                if (dataArray.Length == 11)
                {
                    DataPoint dataPointPressure;
                    dataPointPressure = new DataPoint();
                    dataPointPressure.XValue = DateTime.Now;
                    dataPointPressure.YValue = DataUnitConverter.PressureUnitConvert(GetPressureFromVoltage.GetPressure(
                        (dataArray[7] << 8) + dataArray[8]), (PressureUnit)Enum.Parse(typeof(PressureUnit), PressureUnit4Binding));
                    dataPointPressure.MarkerEnabled = true;
                    _dataSeries1.DataPoints.Add(dataPointPressure);
                    moveLeftControl();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("处理回放数据异常：" + ee.Message);
            }
        }

        private void setXAxisMinMaxValue()
        {
            if (_dataSeries1.DataPoints.Count == 0)
            {
                this.axisX.AxisMinimum = DateTime.Now;
                this.axisX.AxisMaximum = DateTime.Now.AddMinutes(30);
            }
        }

        public void ClearCurve()
        {
            try
            {
                if (_dataSeries1 != null)
                {
                    _dataSeries1.DataPoints.Clear();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("清空曲线异常：" + ee.Message);
            }
        }

        private void changeDataAfterUnitChanged()
        {
            if (_dataSeries1 != null)
            {
                foreach (var item in _dataSeries1.DataPoints)
                {
                    item.YValue = DataUnitConvert.PressureUnitConvertEachOther(item.YValue, (PressureUnit)Enum.Parse(typeof(PressureUnit), _pressureUnit));
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
