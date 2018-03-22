using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using BiBsps.BiGlobalFiies;
using BiBsps.BiOven;
using BiBsps.BiProtocols.TosaBosa;
using BiBsps.BiLog;

namespace BiBsps.BiDrivers.TosaBosa
{
    internal abstract class BaseDriver : BaseBoard
    {
        protected static Dictionary<int, object> FloorLock { get; set; }

        protected DriverLog Log;
        protected BaseProtocol CtrlCom;
        protected InitTemp TempInit;
        protected List<int> Seats { get; set; }
        protected Dictionary<int, double> InitIccSet;
        protected Dictionary<int, string> SnDict;
        protected Dictionary<int, int> CalBias;

        static BaseDriver()
        {
            FloorLock = new Dictionary<int, object>();
            for (var i = 0; i <= 16; i++)
                FloorLock[i] = new object();
        }

        protected BaseDriver()
        {
            Seats = new List<int>();
            InitIccSet = new Dictionary<int, double>();
            SnDict = new Dictionary<int, string>();
            CalBias = new Dictionary<int, int>();

        }


        #region IBoard Implementation

        public override bool EnableBoard()
        {

            var floorNum = DataMember.FloorNum;
            lock (FloorLock[floorNum])
            {
                Log.LogInfo("Enable Board Start...");
                OpenDc();
                Log.LogInfo("Read Icc0...");
                var icc = ReadIcc();
                foreach (var seat in Seats)
                {
                    InitIccSet[seat] = icc[seat - 1];
                }
                CtrlCom.EnableBiasSync(true);


                CalculateTargetBias();
                SaveCurrentInfo();
                CtrlCom.EnableBiasSync(false);
                SetCalBias();

                return true;
            }
        }
        public override bool DisableBoard()
        {
            var floorNum = DataMember.FloorNum;
            lock (FloorLock[floorNum])
            {
                Log.LogInfo("Disable Board Start...");
                CtrlCom.EnableBiasSync(true);
                SetSyncBias(0);
                CtrlCom.EnableBiasSync(false);
                CloseDc();
                Log.LogInfo("Disable Board Finish...");
                return true;
            }
        }

        public override Dictionary<int, string> GetSnSet()
        {
            return null;
        }

        public override void AddSeat(int seat, string sn)
        {

            if (seat <= SeatsCount)
                if (Seats.Contains(seat))
                    Log.LogInfo("seat" + seat + " exists");
                else
                {
                    Seats.Add(seat);
                    SnDict[seat] = sn;
                }
            else
                Log.LogWarn("seat:" + seat + " is Invalid.");
        }

        public override void RemoveSeat(int seat)
        {
            if (seat <= SeatsCount)
                if (!Seats.Contains(seat))
                    Log.LogInfo("seat" + seat + " does not exist.");
                else
                {
                    Seats.Remove(seat);
                    SnDict.Remove(seat);
                }
            else
                Log.LogWarn("seat" + seat + " is Invalid.");
        }

        public override Dictionary<int, List<KeyValuePair<string, string>>> ReadDataSet(string type)
        {
            var boardName = DataMember.BoardName;
            var floorNum = DataMember.FloorNum;
            var boardNum = DataMember.BoardNum;

            lock (FloorLock[floorNum])
            {
                Log.LogInfo("Read Data...");
                double ovenTemperature = 0;
                if (DataMember.OvenTempCheck)
                {
                    ovenTemperature = OvenControl.GetBoardTemperature(floorNum, boardNum);
                }
                var icc = ReadIcc();
                var iTecDac = CtrlCom.ReadItecDac();

                Dictionary<int, double> temp;
                double t1, t2;
                ReadTecTemperature(out temp, out t1, out t2);

                int[] mpdDac, currentDac;
                CtrlCom.ReadCurrentDacAndMpdDac(out currentDac, out mpdDac);


                var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    var entry = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Floor", floorNum.ToString()),
                        new KeyValuePair<string, string>("Seat", seat.ToString()),
                        new KeyValuePair<string, string>("BoardName", boardName)
                    };
                    if (DataMember.OvenTempCheck)
                    {
                        entry.Add(new KeyValuePair<string, string>("OVEN_Temperature", ovenTemperature.ToString(CultureInfo.InvariantCulture)));
                    }
                    for (int channel = 0; channel < 4; channel++)
                    {
                        entry.Add(new KeyValuePair<string, string>("CH" + channel, currentDac[seatIndex * 4 + channel].ToString()));
                        entry.Add(new KeyValuePair<string, string>("MPD" + channel, mpdDac[seatIndex * 4 + channel].ToString()));
                    }
                    entry.Add(new KeyValuePair<string, string>("TEC_TEMPERATURE", temp[seatIndex].ToString(CultureInfo.InvariantCulture)));
                    entry.Add(new KeyValuePair<string, string>("T1", t1.ToString(CultureInfo.InvariantCulture)));
                    entry.Add(new KeyValuePair<string, string>("T2", t2.ToString(CultureInfo.InvariantCulture)));
                    entry.Add(new KeyValuePair<string, string>("ITEC", BiConvert.ConvertDacToITec(iTecDac[seatIndex], DataMember.ReadIteck, DataMember.ReadItecb).ToString(CultureInfo.InvariantCulture)));
                    entry.Add(new KeyValuePair<string, string>("ICC", icc[seatIndex].ToString(CultureInfo.InvariantCulture)));
                    entry.Add(new KeyValuePair<string, string>("PCName", Environment.MachineName));
                    entry.Add(new KeyValuePair<string, string>("BoardName", boardName));
                    entry.Add(new KeyValuePair<string, string>("Seat", seat.ToString()));

                    SeatInit info;
                    if (TempInit.GetCalData(boardName, seat, out info))
                    {
                        entry.Add(new KeyValuePair<string, string>("DAC1", info.CurrentDac0.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("DAC2", info.CurrentDac1.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("DAC3", info.CurrentDac2.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("DAC4", info.CurrentDac3.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("ICC0", info.Icc0.ToString("F2")));
                        // entry.Add(new KeyValuePair<string, string>("LDI", BiConvert.ConvertToLdi(icc[seatIndex], info.icc0, _dataMember.DeltaIcck, _dataMember.DeltaIccb).ToString("F2")));
                    }
                    ret[seat] = entry;
                }
                return ret;
            }
        }

        public override bool SetUpTemperature(double target)
        {
            lock (FloorLock[DataMember.FloorNum])
            {
                CtrlCom.SetTecTemperatureDac(0, BiConvert.ConvertTempToDac(DataMember.TargetTecTemperature, DataMember.SetTemperaturek, DataMember.SetTemperatureb));
                return true;
            }
        }

        public override bool TearDownTemperature()
        {
            lock (FloorLock[DataMember.FloorNum])
            {
                CtrlCom.CloseTecTemperature();
                return true;
            }
        }

        #endregion


        #region Protected Methods

        protected virtual void OpenDc()
        {
            CtrlCom.SelectProductType();
            Thread.Sleep(1000);
            CtrlCom.EnableVoltage(Seats, true);
            Thread.Sleep(2000);
            CtrlCom.EnableLaser(Seats, true);
            Thread.Sleep(1000);
            CtrlCom.InitBurnInMode();
        }

        protected virtual void CloseDc()
        {
            CtrlCom.EnableLaser(Seats, false);
            Thread.Sleep(1000);
            CtrlCom.EnableVoltage(Seats, false);
        }

        protected Dictionary<int, double> ReadIcc()
        {
            Dictionary<int, double> icc = new Dictionary<int, double>();
            var iccdac = CtrlCom.ReadIccDac();
            for (var i = 0; i < SeatsCount; i++)
            {
                icc[i] = BiConvert.ConvertDacToIcc(iccdac[i], DataMember.ReadCurrentk, DataMember.ReadCurrentb);
            }
            return icc;
        }

        protected void SetSyncBias(int value)
        {
            if ((value >= DataMember.SetBiasRangeMin) && (value <= DataMember.SetBiasRangeMax))
            {
                CtrlCom.SetSyncBias(value);

            }
            else
            {
                Log.LogError("The BIAS value: " + value.ToString() + " out of SPEC!");
            }
        }

        protected void StepTuneBias(int target, int refer, int step, Action<int> set)
        {
            var valueToSet = refer;
            while (valueToSet != target)
            {
                var abs = Math.Abs(target - valueToSet);
                valueToSet = abs < step ? target : (valueToSet + step * (target > valueToSet ? 1 : -1));
                set(valueToSet);
                Thread.Sleep(1000);
            }
        }

        protected override Dictionary<int, bool> PreCheckConnections()
        {
            var floorNum = DataMember.FloorNum;
            var boardNum = DataMember.BoardNum;
            Log.LogInfo("Check Connetions...");
            var ret = new Dictionary<int, bool>();
            foreach (var seat in Seats)
                ret[seat] = true;
            Log.LogInfo(GetVersion());

            lock (FloorLock[floorNum])
            {
                SetOvenTemperature();
                OpenDc();
                CtrlCom.EnableBiasSync(true);
                SetSyncBias(0);
                SetSyncBias(DataMember.ConnectCheckBias);

                var icc = ReadIcc();

                Dictionary<int, double> temp;
                double t1;
                double t2;
                ReadTecTemperature(out temp, out t1, out t2);


                SetSyncBias(0);

                CtrlCom.EnableBiasSync(false);
                CloseDc();

                Log.LogInfo("Check Icc & Tec...");
                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    ret[seat] = (icc[seatIndex] > DataMember.ConnectIccRangeMin &&
                                 icc[seatIndex] < DataMember.ConnectIccRangeMax) &&
                                (temp[seatIndex] > DataMember.ConnectTecTempRangeMin &&
                                 temp[seatIndex] < DataMember.ConnectTecTempRangeMax);
                    Log.LogInfo("SEAT:" + seat);
                    Log.LogInfo(
                        $"ICC\tSPEC({DataMember.ConnectIccRangeMin},{DataMember.ConnectIccRangeMax})\tValue:{icc[seatIndex]}");
                    Log.LogInfo(
                        $"TEC\tSPEC({DataMember.ConnectTecTempRangeMin},{DataMember.ConnectTecTempRangeMax})\tValue:{temp[seatIndex]}");
                }
            }

            if (ret.ContainsValue(false))
                return ret;
            if (DataMember.OvenTempCheck)
            {
                Log.LogInfo("Check Oven Temperature...");
                var start = DateTime.Now;
                do
                {
                    double currentTemperature = OvenControl.GetBoardTemperature(floorNum, boardNum);
                    Log.LogInfo("Oven Temperature=" + currentTemperature);
                    if (currentTemperature > DataMember.CheckTemperature)
                    {
                        Log.LogInfo("Oven Temperature Check Pass...");
                        return ret;
                    }

                    Thread.Sleep(1000);
                } while (DateTime.Now.Subtract(start).TotalMinutes < DataMember.OvenTimeout);

                throw new Exception(" Oven Temperature check fail!");
            }
            else
            {
                return ret;
            }
        }

        protected override Dictionary<int, bool> PostCheckConnections()
        {
            return null;
        }

        protected virtual void SetOvenTemperature()
        {
            var floorNum = DataMember.FloorNum;
            var boardNum = DataMember.BoardNum;
            if (DataMember.OvenTempCheck)
            {
                Log.LogInfo("Set Oven Temperature...");
                OvenControl.SetBoardTemperature(floorNum, boardNum, DataMember.TargetOvenTemperature);
                Log.LogInfo("SUCCESS!");

                Log.LogInfo("Start Oven...");
                OvenControl.StartBoardOven(floorNum, boardNum);
                Log.LogInfo("SUCCESS!");
            }
        }

        protected void SetSyncBiasByChannel(int channel, int value)
        {
            if ((value > DataMember.SetBiasRangeMin) && (value < DataMember.SetBiasRangeMax))
            {
                CtrlCom.SetSyncBiasByChannel(channel, value);
            }
            else
            {
                Log.LogError("The BIAS value: " + value.ToString() + " out of SPEC!");
            }
        }

        protected bool SetSingleBiasByChannel(int channel, int value)
        {
            if ((value > DataMember.SetBiasRangeMin) && (value < DataMember.SetBiasRangeMax))
            {

                return CtrlCom.SetSingleBiasByChannel(channel, value);
            }
            else
            {
                Log.LogError("The BIAS value: " + value.ToString() + " out of SPEC!");
                return false;
            }
        }

        protected void SetSingleBias(int seat, int value)
        {
            if ((value >= DataMember.SetBiasRangeMin) && (value <= DataMember.SetBiasRangeMax))
            {
                CtrlCom.SetSingleBias(seat, value);

            }
            else
            {
                Log.LogError("The BIAS value: " + value.ToString() + " out of SPEC!");
            }
        }

        protected void ReadTecTemperature(out Dictionary<int, double> temp, out double t1, out double t2)
        {
            temp = new Dictionary<int, double>();
            t1 = 0;
            t2 = 0;
            int[] tecDac;
            int t1Dac, t2Dac;
            CtrlCom.ReadTecTemperatureDac(out tecDac, out t1Dac, out t2Dac);
            for (int i = 0; i < SeatsCount; i++)
            {
                temp[i] = BiConvert.ConvertDacToTemp(tecDac[i]);
            }
            t1 = BiConvert.ConvertDacToTemp(t1Dac);
            t2 = BiConvert.ConvertDacToTemp(t2Dac);
        }

        #endregion


        protected abstract void CalculateTargetBias();

        protected abstract void SaveCurrentInfo();

        protected abstract void SetCalBias();

        protected static BaseOvenControl OvenControl { get; set; }
    }
}
