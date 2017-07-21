using System;
using System.Collections.Generic;
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
        private SendDataPackage _sendDataPackage = new SendDataPackage();
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
                MessageBox.Show("Socket接收线程异常" + ee.Message);
            }
        }

        private void miReadData_Click(object sender, RoutedEventArgs e)
        {
            try
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
                    MessageBox.Show("Socket为空，请先连接！");
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void miConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string strIP = "192.168.1.10";
                IPEndPoint groundBoxIP = new IPEndPoint(IPAddress.Parse(strIP), 1032);
                _socketConnect = new Socket(groundBoxIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socketConnect.Connect(groundBoxIP);

                MessageBox.Show("连接成功！");
                this.miConnect.Background = new SolidColorBrush(Colors.LightGreen);
                _socketListenThread.Start();
            }
            catch (Exception ee)
            {
                MessageBox.Show("连接异常：" + ee.Message);
            }
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
    }
}
