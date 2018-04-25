using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ValveControlSystem.Classes
{
    public class DateTimeXmlHelper
    {
        private string _xmlPath = System.Environment.CurrentDirectory + @"\LogDateTime.xml";


        public void LogDateTimeXmlInitial()
        {
            bool curveConfigExists = CheckFileExists();
            if (!curveConfigExists)
            {
                File.Create(_xmlPath).Close();
            }
            else
            {
                return;
            }

            XDocument doc = new XDocument
            (
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement
                (
                    "DateTime",
                    new XElement
                    (
                        "LogDateTime",

                        new XElement
                        (
                            "StartDate"
                        ),
                        new XElement
                        (
                            "StartTime"
                        ),
                        new XElement
                        (
                            "EndDate"
                        ),
                        new XElement
                        (
                            "EndTime"
                        )
                    ),
                    new XElement
                    (
                        "PowerOnDateTime",

                        new XElement
                        (
                            "PowerOnDate"
                        ),
                        new XElement
                        (
                            "PowerOnTime"
                        )
                    )
                )

            );

            // 保存为XML文件
            doc.Save(_xmlPath);
        }

        private bool CheckFileExists()
        {
            return File.Exists(_xmlPath);
        }


        public void ModifyLogDateTimeXml(DateTime dayStart, DateTime timeStart, DateTime dayEnd, DateTime timeEnd)
        {
            XDocument xd = XDocument.Load(_xmlPath);
            ///查询修改的元素  
            XElement root = xd.Root;
            XElement startDateElement = root.Element("LogDateTime").Element("StartDate");
            XElement startTimeElement = root.Element("LogDateTime").Element("StartTime");
            XElement endDateElement = root.Element("LogDateTime").Element("EndDate");
            XElement endTimeElement = root.Element("LogDateTime").Element("EndTime");
            ///修改元素  
            setDate(startDateElement, dayStart);
            setTime(startTimeElement, timeStart);
            setDate(endDateElement, dayEnd);
            setTime(endTimeElement, timeEnd);
            xd.Save(_xmlPath);
        }

        public void ModifyPowerOnDateTimeXml(DateTime powerOnDate, DateTime powerOnTime)
        {
            XDocument xd = XDocument.Load(_xmlPath);
            ///查询修改的元素  
            XElement root = xd.Root;
            XElement powerOnDateElement = root.Element("PowerOnDateTime").Element("PowerOnDate");
            XElement powerOnTimeElement = root.Element("PowerOnDateTime").Element("PowerOnTime");
            ///修改元素  
            setDate(powerOnDateElement, powerOnDate);
            setTime(powerOnTimeElement, powerOnTime);
            xd.Save(_xmlPath);
        }
        private void setDate(XElement e, DateTime date)
        {
            if (e != null)
            {
                e.SetValue(date.ToString("yyyy/MM/dd"));
            }
        }

        private void setTime(XElement e, DateTime time)
        {
            if (e != null)
            {
                e.SetValue(time.ToString("HH:mm:ss"));
            }
        }
        private string getXmlLogNodeValue(string Node)
        {
            XDocument xd = XDocument.Load(_xmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("LogDateTime").Element(Node);
            if (element != null)
            {
                return element.Value;
            }
            return string.Empty;
        }
        private string getXmlPowerOnNodeValue(string Node)
        {
            XDocument xd = XDocument.Load(_xmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("PowerOnDateTime").Element(Node);
            if (element != null)
            {
                return element.Value;
            }
            return string.Empty;
        }

        public DateTime? GetStartDateAndTime()
        {
            string startDate = getXmlLogNodeValue("StartDate");
            string startTime = getXmlLogNodeValue("StartTime");
            if (startDate != string.Empty && startTime != string.Empty)
            {
                return DateTime.Parse(startDate + " " + startTime);
            }
            else
            {
                return null;
            }
        }

        public DateTime? GetEndDateAndTime()
        {
            string date = getXmlLogNodeValue("EndDate");
            string time = getXmlLogNodeValue("EndTime");

            if (date != string.Empty && time != string.Empty)
            {
                return DateTime.Parse(date + " " + time);
            }
            else
            {
                return null;
            }
        }


        public DateTime? GetPowerOnDateAndTime()
        {
            string date = getXmlPowerOnNodeValue("PowerOnDate");
            string time = getXmlPowerOnNodeValue("PowerOnTime");

            if (date != string.Empty && time != string.Empty)
            {
                return DateTime.Parse(date + " " + time);
            }
            else
            {
                return null;
            }
        }
    }
}
