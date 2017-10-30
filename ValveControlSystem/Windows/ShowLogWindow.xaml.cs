using ValveControlSystem.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace ValveControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for ShowLogWindow.xaml
    /// </summary>
    public partial class ShowLogWindow : Window
    {
        private static ShowLogWindow _uniqueInstance;
        private DateTime _startTime = new DateTime();
        private DateTime _endTime = new DateTime();
        private ObservableCollection<Log> _logs = new ObservableCollection<Log>();
        private XmlDocument _xmlDoc = new XmlDocument();
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        }
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
            }
        }

        public ObservableCollection<Log> Logs
        {
            get
            {
                return _logs;
            }

            set
            {
                _logs = value;
            }
        }

        private ShowLogWindow()
        {
            InitializeComponent();
        }

        public static ShowLogWindow GetInstance()
        {
            if (_uniqueInstance == null)
            {
                _uniqueInstance = new ShowLogWindow();
            }
            return _uniqueInstance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWin = this.Owner as MainWindow;
                if (mainWin != null)
                {
                    string logPath = System.Environment.CurrentDirectory + @"\Log";
                    DirectoryInfo logWellFolder = new DirectoryInfo(logPath);
                    DirectoryInfo[] yearDirs = logWellFolder.GetDirectories();
                    SortAsFolderName(ref yearDirs);
                    foreach (DirectoryInfo yearDir in yearDirs)
                    {
                        DirectoryInfo[] monthDirs = yearDir.GetDirectories();
                        SortAsFolderName(ref monthDirs);
                        foreach (DirectoryInfo monthDir in monthDirs)
                        {
                            DirectoryInfo[] dayDirs = monthDir.GetDirectories();
                            SortAsFolderName(ref dayDirs);
                            foreach (DirectoryInfo dayDir in dayDirs)
                            {
                                FileInfo[] fileInfo = dayDir.GetFiles();
                                SortAsFileName(ref fileInfo);
                                foreach (FileInfo logFile in fileInfo)
                                {
                                    string fileName = logFile.Name;
                                    DateTime logTime = new DateTime();
                                    IFormatProvider ifp = new CultureInfo("zh-CN", true);
                                    if (DateTime.TryParseExact(fileName.Substring(0, 17), "yyyy-MM-dd HHmmss", ifp, DateTimeStyles.None, out logTime))
                                    {
                                        int resultCompare2StartTime = logTime.CompareTo(this.StartTime);
                                        int resultCompare2EndTime = logTime.CompareTo(this.EndTime);
                                        int resultLastWriteTimeCompare2StartTime = logFile.LastWriteTime.CompareTo(this.StartTime);
                                        if ((resultCompare2StartTime < 0 && resultLastWriteTimeCompare2StartTime >= 0) || (resultCompare2StartTime >= 0 && resultCompare2EndTime <= 0))
                                        {
                                            _xmlDoc.Load(logFile.FullName);
                                            XmlNodeList nodesData = _xmlDoc.SelectNodes("/Log/Datas/Data");
                                            foreach (XmlNode node in nodesData)
                                            {
                                                XmlElement xe = (XmlElement)node;//将子节点类型转换为XmlElement类型 
                                                string strLogTime = xe.GetAttribute("Time");

                                                DateTime tempTime = new DateTime();
                                                if (DateTime.TryParse(strLogTime, out tempTime))
                                                {
                                                    if (tempTime.CompareTo(this.StartTime) < 0)
                                                    {
                                                        continue;
                                                    }
                                                    if (tempTime.CompareTo(this.EndTime) > 0)
                                                    {
                                                        break;
                                                    }
                                                }
                                                string[] data = xe.InnerText.Split('-');
                                                byte[] bytesData = new byte[data.Length];
                                                for (int i = 0; i < data.Length; i++)
                                                {
                                                    bytesData[i] = Convert.ToByte(data[i], 16);
                                                }
                                                for (int i = 0; i < 4; i++)
                                                {
                                                    Log oneLog = new Log();
                                                    oneLog.Time = tempTime;
                                                    AnalyzeLog(ref oneLog, bytesData, i);
                                                    Logs.Add(oneLog);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Logs.Count == 0)
                    {
                        MessageBox.Show("所选时间段不存在数据");
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        /// <summary>
        /// 分析日志，将日志内容解析成Log
        /// </summary>
        /// <param name="oneLog">返回解析好的日志</param>
        /// <param name="logContent">日志内容</param>
        /// <param name="offset">一条日志包含4组Log，表示第几组，可取值0,1,2,3</param>
        private void AnalyzeLog(ref Log oneLog, byte[] bytesData, int offset)
        {
            int[] monitorDataArray = new int[8];
            GetTempFromVoltage getDoubleTemp = new GetTempFromVoltage();


            monitorDataArray[0] = (bytesData[51 + offset * 56] << 8) + bytesData[52 + offset * 56];
            monitorDataArray[1] = (bytesData[53 + offset * 56] << 8) + bytesData[54 + offset * 56];
            monitorDataArray[2] = (bytesData[55 + offset * 56] << 8) + bytesData[56 + offset * 56];
            monitorDataArray[3] = (bytesData[57 + offset * 56] << 8) + bytesData[58 + offset * 56];
            monitorDataArray[4] = (bytesData[59 + offset * 56] << 8) + bytesData[60 + offset * 56];
            monitorDataArray[5] = (bytesData[61 + offset * 56] << 8) + bytesData[62 + offset * 56];
            monitorDataArray[6] = (bytesData[63 + offset * 56] << 8) + bytesData[64 + offset * 56];
            monitorDataArray[7] = (bytesData[65 + offset * 56] << 8) + bytesData[66 + offset * 56];

            oneLog.TotalPackageCount = (bytesData[7] << 8) + bytesData[8];
            oneLog.CurrentPackageNo = (bytesData[9] << 8) + bytesData[10];

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 40; i++, i++)
            {
                sb.Append((bytesData[11 + i + offset * 56] << 8) + bytesData[12 + i + offset * 56]);
                if (i != 38)
                {
                    sb.Append(",");
                }
            }
            oneLog.Pressure20 = sb.ToString();
            oneLog.Temperature = getDoubleTemp.GetTemperature(monitorDataArray[0]);
            oneLog.SolenoidValveVoltage = (double)monitorDataArray[1] * 2 / 1000;
            oneLog.NegativePowerMonitor = (double)monitorDataArray[2] * 8.5 / 1000;
            oneLog.PositivePowerMonitor = (double)monitorDataArray[3] * 6 / 1000;
            oneLog.TestValveCloseDriveCurrent = (int)(monitorDataArray[4] / 1.5);
            oneLog.TestValveOpenDriveCurrent = (int)(monitorDataArray[5] / 1.5);
            oneLog.CycleValveCloseDriveCurrent = (int)(monitorDataArray[6] / 1.5);
            oneLog.CycleValveOpenDriveCurrent = (int)(monitorDataArray[7] / 1.5);

            oneLog.SolenoidValveVoltage = save2FractionalPart(oneLog.SolenoidValveVoltage);
            oneLog.NegativePowerMonitor = save2FractionalPart(oneLog.NegativePowerMonitor);
            oneLog.PositivePowerMonitor = save2FractionalPart(oneLog.PositivePowerMonitor);
        }

        private double save2FractionalPart(double input)
        {
            return (double)((int)(input * 100)) / 100;
        }
        private void SortAsFolderName(ref DirectoryInfo[] dirs)
        {
            Array.Sort(dirs, delegate (DirectoryInfo x, DirectoryInfo y) { return x.Name.CompareTo(y.Name); });
        }
        private void SortAsFileName(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.Name.CompareTo(y.Name); });
        }

        private void showLogWin_Closed(object sender, EventArgs e)
        {
            _uniqueInstance = null;
        }

        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void miExportExcel_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Log
    {
        public DateTime Time { get; set; }
        public int TotalPackageCount { get; set; }
        public int CurrentPackageNo { get; set; }
        public double Temperature { get; set; }
        public double SolenoidValveVoltage { get; set; }
        public double NegativePowerMonitor { get; set; }
        public double PositivePowerMonitor { get; set; }
        public int TestValveCloseDriveCurrent { get; set; }
        public int TestValveOpenDriveCurrent { get; set; }
        public int CycleValveCloseDriveCurrent { get; set; }
        public int CycleValveOpenDriveCurrent { get; set; }
        public string Pressure20 { get; set; }
    }
}
