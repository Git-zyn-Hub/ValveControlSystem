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
                case CommandType.工具循环阀关闭指令:
                case CommandType.工具循环阀打开指令:
                case CommandType.工具测试阀关闭指令:
                case CommandType.工具测试阀打开指令:
                case CommandType.工具氮气打开循环阀指令:
                case CommandType.工具唤醒指令:
                case CommandType.工具休眠指令:
                case CommandType.循环阀氮气关闭指令:
                case CommandType.工具高压启用指令:
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
                default:
                    result = string.Empty;
                    break;
            }
            return ((int)cmdType).ToString() + "." + result;
        }


        public static string CommandType2StringWithNo(CommandType cmdType)
        {
            return "指令" + CommandType2String(cmdType);
        }
    }
}
