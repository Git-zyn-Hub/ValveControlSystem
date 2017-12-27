using ValveControlSystem.Classes;
using ValveControlSystem.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Xml.Linq;
using FastLinkSystem.Classes;

namespace ValveControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for CurveSetWindow.xaml
    /// </summary>
    public partial class CurveSetWindow : Window, INotifyPropertyChanged
    {
        private CurveRealtimeUserControl _curveRealtime;
        private CurveUserControl _curveLookBack;
        private DataSet _myDataSet = new DataSet();
        private ObservableCollection<CurveSetting> _curveSettingList = new ObservableCollection<CurveSetting>();
        private ObservableCollection<int> _curveFontSizeList = new ObservableCollection<int>();
        private CurveGeneralSetting _curveGeneralSet = new CurveGeneralSetting();
        private Rectangle _rect4Binding;
        private string _xmlPath;
        private CurveSetXmlHelper _curveSetXmlHelper = new CurveSetXmlHelper();

        //private List<SolidColorBrush> _colorsBrush = new List<SolidColorBrush>();
        public delegate void SetCurveEventHandler();
        public event SetCurveEventHandler SetCurve;

        public ObservableCollection<CurveSetting> CurveSettingList
        {
            get
            {
                return _curveSettingList;
            }

            set
            {
                _curveSettingList = value;
            }
        }

        public ObservableCollection<int> CurveFontSizeList
        {
            get
            {
                return _curveFontSizeList;
            }

            set
            {
                _curveFontSizeList = value;
            }
        }

        public CurveGeneralSetting CurveGeneralSet
        {
            get
            {
                return _curveGeneralSet;
            }

            set
            {
                if (_curveGeneralSet != value)
                {
                    _curveGeneralSet = value;
                    OnPropertyChanged("CurveGeneralSet");
                }
            }
        }

        public Rectangle Rect4Binding
        {
            get
            {
                return _rect4Binding;
            }

            set
            {
                if (_rect4Binding != value)
                {
                    _rect4Binding = value;
                    CurveGeneralSet.BackgroundColor = (_rect4Binding.Fill as SolidColorBrush).Color;
                    OnPropertyChanged("Rect4Binding");
                }
            }
        }

        public CurveSetWindow(CurveRealtimeUserControl chart,CurveUserControl curveLookBack)
        {
            InitializeComponent();
            this._curveRealtime = chart;
            this._curveLookBack = curveLookBack;
            _xmlPath = System.Environment.CurrentDirectory + @"\Config.xml";
            _curveSetXmlHelper.XmlPath = _xmlPath;
            try
            {
                _curveSetXmlHelper.CurveSettingXmlInitial();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CurveSettingList.Clear();
                CurveSetting curveSetPressureRealtime = _curveSetXmlHelper.GetCurveSetting("PressureCurveRealtime");
                this.CurveSettingList.Add(curveSetPressureRealtime);

                CurveSetting curveSetPressure = _curveSetXmlHelper.GetCurveSetting("PressureCurve");
                this.CurveSettingList.Add(curveSetPressure);

                CurveSetting curveSetTemperature = _curveSetXmlHelper.GetCurveSetting("TemperatureCurve");
                this.CurveSettingList.Add(curveSetTemperature);

                setUnit(curveSetPressure.Unit, curveSetTemperature.Unit);

                CurveGeneralSetting newCGS = _curveSetXmlHelper.GetCurveGeneralSetting();
                this.CurveGeneralSet = newCGS;

                this.CurveFontSizeList.Clear();
                this.cbFontFamily.Items.Clear();
                for (int i = 1; i < 41; i++)
                {
                    this.CurveFontSizeList.Add(i);
                }
                foreach (FontFamily font in Fonts.SystemFontFamilies)
                {
                    this.cbFontFamily.Items.Add(font.Source);
                }

                var backgroundSource = this.cbBackgroundColor.ItemsSource;
                foreach (var item in backgroundSource)
                {
                    Rectangle rect = item as Rectangle;
                    if ((rect.Fill as SolidColorBrush).Color == newCGS.BackgroundColor)
                    {
                        Rect4Binding = rect;
                        break;
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurveSettingList.Count != 3)
                {
                    MessageBox.Show("曲线个数不为3，无法设置。");
                    return;
                }
                File.Delete(_xmlPath);
                _curveSetXmlHelper.CurveSettingXmlInitial();
                this._curveRealtime.SetCurveColorAndLineThickness();
                this._curveRealtime.CurveGeneralSet();
                this._curveLookBack.SetCurveColorAndLineThickness();
                this._curveLookBack.CurveGeneralSet();
                Window_Loaded(sender, e);
                CurveGeneralSetting cgs = _curveSetXmlHelper.GetCurveGeneralSetting();
                this.cbFontSize.SelectedValue = cgs.FontSize;
                this.cbFontFamily.SelectedItem = cgs.FontFamily;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            /*e.Key != Key.Back && e.Key != Key.Decimal && e.Key != Key.OemPeriod && e.Key != Key.Return
            * Key.Back 退格键
            * Key.Return 回车
            * Key.Tab 键
            */
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Return || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入
            try
            {
                TextBox textBox = sender as TextBox;
                TextChange[] change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    //这里只做Double类型转换的检测，如果是Int或者其他类型需要改变num的类型，和TryParse前面类型。
                    int num = 0;
                    if (!int.TryParse(textBox.Text, out num))
                    {
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                        textBox.Select(offset, 0);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void TextBoxDouble_KeyDown(object sender, KeyEventArgs e)
        {
            /*e.Key != Key.Back && e.Key != Key.Decimal && e.Key != Key.OemPeriod && e.Key != Key.Return
             * Key.Back 退格键
             * Key.Return 回车
             * Key.Tab 键
             * Key.Decimal || Key.OemPeriod 句点键
             */
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Return || e.Key == Key.Tab || e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void TextBoxDouble_TextChanged(object sender, TextChangedEventArgs e)
        {
            //屏蔽中文输入和非法字符粘贴输入
            try
            {
                TextBox textBox = sender as TextBox;
                TextChange[] change = new TextChange[e.Changes.Count];
                e.Changes.CopyTo(change, 0);

                int offset = change[0].Offset;
                if (change[0].AddedLength > 0)
                {
                    //这里只做Double类型转换的检测，如果是Int或者其他类型需要改变num的类型，和TryParse前面类型。
                    double num = 0;
                    if (!double.TryParse(textBox.Text, out num))
                    {
                        textBox.Text = textBox.Text.Remove(offset, change[0].AddedLength);
                        textBox.Select(offset, 0);
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurveSettingList.Count == 3)
                {
                    _curveSetXmlHelper.ModifyXmlCurveSettingElement("PressureCurveRealtime", CurveSettingList[0]);
                    _curveSetXmlHelper.ModifyXmlCurveSettingElement("PressureCurve", CurveSettingList[1]);
                    _curveSetXmlHelper.ModifyXmlCurveSettingElement("TemperatureCurve", CurveSettingList[2]);
                    _curveSetXmlHelper.ModifyXmlCurveGeneralSettingElement(CurveGeneralSet);
                }
                else
                {
                    MessageBox.Show("保存到配置文件失败！");
                }
                txtPressureRange_LostFocus(sender, e);
                txtTemperatureRange_LostFocus(sender, e);
                txtPressureThreshold_LostFocus(sender, e);
                if (SetCurve != null)
                {
                    SetCurve();
                }
                this.DialogResult = true;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void cbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbFontSize.SelectedValue != null)
            {
                int fontSize = (int)this.cbFontSize.SelectedValue;
                _curveRealtime.ChangeFontSize(fontSize);
                _curveLookBack.ChangeFontSize(fontSize);
            }
        }

        private void cbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cbFontFamily.SelectedItem != null)
            {
                FontFamily fontFamily = new FontFamily(this.cbFontFamily.SelectedItem.ToString());
                _curveRealtime.ChangeFontFamily(fontFamily);
                _curveLookBack.ChangeFontFamily(fontFamily);
            }
        }

        private void txtPressureRange_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtPressureRange.Text.Trim()))
            {
                double range;
                if (double.TryParse(this.txtPressureRange.Text.Trim(), out range))
                {
                    this._curveRealtime.ChangePressureAxis(range);
                    this._curveLookBack.ChangePressureAxis(range);
                }
                else
                {
                    MessageBox.Show("请在‘压力范围’填写正确的小数！");
                }
            }
            else
            {
                MessageBox.Show("‘压力范围’不能为空！");
            }
        }

        private void txtTemperatureRange_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtTemperatureRange.Text.Trim()))
            {
                int range;
                if (int.TryParse(this.txtTemperatureRange.Text.Trim(), out range))
                {
                    this._curveRealtime.ChangeTemperatureAxis(range);
                    this._curveLookBack.ChangeTemperatureAxis(range);
                }
                else
                {
                    MessageBox.Show("请在‘温度范围’填写正确的整数！");
                }
            }
            else
            {
                MessageBox.Show("‘温度范围’不能为空！");
            }
        }

        private void txtPressureThreshold_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtPressureThreshold.Text.Trim()))
            {
                double integer;
                if (double.TryParse(this.txtPressureThreshold.Text.Trim(), out integer))
                {
                    this._curveRealtime.ChangeTrendLine(integer);
                    this._curveLookBack.ChangeTrendLine(integer);
                }
                else
                {
                    MessageBox.Show("请在‘压力门限’填写正确的整数！");
                }
            }
            else
            {
                MessageBox.Show("‘压力门限’不能为空！");
            }
        }

        private void txtRetainMinutes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtRetainMinutes.Text.Trim()))
            {
                int integer;
                if (int.TryParse(this.txtRetainMinutes.Text.Trim(), out integer))
                {
                    this._curveRealtime.ChangeRetainMinutes(integer);
                }
                else
                {
                    MessageBox.Show("请在‘保留’填写正确的整数！");
                }
            }
            else
            {
                MessageBox.Show("‘保留’不能为空！");
            }
        }

        private void ckbDisplayGrid_Checked(object sender, RoutedEventArgs e)
        {
            this._curveRealtime.ChangeChartGrid(true);
            this._curveLookBack.ChangeChartGrid(true);
        }

        private void ckbDisplayGrid_Unchecked(object sender, RoutedEventArgs e)
        {
            this._curveRealtime.ChangeChartGrid(false);
            this._curveLookBack.ChangeChartGrid(false);
        }

        private void cbBackgroundColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._curveRealtime.ChangeBackground((Color)ColorConverter.ConvertFromString(this.cbBackgroundColor.SelectedValue.ToString()));
            this._curveLookBack.ChangeBackground((Color)ColorConverter.ConvertFromString(this.cbBackgroundColor.SelectedValue.ToString()));
        }

        private void cbBackgroundColor_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void Unit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ComboBox combobox = sender as ComboBox;//触发Change事件的ComboBox
            if (e.RemovedItems == null || e.AddedItems == null || e.RemovedItems.Count == 0 || e.AddedItems.Count == 0)
            {
                return;
            }
            string unit = e.AddedItems[0] as string;//下来框改变值后，新值对应的unit

            //获取当前选中项的索引号
            int num = this.dgCurveSetting.SelectedIndex;

            if (num == -1)
                return;

            //Unit那一列是第4列
            if (num == 0 || num == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    //获取索引为i的行
                    DataGridRow row = this.dgCurveSetting.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                    if (row != null)
                    {
                        ComboBox cb = this.dgCurveSetting.Columns[3].GetCellContent(row) as ComboBox;
                        cb.SelectedItem = unit;
                        BindingExpression be = cb.GetBindingExpression(ComboBox.SelectedItemProperty);
                        be.UpdateSource();
                    }
                }
                this.lblPressureUnit.Content = unit.ToString();
                this.lblPressureThresholdUnit.Content = unit.ToString();
                double pressureRange;
                if (double.TryParse(this.txtPressureRange.Text, out pressureRange))
                {
                    this.txtPressureRange.Text = (Math.Round(DataUnitConvert.PressureUnitConvertEachOther(pressureRange, (PressureUnit)Enum.Parse(typeof(PressureUnit), unit.ToString())), 2)).ToString();
                }
                double pressureThreshold;
                if (double.TryParse(this.txtPressureThreshold.Text, out pressureThreshold))
                {
                    this.txtPressureThreshold.Text = (Math.Round(DataUnitConvert.PressureUnitConvertEachOther(pressureThreshold, (PressureUnit)Enum.Parse(typeof(PressureUnit), unit.ToString())))).ToString();
                }
            }
            if (num == 2)
            {
                this.lblTemperatureUnit.Content = unit;
                UnitConverter unitConverter = new UnitConverter();
                string tempUnit = unitConverter.ConvertBack(unit, null, null, null).ToString();
                int temperatureRange;
                if (int.TryParse(this.txtTemperatureRange.Text, out temperatureRange))
                {
                    this.txtTemperatureRange.Text = (Math.Round(DataUnitConvert.TemperatureUnitConvertEachOther(temperatureRange, (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit), tempUnit.ToString())))).ToString();
                }
            }
        }
        private void setUnit(string preUnit, string tempUnit)
        {
            try
            {
                if (preUnit != null)
                {
                    this.lblPressureUnit.Content = preUnit.ToString();
                    this.lblPressureThresholdUnit.Content = preUnit.ToString();
                }

                if (tempUnit != null)
                {
                    UnitConverter unitConverter = new UnitConverter();
                    string strTempUnit = unitConverter.Convert(tempUnit, null, null, null).ToString();
                    this.lblTemperatureUnit.Content = strTempUnit;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void ckbMoveLeft_Checked(object sender, RoutedEventArgs e)
        {
            this._curveRealtime.ChangeMoveLeft(true);
            this.gridRetainMinutes.Visibility = Visibility.Visible;
        }

        private void ckbMoveLeft_Unchecked(object sender, RoutedEventArgs e)
        {
            this._curveRealtime.ChangeMoveLeft(false);
            this.gridRetainMinutes.Visibility = Visibility.Collapsed;
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
