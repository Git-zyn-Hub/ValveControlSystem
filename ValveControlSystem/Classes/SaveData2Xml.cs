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
        private string _directoryName;
        private static SaveData2Xml _uniqueInstance;

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
            XmlTextWriter writer = new XmlTextWriter(directoryName + @"\" + fileName, null);
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


        private void checkDirectory()
        {
            try
            {
                if (!Directory.Exists(System.Environment.CurrentDirectory + @"\Log"))
                {
                    Directory.CreateDirectory(System.Environment.CurrentDirectory + @"\Log");
                }
                DateTime logTime = System.DateTime.Now;
                _directoryName = System.Environment.CurrentDirectory + @"\Log\" + logTime.ToString("yyyy") + "\\" + logTime.ToString("yyyy-MM") + "\\" + logTime.ToString("yyyy-MM-dd");
                if (!Directory.Exists(_directoryName))
                {
                    Directory.CreateDirectory(_directoryName);
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
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_directoryName + @"\" + _fileName);
                XmlNode nodeDatas = xmlDoc.SelectSingleNode("/Log/Datas");
                if (nodeDatas != null)
                {
                    XmlElement xe = (XmlElement)nodeDatas;//将子节点类型转换为XmlElement类型  
                    XmlElement xe1 = xmlDoc.CreateElement("Data");//创建一个<Data>节点
                    xe.AppendChild(xe1);//添加到<Datas>节点中
                    xe1.SetAttribute("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//设置该节点Time属性
                    xe1.InnerText = BitConverter.ToString(data, 0, length);
                }
                xmlDoc.Save(_directoryName + @"\" + _fileName);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        public void CreatNewFile()
        {
            checkDirectory();
            _fileName = DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".xml";
            creatFile(_directoryName, _fileName);
        }
    }
}
