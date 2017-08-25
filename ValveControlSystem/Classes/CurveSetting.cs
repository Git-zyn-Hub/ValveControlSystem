using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ValveControlSystem.Classes
{
    public class CurveSetting
    {
        public int CurveSettingID { get; set; }
        public int UserID { get; set; }
        public string Culture { get; set; }
        public string CurveName { get; set; }
        public int LineThickness { get; set; }
        public Color LineColor { get; set; }
        public string Unit { get; set; }
        public bool Show { get; set; }
    }
}
