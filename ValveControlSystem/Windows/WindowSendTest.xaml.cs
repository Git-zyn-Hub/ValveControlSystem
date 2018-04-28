using ValveControlSystem.UserControls;
using System;
using System.Collections.Generic;
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
using ValveControlSystem;

namespace NearBitAnalysis.Windows
{
    /// <summary>
    /// Interaction logic for WindowSendTest.xaml
    /// </summary>
    public partial class WindowSendTest : Window
    {
        private bool _hexSend = false;
        private SplitEnum _splitEnum;
        private MainWindow _mainWindow;
        private OriginalDataUserControl _originData;
        public WindowSendTest(MainWindow originalWindow)
        {
            InitializeComponent();
            _mainWindow = originalWindow;
            _originData = _mainWindow.fucOriginData.GridContainer.Children[0] as OriginalDataUserControl;
        }


        private void rbVoidSplit_Checked(object sender, RoutedEventArgs e)
        {
            _splitEnum = SplitEnum.Void;
        }

        private void rbBlankSplit_Checked(object sender, RoutedEventArgs e)
        {
            _splitEnum = SplitEnum.Blank;
        }

        private void rbDashSplit_Checked(object sender, RoutedEventArgs e)
        {
            _splitEnum = SplitEnum.Dash;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _hexSend = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _hexSend = false;
        }

        private void btnClearSend_Click(object sender, RoutedEventArgs e)
        {
            this.txtSend.Text = String.Empty;
            this.txtSend.IsReadOnly = false;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtSend.Text.Trim() == "")
                {
                    _originData.AddDataInfo("不能发送空字符串！", DataLevel.Error);
                    txtSend.Focus();
                    return;
                }


                if (_hexSend)
                {
                    byte[] sendData = getSendBytesHex();
                    if (sendData != null)
                    {
                        _mainWindow.SendWithoutSave(sendData);
                    }
                }
                else
                {
                    string strSend = this.txtSend.Text;
                    byte[] sendData = Encoding.UTF8.GetBytes(strSend);
                    if (sendData != null)
                    {
                        _mainWindow.SendWithoutSave(sendData);
                    }
                }

                //if (comboBoxClient.SelectedIndex < 0)
                //{
                //    listBoxStatu.Items.Add("请在列表中选择发送对象！");
                //    return;
                //}
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private byte[] getSendBytesHex()
        {
            string strInput = this.txtSend.Text.Trim();
            int length = strInput.Length;
            switch (_splitEnum)
            {
                case SplitEnum.Void:
                    {
                        if (length % 2 != 0)
                        {
                            MessageBox.Show("字符个数必须能被2整除", "出错了", MessageBoxButton.OK, MessageBoxImage.Error);
                            return null;
                        }
                        int count = length / 2;
                        string[] strInputs = new string[count];
                        byte[] byteInput = new byte[count];
                        for (int i = 0; i < length; i++, i++)
                        {
                            strInputs[i / 2] = strInput.Substring(i, 2);
                            byteInput[i / 2] = Convert.ToByte(strInputs[i / 2], 16);
                        }
                        return byteInput;
                    }
                case SplitEnum.Blank:
                case SplitEnum.Dash:
                    {
                        int _length = strInput.Length + 1;
                        int _count = length / 3;
                        string[] strInputs = new string[_count];

                        if (_splitEnum == SplitEnum.Blank)
                        {
                            strInputs = strInput.Split(' ');
                        }
                        else if (_splitEnum == SplitEnum.Dash)
                        {
                            strInputs = strInput.Split('-');
                        }
                        _count = strInputs.Length;
                        byte[] _byteInput = new byte[_count];
                        for (int i = 0; i < _count; i++)
                        {
                            _byteInput[i] = Convert.ToByte(strInputs[i], 16);
                        }
                        return _byteInput;
                    }
                default:
                    return null;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.cbxHexSend.IsChecked = true;
        }
    }

    public enum SplitEnum
    {
        Void, //无分隔符
        Blank,//空格分隔
        Dash  //中划线分隔
    }
}
