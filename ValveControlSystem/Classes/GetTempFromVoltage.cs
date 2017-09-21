using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ValveControlSystem.Classes
{
    /// <summary>
    /// 通过已知的点，计算出给出电压下对应的温度值，曲线不是线性的，但认为每两个点之间是线性的关系。
    /// </summary>
    public class GetTempFromVoltage
    {
        private readonly int[] _yTemp = new int[25] { 28, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 147, 150 };
        private readonly int[] _xVoltage = new int[25] { 2799, 2840, 2862, 2883, 2904, 2928, 2950, 2969, 2988, 3006, 3024, 3041, 3060, 3075, 3091, 3101, 3114, 3129, 3144, 3163, 3175, 3191, 3199, 3218, 3229 };

        /// <summary>
        /// 根据y1=a*x1+b;y2=a*x2+b得出a=(y2-y1)/(x2-x1);b=(y1x2-y2x1)/(x2-x1);
        /// </summary>
        /// <param name="voltage">输入电压值</param>
        /// <returns></returns>
        public double GetTemperature(int voltage)
        {
            double result = 0;
            double a = 0;
            double b = 0;
            for (int i = 0; i < _yTemp.Length - 1; i++)
            {
                if ((voltage >= _xVoltage[i]) && (voltage < _xVoltage[i + 1]))
                {
                    a = (double)(_yTemp[i + 1] - _yTemp[i]) / (double)(_xVoltage[i + 1] - _xVoltage[i]);
                    b = (double)(_yTemp[i] * _xVoltage[i + 1] - _yTemp[i + 1] * _xVoltage[i]) / (double)(_xVoltage[i + 1] - _xVoltage[i]);
                    result = a * voltage + b;
                    break;
                }
                else if (voltage < _xVoltage[0])
                {
                    i = 0;
                    a = (double)(_yTemp[i + 1] - _yTemp[i]) / (double)(_xVoltage[i + 1] - _xVoltage[i]);
                    b = (double)(_yTemp[i] * _xVoltage[i + 1] - _yTemp[i + 1] * _xVoltage[i]) / (double)(_xVoltage[i + 1] - _xVoltage[i]);
                    result = a * voltage + b;
                    break;
                }
                else if (voltage >= _xVoltage[24])
                {
                    //对于大于数组中最大值，用index 21和24拟合曲线比较合适。
                    a = (double)(_yTemp[24] - _yTemp[21]) / (double)(_xVoltage[24] - _xVoltage[21]);
                    b = (double)(_yTemp[21] * _xVoltage[24] - _yTemp[24] * _xVoltage[21]) / (double)(_xVoltage[24] - _xVoltage[21]);
                    result = a * voltage + b;
                    break;
                }
            }
            return Math.Round(result, 2);
        }
    }
}
