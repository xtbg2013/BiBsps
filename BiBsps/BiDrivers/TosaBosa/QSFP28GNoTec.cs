using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using log4net;
using BiBsps.BiGlobalFiies;

namespace BiBsps.BiDrivers.TosaBosa
{
    internal class Qsfp28GNoTec: Qsfp28G
    {
        public Qsfp28GNoTec(Dictionary<string, string> param, ILog logger):base(param,logger)
        {
            TempInit = new InitTemp(GetTempFile());
        }
        protected new string GetTempFile()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configurations\TEMP_TOSA28GGen2.xml");
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
                SetSyncBias(0);
                CtrlCom.EnableBiasSync(false);
                CloseDc();

                Log.LogInfo("Check Icc & Tec...");
                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    ret[seat] = (icc[seatIndex] > DataMember.ConnectIccRangeMin && icc[seatIndex] < DataMember.ConnectIccRangeMax);
                                
                    Log.LogInfo("SEAT:" + seat);
                    Log.LogInfo("ICC\tSPEC(<min>,<max>)\tValue:<value>".Replace("<min>", DataMember.ConnectIccRangeMin.ToString("F2")).Replace("<max>", DataMember.ConnectIccRangeMax.ToString()).Replace("<value>", icc[seatIndex].ToString(CultureInfo.InvariantCulture)));
                    
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
                    var currentTemperature = OvenControl.GetBoardTemperature(floorNum, boardNum);
                    Log.LogInfo("Oven Temperature=" + currentTemperature);
                    if (currentTemperature > DataMember.CheckTemperature)
                    {
                        Log.LogInfo("Oven Temperature Check Pass...");
                        return ret;
                    }
                    Thread.Sleep(1000);
                } while (DateTime.Now.Subtract(start).TotalMinutes < DataMember.OvenTimeout);
                throw new Exception("Oven Temperature check fail!");
            }
            else
            {
                return ret;
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

                int[] mpdDac, currentDac;
                CtrlCom.ReadCurrentDacAndMpdDac(out currentDac, out mpdDac);

                Dictionary<int, double> temp;
                double t1, t2;
                ReadTecTemperature(out temp, out t1, out t2);

                var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    var entry = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("Floor", floorNum.ToString()),
                        new KeyValuePair<string, string>("Seat", seat.ToString()),
                        new KeyValuePair<string, string>("BoardName", boardName),
                        new KeyValuePair<string, string>("T1", t1.ToString("F2")),
                        new KeyValuePair<string, string>("ICC", icc[seatIndex].ToString("F2")),
                        new KeyValuePair<string, string>("PCName",Environment.MachineName),
                        new KeyValuePair<string, string>("BoardName", boardName),
                        new KeyValuePair<string, string>("Seat", seat.ToString())
                    };
                    if (DataMember.OvenTempCheck)
                    {
                        entry.Add(new KeyValuePair<string, string>("OVEN_Temperature", ovenTemperature.ToString("F2")));
                    }
                    for (var channel = 0; channel < 4; channel++)
                    {
                        entry.Add(new KeyValuePair<string, string>("CH" + channel, currentDac[seatIndex * 4 + channel].ToString()));
                        entry.Add(new KeyValuePair<string, string>("MPD" + channel, mpdDac[seatIndex * 4 + channel].ToString()));
                    }
                    SeatInit info;
                    if (TempInit.GetCalData(boardName, seat, out info))
                    {
                        entry.Add(new KeyValuePair<string, string>("DAC1", info.CurrentDac0.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("DAC2", info.CurrentDac1.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("DAC3", info.CurrentDac2.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("DAC4", info.CurrentDac3.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("ICC0", info.Icc0.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("LDI", BiConvert.ConvertToLdi(icc[seatIndex], info.Icc0, DataMember.DeltaIcck, DataMember.DeltaIccb).ToString("F2")));
                    }
                    ret[seat] = entry;
                }
                return ret;
            }
        }

        public override bool SetUpTemperature(double target)
        {
            return true;
        }
        public override bool TearDownTemperature()
        {
            return true;
        }
        public override string GetVersion()
        {
            return "QSFP28NOTEC:V2.00";
        }
    }
}
