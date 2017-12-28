using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveControlSystem.Classes
{
    public class GetPressureFromVoltage
    {
        /// <summary>
        /// 根据电压得到压力值，拟合曲线的公式y=0.1586x-0.6932
        /// </summary>
        /// <param name="voltage">电压，单位mv</param>
        /// <returns>压力单位MPa</returns>
        public static double GetPressure(double voltage)
        {
            return 0.1586 * voltage - 0.6932;
        }
    }
}
