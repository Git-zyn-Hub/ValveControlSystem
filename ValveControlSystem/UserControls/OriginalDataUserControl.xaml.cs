using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// OriginalDataUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class OriginalDataUserControl : UserControl
    {
        public StackPanel StpContainer { get; set; }
        private DockPanel _dcpInfo;
        private bool _stopScroll = false;
        public OriginalDataUserControl()
        {
            InitializeComponent();
            StpContainer = this.stpContainer;
        }

        public void AddSendData(byte[] sendData)
        {
            _dcpInfo = new DockPanel();
            TextBlock txtSendData = new TextBlock();
            txtSendData.Text = "发送->";
            txtSendData.Text += System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            txtSendData.Text += " 长度:" + sendData.Length.ToString();
            txtSendData.FontSize = 15;
            txtSendData.Padding = new Thickness(3);
            _dcpInfo.Children.Add(txtSendData);
            this.stpContainer.Children.Add(_dcpInfo);

            int comeInCount = 0;
            for (int i = 0; i < sendData.Length; i += 16)
            {
                int totalTime = sendData.Length / 16;
                comeInCount++;
                TextBlock oneSendDataLine = new TextBlock();
                oneSendDataLine.FontSize = 15;
                oneSendDataLine.Padding = new Thickness(3);
                oneSendDataLine.Text = "0x" + i.ToString("X2") + "   ";
                if (comeInCount - 1 < totalTime)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        oneSendDataLine.Text += "0x" + sendData[(comeInCount - 1) * 16 + j].ToString("X2") + ",";
                    }
                }
                else if (comeInCount - 1 == totalTime)
                {
                    for (int j = 0; j < (sendData.Length - totalTime * 16); j++)
                    {
                        oneSendDataLine.Text += "0x" + sendData[(comeInCount - 1) * 16 + j].ToString("X2") + ",";
                    }
                }

                this.stpContainer.Children.Add(oneSendDataLine);
            }
            scrollControl();
        }

        public void AddReceiveData(byte[] recvData, int dataLength)
        {
            _dcpInfo = new DockPanel();
            TextBlock txtSendData = new TextBlock();
            txtSendData.Text = "接收<-";
            txtSendData.Text += System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            txtSendData.Text += " 长度:" + dataLength.ToString();
            txtSendData.FontSize = 15;
            txtSendData.Padding = new Thickness(3);
            _dcpInfo.Children.Add(txtSendData);
            this.stpContainer.Children.Add(_dcpInfo);

            int comeInCount = 0;
            for (int i = 0; i < dataLength; i += 16)
            {
                int totalTime = dataLength / 16;
                comeInCount++;
                TextBlock oneSendDataLine = new TextBlock();
                oneSendDataLine.FontSize = 15;
                oneSendDataLine.Padding = new Thickness(3);
                oneSendDataLine.Text = "0x" + i.ToString("X2") + "   ";
                if (comeInCount - 1 < totalTime)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        oneSendDataLine.Text += "0x" + recvData[(comeInCount - 1) * 16 + j].ToString("X2") + ",";
                    }
                }
                else if (comeInCount - 1 == totalTime)
                {
                    for (int j = 0; j < (dataLength - totalTime * 16); j++)
                    {
                        oneSendDataLine.Text += "0x" + recvData[(comeInCount - 1) * 16 + j].ToString("X2") + ",";
                    }
                }

                this.stpContainer.Children.Add(oneSendDataLine);
                //只存储20条数据
                //while (this.stpContainer.Children.Count > 20)
                //{
                //    this.stpContainer.Children.RemoveAt(0);
                //}
            }
            scrollControl();
        }

        public void AddDataInfo(string info, DataLevel dataLevel)
        {
            try
            {
                TextBlock txtInfo = new TextBlock();
                switch (dataLevel)
                {
                    case DataLevel.Default:
                        txtInfo.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                    case DataLevel.Normal:
                        txtInfo.Foreground = new SolidColorBrush(Colors.Green);
                        break;
                    case DataLevel.Warning:
                        txtInfo.Foreground = new SolidColorBrush(Colors.Orange);
                        break;
                    case DataLevel.Error:
                        txtInfo.Foreground = new SolidColorBrush(Colors.Red);
                        break;
                    default:
                        txtInfo.Foreground = new SolidColorBrush(Colors.Black);
                        break;
                }
                txtInfo.Text = info;
                txtInfo.FontSize = 16;
                txtInfo.Margin = new Thickness(10, 3, 0, 0);
                _dcpInfo.Children.Add(txtInfo);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void scrollControl()
        {
            if (!_stopScroll)
            {
                this.scrollViewer.ScrollToEnd();
            }
        }

        private void tbtnPin_Checked(object sender, RoutedEventArgs e)
        {
            _stopScroll = true;
        }

        private void tbtnPin_Unchecked(object sender, RoutedEventArgs e)
        {
            _stopScroll = false;
        }
        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            if (b != null)
            {
                b.BorderThickness = new Thickness(0);
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ToggleButton b = sender as ToggleButton;
            if (b != null)
            {
                b.BorderThickness = new Thickness(1);
            }
        }

        public void ClearPanel()
        {
            this.stpContainer.Children.Clear();
        }
    }
}
