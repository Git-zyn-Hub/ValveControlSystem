using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ValveControlSystem.Classes;
using ValveControlSystem.UserControls;
using ValveControlSystem.Windows;
using Floatable = FloatableUserControl;

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
        private CommandType _cmdTypeLastSend;
        private List<Window> _childrenWindow = new List<Window>();
        private OriginalDataUserControl _originData = new OriginalDataUserControl();
        private DataTableUserControl _dataTable = new DataTableUserControl();
        private CurveUserControl _curve = new CurveUserControl();
        private List<Floatable.FloatableUserControl> _floatUserCtrlList = new List<Floatable.FloatableUserControl>();
        private double _rowDataTableAndOriginDataHeight;
        private ToolNo _toolNoSetted = ToolNo.Undefined;

        public List<Window> ChildrenWindow
        {
            get
            {
                return _childrenWindow;
            }

            set
            {
                _childrenWindow = value;
            }
        }
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

                    //string strTest = string.Empty;
                    //for (int i = 0; i < bytesActualRecv.Length; i++)
                    //{
                    //    strTest += bytesActualRecv[i].ToString("X2");
                    //}
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        //this.txtTestReceive.Text = strTest;
                        this._originData.AddReceiveData(bytesActualRecv, bytesActualRecv.Length);
                    }));
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

                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        handleReceivedData(bytesActualRecv);
                    }));
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Socket接收线程异常:编号" + ex.ErrorCode + "," + ex.Message);
                switch (ex.ErrorCode)
                {
                    case 10053:
                    case 10054:
                        break;
                    default:
                        MessageBox.Show("发生未处理异常！");
                        break;
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DisableEthernetConnect();
                }));
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
                Thread.Sleep(800);
                byte[] buffer = new byte[4096];
                int bytesCount = _serialPort1.Read(buffer, 0, 4096);

                int checkSum;
                byte[] receivedData = new byte[bytesCount];
                for (int i = 0; i < bytesCount; i++)
                {
                    receivedData[i] = buffer[i];
                }

                //string strTest = string.Empty;
                //for (int i = 0; i < receivedData.Length; i++)
                //{
                //    strTest += receivedData[i].ToString("X2");
                //}
                this.Dispatcher.Invoke(new Action(() =>
                {
                    //this.txtTestReceive.Text = strTest;
                    this._originData.AddReceiveData(receivedData, receivedData.Length);
                }));

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
                this.Dispatcher.Invoke(new Action(() =>
                {
                    handleReceivedData(receivedData);
                }));
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
                                //SelectToolNoWindow theSelectToolNoWin = new SelectToolNoWindow();
                                //theSelectToolNoWin.Owner = this;
                                //theSelectToolNoWin.ShowDialog();
                                //if (theSelectToolNoWin.DialogResult.Value)
                                //{
                                byte[] sendData = _sendDataPackage.PackageSendData(CommandTypeCommon.回放指令, new byte[1] { 0 });
                                _socketConnect.Send(sendData, SocketFlags.None);

                                this._originData.AddSendData(sendData);
                                this._originData.AddDataInfo("回放指令", DataLevel.Default);
                                //}
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
                                //SelectToolNoWindow theSelectToolNoWin = new SelectToolNoWindow();
                                //theSelectToolNoWin.Owner = this;
                                //theSelectToolNoWin.ShowDialog();
                                //if (theSelectToolNoWin.DialogResult.Value)
                                //{
                                byte[] buffer = _sendDataPackage.PackageSendData(CommandTypeCommon.回放指令, new byte[1] { 0 });
                                _serialPort1.Write(buffer, 0, buffer.Length);

                                this._originData.AddSendData(buffer);
                                this._originData.AddDataInfo("回放指令", DataLevel.Default);
                                //}
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
                _socketListenThread = new Thread(socketListening);
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

        private void miCommand_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //显示版本信息
                string version = "  v" + GetCurrentApplicationVersion();
                this.Title += version;
                //绑定可悬浮窗口关闭事件。
                fucCurve.Closed += FucCurve_Closed;
                fucOriginData.Closed += FucOriginData_Closed;
                fucDataTable.Closed += FucDataTable_Closed;

                this.fucOriginData.GridContainer.Children.Add(_originData);
                this.fucDataTable.GridContainer.Children.Add(_dataTable);
                this.fucCurve.GridContainer.Children.Add(_curve);

                //添加_dataTable的左键单击事件。
                _dataTable.MouseLeftButtonDown += DataTable_MouseLeftButtonDown;
                _curve.MouseLeftButtonDown += Curve_MouseLeftButtonDown;
                //加载27条指令菜单。
                foreach (int cmdNo in Enum.GetValues(typeof(CommandType)))
                {
                    string strName = Enum.GetName(typeof(CommandType), cmdNo);//获取名称
                    CommandType cmdType = (CommandType)Enum.Parse(typeof(CommandType), strName);
                    MenuItem oneMenuItem = new MenuItem();
                    oneMenuItem.Header = CommandType2StringConverter.CommandType2String(cmdType);
                    oneMenuItem.Click += CommandMenuItem_Click;
                    oneMenuItem.Tag = cmdType;
                    this.miCommand.Items.Add(oneMenuItem);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("窗口加载异常：" + ee.Message);
            }
        }

        private void CommandMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_connType == ConnectType.Notconnected)
                {
                    MessageBox.Show("未连接，请先连接！");
                    return;
                }
                if (_toolNoSetted == ToolNo.Undefined)
                {
                    MessageBox.Show("请先进行地面预设！");
                    return;
                }
                MenuItem miCommandSender = sender as MenuItem;
                //ToolNo toolNoSend = ToolNo.Undefined;
                //if (first2WordsIsGongJu(miCommandSender.Tag.ToString()))
                //{
                //    toolNoSend = _toolNoSetted;
                //}
                //else
                //{
                //    toolNoSend = ToolNo.None;
                //}
                CommandType cmdType = (CommandType)Enum.Parse(typeof(CommandType), miCommandSender.Tag.ToString());
                byte[] sendData = _sendDataPackage.PackageSendData((byte)_toolNoSetted, cmdType);

                switch (_connType)
                {
                    case ConnectType.Notconnected:
                        return;
                    case ConnectType.Ethernet:
                        {
                            if (_socketConnect != null)
                            {
                                _socketConnect.Send(sendData, SocketFlags.None);
                            }
                            else
                            {
                                MessageBox.Show("Socket为空，请检查‘以太网’是否连接！");
                                return;
                            }
                        }
                        break;
                    case ConnectType.SerialPort:
                        {
                            if (_serialPort1.IsOpen)
                            {
                                _serialPort1.Write(sendData, 0, sendData.Length);
                            }
                            else
                            {
                                MessageBox.Show("串口已经关闭。");
                                return;
                            }
                        }
                        break;
                    default:
                        return;
                }

                //string strTest = string.Empty;
                //for (int i = 0; i < sendData.Length; i++)
                //{
                //    strTest += sendData[i].ToString("X2");
                //}
                string dataInfo = CommandType2StringConverter.CommandType2StringWithNo(cmdType);
                _cmdTypeLastSend = cmdType;
                this.Dispatcher.Invoke(new Action(() =>
                {
                    //this.txtTestSend.Text = strTest;
                    this._originData.AddSendData(sendData);
                    this._originData.AddDataInfo(dataInfo, DataLevel.Default);
                }));
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Socket发送异常:编号" + ex.ErrorCode + "," + ex.Message);
                switch (ex.ErrorCode)
                {
                    case 10054:
                        break;
                    default:
                        MessageBox.Show("发生未处理异常！");
                        break;
                }
                this.Dispatcher.Invoke(new Action(() =>
                {
                    DisableEthernetConnect();
                }));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private int? getFirstNumInString(string str)
        {

            char[] charArray = str.ToCharArray();
            int num;
            for (int i = 0; i < charArray.Length; i++)
            {
                if (!int.TryParse(charArray[i].ToString(), out num))
                {
                    continue;
                }
                else
                {
                    return num;
                }
            }
            return null;
        }

        private bool first2WordsIsGongJu(string input)
        {
            string sub = input.Substring(0, 2);
            return sub == "工具" ? true : false;
        }

        private void handleReceivedData(byte[] receivedData)
        {
            if (receivedData[0] == 0xff && receivedData[1] == 0 && receivedData[2] == 0xaa && receivedData[3] == 0x55)
            {
                if (receivedData[4] == 0x00)
                {
                    //指令类型不能为3，因为已经被“状态异常”占用。
                    if (receivedData[5] == 3 && receivedData[6] == (byte)CommandState.状态异常)
                    {
                        string receiveDataInfo = CommandType2StringConverter.CommandType2StringWithNo(_cmdTypeLastSend) + " ";
                        receiveDataInfo += CommandState.状态异常.ToString();
                        this._originData.AddDataInfo(receiveDataInfo, DataLevel.Error);
                    }
                    else
                    {
                        switch (receivedData[5])
                        {
                            case (byte)CommandTypeCommon.普通指令:
                                {
                                    if (receivedData[6] == 5 && receivedData[9] == (byte)CommandState.状态正常)
                                    {
                                        CommandType cmdType = (CommandType)receivedData[8];
                                        string receiveDataInfo = CommandType2StringConverter.CommandType2StringWithNo(cmdType) + " ";
                                        receiveDataInfo += CommandState.状态正常.ToString();
                                        this._originData.AddDataInfo(receiveDataInfo, DataLevel.Normal);
                                    }
                                }
                                break;
                            case (byte)CommandTypeCommon.回放指令:
                                {
                                    _dataTable.ClearTable();
                                    _curve.ClearCurve();
                                    _dataTable.HandleData(receivedData);
                                    _curve.HandleData(receivedData);
                                    this._originData.AddDataInfo("回放数据", DataLevel.Default);
                                }
                                break;
                            case (byte)CommandTypeCommon.擦除指令:
                                {
                                    string receiveDataInfo = "指令 擦除Flash ";
                                    receiveDataInfo += CommandState.状态正常.ToString();
                                    this._originData.AddDataInfo(receiveDataInfo, DataLevel.Normal);
                                }
                                break;
                            case (byte)CommandTypeCommon.地面预设指令:
                                {
                                    if (receivedData[9] == (byte)CommandState.状态正常)
                                    {
                                        string receiveDataInfo = "地面预设指令 ";
                                        receiveDataInfo += CommandState.状态正常.ToString();
                                        this._originData.AddDataInfo(receiveDataInfo, DataLevel.Normal);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    //else
                    //{
                    //    this.originalData.AddDataInfo("回送的状态位错误或长度错误", DataLevel.Error);
                    //}
                }
                else
                {
                    MessageBox.Show("接收数据方向错误！");
                }
            }
            else
            {
                MessageBox.Show("接收数据帧头错误！");
            }
        }

        private void miCurve_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _floatUserCtrlList)
            {
                if (item.Name == "fucCurve")
                {
                    item.AddControlAndSetGrid();
                    item.State = Floatable.UserControlState.Dock;
                    _floatUserCtrlList.Remove(item);
                    break;
                }
            }
            this.fucCurve.FocusTitleRect();
        }

        private void miOriginData_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _floatUserCtrlList)
            {
                if (item.Name == "fucOriginData")
                {
                    item.AddControlAndSetGrid();
                    item.State = Floatable.UserControlState.Dock;
                    _floatUserCtrlList.Remove(item);
                    break;
                }
            }
            if (this.rowDataTableAndOriginData.Height.Value == 0)
            {
                this.rowDataTableAndOriginData.Height = new GridLength(_rowDataTableAndOriginDataHeight, GridUnitType.Star);
            }
            this.fucOriginData.FocusTitleRect();
        }

        private void miDataTable_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in _floatUserCtrlList)
            {
                if (item.Name == "fucDataTable")
                {
                    item.AddControlAndSetGrid();
                    item.State = FloatableUserControl.UserControlState.Dock;
                    _floatUserCtrlList.Remove(item);
                    break;
                }
            }
            if (this.rowDataTableAndOriginData.Height.Value == 0)
            {
                this.rowDataTableAndOriginData.Height = new GridLength(_rowDataTableAndOriginDataHeight, GridUnitType.Star);
            }
            this.fucDataTable.FocusTitleRect();
        }
        private void miCurveSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurveSetWindow newCurveSetWin = new CurveSetWindow(this._curve);
                newCurveSetWin.Owner = this;
                newCurveSetWin.SetCurve += _curve.SetCurveColorAndLineThickness;
                newCurveSetWin.ShowDialog();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void miTimeSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_connType == ConnectType.Notconnected)
                {
                    MessageBox.Show("未连接，请先连接！");
                    return;
                }
                DateTime now = DateTime.Now;
                byte[] nowContent = new byte[3] { (byte)now.Hour, (byte)now.Minute, (byte)now.Second };
                byte[] sendData = _sendDataPackage.PackageSendData(CommandTypeCommon.对时指令, nowContent);

                Send(sendData);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this._originData.AddSendData(sendData);
                    this._originData.AddDataInfo("对时 " + now.ToString("HH:mm:ss"), DataLevel.Default);
                }));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void FucCurve_Closed()
        {
            if (!_floatUserCtrlList.Contains(fucCurve))
            {
                _floatUserCtrlList.Add(fucCurve);
            }
        }

        private void FucOriginData_Closed()
        {
            if (!_floatUserCtrlList.Contains(fucOriginData))
            {
                _floatUserCtrlList.Add(fucOriginData);
            }
            if (this.gridDataTableAndOriginData.Children.Count == 1)
            {
                _rowDataTableAndOriginDataHeight = this.rowDataTableAndOriginData.ActualHeight;
                this.rowDataTableAndOriginData.Height = new GridLength(0);
            }
        }

        private void FucDataTable_Closed()
        {
            if (!_floatUserCtrlList.Contains(fucDataTable))
            {
                _floatUserCtrlList.Add(fucDataTable);
            }
            if (this.gridDataTableAndOriginData.Children.Count == 1)
            {
                _rowDataTableAndOriginDataHeight = this.rowDataTableAndOriginData.ActualHeight;
                this.rowDataTableAndOriginData.Height = new GridLength(0);
            }
        }
        private void DataTable_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.fucDataTable.FocusTitleRect();
        }

        private void Curve_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.fucCurve.FocusTitleRect();
        }

        private void Send(byte[] sendData)
        {
            switch (_connType)
            {
                case ConnectType.Notconnected:
                    return;
                case ConnectType.Ethernet:
                    {
                        if (_socketConnect != null)
                        {
                            _socketConnect.Send(sendData, SocketFlags.None);
                        }
                        else
                        {
                            MessageBox.Show("Socket为空，请检查‘以太网’是否连接！");
                            return;
                        }
                    }
                    break;
                case ConnectType.SerialPort:
                    {
                        if (_serialPort1.IsOpen)
                        {
                            _serialPort1.Write(sendData, 0, sendData.Length);
                        }
                        else
                        {
                            MessageBox.Show("串口已经关闭。");
                            return;
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        private string GetCurrentApplicationVersion()
        {

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            string versionStr = string.Format(" {0}.{1}.{2}", fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart);
            return versionStr;
        }

        private void miEraseFlash_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_connType == ConnectType.Notconnected)
                {
                    MessageBox.Show("未连接，请先连接！");
                    return;
                }
                byte[] sendData = _sendDataPackage.PackageSendData(CommandTypeCommon.擦除指令, new byte[2] { 0, 0x28 });

                Send(sendData);
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this._originData.AddSendData(sendData);
                    this._originData.AddDataInfo("擦除Flash", DataLevel.Default);
                }));
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void miSurfacePreset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_connType == ConnectType.Notconnected)
                {
                    MessageBox.Show("未连接，请先连接！");
                    return;
                }
                SurfacePresetWindow presetWin = new SurfacePresetWindow();
                bool? dialogResult = presetWin.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    this._toolNoSetted = presetWin.ToolNoSet;
                    byte[] bytesAutomaticClosurePressure = convertInt2Bytes(presetWin.SurfacePrs.AutomaticClosurePressure);
                    byte[] bytesAVS_TriggerPressure = convertInt2Bytes(presetWin.SurfacePrs.AVS_TriggerPressure);
                    byte[] bytesAVS4UnderPressureLimit = convertInt2Bytes(presetWin.SurfacePrs.AVS4UnderPressureLimit);
                    byte[] bytesAVS4OverPressureLimit = convertInt2Bytes(presetWin.SurfacePrs.AVS4OverPressureLimit);
                    byte[] bytesSUD_Setting = convertInt2Bytes(presetWin.SurfacePrs.SUD_Setting);

                    byte[] content = new byte[17]
                    {
                        (byte)presetWin.SurfacePrs.AutomaticClosureValve,
                        bytesAutomaticClosurePressure[0],
                        bytesAutomaticClosurePressure[1],
                        (byte)presetWin.SurfacePrs.AVS_A_Option,
                        bytesAVS_TriggerPressure[0],
                        bytesAVS_TriggerPressure[1],
                        (byte)presetWin.SurfacePrs.AVS_B_Option,
                        (byte)presetWin.SurfacePrs.AVS4TimeLimit,
                        bytesAVS4UnderPressureLimit[0],
                        bytesAVS4UnderPressureLimit[1],
                        bytesAVS4OverPressureLimit[0],
                        bytesAVS4OverPressureLimit[1],
                        bytesSUD_Setting[0],
                        bytesSUD_Setting[1],
                        (byte)presetWin.SurfacePrs.ToolNumber,
                        (byte)presetWin.SurfacePrs.CircleValveState,
                        (byte)presetWin.SurfacePrs.TestValveState
                    };
                    byte[] sendData = _sendDataPackage.PackageSendData(CommandTypeCommon.地面预设指令, content);
                    Send(sendData);
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this._originData.AddSendData(sendData);
                        this._originData.AddDataInfo("地面预设指令", DataLevel.Default);
                    }));
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private byte[] convertInt2Bytes(int input)
        {
            byte[] result = new byte[2];

            result[0] = (byte)((input >> 8) & 255);
            result[1] = (byte)(input & 255);

            return result;
        }
    }
}
