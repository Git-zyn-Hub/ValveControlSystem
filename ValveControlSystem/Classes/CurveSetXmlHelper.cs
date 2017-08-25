﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;

namespace ValveControlSystem.Classes
{
    public class CurveSetXmlHelper
    {
        private string _xmlPath;

        public string XmlPath
        {
            get
            {
                return _xmlPath;
            }

            set
            {
                _xmlPath = value;
            }
        }

        public void CurveSettingXmlInitial()
        {
            bool curveConfigExists = CheckCurveConfigExists();
            if (!curveConfigExists)
            {
                File.Create(XmlPath).Close();
            }
            else
            {
                return;
            }
            CurveSetting pressureCurve = new CurveSetting()
            {
                CurveName = "压力",
                LineThickness = 2,
                LineColor = Colors.Red,
                Show = true
            };
            CurveSetting temperatureCurve = new CurveSetting()
            {
                CurveName = "温度",
                LineThickness = 2,
                LineColor = Colors.Green,
                Show = true
            };
            CurveGeneralSetting curveGeneralSet = new CurveGeneralSetting()
            {
                PressureRange = 66000,
                TemperatureRange = 100,
                PressureThreshold = 0,
                FontFamily = "Microsoft YaHei UI",
                FontSize = 10,
                BackgroundColor = Colors.White,
                DisplayGrid = true,
            };

            XDocument doc = new XDocument
            (
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement
                (
                    "Config",
                    new XElement
                    (
                        "CurveSet",
                        new XElement
                        (
                            "PressureCurve",
                            new XAttribute("CurveName", pressureCurve.CurveName),
                            new XAttribute("LineThickness", pressureCurve.LineThickness),
                            new XAttribute("LineColor", pressureCurve.LineColor),
                            new XAttribute("Show", pressureCurve.Show)
                        ),
                        new XElement
                        (
                            "TemperatureCurve",
                            new XAttribute("CurveName", temperatureCurve.CurveName),
                            new XAttribute("LineThickness", temperatureCurve.LineThickness),
                            new XAttribute("LineColor", temperatureCurve.LineColor),
                            new XAttribute("Show", temperatureCurve.Show)
                        ),
                        new XElement
                        (
                            "CurveGeneralSetting",
                            new XAttribute("PressureRange", curveGeneralSet.PressureRange),
                            new XAttribute("TemperatureRange", curveGeneralSet.TemperatureRange),
                            new XAttribute("PressureThreshold", curveGeneralSet.PressureThreshold),
                            new XAttribute("FontFamily", curveGeneralSet.FontFamily),
                            new XAttribute("FontSize", curveGeneralSet.FontSize),
                            new XAttribute("BackgroundColor", curveGeneralSet.BackgroundColor),
                            new XAttribute("DisplayGrid", curveGeneralSet.DisplayGrid)
                        )
                    )
                )
            );

            // 保存为XML文件
            doc.Save(XmlPath);
        }

        private bool CheckCurveConfigExists()
        {
            return File.Exists(XmlPath);
        }


        public void ModifyXmlCurveSettingElement(string strElement, CurveSetting curveSet)
        {
            XDocument xd = XDocument.Load(XmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("CurveSet").Element(strElement);
            ///修改元素  
            if (element != null)
            {
                ///设置新的属性  
                element.SetAttributeValue("CurveName", curveSet.CurveName);
                element.SetAttributeValue("LineThickness", curveSet.LineThickness);
                element.SetAttributeValue("LineColor", curveSet.LineColor);
                element.SetAttributeValue("Show", curveSet.Show);
            }
            xd.Save(XmlPath);
        }

        public void ModifyXmlCurveGeneralSettingElement(CurveGeneralSetting curveGeneralSet)
        {
            XDocument xd = XDocument.Load(XmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("CurveSet").Element("CurveGeneralSetting");
            ///修改元素  
            if (element != null)
            {
                ///设置新的属性  
                element.SetAttributeValue("PressureRange", curveGeneralSet.PressureRange);
                element.SetAttributeValue("TemperatureRange", curveGeneralSet.TemperatureRange);
                element.SetAttributeValue("PressureThreshold", curveGeneralSet.PressureThreshold);
                element.SetAttributeValue("FontFamily", curveGeneralSet.FontFamily);
                element.SetAttributeValue("FontSize", curveGeneralSet.FontSize);
                element.SetAttributeValue("BackgroundColor", curveGeneralSet.BackgroundColor);
                element.SetAttributeValue("DisplayGrid", curveGeneralSet.DisplayGrid);
            }
            xd.Save(XmlPath);
        }

        private string getXmlAttributeValue(string Node, string Attribute)
        {
            XDocument xd = XDocument.Load(XmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("CurveSet").Element(Node);
            if (element != null)
            {
                return element.Attribute(Attribute).Value;
            }
            return string.Empty;
        }

        public CurveSetting GetCurveSetting(string curveNameNode)
        {
            CurveSetting curveSetPressure = new CurveSetting();
            curveSetPressure.CurveName = getXmlAttributeValue(curveNameNode, "CurveName");
            curveSetPressure.LineThickness = int.Parse(getXmlAttributeValue(curveNameNode, "LineThickness"));
            curveSetPressure.LineColor = (Color)ColorConverter.ConvertFromString(getXmlAttributeValue(curveNameNode, "LineColor"));
            curveSetPressure.Show = bool.Parse(getXmlAttributeValue(curveNameNode, "Show"));
            return curveSetPressure;
        }

        public CurveGeneralSetting GetCurveGeneralSetting()
        {
            CurveGeneralSetting newCGS = new CurveGeneralSetting();
            newCGS.PressureRange = int.Parse(getXmlAttributeValue("CurveGeneralSetting", "PressureRange"));
            newCGS.TemperatureRange = int.Parse(getXmlAttributeValue("CurveGeneralSetting", "TemperatureRange"));
            newCGS.PressureThreshold = int.Parse(getXmlAttributeValue("CurveGeneralSetting", "PressureThreshold"));
            newCGS.FontFamily = getXmlAttributeValue("CurveGeneralSetting", "FontFamily");
            newCGS.FontSize = int.Parse(getXmlAttributeValue("CurveGeneralSetting", "FontSize"));
            newCGS.BackgroundColor = (Color)ColorConverter.ConvertFromString(getXmlAttributeValue("CurveGeneralSetting", "BackgroundColor"));
            newCGS.DisplayGrid = bool.Parse(getXmlAttributeValue("CurveGeneralSetting", "DisplayGrid"));
            return newCGS;
        }
    }
}
