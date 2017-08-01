using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveControlSystem
{
    public enum ConnectType
    {
        Notconnected,
        Ethernet,
        SerialPort
    }

    public enum CommandType
    {
        工具1循环阀关闭指令 = 1,
        工具1循环阀打开指令,
        工具1测试阀关闭指令,
        工具1测试阀打开指令,
        工具2循环阀关闭指令,
        工具2循环阀打开指令,
        工具2测试阀关闭指令,
        工具2测试阀打开指令,
        工具1氮气打开循环阀指令,
        工具2氮气打开循环阀指令,
        工具1唤醒指令,
        工具1休眠指令,
        工具2唤醒指令,
        工具2休眠指令,
        循环阀氮气关闭指令,
        工具1高压启用指令,
        工具2高压启用指令,
        低压禁用指令,
        高压禁用指令_测试阀可能处于锁定开井状态,
        高压禁用指令,
        软件欠压禁用指令,
        阀自动序列A型指令_AVS_A,
        序列模式测试阀打开指令,
        序列模式测试阀关闭指令,
        阀自动关闭指令_AVC,
        阀自动序列B型指令_AVS_B,
        阀自动序列A型或者B型_AVS
    }

    public enum DataLevel
    {
        Default,
        Normal,
        Warning,
        Error
    }

    public enum CommandState
    {
        状态正常 = 0x00,
        状态异常 = 0x99
    }
}
