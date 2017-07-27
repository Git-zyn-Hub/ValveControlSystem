using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using ValveControlSystem.UserControls;
using ValveControlSystem.Windows;

namespace ValveControlSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _socketConnect;
        private Thread _socketListenThread;
        private Window _container;
        private SerialPort _serialPort1 = new SerialPort();
        private SendDataPackage _sendDataPackage = new SendDataPackage();
        private ConnectType _connType = ConnectType.Notconnected;

        public SerialPort SerialPort
        {
            get { return _serialPort1; }
            set
            {
                if (_serialPort1 != value)
                {
                    _serialPort1 = value;
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _socketListenThread = new Thread(socketListening);
        }

        private void socketListening()
        {
            try
            {
                while (true)
                {
                    byte[] receivedBytes = new byte[4096];
                    int numBytes = _socketConnect.Receive(receivedBytes, SocketFlags.None);

                    int checkSum;
                    byte[] bytesActualRecv = new byte[numBytes];
                    for (int i = 0; i < numBytes; i++)
                    {
                        bytesActualRecv[i] = receivedBytes[i];
                    }
                    //计算校验和
                    checkSum = 0;
                    for (int i = 0; i < bytesActualRecv.Length; i++)
                    {
                        if (i < bytesActualRecv.Length - 2)
                        {
                            checkSum += bytesActualRecv[i];
                        }
                    }
                    if (bytesActualRecv.Length - 2 < 0)
                    {
                        MessageBox.Show("接收数据长度小于2");
                        continue;
                    }
                    if (!checkCheckSum(checkSum, bytesActualRecv))
                    {
                        MessageBox.Show("校验和出错！");
                        continue;
                    }
                    if (bytesActualRecv[0] == 0xff && bytesActualRecv[1] == 0 && bytesActualRecv[2] == 0xaa && bytesActualRecv[3] == 0x55)
                    {
                        if (bytesActualRecv[4] == 0x0f)
                        {
                            if (bytesActualRecv[5] == 0x01)
                            {

                            }
                            else
                            {
                                MessageBox.Show("接收数据类型错误！");
                            }
                        }
                        else
                        {
                            MessageBox.Show("接收数据地址错误！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("接收数据帧头错误！");
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Socket接收线程异常:" + ee.Message);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DisableEthernetConnect();
                }));
            }
        }
        public void SettingWinClose()
        {
            this._container.Close();
            this._container = null;
        }
        public bool SerialPortInitialize()
        {
            if (_serialPort1.IsOpen)
            {
                MessageBox.Show("串口早就打开了有木有!");
                return false;
            }
            else
            {
                try
                {
                    _serialPort1.Open();
                    _serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
                    MessageBox.Show("端口打开！");
                    SettingWinClose();
                    return true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("端口无法打开! " + ee.Message);
                    return false;
                }
            }
        }
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[4096];
                int bytesCount = _serialPort1.Read(buffer, 0, 4096);

                int checkSum;
                byte[] receivedData = new byte[bytesCount];
                for (int i = 0; i < bytesCount; i++)
                {
                    receivedData[i] = buffer[i];
                }

                //计算校验和
                checkSum = 0;
                for (int i = 0; i < receivedData.Length; i++)
                {
                    if (i < receivedData.Length - 2)
                    {
                        checkSum += receivedData[i];
                    }
                }
                if (receivedData.Length - 2 < 0)
                {
                    MessageBox.Show("接收数据长度小于2");
                    return;
                }
                if (!checkCheckSum(checkSum, receivedData))
                {
                    MessageBox.Show("校验和出错！");
                    return;
                }
                if (receivedData[0] == 0xff && receivedData[1] == 0 && receivedData[2] == 0xaa && receivedData[3] == 0x55)
                {
                    if (receivedData[4] == 0x0f)
                    {
                        if (receivedData[5] == 0x01)
                        {

                        }
                        else
                        {
                            MessageBox.Show("接收数据类型错误！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("接收数据地址错误！");
                    }
                }
                else
                {
                    MessageBox.Show("接收数据帧头错误！");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void miReadData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (_connType)
                {
                    case ConnectType.Notconnected:
                        MessageBox.Show("未连接，请先连接！");
                        break;
                    case ConnectType.Ethernet:
                        {
                            if (_socketConnect != null)
                            {
                                SelectToolNoWindow theSelectToolNoWin = new SelectToolNoWindow();
                                theSelectToolNoWin.Owner = this;
                                theSelectToolNoWin.ShowDialog();
                                if (theSelectToolNoWin.DialogResult.Value)
                                {
                                    byte[] sendData = _sendDataPackage.PackageSendData(0xff, 0x01, (byte)theSelectToolNoWin.ToolNo);
                                    _socketConnect.Send(sendData, SocketFlags.None);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Socket为空，请检查‘以太网’是否连接！");
                            }
                        }
                        break;
                    case ConnectType.SerialPort:
                        {
                            if (_serialPort1.IsOpen)
                            {
                                SelectToolNoWindow theSelectToolNoWin = new SelectToolNoWindow();
                                theSelectToolNoWin.Owner = this;
                                theSelectToolNoWin.ShowDialog();
                                if (theSelectToolNoWin.DialogResult.Value)
                                {
                                    byte[] buffer = _sendDataPackage.PackageSendData(0xff, 0x01, (byte)theSelectToolNoWin.ToolNo);
                                    _serialPort1.Write(buffer, 0, buffer.Length);
                                }
                            }
                            else
                            {
                                MessageBox.Show("串口已经关闭。");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void miEthernetConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strIP = "192.168.1.10";
                IPEndPoint groundBoxIP = new IPEndPoint(IPAddress.Parse(strIP), 1032);
                _socketConnect = new Socket(groundBoxIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socketConnect.Connect(groundBoxIP);

                MessageBox.Show("以太网连接成功！");
                EnableEthernetConnect();
                _socketListenThread.Start();
            }
            catch (Exception ee)
            {
                MessageBox.Show("以太网连接异常：" + ee.Message);
            }
        }

        private void miPortConnect_Click(object sender, RoutedEventArgs e)
        {
            PortSettingUserControl newSettings = new PortSettingUserControl(this);
            if (_container == null)
            {
                _container = new Window();
                _container.Height = 300;
                _container.Width = 300;
                _container.Owner = this;
                _container.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                _container.Closed += container_Closed;

                _container.Content = newSettings;
                _container.Show();
            }
        }
        private void container_Closed(object sender, EventArgs e)
        {
            SettingWinClose();
            this.Activate();
        }

        public void EnableEthernetConnect()
        {
            this.miEthernetConnect.Background = new SolidColorBrush(Colors.LightGreen);
            this.miPortConnect.IsEnabled = false;
            _connType = ConnectType.Ethernet;
        }

        public void EnablePortConnect()
        {
            this.miEthernetConnect.IsEnabled = false;
            this.miPortConnect.Background = new SolidColorBrush(Colors.LightGreen);
            _connType = ConnectType.SerialPort;
        }

        public void DisableEthernetConnect()
        {
            this.miEthernetConnect.Background = new SolidColorBrush(Color.FromArgb(0, 0xff, 0xff, 0xff));
            this.miPortConnect.IsEnabled = true;
            _connType = ConnectType.Notconnected;
        }

        public void DisablePortConnect()
        {
            this.miEthernetConnect.IsEnabled = true;
            this.miPortConnect.Background = new SolidColorBrush(Color.FromArgb(0, 0xff, 0xff, 0xff));
            _connType = ConnectType.Notconnected;
        }

        /// <summary>
        /// 检查校验和
        /// </summary>
        /// <param name="checkSum">校验和</param>
        /// <param name="bytesReceived">接收到的全部数据，最后两字节代表校验和</param>
        /// <returns>校验和正确返回true，错误返回false</returns>
        private bool checkCheckSum(int checkSum, byte[] bytesReceived)
        {
            //判断校验和
            int sumHigh;
            int sumLow;
            sumHigh = (checkSum & 0xff00) >> 8;
            sumLow = checkSum & 0xff;
            if (bytesReceived.Length - 2 >= 0)
            {
                if (sumHigh != bytesReceived[bytesReceived.Length - 2] || sumLow != bytesReceived[bytesReceived.Length - 1])
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
