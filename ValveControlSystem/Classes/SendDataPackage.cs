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
        private byte[] _delayBytes = new byte[8];
        private int _checksum = 0;
        public SendDataPackage()
        {

        }
        /// <summary>
        /// 数据帧头	 方向  类型  长度    工具编号	   校验和
        ///   4字节  1字节 1字节 1字节    1字节      2字节
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
        /// 数据帧头	 方向   类型   长度    工具编号    指令编号   校验和
        ///   4字节  1字节  1字节  1字节     1字节      1字节     2字节
        /// </summary>
        /// <param name="toolNo">工具编号</param>
        /// <param name="cmdType">指令编号</param>
        /// <returns></returns>
        public byte[] PackageSendData(byte toolNo, CommandType cmdType)
        {
            byte[] result;
            //int lengthContent = 0;
            //lengthContent = 2 + dataContent.Length;//内容长度为长度后面的字节数：信息长度+2字节校验和。
            int length = 4;
            int lengthTotal = 7 + length;
            result = new byte[lengthTotal];
            result[0] = _frameHeader1;
            result[1] = _frameHeader2;
            result[2] = _frameHeader3;
            result[3] = _frameHeader4;
            result[4] = _orientation;
            result[5] = (byte)CommandTypeCommon.普通指令;
            result[6] = (byte)length;
            result[7] = toolNo;
            result[8] = (byte)cmdType;

            _checksum = 0;
            for (int i = 0; i < lengthTotal - 2; i++)
            {
                _checksum += result[i];
            }
            result[lengthTotal - 2] = (byte)((_checksum & 0xff00) >> 8);
            result[lengthTotal - 1] = (byte)(_checksum & 0xff);

            return combineBytes(_delayBytes, result);
        }

        /// <summary>
        /// 数据帧头	 方向  类型  长度    内容	   校验和
        ///   4字节  1字节 1字节 1字节   n字节   2字节
        /// </summary>
        /// <param name="cmdTypeCommon">类型</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public byte[] PackageSendData(CommandTypeCommon cmdTypeCommon, byte[] content)
        {
            byte[] result;
            //int lengthContent = 0;
            //lengthContent = 2 + dataContent.Length;
            int length = content.Length + 2;//内容长度为长度后面的字节数：信息长度+2字节校验和。
            int lengthTotal = 7 + length;
            result = new byte[lengthTotal];
            result[0] = _frameHeader1;
            result[1] = _frameHeader2;
            result[2] = _frameHeader3;
            result[3] = _frameHeader4;
            result[4] = _orientation;
            result[5] = (byte)cmdTypeCommon;
            result[6] = (byte)length;
            for (int i = 0; i < content.Length; i++)
            {
                result[i + 7] = content[i];
            }

            _checksum = 0;
            for (int i = 0; i < lengthTotal - 2; i++)
            {
                _checksum += result[i];
            }
            result[lengthTotal - 2] = (byte)((_checksum & 0xff00) >> 8);
            result[lengthTotal - 1] = (byte)(_checksum & 0xff);

            return combineBytes(_delayBytes, result);
        }

        private byte[] combineBytes(byte[] firstBytes, byte[] secondBytes)
        {
            byte[] result = new byte[firstBytes.Length + secondBytes.Length];
            Buffer.BlockCopy(firstBytes, 0, result, 0, firstBytes.Length);
            Buffer.BlockCopy(secondBytes, 0, result, firstBytes.Length, secondBytes.Length);
            return result;
        }
    }
}
