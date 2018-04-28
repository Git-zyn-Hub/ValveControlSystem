using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace ValveControlSystem.Classes
{
    public class SaveData2Xml
    {
        private string _fileName;
        private string _directoryName = string.Empty;
        private static SaveData2Xml _uniqueInstance;
        private DateTimeXmlHelper _dateTimeXmlHelper = new DateTimeXmlHelper();
        private DateTime? _lastCreateDateTime = null;
        public delegate void ShowMessageEventHandler(string info, DataLevel level);
        public event ShowMessageEventHandler ShowMessage;

        public string DirectoryName
        {
            get
            {
                return _directoryName;
            }

            set
            {
                _directoryName = value;
            }
        }

        private SaveData2Xml()
        {
        }

        public static SaveData2Xml GetInstance()
        {
            if (_uniqueInstance == null)
            {
                _uniqueInstance = new SaveData2Xml();
            }
            return _uniqueInstance;
        }

        public static void CloseInstance()
        {
            _uniqueInstance = null;
        }

        private void creatFile(string directoryName, string fileName)
        {
            string filePath = directoryName + @"\" + fileName;
            if (!File.Exists(filePath))
            {
                XmlTextWriter writer = new XmlTextWriter(filePath, null);
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("Log");
                writer.WriteStartElement("Datas");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }


        private void checkDirectory()
        {
            try
            {
                if (!Directory.Exists(System.Environment.CurrentDirectory + @"\Log"))
                {
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\Log");
                }
                DateTime logTime = _lastCreateDateTime.Value;
                DirectoryName = System.Environment.CurrentDirectory + @"\Log\" + logTime.ToString("yyyy") + "\\" + logTime.ToString("yyyy-MM") + "\\" + logTime.ToString("yyyy-MM-dd");
                if (!Directory.Exists(DirectoryName))
                {
                    Directory.CreateDirectory(DirectoryName);
                }
            }
            catch (NotSupportedException ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }


        public void SaveData(byte[] data, int length)
        {
            try
            {
                if (length != 242)
                {
                    ShowMessage?.Invoke("数据长度不是242！", DataLevel.Error);
                    return;
                }
                DateTime? powerOnTime = _dateTimeXmlHelper.GetPowerOnDateAndTime();
                if (powerOnTime == null)
                {
                    ShowMessage?.Invoke("未设置上电时间，该条数据无法保存，且无法显示曲线", DataLevel.Error);
                    return;
                }
                uint secondFromStart = (uint)(data[235] << 24) + (uint)(data[236] << 16) + (uint)(data[237] << 8) + (uint)(data[238]);
                DateTime occurTime = powerOnTime.Value.AddSeconds(secondFromStart);
                createNewFile(occurTime);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(DirectoryName + @"\" + _fileName);
                XmlNode nodeDatas = xmlDoc.SelectSingleNode("/Log/Datas");
                if (nodeDatas != null)
                {
                    XmlElement xe = (XmlElement)nodeDatas;//将子节点类型转换为XmlElement类型  
                    XmlElement xe1 = xmlDoc.CreateElement("Data");//创建一个<Data>节点
                    xe.AppendChild(xe1);//添加到<Datas>节点中
                    xe1.SetAttribute("Time", occurTime.ToString("yyyy-MM-dd HH:mm:ss"));//设置该节点Time属性
                    xe1.InnerText = BitConverter.ToString(data, 0, length);
                }
                xmlDoc.Save(DirectoryName + @"\" + _fileName);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        public void CreatNewFile(DateTime now)
        {
            checkDirectory();
            _fileName = now.ToString("yyyy-MM-dd HHmmss") + ".xml";
            creatFile(DirectoryName, _fileName);
        }

        private DateTime getLastCreateFileDateTime(DateTime newestDateTime)
        {
            if (_lastCreateDateTime == null)
            {
                _lastCreateDateTime = _dateTimeXmlHelper.GetLastCreateDateAndTime();
                if (_lastCreateDateTime == null)
                {
                    _lastCreateDateTime = newestDateTime;
                }
                CreatNewFile(_lastCreateDateTime.Value);
                _dateTimeXmlHelper.ModifyLastCreateFileDateTimeXml(_lastCreateDateTime.Value, _lastCreateDateTime.Value);
                //else
                //{
                //    checkDirectory();
                //    _fileName = _lastCreateDateTime.Value.ToString("yyyy-MM-dd HHmmss") + ".xml";
                //}
            }
            return _lastCreateDateTime.Value;
        }

        private void createNewFile(DateTime newestDateTime)
        {
            TimeSpan span = newestDateTime.Subtract(getLastCreateFileDateTime(newestDateTime));
            if (span.TotalMinutes > 30)
            {
                _fileName = newestDateTime.ToString("yyyy-MM-dd HHmmss") + ".xml";
                _lastCreateDateTime = newestDateTime;
                _dateTimeXmlHelper.ModifyLastCreateFileDateTimeXml(_lastCreateDateTime.Value, _lastCreateDateTime.Value);
                checkDirectory();
                creatFile(DirectoryName, _fileName);
            }
        }
    }
}
