using System;
using BiBsps.BiGlobalFiies;

namespace BiBsps.BiOven
{
    public class OvenControl: BaseOvenControl
    {
        private readonly BaseCom _com;
        private readonly int _delayTime;
        private readonly object _ovenLock = new object();
        public OvenControl(BaseCom com)
        {
            _com = com;
            _delayTime = 5000;
        }
        
        public override double GetBoardTemperature(int floor, int number)
        {
            var addr = GetAddress(floor, number);
            const int cmdWord = 0x0052;
            const int paraWord = 0x0000;
            var ret = Query(GetCommand(addr, cmdWord, paraWord), _delayTime);
            if (ret.Length == 10)
                return (Convert.ToDouble(ret[0]) + Convert.ToDouble(ret[1]) * 0x100) * 0.1;
            throw new Exception("Get Board Temperature Exception.");
        }

        public override void SetBoardTemperature(int floor, int number, double temperature)
        {
            var addr = GetAddress(floor, number);
            const int cmdWord = 0x0043;
            var paraWord = Convert.ToInt32(temperature * 10);
            var ret = Query(GetCommand(addr, cmdWord, paraWord), _delayTime);
            if (ret.Length == 10)
                return;
            throw new Exception("Set Board Temperature Exception");
        }

        public override void StartBoardOven(int floor, int number)
        {
            var addr = GetAddress(floor, number);
            const int cmdWord = 0x0643;
            const int paraWord = 0x0000;
            var ret = Query(GetCommand(addr, cmdWord, paraWord), _delayTime);
            if (ret.Length == 10)
                return;
            throw new Exception("Start Board Oven Exception");
        }

        public override void StopBoardOven(int floor, int number)
        {
            var addr = GetAddress(floor, number);
            const int cmdWord = 0x1543;
            var paraWord = 0x000C;
            var ret = Query(GetCommand(addr, cmdWord, paraWord), _delayTime);
            if (ret.Length == 10)
                return;
            throw new Exception("Stop Board Oven Exception");
        }

        public override void SetTempOffset(int floor, int number, double temperature)
        {
            var addr = GetAddress(floor, number);
            const int cmdWord = 0x1043;
            var paraWord = Convert.ToInt32(temperature * 10);
            var ret = Query(GetCommand(addr, cmdWord, paraWord), _delayTime);
            if (ret.Length == 10)
                return;
            throw new Exception("Set TOffset Exception");
        }
    
        private static byte[] GetCommand(int addr, int cmdWord, int paraWord)
        {
            var crcWord = GetCrcWord(addr, cmdWord, paraWord);
            return new[] { (byte)addr, (byte)addr
                ,(byte)(cmdWord%0x100), (byte)(cmdWord/0x100)
                , (byte)(paraWord%0x100),(byte)(paraWord/0x100)
                ,(byte)(crcWord%0x100),(byte)(crcWord/0x100)};
        }

        private static int GetCrcWord(int addr, int cmdWord, int paraWord)
        {
            var crc = (addr + cmdWord + paraWord - 0x80) % 0x10000;
            return crc;
        }

        private static int GetAddress(int floor, int number)
        {
            var addr = (floor - 1) * 4 + number + 0x80;
            return addr;
        }
        private byte[] Query(byte[] cmd, int sleep = 0)
        {
            lock (_ovenLock)
            {
                return _com.Query(cmd, sleep);
            }
        }
    }
}
