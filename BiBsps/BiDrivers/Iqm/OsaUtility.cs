using BiBsps.BiGlobalFiies;
using System;
using System.Collections.Generic;

namespace BiBsps.BiDrivers.Iqm
{
    public static class OsaUtility
    {
        private static Dictionary<int, Comm> commMap; 
        static OsaUtility()
        {
            commMap = new Dictionary<int, Comm>();
            for (int i = 1; i <= 3; i++)
                commMap[i] = new Comm(ConfigReader.GetItem("Floor" + i));
        }

        private static byte[] WrapCmd(byte[] cmd, byte boardAddress)
        {
            Console.WriteLine("SEND:\t" + GetString(cmd));
            InsertTxChecksum(ref cmd);
            List<byte> sendCmd = new List<byte>();
            sendCmd.AddRange(new byte[] { 0xaa, 0x55, boardAddress });
            sendCmd.AddRange(cmd);
            sendCmd.AddRange(new byte[] { 0x55, 0xaa });
            return sendCmd.ToArray();
        }
        private static void InsertTxChecksum(ref byte[] rData)
        {
            byte bip8 = Convert.ToByte((rData[0] & 0x0F) ^ rData[1] ^ rData[2] ^ rData[3]);
            byte bip4 = Convert.ToByte(((bip8 & 0xF0) >> 4) ^ (bip8 & 0x0F));
            bip4 <<= 4;
            bip4 &= 0xF0;
            rData[0] |= bip4;
        }
        private static string GetString(byte[] bytes)
        {
            string temp = "";
            foreach (byte b in bytes)
            {
                temp += b.ToString("X") + "\t";
            }
            return temp;
        }

        public static byte[] Query(int floor,int number, byte[] cmd, int delay)
        {
            return commMap[floor].Query(WrapCmd(cmd, (byte) number), 5000, delay);
        }
    }
}
