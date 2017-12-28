using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ValveControlSystem.Classes
{
    class DataUnitConverter
    {
        public static double PressureUnitConvert(double inputData, PressureUnit destPressureUnit)
        {
            switch (destPressureUnit)
            {
                case PressureUnit.MPa:
                    return inputData;
                //return inputData * 145.03;
                case PressureUnit.PSI:
                    return inputData * 145.03; ;
                //return inputData / 145.03;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }
    }
}
