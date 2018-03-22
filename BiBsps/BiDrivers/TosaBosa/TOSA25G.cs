using System;
using System.Collections.Generic;
using System.Linq;
using BiBsps.BiGlobalFiies;
using BiBsps.BiOven;
using BiBsps.BiProtocols.TosaBosa;
using BiBsps.BiLog;
using log4net;
namespace BiBsps.BiDrivers.TosaBosa
{
    internal class Tosa25G: BaseDriver
    {
        protected  string GetTempFile()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configurations\TEMP_TOSA25G.xml");
        }
        
        protected override void CalculateTargetBias()
        {
            Log.LogInfo("Calculate Target Bias ...");

            var calBias = DataMember.CalBiasPoint.ToArray();

            var iccData = new List<Dictionary<int, double>>();
            foreach (var bias in calBias)
            {
                SetSyncBias(bias);
                iccData.Add(ReadIcc());
            }
            StepTuneBias(0, calBias[calBias.Length - 1], DataMember.SetBiasStep, SetSyncBias);
            
            foreach (var seat in Seats)
            {
                var yColumn = (from x in calBias select Convert.ToDouble(x)).ToArray();//40,60
                var xColumn = (from x in iccData select x[seat-1] - InitIccSet[seat]).ToArray();
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
       
        private void InitParam(Dictionary<string, string> param)
        {
            try
            {
                DataMember.FloorNum = int.Parse(param["Floor"]);
                DataMember.BoardNum = int.Parse(param["Locate"]);
                DataMember.BoardName = param["Name"];
                DataMember.SeatsCount = int.Parse(param["SEATS_COUNT"]);
                SeatsCount = DataMember.SeatsCount;
                DataMember.SetBiasStep = int.Parse(param["SET_BIAS_STEP"]);
                DataMember.TargetOvenTemperature = int.Parse(param["TARGET_OVEN_TEMPERATURE"]);
                DataMember.TargetTecTemperature = int.Parse(param["TARGET_TEC_TEMPERATURE"]);
                DataMember.ConnectCheckBias = int.Parse(param["CONNECT_CHECK_BIAS"]);
                var iccRange = param["CONNECT_CHECK_ICC_RANGE"].Split(',');
                DataMember.ConnectIccRangeMin = int.Parse(iccRange[0]); //40
                DataMember.ConnectIccRangeMax = int.Parse(iccRange[1]); //300

                var calBias = param["CAL_BIAS_ARRAY"].Split(',');
                foreach (var val in calBias)
                {
                    DataMember.CalBiasPoint.Add(int.Parse(val));
                }
                 

                DataMember.TargetCurrent = double.Parse(param["TARGET_CURRENT"]);
                var biasRande = param["SET_BIAS_RANGE"].Split(',');
                DataMember.SetBiasRangeMin = int.Parse(biasRande[0]);
                DataMember.SetBiasRangeMax = int.Parse(biasRande[1]);


                var tecRange = param["CONNECT_CHECK_TEC_RANGE"].Split(',');
                DataMember.ConnectTecTempRangeMin = int.Parse(tecRange[0]);
                DataMember.ConnectTecTempRangeMax = int.Parse(tecRange[1]);



                DataMember.OvenTimeout = int.Parse(param["OVEN_TIMEOUT"]);
                DataMember.ReadCurrentk = double.Parse(param["READ_CURRENTK"]);//READ_CURRENTSB
                DataMember.ReadCurrentb = double.Parse(param["READ_CURRENTB"]);
                DataMember.ReadIteck = double.Parse(param["READ_ITECK"]);//READ_CURRENTSB
                DataMember.ReadItecb = double.Parse(param["READ_ITECB"]);
                DataMember.OvenPort = param["OvenPort"];
                DataMember.CtrlPort = param["ControlPort"];
                DataMember.OvenTempCheck = bool.Parse(param["OVEN_TEMP_CHECK"]);

                DataMember.DeltaIcck = double.Parse(param["DELTA_ICCK"]);
                DataMember.DeltaIccb = double.Parse(param["DELTA_ICCB"]);
                DataMember.MesStep = param["MES_STEP"];
                DataMember.SetTemperaturek = double.Parse(param["SET_TEMPERATUREK"]);
                DataMember.SetTemperatureb = double.Parse(param["SET_TEMPERATUREB"]);
                DataMember.IsMesCheck = bool.Parse(param["MES_CHECK"]);
                DataMember.IsHold = bool.Parse(param["HOLD_FLAG"]);
                
            }
            catch (Exception ex)
            {
                Log.LogError("load param unsuccess: " + ex.Message);
                Log.LogError(ex.StackTrace);
            }
            Log.LogInfo("load param success");

        }
        public Tosa25G(Dictionary<string, string> param, ILog logger)
        {
            Log = new DriverLog(logger, "TOSA25G", param["Name"]);
            TempInit = new InitTemp(GetTempFile());
            InitParam(param);
            if (OvenControl == null)
                OvenControl = new OvenControl(new Comm(DataMember.OvenPort,9600));
            CtrlCom = new ProtocolTosa25G( DataMember.BoardNum, new Comm(DataMember.CtrlPort), Log);
            
        }
        public Tosa25G(BaseProtocol protocolControl, BaseOvenControl ovenControl, Dictionary<string, string> param, ILog logger)
        {
            Log = new DriverLog(logger, "TOSA25G", param["Name"]);
            InitParam(param);
            CtrlCom = protocolControl;
            OvenControl = ovenControl;
            
        }

       
        public override void CatchException(int seat)
        {

            var floorNum = DataMember.FloorNum;
            lock (FloorLock[floorNum])
            {
                SeatInit info;
                if (TempInit.GetCalData(DataMember.BoardName,seat, out info))
                {
                    StepTuneBias(0, info.CurrentDac0, DataMember.SetBiasStep, x => { SetSingleBias(seat, x); });
                }
            }

        }
       
        public override bool EnableBoard()
        {
            var floorNum = DataMember.FloorNum;
            lock (FloorLock[floorNum])
            {
                Log.LogInfo("Enable Board Start...");

                OpenDc();
                ReadInitialIcc(Seats);
                CtrlCom.EnableBiasSync(true);
                CalculateTargetBias();
                SaveCurrentInfo();
                CtrlCom.EnableBiasSync(false);
                SetCalBias();
                Log.LogInfo("Enable Board Finish...");
                return true;
            }
        }


        public override Dictionary<int, List<KeyValuePair<string, string>>> ReadDataSet(string type)
        {
            var floorNum = DataMember.FloorNum;
            var boardName = DataMember.BoardName;
            var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
            Log.LogInfo("read oven temperature.");
            
            lock (FloorLock[floorNum])
            {
                double oventemperature = 0;
                if (DataMember.OvenTempCheck)
                {
                    oventemperature = OvenControl.GetBoardTemperature(DataMember.FloorNum, DataMember.BoardNum);
                }
                 
        
                int[] channelArray;
                int[] mpdArray;
                CtrlCom.ReadCurrentDacAndMpdDac(out channelArray, out mpdArray);
              

                var icc = ReadIcc();
                var itecSet = CtrlCom.ReadItecDac();

             
                int[] therTempSet;
                int t1Set;
                int t2Set;
                CtrlCom.ReadTecTemperatureDac(out therTempSet, out t1Set, out t2Set);

                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    var record = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("PCName", Environment.MachineName),
                        new KeyValuePair<string, string>("Floor", DataMember.FloorNum.ToString()),
                        new KeyValuePair<string, string>("BoardName", DataMember.BoardNum.ToString()),
                        new KeyValuePair<string, string>("Seat", seat.ToString()),
                        new KeyValuePair<string, string>("OVEN_Temperature", oventemperature.ToString("F1")),
                        new KeyValuePair<string, string>("ICC",icc[seatIndex].ToString("F2")),
                        new KeyValuePair<string, string>("ITEC",BiConvert.ConvertDacToITec(itecSet[seatIndex],DataMember.ReadIteck,DataMember.ReadItecb).ToString("F2")),
                        new KeyValuePair<string, string>("TEC_Temperature",BiConvert.ConvertDacToTemp(therTempSet[seatIndex]).ToString("F2")),
                        new KeyValuePair<string, string>("T1",BiConvert.ConvertDacToTemp(t1Set).ToString("F2")),
                        new KeyValuePair<string, string>("T2",BiConvert.ConvertDacToTemp(t2Set).ToString("F2"))
                    };
                    SeatInit info;
                    if (TempInit.GetCalData(boardName, seat, out info))
                    {
                        record.Add(new KeyValuePair<string, string>("DAC", info.CurrentDac0.ToString("F2")));
                        record.Add(new KeyValuePair<string, string>("ICC0", info.Icc0.ToString("F2")));
                        record.Add(new KeyValuePair<string, string>("LDI", BiConvert.ConvertToLdi(icc[seatIndex], info.Icc0, DataMember.DeltaIcck, DataMember.DeltaIccb).ToString("F2")));
                    }

                    for (var channel = 0; channel < 4; channel++)
                    {
                        record.Add(new KeyValuePair<string, string>("Ch" + channel, channelArray[seatIndex * 4 + channel].ToString()));
                        record.Add(new KeyValuePair<string, string>("MPD" + channel, mpdArray[seatIndex * 4 + channel].ToString()));
                    }
                    ret[seat] = record;
                }
                return ret;
            }
           
        }
        
 
        public override string GetVersion()
        {
            return "TOSA25G Model Version:V2.01";
        }
       
        private void ReadInitialIcc(List<int> seats)
        {
            Log.LogInfo("Read Icc0...");
            const int iTimes = 5;
            var iccSet = new Dictionary<int, double>();
            foreach (var seat in seats)
            {
                iccSet[seat] = 0;
            }
            for (var i = 0; i < iTimes; i++)
            {
                Log.LogInfo(DataMember.BoardName + $" read icc {i + 1} times");
                var tempIccSet =  ReadIcc();
                foreach (var seat in seats)
                {
                    iccSet[seat] += tempIccSet[seat-1];
                }
            }
            //calculate the avg
            foreach (var seat in seats)
            {
                InitIccSet[seat] = iccSet[seat] / iTimes;
            }

        }

         
        private static double CalculateLinearFittingValue(double[] xColumn, double[] yColumn, double xTarget)
        {
            return Calculation.Slope(yColumn, xColumn) * xTarget + Calculation.Intercept(yColumn, xColumn);
        }


        public override bool SetUpTemperature(double target)
        {
            lock (FloorLock[DataMember.FloorNum])
            {
                Log.LogInfo("Set TEC Temperature...");
                foreach (var seat in Seats)
                    CtrlCom.SetTecTemperatureDac(seat, BiConvert.ConvertTempToDac(DataMember.TargetTecTemperature, DataMember.SetTemperaturek, DataMember.SetTemperatureb));
                
                return true;
            }
        }
        public override bool TearDownTemperature()
        {
            lock (FloorLock[DataMember.FloorNum])
            {
                Log.LogInfo("Close TEC Temperature...");
                foreach (var seat in Seats)
                    CtrlCom.CloseTecTemperature(seat);
                return true;
            }
        }


    }
}
