using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ValveControlSystem.UserControls
{
    /// <summary>
    /// SettingUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PortSettingUserControl : UserControl
    {
        private ObservableCollection<string> _serialPortItems = new ObservableCollection<string>();
        public ObservableCollection<string> SerialPortItems
        {
            get { return _serialPortItems; }
            set
            {
                if (_serialPortItems != value)
                {
                    _serialPortItems = value;
                }
            }
        }

        private MainWindow _mainWindow;
        public PortSettingUserControl()
        {
            InitializeComponent();
        }

        public PortSettingUserControl(MainWindow originalWindow)
        {
            InitializeComponent();
            _mainWindow = originalWindow;
        }

        private void cmbSerialPort_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 256; i++)
            {
                string oneItem = "COM" + i;
                _serialPortItems.Add(oneItem);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_mainWindow.SerialPort.IsOpen)
                {
                    _mainWindow.SerialPort.PortName = (string)this.cmbSerialPort.SelectedValue;
                    int baudRate;
                    if (!int.TryParse((string)this.cmbBaudRate.SelectedValue, out baudRate))
                    {
                        MessageBox.Show("下拉框内容转换失败！");
                    }
                    _mainWindow.SerialPort.BaudRate = baudRate;
                    _mainWindow.SerialPort.Parity = paritySelect((string)this.cmbCheckBit.SelectedValue);

                    int dataBit;
                    if (!int.TryParse((string)this.cmbDataBit.SelectedValue, out dataBit))
                    {
                        MessageBox.Show("下拉框内容转换失败！");
                    }

                    _mainWindow.SerialPort.DataBits = dataBit;
                    _mainWindow.SerialPort.StopBits = stopBitsSelect((string)this.cmbStopBit.SelectedValue);
                    if (_mainWindow.SerialPortInitialize())
                    {
                        _mainWindow.EnablePortConnect();
                    }
                }
                else
                {
                    MessageBox.Show("端口已经打开！");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private Parity paritySelect(string selectParity)
        {
            Parity result = Parity.None;
            switch (selectParity)
            {
                case "NONE":
                    result = Parity.None;
                    break;
                case "ODD":
                    result = Parity.Odd;
                    break;
                case "EVEN":
                    result = Parity.Even;
                    break;
                case "MARK":
                    result = Parity.Mark;
                    break;
                case "SPACE":
                    result = Parity.Space;
                    break;
                default:
                    result = Parity.None;
                    break;
            }

            return result;
        }

        private StopBits stopBitsSelect(string selectStopBits)
        {
            StopBits result;
            switch (selectStopBits)
            {
                case "1":
                    result = StopBits.One;
                    break;
                case "1.5":
                    result = StopBits.OnePointFive;
                    break;
                case "2":
                    result = StopBits.Two;
                    break;
                default:
                    result = StopBits.None;
                    break;
            }
            return result;
        }
        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.SettingWinClose();
        }


    }
    public class TestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
