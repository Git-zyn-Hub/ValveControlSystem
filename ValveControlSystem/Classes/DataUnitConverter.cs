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
                    return inputData * 145.03;
                //return inputData / 145.03;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }

        /// <summary>
        /// 根据数据的源单位类型，将源数据改成以PSI为单位的数据。
        /// </summary>
        /// <param name="inputData">输入的数据</param>
        /// <param name="sourcePressureUnit">源压力类型</param>
        /// <returns></returns>
        public static int ConvertPressureUnit2PSI(int inputData, PressureUnit sourcePressureUnit)
        {
            switch (sourcePressureUnit)
            {
                case PressureUnit.MPa:
                    //return inputData;
                    return (int)(inputData * 145.03);
                case PressureUnit.PSI:
                    return inputData;
                //return inputData / 145.03;
                default:
                    MessageBox.Show("目标数据单位未设置！");
                    return 0;
            }
        }
    }
}
