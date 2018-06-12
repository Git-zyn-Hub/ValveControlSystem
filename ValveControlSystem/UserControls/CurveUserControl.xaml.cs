using FastLinkSystem.Classes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ValveControlSystem.Classes;
using Visifire.Charts;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// Interaction logic for CurveUserControl.xaml
    /// </summary>
    public partial class CurveUserControl : UserControl, INotifyPropertyChanged
    {
        //压力曲线
        private DataSeries _dataSeries1;
        //温度曲线
        private DataSeries _dataSeries2;
        //指令采集压力
        private DataSeries _dataSeries3;
        //指令采集温度
        private DataSeries _dataSeries4;
        private bool _showChartGrid;
        private CurveSetXmlHelper _curveSetXmlHelper = new CurveSetXmlHelper();
        private const int _headerLength = 7;
        private DataPoint[] _dataPointsTemp;
        private DataPoint[] _dataPointsPres;
        private int _xAxisMax = 0;
        private string _pressureUnit;
        private string _temperatureUnit;
        private DateTimeXmlHelper _dateTimeXmlHelper = new DateTimeXmlHelper();
        private LookBackManager _lookBackMgr;
        private DispatcherTimer _showLastPageTimer = new DispatcherTimer();
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
                    changePressureAfterUnitChanged();
                }
            }
        }
        public string TemperatureUnit4Binding
        {
            get
            {
                return _temperatureUnit;
            }

            set
            {
                if (_temperatureUnit != value)
                {
                    _temperatureUnit = value;
                    OnPropertyChanged("TemperatureUnit4Binding");
                    changeTemperatureAfterUnitChanged();
                }
            }
        }


        public CurveUserControl()
        {
            try
            {
                InitializeComponent();
                _curveSetXmlHelper.XmlPath = System.Environment.CurrentDirectory + @"\Config.xml";
                _curveSetXmlHelper.CurveSettingXmlInitial();
                _lookBackMgr = LookBackManager.GetInstance();
                _showLastPageTimer.Interval = TimeSpan.FromMinutes(3);
                _showLastPageTimer.Tick += ShowLastPageTimer_Tick;
                if (chartCurve.Series.Count == 0)
                {
                    _dataSeries1 = new DataSeries();
                    chartCurve.Series.Add(_dataSeries1);
                    _dataSeries1.RenderAs = RenderAs.Line;
                    _dataSeries1.XValueType = ChartValueTypes.DateTime;
                    _dataSeries1.AxisYType = AxisTypes.Primary;
                    _dataSeries1.MarkerEnabled = false;
                    _dataSeries1.LegendText = "常规采集压力";
                }
                if (chartCurve.Series.Count == 1)
                {
                    _dataSeries2 = new DataSeries();
                    chartCurve.Series.Add(_dataSeries2);
                    _dataSeries2.RenderAs = RenderAs.Line;
                    _dataSeries2.XValueType = ChartValueTypes.DateTime;
                    _dataSeries2.AxisYType = AxisTypes.Secondary;
                    _dataSeries2.MarkerEnabled = true;
                    _dataSeries2.LegendText = "常规采集温度";
                }
                if (chartCurve.Series.Count == 2)
                {
                    _dataSeries3 = new DataSeries();
                    chartCurve.Series.Add(_dataSeries3);
                    _dataSeries3.RenderAs = RenderAs.Line;
                    _dataSeries3.XValueType = ChartValueTypes.DateTime;
                    _dataSeries3.AxisYType = AxisTypes.Primary;
                    _dataSeries3.MarkerEnabled = false;
                    _dataSeries3.LegendText = "指令采集压力";
                }
                if (chartCurve.Series.Count == 3)
                {
                    _dataSeries4 = new DataSeries();
                    chartCurve.Series.Add(_dataSeries4);
                    _dataSeries4.RenderAs = RenderAs.Line;
                    _dataSeries4.XValueType = ChartValueTypes.DateTime;
                    _dataSeries4.AxisYType = AxisTypes.Secondary;
                    _dataSeries4.MarkerEnabled = true;
                    _dataSeries4.LegendText = "指令采集温度";
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("曲线用户控件初始化异常：" + ee.Message);
            }
        }

        private void ShowLastPageTimer_Tick(object sender, EventArgs e)
        {
            _showLastPageTimer.Stop();
            loadSomeDayDataPoints((_lookBackMgr.PagesTotal - 1) * _lookBackMgr.DayCount1Page,
                    _lookBackMgr.ListCurveNorPres.Count);
            _lookBackMgr.InitialParameters();
        }

        private void resetShowLastPageTimer()
        {
            _showLastPageTimer.Stop();
            _showLastPageTimer.Start();
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
            try
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
                this.axisYTemperature.TitleFontFamily = fontFamily;
                this.axisYTemperature.AxisLabels.FontFamily = fontFamily;
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
                this.axisYTemperature.AxisMaximum = range;
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
                CurveSetting preCurveSetting = _curveSetXmlHelper.GetCurveSetting("PressureCurve");
                CurveSetting tempCurveSetting = _curveSetXmlHelper.GetCurveSetting("TemperatureCurve");
                CurveSetting preCurveCmdSetting = _curveSetXmlHelper.GetCurveSetting("PressureCurveCmd");
                CurveSetting tempCurveCmdSetting = _curveSetXmlHelper.GetCurveSetting("TemperatureCurveCmd");
                setCurveColorAndLineThickness(_dataSeries1, preCurveSetting);
                setCurveColorAndLineThickness(_dataSeries2, tempCurveSetting);
                setCurveColorAndLineThickness(_dataSeries3, preCurveCmdSetting);
                setCurveColorAndLineThickness(_dataSeries4, tempCurveCmdSetting);

                PressureUnit4Binding = preCurveSetting.Unit;
                TemperatureUnit4Binding = tempCurveSetting.Unit;
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
                if (dataArray.Length == 242)
                {
                    DateTime? powerOnTime = _dateTimeXmlHelper.GetPowerOnDateAndTime();
                    if (powerOnTime == null)
                    {
                        return;
                    }
                    if (dataArray[GlobalVariable.INDEX_CMD_OR_NORMAL_FLAG] == 1)
                    {
                        //uint secondFromStart = (uint)(dataArray[235] << 24) + (uint)(dataArray[236] << 16) + (uint)(dataArray[237] << 8) + (uint)(dataArray[238]);
                        int secondFromStart = (dataArray[235] << 24) + (dataArray[236] << 16) + (dataArray[237] << 8) + (dataArray[238]);
                        _dataPointsTemp = new DataPoint[4];
                        _dataPointsPres = new DataPoint[76];
                        int packageNo = (dataArray[_headerLength + 2] << 8) + dataArray[_headerLength + 3];
                        for (int i = 0; i < 4; i++)
                        {
                            DataPoint dataPointTemperature;
                            dataPointTemperature = new DataPoint();
                            dataPointTemperature.XValue = powerOnTime.Value.AddSeconds(secondFromStart - 240 + 240 / 4 * i);
                            dataPointTemperature.YValue = DataUnitConvert.TemperatureUnitConvert(GetTempFromVoltage.GetTemperatureNew(
                                (dataArray[_headerLength + i * 56 + 44] << 8) + dataArray[_headerLength + i * 56 + 45]),
                                (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), TemperatureUnit4Binding));
                            dataPointTemperature.MarkerEnabled = true;
                            _dataPointsTemp[i] = dataPointTemperature;
                            for (int j = 0; j < 38; j++, j++)
                            {
                                DataPoint dataPointPressure;
                                dataPointPressure = new DataPoint();
                                dataPointPressure.XValue = powerOnTime.Value.AddSeconds(secondFromStart - 240 + 240d / 76d * (i * 19 + j / 2));
                                dataPointPressure.YValue = DataUnitConverter.PressureUnitConvert(GetPressureFromVoltage.GetPressure(
                                    (dataArray[_headerLength + i * 56 + j + 6] << 8) + dataArray[_headerLength + i * 56 + j + 7]),
                                    (PressureUnit)Enum.Parse(typeof(PressureUnit), PressureUnit4Binding));
                                dataPointPressure.MarkerEnabled = false;
                                _dataPointsPres[i * 19 + j / 2] = dataPointPressure;
                            }
                        }
                        addData(_dataPointsTemp, _dataPointsPres);
                    }
                    else if (dataArray[GlobalVariable.INDEX_CMD_OR_NORMAL_FLAG] == 2)
                    {
                        int secondFromStart = (dataArray[235] << 24) + (dataArray[236] << 16) + (dataArray[237] << 8) + (dataArray[238]);
                        _dataPointsTemp = new DataPoint[4];
                        _dataPointsPres = new DataPoint[76];
                        int packageNo = (dataArray[_headerLength + 2] << 8) + dataArray[_headerLength + 3];
                        for (int i = 0; i < 4; i++)
                        {
                            DataPoint dataPointTemperature;
                            dataPointTemperature = new DataPoint();
                            dataPointTemperature.XValue = powerOnTime.Value.AddSeconds(secondFromStart - 4 + i);
                            dataPointTemperature.YValue = DataUnitConvert.TemperatureUnitConvert(GetTempFromVoltage.GetTemperatureNew(
                                (dataArray[_headerLength + i * 56 + 44] << 8) + dataArray[_headerLength + i * 56 + 45]),
                                (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), TemperatureUnit4Binding));
                            dataPointTemperature.MarkerEnabled = true;
                            _dataPointsTemp[i] = dataPointTemperature;
                            for (int j = 0; j < 38; j++, j++)
                            {
                                DataPoint dataPointPressure;
                                dataPointPressure = new DataPoint();
                                dataPointPressure.XValue = powerOnTime.Value.AddSeconds(secondFromStart - 4 + 4d / 76d * (i * 19 + j / 2));
                                dataPointPressure.YValue = DataUnitConverter.PressureUnitConvert(GetPressureFromVoltage.GetPressure(
                                    (dataArray[_headerLength + i * 56 + j + 6] << 8) + dataArray[_headerLength + i * 56 + j + 7]),
                                    (PressureUnit)Enum.Parse(typeof(PressureUnit), PressureUnit4Binding));
                                dataPointPressure.MarkerEnabled = false;
                                _dataPointsPres[i * 19 + j / 2] = dataPointPressure;
                            }
                        }
                        addDataCommand(_dataPointsTemp, _dataPointsPres);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("处理回放数据异常：" + ee.Message);
            }
        }

        private void addData(DataPoint[] dataPointsTemp, DataPoint[] dataPointsPres)
        {
            try
            {
                if (dataPointsPres != null && dataPointsPres.Length == 76)
                {
                    foreach (var presPoint in dataPointsPres)
                    {
                        if (!_showLastPageTimer.IsEnabled)
                        {
                            _dataSeries1.DataPoints.Add(presPoint);
                        }
                        _lookBackMgr.SavePointsNorPres(presPoint);
                    }
                }
                if (dataPointsTemp != null && dataPointsTemp.Length == 4)
                {
                    foreach (var tempPoint in dataPointsTemp)
                    {
                        if (!_showLastPageTimer.IsEnabled)
                        {
                            _dataSeries2.DataPoints.Add(tempPoint);
                        }
                        _lookBackMgr.SavePointsNorTemp(tempPoint);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("添加数据异常：" + ee.Message);
            }
        }

        private void addDataCommand(DataPoint[] dataPointsTemp, DataPoint[] dataPointsPres)
        {
            try
            {
                if (dataPointsPres != null && dataPointsPres.Length == 76)
                {
                    foreach (var presPoint in dataPointsPres)
                    {
                        if (!_showLastPageTimer.IsEnabled)
                        {
                            _dataSeries3.DataPoints.Add(presPoint);
                        }
                        _lookBackMgr.SavePointsCmdPres(presPoint);
                    }
                    DataPoint voidPoint = new DataPoint();
                    voidPoint.XValue = ((DateTime)(dataPointsPres[dataPointsPres.Length - 1].XValue)).AddSeconds(1);
                    if (!_showLastPageTimer.IsEnabled)
                    {
                        _dataSeries3.DataPoints.Add(voidPoint);
                    }
                    _lookBackMgr.SavePointsCmdPres(voidPoint);
                }
                if (dataPointsTemp != null && dataPointsTemp.Length == 4)
                {
                    foreach (var tempPoint in dataPointsTemp)
                    {
                        if (!_showLastPageTimer.IsEnabled)
                        {
                            _dataSeries4.DataPoints.Add(tempPoint);
                        }
                        _lookBackMgr.SavePointsCmdTemp(tempPoint);
                    }
                    DataPoint voidPoint = new DataPoint();
                    voidPoint.XValue = ((DateTime)(dataPointsTemp[dataPointsTemp.Length - 1].XValue)).AddSeconds(1);
                    if (!_showLastPageTimer.IsEnabled)
                    {
                        _dataSeries4.DataPoints.Add(voidPoint);
                    }
                    _lookBackMgr.SavePointsCmdTemp(voidPoint);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("添加数据异常：" + ee.Message);
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
                if (_dataSeries3 != null && _dataSeries4 != null)
                {
                    _dataSeries3.DataPoints.Clear();
                    _dataSeries4.DataPoints.Clear();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("清空曲线异常：" + ee.Message);
            }
        }


        private void changePressureAfterUnitChanged()
        {
            if (_dataSeries1 != null)
            {
                foreach (var item in _dataSeries1.DataPoints)
                {
                    item.YValue = DataUnitConvert.PressureUnitConvertEachOther(item.YValue, (PressureUnit)Enum.Parse(typeof(PressureUnit), _pressureUnit));
                }
            }
            if (_dataSeries3 != null)
            {
                foreach (var item in _dataSeries3.DataPoints)
                {
                    item.YValue = DataUnitConvert.PressureUnitConvertEachOther(item.YValue, (PressureUnit)Enum.Parse(typeof(PressureUnit), _pressureUnit));
                }
            }
        }

        private void changeTemperatureAfterUnitChanged()
        {
            if (_dataSeries2 != null)
            {
                foreach (var item in _dataSeries2.DataPoints)
                {
                    item.YValue = DataUnitConvert.TemperatureUnitConvertEachOther(item.YValue, (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), _temperatureUnit));
                }
            }
            if (_dataSeries4 != null)
            {
                foreach (var item in _dataSeries4.DataPoints)
                {
                    item.YValue = DataUnitConvert.TemperatureUnitConvertEachOther(item.YValue, (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), _temperatureUnit));
                }
            }
        }


        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                b.BorderThickness = new Thickness(0);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                b.BorderThickness = new Thickness(1);
            }
        }


        private void btnLookBackFullScreen_Click(object sender, RoutedEventArgs e)
        {
            resetShowLastPageTimer();
            _lookBackMgr.Back();
            while (_dataSeries1.DataPoints.Count > 0)
            {
                _dataSeries1.DataPoints.RemoveAt(0);
            };
            if (_lookBackMgr.PagesTotal == _lookBackMgr.PageCurrent)
            {
                loadSomeDayDataPoints((_lookBackMgr.PageCurrent - 1) * _lookBackMgr.DayCount1Page,
                    _lookBackMgr.ListCurveNorPres.Count);
            }
            else
            {
                loadSomeDayDataPoints((_lookBackMgr.PageCurrent - 1) * _lookBackMgr.DayCount1Page,
                    _lookBackMgr.PageCurrent * _lookBackMgr.DayCount1Page);
            }
            setPageCount(_lookBackMgr.PagesBack, _lookBackMgr.PagesForward);
        }

        private void btnLookForwardFullScreen_Click(object sender, RoutedEventArgs e)
        {
            resetShowLastPageTimer();
            _lookBackMgr.Forward();
            while (_dataSeries1.DataPoints.Count > 0)
            {
                _dataSeries1.DataPoints.RemoveAt(0);
            }
            if (_lookBackMgr.PagesTotal == _lookBackMgr.PageCurrent)
            {
                loadSomeDayDataPoints((_lookBackMgr.PageCurrent - 1) * _lookBackMgr.DayCount1Page,
                    _lookBackMgr.ListCurveNorPres.Count);
            }
            else
            {
                loadSomeDayDataPoints((_lookBackMgr.PageCurrent - 1) * _lookBackMgr.DayCount1Page,
                    _lookBackMgr.PageCurrent * _lookBackMgr.DayCount1Page);
            }
            setPageCount(_lookBackMgr.PagesBack, _lookBackMgr.PagesForward);
        }

        private void loadSomeDayDataPoints(int dayStart, int dayEndPlus1)
        {
            if (dayStart < 0 || dayEndPlus1 > _lookBackMgr.ListCurveNorPres.Count)
            {
                return;
            }
            for (int i = dayStart; i < dayEndPlus1; i++)
            {
                foreach (var item in _lookBackMgr.ListCurveNorPres[i])
                {
                    _dataSeries1.DataPoints.Add(item);
                }
                foreach (var item in _lookBackMgr.ListCurveNorTemp[i])
                {
                    _dataSeries2.DataPoints.Add(item);
                }
                foreach (var item in _lookBackMgr.ListCurveCmdPres[i])
                {
                    _dataSeries3.DataPoints.Add(item);
                }
                foreach (var item in _lookBackMgr.ListCurveCmdTemp[i])
                {
                    _dataSeries4.DataPoints.Add(item);
                }
            }
        }

        private void setPageCount(int back, int forward)
        {
            this.lblBackPageCount.Content = back;
            this.lblForwardPageCount.Content = forward;
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
