using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using BiBsps.BiGlobalFiies;
using BiBsps.BiOven;
using BiBsps.BiProtocols.TosaBosa;
using BiBsps.BiLog;
using log4net;
namespace BiBsps.BiDrivers.TosaBosa
{
    internal class Tosa32G:BaseDriver
    {
        protected void InitParam(Dictionary<string, string> param)
        {
            try
            {
                DataMember.FloorNum = int.Parse(param["Floor"]);
                DataMember.BoardNum = int.Parse(param["Locate"]);
                DataMember.BoardName = param["Name"];
                DataMember.SeatsCount = int.Parse(param["SEATS_COUNT"]);
                SeatsCount = DataMember.SeatsCount;
                DataMember.ReadCurrentk = double.Parse(param["READ_CURRENTK"]);
                DataMember.ReadCurrentb = double.Parse(param["READ_CURRENTB"]);
                var calBias = param["CAL_BIAS_ARRAY"].Split(',');
                foreach (var val in calBias)
                {
                    DataMember.CalBiasPoint.Add(int.Parse(val));
                }

                DataMember.TargetCurrent = double.Parse(param["TARGET_CURRENT"]);
                DataMember.SetBiasStep = int.Parse(param["BIAS_STEP"]);
                var setBiasRange = param["SET_BIAS_RANGE"].Split(',');
                DataMember.SetBiasRangeMin = int.Parse(setBiasRange[0]);
                DataMember.SetBiasRangeMax = int.Parse(setBiasRange[1]);

                DataMember.ConnectCheckBias = int.Parse(param["CONNECT_CHECK_BIAS"]);

                var iccRange = param["CONNECT_CHECK_ICC_RANGE"].Split(',');
                DataMember.ConnectIccRangeMin = int.Parse(iccRange[0]);
                DataMember.ConnectIccRangeMax = int.Parse(iccRange[1]);

                DataMember.TargetOvenTemperature = double.Parse(param["TARGET_OVEN_TEMPERATURE"]);

                DataMember.DeltaIcck = double.Parse(param["DELTA_ICCK"]);
                DataMember.DeltaIccb = double.Parse(param["DELTA_ICCB"]);

                DataMember.MesStep = param["MES_STEP"];
                DataMember.OvenPort = param["OvenPort"];
                DataMember.CtrlPort = param["ControlPort"];
                DataMember.IsMesCheck = bool.Parse(param["MES_CHECK"]);
                DataMember.OvenTempCheck = bool.Parse(param["OVEN_TEMP_CHECK"]);
                DataMember.IsHold = bool.Parse(param["HOLD_FLAG"]);
            }
            catch (Exception ex)
            {
                Log.LogError("load param unsuccess: " + ex.Message);
                Log.LogError(ex.StackTrace);
            }
            Log.LogInfo("load param success");

        }

        protected  string GetTempFile()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configurations\TEMP_TOSA32G.xml");
        }

        protected override void CalculateTargetBias()
        {
            var calBias = DataMember.CalBiasPoint.ToArray();

            var iccData = new List<Dictionary<int, double>>();
            foreach (var bias in calBias)
            {
                SetSyncBias(bias);
                Log.LogInfo("read each position icc.");
                iccData.Add(ReadIcc());
            }
            StepTuneBias(0, calBias[calBias.Length - 1], DataMember.SetBiasStep, SetSyncBias);

            foreach (var seat in Seats)
            {
                var yColumn = (from x in calBias select Convert.ToDouble(x)).ToArray();//40,60
                var xColumn = (from x in iccData select x[seat - 1] - InitIccSet[seat]).ToArray();
                var bias = Convert.ToInt32(CalculateLinearFittingValue(xColumn, yColumn, DataMember.TargetCurrent));
                CalBias[seat] = bias;
                var result = bias >= DataMember.SetBiasRangeMin && bias <= DataMember.SetBiasRangeMax;
                Log.LogInfo(" product:[" + SnDict[seat] + "]  calculate target currents :" + (result ? "PASS" : "FAIL\t target_currents_adc=" + bias + "|[" + DataMember.SetBiasRangeMin + "," + DataMember.SetBiasRangeMax + "]"));
            }
            SaveCurrentInfo();

        }
        protected override void SetCalBias()
        {
            Log.LogInfo("Set Target Bias ...");
            lock (FloorLock[DataMember.FloorNum])
            {
                foreach (var seat in Seats)
                {
                    StepTuneBias(CalBias[seat], 0, DataMember.SetBiasStep, x => { SetSingleBias(seat, x); });
                }
            }
        }
        protected override void SaveCurrentInfo()
        {
            Log.LogInfo("Save Target Bias ...");
            var currents = new BoardCurrents {BoardName = DataMember.BoardName};
            foreach (var seat in Seats)
            {
                var curs = new SeatInit()
                {
                    Sn = SnDict[seat],
                    Position = seat,
                    CurrentDac0 = CalBias[seat],
                    Icc0 = InitIccSet[seat]
                };
                currents.SlotInit.Add(curs);
            }
            TempInit.SaveCalData(currents);
        }
     
        private static double CalculateLinearFittingValue(double[] xColumn, double[] yColumn, double xTarget)
        {
            return Calculation.Slope(yColumn, xColumn) * xTarget + Calculation.Intercept(yColumn, xColumn);
        }
        public Tosa32G(Dictionary<string, string> param, ILog logger)
        {
            Log = new DriverLog(logger, "TOSA32G", param["Name"]);
            TempInit = new InitTemp(GetTempFile());
            InitParam(param);
            
            if (OvenControl == null)
                OvenControl = new OvenControl(new Comm(DataMember.OvenPort, 9600));
            CtrlCom = new ProtocolTosa32G(DataMember.BoardNum, new Comm(DataMember.CtrlPort), Log);
            
        }
        public override string GetVersion()
        {
            return "TOSA32G Version:  V2.00";
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
            SetOvenTemperature();
            lock (FloorLock[floorNum])
            {
                OpenDc();
            
                CtrlCom.EnableBiasSync(true);
                SetSyncBias(0);
                Thread.Sleep(1000);
                SetSyncBias(DataMember.ConnectCheckBias);
                Thread.Sleep(1000);
                var icc = ReadIcc();
                SetSyncBias(0);
                CtrlCom.EnableBiasSync(false);
                CloseDc();

                Log.LogInfo("Check Icc & Tec...");
                foreach (var seat in Seats)
                {
                    int seatIndex = seat - 1;
                    ret[seat] = (icc[seatIndex] > DataMember.ConnectIccRangeMin && icc[seatIndex] < DataMember.ConnectIccRangeMax);
                               
                    Log.LogInfo("SEAT:" + seat);
                    Log.LogInfo("ICC\tSPEC(<min>,<max>)\tValue:<value>".Replace("<min>", DataMember.ConnectIccRangeMin.ToString()).Replace("<max>", DataMember.ConnectIccRangeMax.ToString()).Replace("<value>", icc[seatIndex].ToString(CultureInfo.InvariantCulture)));
                   
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
                throw new Exception(" Oven Temperature check fail!");
            }
            else
            {
                return ret;
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

               
                var iccDac = ReadIcc();
                
                int[] mpdDac, currentDac;
                CtrlCom.ReadCurrentDacAndMpdDac(out currentDac, out mpdDac);
               

                var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    var entry = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("PCName", Environment.MachineName),
                        new KeyValuePair<string, string>("BoardName", boardName),
                        new KeyValuePair<string, string>("Floor", floorNum.ToString()),
                        new KeyValuePair<string, string>("Seat", seat.ToString()),
                        new KeyValuePair<string, string>("BoardName", boardName),
                        new KeyValuePair<string, string>("CH", currentDac[seatIndex * 4].ToString()),
                        new KeyValuePair<string, string>("MPD", mpdDac[seatIndex * 4].ToString()),
                        new KeyValuePair<string, string>("ICC",iccDac[seatIndex].ToString("F2"))
                    };
                    if (DataMember.OvenTempCheck)
                    {
                        entry.Add(new KeyValuePair<string, string>("OVEN_Temperature", ovenTemperature.ToString("F2")));
                    }
                   
                    SeatInit info;
                    if (TempInit.GetCalData(boardName, seat, out info))
                    {
                        entry.Add(new KeyValuePair<string, string>("DAC", info.CurrentDac0.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("ICC0", info.Icc0.ToString("F2")));
                        entry.Add(new KeyValuePair<string, string>("LDI", BiConvert.ConvertToLdi(iccDac[seatIndex], info.Icc0, DataMember.DeltaIcck, DataMember.DeltaIccb).ToString("F2")));
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
        public override void CatchException(int seat)
        {
            
            lock (FloorLock[DataMember.FloorNum])
            {
                StepTuneBias(0, CalBias[seat], DataMember.SetBiasStep, x => { SetSingleBias(seat, x); });
            }
        }
     
    }
}
