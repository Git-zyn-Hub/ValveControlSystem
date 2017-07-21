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
        private int _checksum = 0;
        public SendDataPackage()
        {

        }
        /// <summary>
        /// 数据帧头	 地址	类型	   长度    工具编号	  校验和
        ///   4字节  1字节  1字节   1字节    1字节      2字节
        /// </summary>
        /// <param name="destinationAddr">目的地址</param>
        /// <param name="dataType">类型</param>
        /// <param name="dataContent">信息净荷</param>
        /// <returns></returns>
        public byte[] PackageSendData(byte destinationAddr, byte dataType, byte toolNo)
        {
            byte[] result;
            //int lengthContent = 0;
            //lengthContent = 2 + dataContent.Length;//内容长度为长度后面的字节数：信息长度+2字节校验和。
            int lengthTotal = 10;
            result = new byte[lengthTotal];
            result[0] = _frameHeader1;
            result[1] = _frameHeader2;
            result[2] = _frameHeader3;
            result[3] = _frameHeader4;
            result[4] = destinationAddr;
            result[5] = dataType;
            result[6] = (byte)lengthTotal;
            result[7] = toolNo;

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
