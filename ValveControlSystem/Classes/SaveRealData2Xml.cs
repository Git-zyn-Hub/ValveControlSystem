using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace ValveControlSystem.Classes
{
    public class SaveRealData2Xml
    {
        private string _fileName;
        private string _directoryName = string.Empty;
        private static SaveRealData2Xml _uniqueInstance;
        private DateTimeXmlHelper _dateTimeXmlHelper = new DateTimeXmlHelper();
        private DateTime? _lastCreateDateTime = null;

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

        private SaveRealData2Xml()
        {

        }

        public static SaveRealData2Xml GetInstance()
        {
            if (_uniqueInstance == null)
            {
                _uniqueInstance = new SaveRealData2Xml();
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
                writer.WriteStartElement("LogReal");
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
                if (!Directory.Exists(System.Environment.CurrentDirectory + @"\LogReal"))
                {
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\LogReal");
                }
                DateTime logTime = _lastCreateDateTime.Value;
                DirectoryName = System.Environment.CurrentDirectory + @"\LogReal\" + logTime.ToString("yyyy") + "\\" + logTime.ToString("yyyy-MM") + "\\" + logTime.ToString("yyyy-MM-dd");
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
                createNewFile();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(DirectoryName + @"\" + _fileName);
                XmlNode nodeDatas = xmlDoc.SelectSingleNode("/LogReal/Datas");
                if (nodeDatas != null)
                {
                    XmlElement xe = (XmlElement)nodeDatas;//将子节点类型转换为XmlElement类型  
                    XmlElement xe1 = xmlDoc.CreateElement("Data");//创建一个<Data>节点
                    xe.AppendChild(xe1);//添加到<Datas>节点中
                    xe1.SetAttribute("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//设置该节点Time属性
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

        private DateTime getLastCreateFileDateTime()
        {
            if (_lastCreateDateTime == null)
            {
                DateTime now = DateTime.Now;
                _lastCreateDateTime = now;
                CreatNewFile(now);
            }
            return _lastCreateDateTime.Value;
        }

        private void createNewFile()
        {
            DateTime now = DateTime.Now;
            TimeSpan span = now.Subtract(getLastCreateFileDateTime());
            if (span.TotalMinutes > 30)
            {
                _fileName = now.ToString("yyyy-MM-dd HHmmss") + ".xml";
                _lastCreateDateTime = now;
                checkDirectory();
                creatFile(DirectoryName, _fileName);
            }
        }
    }
}
