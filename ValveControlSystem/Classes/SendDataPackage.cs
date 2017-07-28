using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValveControlSystem
{
    public class SendDataPackage
    {
        private const byte _frameHeader1 = 0xFF;
        private const byte _frameHeader2 = 0x00;
        private const byte _frameHeader3 = 0xAA;
        private const byte _frameHeader4 = 0x55;
        private const byte _orientation = 0xFF;
        private int _checksum = 0;
        public SendDataPackage()
        {

        }
        /// <summary>
        /// 数据帧头	 方向   长度    工具编号	  校验和
        ///   4字节  1字节  1字节    1字节      2字节
        /// </summary>
        /// <param name="toolNo">工具编号</param>
        /// <returns></returns>
        public byte[] PackageSendData(byte toolNo)
        {
            byte[] result;
            //int lengthContent = 0;
            //lengthContent = 2 + dataContent.Length;//内容长度为长度后面的字节数：信息长度+2字节校验和。
            int length = 3;
            int lengthTotal = 6 + length;
            result = new byte[lengthTotal];
            result[0] = _frameHeader1;
            result[1] = _frameHeader2;
            result[2] = _frameHeader3;
            result[3] = _frameHeader4;
            result[4] = _orientation;
            result[5] = (byte)length;
            result[6] = toolNo;

            _checksum = 0;
            for (int i = 0; i < lengthTotal - 2; i++)
            {
                _checksum += result[i];
            }
            result[lengthTotal - 2] = (byte)((_checksum & 0xff00) >> 8);
            result[lengthTotal - 1] = (byte)(_checksum & 0xff);
            return result;
        }

        /// <summary>
        /// 数据帧头	 方向   长度    工具编号    指令编号   校验和
        ///   4字节  1字节  1字节     1字节      1字节     2字节
        /// </summary>
        /// <param name="toolNo">工具编号</param>
        /// <param name="cmdType">指令编号</param>
        /// <returns></returns>
        public byte[] PackageSendData(byte toolNo,CommandType cmdType)
        {
            byte[] result;
            //int lengthContent = 0;
            //lengthContent = 2 + dataContent.Length;//内容长度为长度后面的字节数：信息长度+2字节校验和。
            int length = 4;
            int lengthTotal = 6 + length;
            result = new byte[lengthTotal];
            result[0] = _frameHeader1;
            result[1] = _frameHeader2;
            result[2] = _frameHeader3;
            result[3] = _frameHeader4;
            result[4] = _orientation;
            result[5] = (byte)length;
            result[6] = toolNo;
            result[7] = (byte)cmdType;

            _checksum = 0;
            for (int i = 0; i < lengthTotal - 2; i++)
            {
                _checksum += result[i];
            }
            result[lengthTotal - 2] = (byte)((_checksum & 0xff00) >> 8);
            result[lengthTotal - 1] = (byte)(_checksum & 0xff);
            return result;
        }
    }
}
