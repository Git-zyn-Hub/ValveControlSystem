using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ValveControlSystem.Classes
{
    public class CurveGeneralSetting
    {
        public int CurveGeneralSettingID { get; set; }
        public int UserID { get; set; }
        public string Culture { get; set; }
        public double PressureRange
        {
            get;
            set;
        }
        public int TemperatureRange
        {
            get;
            set;
        }
        public double PressureThreshold
        {
            get;
            set;
        }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public Color BackgroundColor { get; set; }
        public bool MoveLeftRealtime { get; set; }
        public bool DisplayGrid { get; set; }
        public int RetainMinutes { get; set; }
    }
}
