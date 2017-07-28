using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveControlSystem.Classes
{
    public class CommandType2StringConverter
    {
        public static string CommandType2String(CommandType cmdType)
        {
            string result;
            switch (cmdType)
            {
                case CommandType.工具1循环阀关闭指令:
                case CommandType.工具1循环阀打开指令:
                case CommandType.工具1测试阀关闭指令:
                case CommandType.工具1测试阀打开指令:
                case CommandType.工具2循环阀关闭指令:
                case CommandType.工具2循环阀打开指令:
                case CommandType.工具2测试阀关闭指令:
                case CommandType.工具2测试阀打开指令:
                case CommandType.工具1氮气打开循环阀指令:
                case CommandType.工具2氮气打开循环阀指令:
                case CommandType.工具1唤醒指令:
                case CommandType.工具1休眠指令:
                case CommandType.工具2唤醒指令:
                case CommandType.工具2休眠指令:
                case CommandType.循环阀氮气关闭指令:
                case CommandType.工具1高压启用指令:
                case CommandType.工具2高压启用指令:
                case CommandType.低压禁用指令:
                case CommandType.高压禁用指令:
                case CommandType.软件欠压禁用指令:
                case CommandType.序列模式测试阀打开指令:
                case CommandType.序列模式测试阀关闭指令:
                    result = cmdType.ToString();
                    break;
                case CommandType.高压禁用指令_测试阀可能处于锁定开井状态:
                    result = "高压禁用指令(测试阀可能处于锁定开井状态)";
                    break;
                case CommandType.阀自动序列A型指令_AVS_A:
                    result = "阀自动序列A型指令(AVS-A)";
                    break;
                case CommandType.阀自动关闭指令_AVC:
                    result = "阀自动关闭指令(AVC)";
                    break;
                case CommandType.阀自动序列B型指令_AVS_B:
                    result = "阀自动序列B型指令(AVS-B)";
                    break;
                case CommandType.阀自动序列A型或者B型_AVS:
                    result = "阀自动序列A型或者B型(AVS)";
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return ((int)cmdType).ToString() + "." + result;
        }
    }
}
