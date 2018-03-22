using System;
using System.Collections.Generic;
using System.Linq;
using BiBsps.BiGlobalFiies;
using BiBsps.BiLog;

namespace BiBsps.BiProtocols.TosaBosa
{

    public abstract class BaseProtocol:IProtocol
    {
        protected int SeatsCount;
        protected int BoardNum;
        protected BaseCom Comm;
        protected DriverLog Log;
      
       
        protected byte[] WrapCmd(byte[] cmd, byte boardAddress)
        {
            InsertTxChecksum(ref cmd);
            var sendCmd = new List<byte>();
            sendCmd.AddRange(new byte[] { 0xaa, 0x55, boardAddress });
            sendCmd.AddRange(cmd);
            sendCmd.AddRange(new byte[] { 0x55, 0xaa });
            return sendCmd.ToArray();
        }
        protected void InsertTxChecksum(ref byte[] rData)
        {
            var bip8 = Convert.ToByte((rData[0] & 0x0F) ^ rData[1] ^ rData[2] ^ rData[3]);
            var bip4 = Convert.ToByte(((bip8 & 0xF0) >> 4) ^ (bip8 & 0x0F));
            bip4 <<= 4;
            bip4 &= 0xF0;
            rData[0] |= bip4;
        }
        private static string GetDataStr(IEnumerable<byte> bytes)
        {
            return bytes.Aggregate("", (current, t) => current + (t.ToString("X2") + " "));
        }
        protected byte[] QueryProduct(byte[] cmd, int delay)
        {
            var address = BoardNum;
            var send = WrapCmd(cmd, (byte)address);
            Log.LogDebug ("send data:\t"+GetDataStr(send));
            var recv = Comm.Query(send, delay);
            Log.LogDebug("recv data:\t" + GetDataStr(recv));
            return recv;
        }

        protected byte[] QueryTec(byte[] cmd, int delay)
        { 
            var address = BoardNum + 4;
            var send = WrapCmd(cmd, (byte)address);
            Log.LogDebug("send data:\t" + GetDataStr(send));
            var recv = Comm.Query(send, delay);
            Log.LogDebug("recv data:\t" + GetDataStr(recv));
            return recv ;
        }

        protected BaseProtocol( int slotNum, BaseCom com,DriverLog log)
        {
            SeatsCount = 16;
            BoardNum = slotNum;
            Comm = com;
            Log = log;
        }
       
        public abstract void SelectProductType();
        public abstract void InitBurnInMode();
        public abstract void EnableVoltage(List<int> seats, bool enable);
        public abstract void EnableLaser(List<int> seats, bool enable);
        public abstract void EnableBiasSync( bool enable);
        public abstract void SetSyncBias(int value);
        public abstract void SetSyncBiasByChannel(int channel, int value);
        public abstract void SelectProduct(int position);
        public abstract bool SetSingleBiasByChannel(int channel, int value);
        public abstract void SetSingleBias(int position, int value);
        public abstract int[] ReadIccDac();
        public abstract int[] ReadItecDac();
        public abstract void ReadCurrentDacAndMpdDac(out int[] currentDac, out int[] mpdDac);
        public abstract void ReadTecTemperatureDac(out int[] tecDac, out int t1Dac, out int t2Dac);
        public abstract void SetTecTemperatureDac(int position,int tecDac);
        public abstract void CloseTecTemperature(int positon = 0);
        public abstract bool[] GetVoltageState();
    }
}
