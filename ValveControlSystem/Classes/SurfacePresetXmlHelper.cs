using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ValveControlSystem.Classes
{
    public class SurfacePresetXmlHelper
    {
        private string _xmlPath = System.Environment.CurrentDirectory + "\\Remember.xml";

        public string XmlPath
        {
            get
            {
                return _xmlPath;
            }
        }

        public void SurfacePresetXmlInitial()
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
            SurfacePreset preset = new SurfacePreset()
            {
                AutomaticClosureValve = 0,
                AutomaticClosurePressure = 499,
                AVS_A_Option = 0,
                AVS_TriggerPressure = 0,
                AVS_B_Option = 0,
                AVS4TimeLimit = 0,
                AVS4UnderPressureLimit = 500,
                AVS4OverPressureLimit = 500,
                SUD_Setting = 100,
                ToolNumber = 1
            };

            XDocument doc = new XDocument
            (
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement
                (
                    "Remember",
                    new XElement
                    (
                        "SurfacePreset",

                        new XAttribute("AutomaticClosureValve", preset.AutomaticClosureValve),
                        new XAttribute("AutomaticClosurePressure", preset.AutomaticClosurePressure),
                        new XAttribute("AVS_A_Option", preset.AVS_A_Option),
                        new XAttribute("AVS_TriggerPressure", preset.AVS_TriggerPressure),
                        new XAttribute("AVS_B_Option", preset.AVS_B_Option),
                        new XAttribute("AVS4TimeLimit", preset.AVS4TimeLimit),
                        new XAttribute("AVS4UnderPressureLimit", preset.AVS4UnderPressureLimit),
                        new XAttribute("AVS4OverPressureLimit", preset.AVS4OverPressureLimit),
                        new XAttribute("SUD_Setting", preset.SUD_Setting),
                        new XAttribute("ToolNumber", preset.ToolNumber)
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

        public void ModifyXmlSurfacePresetElement(SurfacePreset preset)
        {
            XDocument xd = XDocument.Load(XmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("SurfacePreset");
            ///修改元素  
            if (element != null)
            {
                ///设置新的属性  
                element.SetAttributeValue("AutomaticClosureValve", preset.AutomaticClosureValve);
                element.SetAttributeValue("AutomaticClosurePressure", preset.AutomaticClosurePressure);
                element.SetAttributeValue("AVS_A_Option", preset.AVS_A_Option);
                element.SetAttributeValue("AVS_TriggerPressure", preset.AVS_TriggerPressure);
                element.SetAttributeValue("AVS_B_Option", preset.AVS_B_Option);
                element.SetAttributeValue("AVS4TimeLimit", preset.AVS4TimeLimit);
                element.SetAttributeValue("AVS4UnderPressureLimit", preset.AVS4UnderPressureLimit);
                element.SetAttributeValue("AVS4OverPressureLimit", preset.AVS4OverPressureLimit);
                element.SetAttributeValue("SUD_Setting", preset.SUD_Setting);
                element.SetAttributeValue("ToolNumber", preset.ToolNumber);
            }
            xd.Save(XmlPath);
        }

        private string getXmlAttributeValue(string Attribute)
        {
            XDocument xd = XDocument.Load(XmlPath);
            ///查询修改的元素  
            XElement element = xd.Root.Element("SurfacePreset");
            if (element != null)
            {
                return element.Attribute(Attribute).Value;
            }
            return string.Empty;
        }

        public SurfacePreset GetSurfacePreset()
        {
            SurfacePreset preset = new SurfacePreset();
            preset.AutomaticClosureValve = int.Parse(getXmlAttributeValue("AutomaticClosureValve"));
            preset.AutomaticClosurePressure = int.Parse(getXmlAttributeValue("AutomaticClosurePressure"));
            preset.AVS_A_Option = int.Parse(getXmlAttributeValue("AVS_A_Option"));
            preset.AVS_TriggerPressure = int.Parse(getXmlAttributeValue("AVS_TriggerPressure"));
            preset.AVS_B_Option = int.Parse(getXmlAttributeValue("AVS_B_Option"));
            preset.AVS4TimeLimit = int.Parse(getXmlAttributeValue("AVS4TimeLimit"));
            preset.AVS4UnderPressureLimit = int.Parse(getXmlAttributeValue("AVS4UnderPressureLimit"));
            preset.AVS4OverPressureLimit = int.Parse(getXmlAttributeValue("AVS4OverPressureLimit"));
            preset.SUD_Setting = int.Parse(getXmlAttributeValue("SUD_Setting"));
            preset.ToolNumber = int.Parse(getXmlAttributeValue("ToolNumber"));
            return preset;
        }
    }
}
