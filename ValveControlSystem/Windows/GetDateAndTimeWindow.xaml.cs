using ValveControlSystem.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using System.Xml;

namespace ValveControlSystem.Windows
{
    /// <summary>
    /// Interaction logic for ReadLogWindow.xaml
    /// </summary>
    public partial class GetDateAndTimeWindow : Window
    {
        public int YearStart { get; set; }
        public int MonthStart { get; set; }
        public int DayStart { get; set; }
        public int HourStart { get; set; }
        public int MinuteStart { get; set; }
        public int SecondStart { get; set; }
        public int YearEnd { get; set; }
        public int MonthEnd { get; set; }
        public int DayEnd { get; set; }
        public int HourEnd { get; set; }
        public int MinuteEnd { get; set; }
        public int SecondEnd { get; set; }

        private DateTime _startTime = new DateTime();
        private DateTime _endTime = new DateTime();
        //private User _userLogin;
        //private DbConn _dbConnect = new DbConn();
        //private GetTimeMode _windowMode = GetTimeMode.其他模式;
        //private Time2FrameNoCalculate _getTime = new Time2FrameNoCalculate();

        public int NewestFrameNo { get; set; }

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

        //public GetTimeMode WindowMode
        //{
        //    get
        //    {
        //        return _windowMode;
        //    }

        //    set
        //    {
        //        _windowMode = value;
        //    }
        //}

        public GetDateAndTimeWindow()
        {
            InitializeComponent();
            try
            {

            }
            catch (Exception ee)
            {
                MessageBox.Show("选择时间窗口初始化异常："+ee.Message);
            }
        }
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dpStartDate.SelectedDate.HasValue)
                {
                    this.YearStart = this.dpStartDate.SelectedDate.Value.Year;
                    this.MonthStart = this.dpStartDate.SelectedDate.Value.Month;
                    this.DayStart = this.dpStartDate.SelectedDate.Value.Day;
                }
                else
                {
                    MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("warningMustSelectOneValue").ToString(),
                                    string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("startTime").ToString())));
                    return;
                }
                if (this.tpStartTime.Value.HasValue)
                {
                    this.HourStart = this.tpStartTime.Value.Value.Hour;
                    this.MinuteStart = this.tpStartTime.Value.Value.Minute;
                    this.SecondStart = this.tpStartTime.Value.Value.Second;
                }
                this.StartTime = new DateTime(YearStart, MonthStart, DayStart, HourStart, MinuteStart, SecondStart);

                //if (WindowMode == GetTimeMode.回放模式)
                //{
                //    DateTime earliestTime = _getTime.GetTimeFromFrameNo(0);
                //    int compareStartAndEarliest = this.StartTime.CompareTo(earliestTime);
                //    if (compareStartAndEarliest < 0)
                //    {
                //        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("msgStartTimeCanNotEarlierThanEarliest").ToString())
                //            + "：" + earliestTime.ToString("yyyy-M-d H:mm:ss"));
                //        return;
                //    }
                //}
                if (this.dpEndDate.SelectedDate.HasValue)
                {
                    this.YearEnd = this.dpEndDate.SelectedDate.Value.Year;
                    this.MonthEnd = this.dpEndDate.SelectedDate.Value.Month;
                    this.DayEnd = this.dpEndDate.SelectedDate.Value.Day;
                }
                else
                {
                    MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("warningMustSelectOneValue").ToString(),
                                    string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("endTime").ToString())));
                    return;
                }
                if (this.tpEndTime.Value.HasValue)
                {
                    this.HourEnd = this.tpEndTime.Value.Value.Hour;
                    this.MinuteEnd = this.tpEndTime.Value.Value.Minute;
                    this.SecondEnd = this.tpEndTime.Value.Value.Second;
                }
                this.EndTime = new DateTime(YearEnd, MonthEnd, DayEnd, HourEnd, MinuteEnd, SecondEnd);
                int compareResult = this.StartTime.CompareTo(this.EndTime);
                if (compareResult > 0)
                {
                    MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("msgStartTimeCanNotLaterThanEndTime").ToString()));
                    return;
                }

                //int logReadTimeID = _dbConnect.CheckLogReadTimeExists(_userLogin.UserID);
                //if (logReadTimeID == 0)
                //{
                //    setNullValue();
                //    _dbConnect.AddLogReadTime(_userLogin.UserID,
                //        this.dpStartDate.SelectedDate.Value,
                //        this.tpStartTime.Value.Value,
                //        this.dpEndDate.SelectedDate.Value,
                //        this.tpEndTime.Value.Value);
                //}
                //else
                //{
                //    setNullValue();
                //    _dbConnect.UpdateLogReadTime(_userLogin.UserID,
                //        this.dpStartDate.SelectedDate.Value,
                //        this.tpStartTime.Value.Value,
                //        this.dpEndDate.SelectedDate.Value,
                //        this.tpEndTime.Value.Value);
                //}
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void setNullValue()
        {
            if (!this.tpStartTime.Value.HasValue)
            {
                this.tpStartTime.Value = this.dpStartDate.SelectedDate.Value;
            }
            if (!this.tpEndTime.Value.HasValue)
            {
                this.tpEndTime.Value = this.dpEndDate.SelectedDate;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnToNow_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = System.DateTime.Now;
            this.dpEndDate.SelectedDate = now;
            this.tpEndTime.Value = now;
        }

        //public void HiddenButtonAll()
        //{
        //    this.btnAll.Visibility = Visibility.Collapsed;
        //}

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mainWin = this.Owner as MainWindow;
                if (mainWin != null)
                {
                    string logPath = System.Environment.CurrentDirectory + @"\Log\";
                    DirectoryInfo logWellFolder = new DirectoryInfo(logPath);
                    DirectoryInfo[] yearDirs = logWellFolder.GetDirectories();
                    SortAsFolderName(ref yearDirs);
                    if (yearDirs.Length > 0)
                    {
                        //获取第一个日期
                        DirectoryInfo firstYear = yearDirs[0];
                        DirectoryInfo[] monthDirs = firstYear.GetDirectories();
                        SortAsFolderName(ref monthDirs);
                        if (monthDirs.Length > 0)
                        {
                            DirectoryInfo firstMonth = monthDirs[0];
                            DirectoryInfo[] dayDirs = firstMonth.GetDirectories();
                            SortAsFolderName(ref dayDirs);
                            if (dayDirs.Length > 0)
                            {
                                DirectoryInfo firstDay = dayDirs[0];
                                FileInfo[] fileInfo = firstDay.GetFiles();
                                SortAsFileName(ref fileInfo);
                                if (fileInfo.Length > 0)
                                {
                                    string firstFileName = fileInfo[0].Name;
                                    DateTime earliestLogTime = new DateTime();
                                    IFormatProvider ifp = new CultureInfo("zh-CN", true);
                                    if (DateTime.TryParseExact(firstFileName.Substring(0, 17), "yyyy-MM-dd HHmmss", ifp, DateTimeStyles.None, out earliestLogTime))
                                    {
                                        this.dpStartDate.SelectedDate = earliestLogTime;
                                        this.tpStartTime.Value = earliestLogTime;
                                    }
                                }
                            }
                        }
                        //获取最后一个日期
                        DirectoryInfo lastYear = yearDirs[yearDirs.Length - 1];
                        DirectoryInfo[] monthDirsInLastYear = lastYear.GetDirectories();
                        SortAsFolderName(ref monthDirsInLastYear);
                        if (monthDirsInLastYear.Length > 0)
                        {
                            DirectoryInfo lastMonth = monthDirsInLastYear[monthDirsInLastYear.Length - 1];
                            DirectoryInfo[] dayDirsInLastMonth = lastMonth.GetDirectories();
                            SortAsFolderName(ref dayDirsInLastMonth);
                            if (dayDirsInLastMonth.Length > 0)
                            {
                                DirectoryInfo lastDay = dayDirsInLastMonth[dayDirsInLastMonth.Length - 1];
                                FileInfo[] fileInfo = lastDay.GetFiles();
                                SortAsFileName(ref fileInfo);
                                if (fileInfo.Length > 0)
                                {
                                    XmlDocument xmlDoc = new XmlDocument();
                                    xmlDoc.Load(fileInfo[fileInfo.Length - 1].FullName);
                                    XmlNodeList nodesData = xmlDoc.SelectNodes("/Log/Datas/Data");
                                    if (nodesData.Count > 0)
                                    {
                                        XmlNode node = nodesData[nodesData.Count - 1];
                                        XmlElement xe = (XmlElement)node;//将子节点类型转换为XmlElement类型 
                                        if (xe != null)
                                        {
                                            string strLogTime = xe.GetAttribute("Time");
                                            DateTime lastLogTime = new DateTime();
                                            if (DateTime.TryParse(strLogTime, out lastLogTime))
                                            {
                                                this.dpEndDate.SelectedDate = lastLogTime;
                                                this.tpEndTime.Value = lastLogTime;
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("无法读取日志！");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
        private void SortAsFolderName(ref DirectoryInfo[] dirs)
        {
            Array.Sort(dirs, delegate (DirectoryInfo x, DirectoryInfo y) { return x.Name.CompareTo(y.Name); });
        }
        private void SortAsFileName(ref FileInfo[] arrFi)
        {
            Array.Sort(arrFi, delegate (FileInfo x, FileInfo y) { return x.Name.CompareTo(y.Name); });
        }

        //private void btnNewest_Click(object sender, RoutedEventArgs e)
        //{
        //    if (NewestFrameNo == -1)
        //    {
        //        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("msgWaitForOneRealtimeFrame").ToString()));
        //        return;
        //    }
        //    DateTime newestTime = _getTime.GetTimeFromFrameNo(NewestFrameNo);
        //    this.dpEndDate.SelectedDate = newestTime;
        //    this.tpEndTime.Value = newestTime;
        //}

        //private void btnEarliest_Click(object sender, RoutedEventArgs e)
        //{
        //    DateTime earliestTime = _getTime.GetTimeFromFrameNo(0);
        //    this.dpStartDate.SelectedDate = earliestTime;
        //    this.tpStartTime.Value = earliestTime;
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (WindowMode == GetTimeMode.回放模式)
            //{
            //    this.btnAll.Visibility = Visibility.Collapsed;
            //    this.btnToNow.Visibility = Visibility.Collapsed;
            //    this.btnEarliest.Visibility = Visibility.Visible;
            //    this.btnNewest.Visibility = Visibility.Visible;
            //}
        }
    }
}
