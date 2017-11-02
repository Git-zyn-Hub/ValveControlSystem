using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using excel = ClosedXML.Excel;
using io = System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Collections.ObjectModel;
using ValveControlSystem.Windows;

namespace ValveControlSystem.Classes
{
    public class Logs2Excel
    {
        private ObservableCollection<Log> _logs;
        private string[] _excelHeader = new string[12];
        private excel.XLWorkbook _workBook;
        private excel.IXLWorksheet _workSheet;
        public static int LineCount = 0;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        private static Logs2Excel _uniqueInstance;
        private string _fName;
        public bool SaveDialogResult { get; set; }

        public static Logs2Excel GetInstance(ObservableCollection<Log> logs)
        {
            if (_uniqueInstance == null)
            {
                _uniqueInstance = new Logs2Excel(logs);
            }
            return _uniqueInstance;
        }

        public static void CloseInstance()
        {
            _uniqueInstance = null;
        }

        private Logs2Excel(ObservableCollection<Log> logs)
        {
            try
            {
                _excelHeader[0] = "时间";
                _excelHeader[1] = "总包数";
                _excelHeader[2] = "当前包号";
                _excelHeader[3] = "温度";
                _excelHeader[4] = "电磁阀监视电压";
                _excelHeader[5] = "负电源监视";
                _excelHeader[6] = "正电源监视";
                _excelHeader[7] = "测试阀关闭驱动电流";
                _excelHeader[8] = "测试阀打开驱动电流";
                _excelHeader[9] = "循环阀关闭驱动电流";
                _excelHeader[10] = "循环阀打开驱动电流";
                _excelHeader[11] = "压力（20个）";

                _workBook = new excel.XLWorkbook();
                _workSheet = _workBook.AddWorksheet("Sheet1");

                _workSheet.Column(1).Style.NumberFormat.Format = "yyyy/m/d h:mm:ss";
                _workSheet.Columns(4, 7).Style.NumberFormat.Format = "#,##0.00";
                _workSheet.Column(12).Style.NumberFormat.Format = "@";
                //设置第1列的宽度
                _workSheet.Column(1).Width = 18;
                //设置第2到12列的宽度
                _workSheet.Column(2).Width = 7;
                _workSheet.Column(3).Width = 8;
                _workSheet.Column(4).Width = 7;
                _workSheet.Column(5).Width = 14;
                _workSheet.Columns(6, 7).Width = 10;
                _workSheet.Columns(8, 11).Width = 18;
                _workSheet.Column(12).Width = 80;

                for (int i = 0; i < _excelHeader.Length; i++)
                {
                    _workSheet.Cell(1, i + 1).Value = _excelHeader[i];
                }
                _logs = logs;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        public void SaveLogs2Excel()
        {
            try
            {
                LineCount = _logs.Count;
                for (int i = 0; i < LineCount; i++)
                {
                    _workSheet.Cell(i + 2, 1).Value = _logs[i].Time;
                    _workSheet.Cell(i + 2, 2).Value = _logs[i].TotalPackageCount;
                    _workSheet.Cell(i + 2, 3).Value = _logs[i].CurrentPackageNo;
                    _workSheet.Cell(i + 2, 4).Value = _logs[i].Temperature;
                    _workSheet.Cell(i + 2, 5).Value = _logs[i].SolenoidValveVoltage;
                    _workSheet.Cell(i + 2, 6).Value = _logs[i].NegativePowerMonitor;
                    _workSheet.Cell(i + 2, 7).Value = _logs[i].PositivePowerMonitor;
                    _workSheet.Cell(i + 2, 8).Value = _logs[i].TestValveCloseDriveCurrent;
                    _workSheet.Cell(i + 2, 9).Value = _logs[i].TestValveOpenDriveCurrent;
                    _workSheet.Cell(i + 2, 10).Value = _logs[i].CycleValveCloseDriveCurrent;
                    _workSheet.Cell(i + 2, 11).Value = _logs[i].CycleValveOpenDriveCurrent;
                    _workSheet.Cell(i + 2, 12).Value = _logs[i].Pressure20;
                }
                this.SaveEnd();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        //public void AddReplayData(int meterNo, int frameNo, List<int> pressuresData, byte temperature)
        //{
        //    DateTime time = time2FrameNo.GetTimeFromFrameNo(frameNo);
        //    _dataCount = pressuresData.Count;

        //    for (int i = 0; i < _dataCount; i++)
        //    {
        //        DateTime itemTime = time.AddSeconds(30 / _dataCount * i);
        //        double pressure = (double)pressuresData[i];
        //        switch (meterNo)
        //        {
        //            case 1:
        //                {
        //                    _workSheet.Cell(i + LineCount + 2, 1).Value = itemTime;
        //                    _workSheet.Cell(i + LineCount + 2, 2).Value = pressure / 512;
        //                    _workSheet.Cell(i + LineCount + 2, 5).Value = temperature;
        //                }
        //                break;
        //            case 2:
        //                {
        //                    _workSheet.Cell(i + LineCount + 2, 1).Value = itemTime;
        //                    _workSheet.Cell(i + LineCount + 2, 3).Value = pressure / 512;
        //                    _workSheet.Cell(i + LineCount + 2, 6).Value = temperature;
        //                }
        //                break;
        //            case 3:
        //                {
        //                    _workSheet.Cell(i + LineCount + 2, 1).Value = itemTime;
        //                    _workSheet.Cell(i + LineCount + 2, 4).Value = pressure / 512;
        //                    _workSheet.Cell(i + LineCount + 2, 7).Value = temperature;
        //                }
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    if (LineCount > _saveFlag)
        //    {
        //        Save();
        //        _saveFlag += 1000;
        //    }
        //    LineCount += _dataCount;
        //}

        //public void Save()
        //{
        //    _workBook.SaveAs(_fName, true);
        //}

        public void SaveEnd()
        {
            _workBook.SaveAs(_fName, true);
            MessageBox.Show(string.Format("保存成功，总行数：") + LineCount.ToString() + "\n" + "位置：" + _fName);
            LineCount = 0;
            CloseInstance();
        }

        public void SaveExcel()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel File|*.xlsx|All File|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                _fName = string.Empty;
                string defaultFileName = ("日志" + StartTime.ToString("yyyyMMddHHmmss") + "-" + EndTime.ToString("yyyyMMddHHmmss"));
                saveFileDialog.FileName = defaultFileName;

                if (saveFileDialog.ShowDialog().Value)
                {
                    _fName = saveFileDialog.FileName;
                    if (!io.File.Exists(_fName))
                    {
                        io.File.Create(_fName).Close();
                    }
                    if (!string.IsNullOrEmpty(_fName))
                    {
                        //给Excel文件添加"Everyone,Users"用户组的完全控制权限  
                        io.FileInfo fi = new io.FileInfo(_fName);
                        FileSecurity fileSecurity = fi.GetAccessControl();
                        fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                        fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
                        fi.SetAccessControl(fileSecurity);

                        //给Excel文件所在目录添加"Everyone,Users"用户组的完全控制权限  
                        io.DirectoryInfo di = new io.DirectoryInfo(io.Path.GetDirectoryName(_fName));
                        DirectorySecurity dirSecurity = di.GetAccessControl();
                        dirSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
                        dirSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
                        di.SetAccessControl(dirSecurity);

                        try
                        {
                            _workBook.SaveAs(_fName);
                            //LineCount = 0;
                            //CloseInstance();
                        }
                        catch (io.IOException)
                        {
                            throw;
                        }
                    }
                    SaveDialogResult = true;
                }
                else
                {
                    _workSheet.Clear();
                    SaveDialogResult = false;
                    //LineCount = 0;
                    //CloseInstance();
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        //public void GetDefaultName()
        //{
        //    if (string.IsNullOrEmpty(_fName))
        //    {
        //        checkDirectory();
        //        string defaultFileName = string.Empty;
        //        string pressureMeterName = string.Empty;
        //        switch (MeterNo)
        //        {
        //            case 1:
        //                {
        //                    pressureMeterName = string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("pressureMeter1").ToString());
        //                }
        //                break;
        //            case 2:
        //                {
        //                    pressureMeterName = string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("pressureMeter2").ToString());
        //                }
        //                break;
        //            case 3:
        //                {
        //                    pressureMeterName = string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("pressureMeter3").ToString());
        //                }
        //                break;
        //            default:
        //                {
        //                    pressureMeterName = string.Format(CultureInfo.CurrentCulture, Application.Current.TryFindResource("undefined").ToString());
        //                }
        //                break;
        //        }
        //        int i = 1;
        //        do
        //        {
        //            defaultFileName = pressureMeterName + ("-" + StartTime.ToString("yyyyMMddHHmmss") + "-" + EndTime.ToString("yyyyMMddHHmmss") + "-" + i.ToString() + ".xlsx");
        //            _fName = System.Environment.CurrentDirectory + "\\ReplayExcels\\" + defaultFileName;
        //            i++;
        //        } while (io.File.Exists(_fName));
        //    }
        //}

        //private void checkDirectory()
        //{
        //    try
        //    {
        //        if (!io.Directory.Exists(System.Environment.CurrentDirectory + "\\ReplayExcels"))
        //        {
        //            io.Directory.CreateDirectory(System.Environment.CurrentDirectory + "\\ReplayExcels");
        //        }
        //    }
        //    catch (NotSupportedException ee)
        //    {
        //        MessageBox.Show(ee.Message);
        //        return;
        //    }
        //    catch (Exception ee)
        //    {
        //        MessageBox.Show(ee.Message);
        //    }
        //}
    }
}
