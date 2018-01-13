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
        工具循环阀关闭指令 = 1,
        工具循环阀打开指令,
        工具测试阀关闭指令,
        工具测试阀打开指令,
        工具氮气打开循环阀指令,
        工具唤醒指令,
        工具休眠指令,
        循环阀氮气关闭指令,
        工具高压启用指令,
        低压禁用指令,
        高压禁用指令_测试阀可能处于锁定开井状态,
        高压禁用指令,
        软件欠压禁用指令,
        序列模式测试阀打开指令,
        序列模式测试阀关闭指令,
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

    public enum CommandTypeCommon
    {
        /// <summary>
        /// 27条指令
        /// </summary>
        普通指令 = 0x00,
        回放指令 = 0x01,
        擦除指令 = 0x02,
        地面预设指令 = 0x04,
        对时指令 = 0x05,
        实时数据 = 0x06,
        开始or停止实时数据 = 0x07
    }

    public enum ToolNo
    {
        None = 0,
        Tool_1,
        Tool_2,
        Undefined
    }

    public enum PressureUnit
    {
        MPa,
        PSI
    }
    public enum TemperatureUnit
    {
        摄氏度,
        华氏度
    }
}
