using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ValveControlSystem;

namespace FastLinkSystem.Classes
{
    public class DataUnitConvert
    {
        public static double PressureUnitConvert(double inputData, PressureUnit destPressureUnit)
        {
            switch (destPressureUnit)
            {
                case PressureUnit.MPa:
                    return inputData / 145.03;
                //return inputData * 145.03;
                case PressureUnit.PSI:
                    return inputData;
                //return inputData / 145.03;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }

        public static double TemperatureUnitConvert(double inputData, TemperatureUnit destTempUnit)
        {
            switch (destTempUnit)
            {
                case TemperatureUnit.摄氏度:
                    return inputData;
                //return (inputData - 32) / 1.8;
                case TemperatureUnit.华氏度:
                    return inputData * 1.8 + 32;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }

        public static double PressureUnitConvertEachOther(double inputData, PressureUnit destPressureUnit)
        {
            switch (destPressureUnit)
            {
                case PressureUnit.MPa:
                    return inputData / 145.03;
                case PressureUnit.PSI:
                    return inputData * 145.03;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }

        public static double TemperatureUnitConvertEachOther(double inputData, TemperatureUnit destTempUnit)
        {
            switch (destTempUnit)
            {
                case TemperatureUnit.摄氏度:
                    return (inputData - 32) / 1.8;
                case TemperatureUnit.华氏度:
                    return inputData * 1.8 + 32;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }
    }
}
