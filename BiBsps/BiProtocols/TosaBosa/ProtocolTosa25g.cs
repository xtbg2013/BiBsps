using System;
using System.Collections.Generic;
using System.Linq;
using BiBsps.BiGlobalFiies;
using BiBsps.BiLog;

namespace BiBsps.BiProtocols.TosaBosa
{
    public class ProtocolTosa25G : BaseProtocol
    {
        private readonly int _delayTime;
        public ProtocolTosa25G( int boardNum, BaseCom com, DriverLog log) : base(boardNum, com, log)
        {
            _delayTime = 10000;
        }
        public override void SelectProductType()
        {
             
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Select product type");
            do
            {
                var cmd = new byte[] { 0x01, 0x0e, 0xbe, 0xef };
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Select product type expeption ");
            //throw new Exception("Initilize firmware exception");
        }
        public override void InitBurnInMode()
        {
           
            const int espectLength = 260;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Init burnin mode");
            do
            {
                var cmd = new byte[] { 0x00, 0x09, 0x00, 0x00 };
              
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                {
                    return;
                }
                retry++;
            } while (retry < retryOut);
            Log.LogError("Init burnin mode Exception");
            //throw new Exception("Set BurnIn Mode Exception.");

        }
        public override void EnableVoltage(List<int> seats, bool enable)
        {
            var reg = 0x0;
            if (enable)
            {
                reg = seats.Select(seat => seat - 1).Aggregate(reg, (current, seatIndex) => current | 0x0001 << seatIndex);
            }
            var high = (byte)(reg % 0x10000 / 0x100);
            var low = (byte)(reg % 0x100);
            const int espectLength = 4;
            const int retryOut = 2;
            var retry = 0;
            Log.LogInfo("Enable voltage,enalble = "+enable);
            do
            {
                var cmd = new byte[] { 0x01, 0xC3, high, low };
                
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength && response[2] == cmd[2] && response[3] == cmd[3])
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Open voltage exception");
            //throw new Exception("Switch Voltage Exception");
        }
        public override void EnableLaser(List<int> seats, bool enable)
        {
            var reg = 0x0;
            if (enable)
            {
                reg = seats.Select(seat => seat - 1).Aggregate(reg, (current, seatIndex) => current | 0x0001 << seatIndex);
            }
            var high = (byte)(reg % 0x10000 / 0x100);
            var low = (byte)(reg % 0x100);
            const int espectLength = 4;
            const int retryOut = 2;
            var retry = 0;
            Log.LogInfo("Enable laser,enable = "+enable);
            do
            {
                var cmd = new byte[] { 0x01, 0xc2, high, low };
               
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength && response[2] == cmd[2] && response[3] == cmd[3])
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Open laser exception");
           // throw new Exception("Switch Laser Exception");
        }
        public override void EnableBiasSync(bool enable)
        {
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Enbale set sync bias,enable = "+ enable);
            do
            {
                var cmd = new byte[] { 0x01, 0xc0, (byte)(enable ? 0xbe : 0xca), (byte)(enable ? 0xef : 0xfe) };
                
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Enbale set sync bias exception");
            //throw new Exception("Switch Sync Bias Exception");
        }
        public override void SetSyncBias(int value)
        {
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Set sync bias: " + value);
            do
            {
                var cmd = new byte[] { 0x01, 0xd2, (byte)value, 0x01 };
                
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Set sync bias: " + value + " exception");
           // throw new Exception("Set Sync Bias Exception");
        }
        public override void SetSyncBiasByChannel(int channel, int value)
        {
            throw new NotImplementedException();
        }
        public override void SelectProduct(int position)
        {
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Select product,position =" + position);
            if (position < 1 || position > 16)
            {
                Log.LogError("position is invalid, position must in the range[1, 16]");
                return;
            }
            do
            {
                var cmd = new byte[] { 0x01, 0x0f, 0x00, Convert.ToByte(position-1) };
                 
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Select product, position = " + position + " exception");
           // throw new Exception("SelectProduct Exception");
        }
        public override bool SetSingleBiasByChannel(int channel, int value)
        {
            throw new NotImplementedException();

        }
        public override void SetSingleBias(int position, int value)
        {
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Set single bias " + value);
            SelectProduct(position);
            do
            {
                var cmd = new byte[] { 0x01, 0xAD, (byte)value, 0x01 };
                
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Set single bias " + value);
            //throw new Exception("Set single bias " + value);
        }
        public override int[] ReadIccDac()
        {
            const int espectLength = 36;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Read icc dac");
            var ret = new int[SeatsCount];
            do
            {
                var cmd = new byte[] { 0x00, 0x0A, 0x00, 0x00 };
                 
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                {
                    
                    for (var i = 0; i < SeatsCount; i++)
                    {
                        var high = Convert.ToInt32(response[i * 2]) << 4;
                        var low = Convert.ToInt32(response[i * 2 + 1]) >> 4;
                        ret[i] = high + low;
                    }
                    return ret;
                }
                retry++;
            } while (retry < retryOut);
            Log.LogError("Read icc dac exception");
            return ret;
            //throw new Exception("Read Icc Exception");
        }
        public override int[] ReadItecDac()
        {
            const int espectLength = 36;
            const int retryOut = 1;
            var retry = 0;
            var ret = new int[SeatsCount];
            Log.LogInfo("Read itec dac");
            do
            {
                var cmd = new byte[] { 0x00, 0x40, 0x00, 0x00 };
                 
                var response = QueryTec(cmd, _delayTime);
                if (response.Length == espectLength)
                {
                    
                    for (var i = 0; i < SeatsCount; i++)
                    {
                        var high = Convert.ToInt32(response[i * 2]) << 4;
                        var low = Convert.ToInt32(response[i * 2 + 1]) >> 4;
                        ret[i] = high + low;
                    }
                    return ret;
                }
                retry++;
            } while (retry < retryOut);
            Log.LogError("Read itec dac exception");
            return ret;
            //throw new Exception("Read ITec Dac Exception");
        }
        public override void ReadCurrentDacAndMpdDac(out int[] currentDac, out int[] mpdDac)
        {
            const int espectLength = 260;
            const int retryOut = 1;
            var retry = 0;
            currentDac = new int[SeatsCount * 4];
            mpdDac = new int[SeatsCount * 4];
            Log.LogInfo("Read current dac and mdp dac");
            do
            {
                var cmd = new byte[] { 0x00, 0x09, 0x00, 0x00 };
                 
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                {
                    for (var i = 0; i < SeatsCount * 4; i++)
                    {
                        var locate = (i / 4) * 16 + (i % 4) * 2;
                        mpdDac[i] = (Convert.ToInt32(response[locate]) << 4) + (Convert.ToInt32(response[locate + 1]) >> 4);
                        currentDac[i] = (Convert.ToInt32(response[locate + 8]) << 4) + (Convert.ToInt32(response[locate + 8 + 1]) >> 4);
                    }
                    return;
                }
                retry++;
            } while (retry < retryOut);
            Log.LogError("Read current dac and mdp dac excepiton");
           // throw new Exception("ReadCurrentDacAndMpdDac Exception");
        }
        public override void ReadTecTemperatureDac(out int[] tecDac, out int t1Dac, out int t2Dac)
        {
            const int espectLength = 68;
            const int retryOut = 1;
            var retry = 0;
            t1Dac = 0;
            t2Dac = 0;
            tecDac = new int[SeatsCount];
            Log.LogInfo("read tec temperature dac");
            do
            {
                var cmd = new byte[] { 0x00, 0x41, 0x00, 0x00 };
                 
                var response = QueryTec(cmd, _delayTime);
                if (response.Length == espectLength)
                {
                    for (var i = 0; i < SeatsCount; i++)
                    {
                        tecDac[i] = (Convert.ToInt32(response[i * 2]) << 4) + (Convert.ToInt32(response[i * 2 + 1]) >> 4);
                    }
                    t1Dac = (Convert.ToInt32(response[32]) << 4) + (Convert.ToInt32(response[33]) >> 4);
                    t2Dac = (Convert.ToInt32(response[34]) << 4) + (Convert.ToInt32(response[35]) >> 4);
                    return;
                }
                retry++;
            } while (retry < retryOut);
            Log.LogError("read tec temperature dac exception");
            //throw new Exception("ReadTemperatureDac Exception");
        }
        public override void SetTecTemperatureDac(int position,int tecDac)
        {

            var high = (byte)(tecDac >> 8);
            var low = (byte)(tecDac);
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Set tec temperature");
            do
            {
                var cmd = new byte[] { 0x00, Convert.ToByte(0x12+position-1), high, low };
                 
                var response = QueryTec(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Set tec temperature exception");
           // throw new Exception("SetTecTemperature Exception");
        }

        public override void CloseTecTemperature(int position = 0)
        {
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;
            Log.LogInfo("Close the tec temperature,posotion = " + position);

            do
            {
                var cmd = new byte[] { 0x00, Convert.ToByte(0x22 + position-1), 0x00, 0x00 };
                byte[] response = QueryTec(cmd, _delayTime);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            Log.LogError("Close the tec temperature,posotion = " + position + " exception");
            //throw new Exception("CloseTecTemperature Exception");
        }
        public override bool[] GetVoltageState()
        {
            const int espectLength = 4;
            const int retryOut = 1;
            var retry = 0;

            var voltageState = new bool[SeatsCount];
            Log.LogInfo("Get voltage state");
            do
            {
                var cmd = new byte[] { 0x01, 0x04, 0x00, 0x00 };
                var response = QueryProduct(cmd, _delayTime);
                if (response.Length == espectLength)
                {
                   
                    for (var pos = 0;pos <SeatsCount;pos++)
                    {
                        bool state;
                        var val = Convert.ToByte(Convert.ToInt32(Math.Pow(Convert.ToDouble(2), Convert.ToDouble(pos % 8))));
                        if (pos < 8)
                        {
                            state = Convert.ToInt32(response[3] & val) > 0;
                        }
                        else
                        {
                            state = Convert.ToInt32(response[2] & val) > 0;
                        }
                        voltageState[pos] = state;
                    }
                    return voltageState;
                   
                }
                retry++;
            } while (retry < retryOut);
            Log.LogError("Get voltage state exception");
            return voltageState;
           // throw new Exception("Get voltage state Exception");
        }
    }
}
