using System;
using System.Collections.Generic;
using System.Threading;
using BiBsps.BiGlobalFiies;
using BiBsps.BiOven;
using BiBsps.BiProtocols.TosaBosa;
using BiBsps.BiLog;
using log4net;

namespace BiBsps.BiDrivers.TosaBosa
{
    internal class Cfp8Tosa : BaseDriver
    {
        private static readonly Dictionary<string ,List<string>> Bh1PnTable;
        private static readonly Dictionary<string, List<string>> Bh2PnTable;
        protected string GetTempFile()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configurations\TEMP_CFP8.xml");
        }
        protected override void CalculateTargetBias()
        {
            Log.LogInfo("Calculate Target Bias ...");
            var startingDac = DataMember.CalBiasPoint[0];
            var deltaIccb = DataMember.DeltaIccb;
            var deltaIcck = DataMember.DeltaIcck;
            var targetCurrent = DataMember.TargetCurrent;
            var dacMa = DataMember.DacMa;

            for (int channelIndex = 0; channelIndex < 4; channelIndex++)
            {
                Log.LogInfo("Set Sync Bias: " + startingDac / 2);
                SetSyncBiasByChannel(channelIndex, startingDac / 2);
                Log.LogInfo("SUCCESS!");

                Thread.Sleep(2000);

                Log.LogInfo("Set Sync Bias: " + startingDac);
                SetSyncBiasByChannel(channelIndex, startingDac);
                Log.LogInfo("SUCCESS!");

                Thread.Sleep(2000);

                Log.LogInfo("Read Icc...");
                var curIcc = ReadIcc();
                Log.LogInfo("SUCCESS!");

                Log.LogInfo("Set Sync Bias: " + startingDac / 2);
                SetSyncBias(startingDac / 2);
                Log.LogInfo("SUCCESS!");

                Thread.Sleep(2000);

                Log.LogInfo("Set Sync Bias: 0");
                SetSyncBias(0);
                Log.LogInfo("SUCCESS!");

                Thread.Sleep(2000);

                foreach (var seat in Seats)
                {
                    var seatIndex = seat - 1;
                    var icc = curIcc[seatIndex];
                    var icc0 = InitIccSet[seat];
                    var targetDeltaIcc = (targetCurrent + deltaIccb) / deltaIcck;
                    var calibrateDac = Convert.ToInt32(startingDac + (targetDeltaIcc - icc + icc0) / dacMa);
                    CalBias[seat * 100 + channelIndex] = calibrateDac;
                }
            }

        }
        protected override void SetCalBias()
        {
            Log.LogInfo("Set Target Bias ...");
            foreach (var seat in Seats)
            {
                int seatIndex = seat - 1;
                CtrlCom.SelectProduct(seatIndex);
                for (int channel = 0; channel < 4; channel++)
                {
                    int setBias = CalBias[seat * 100 + channel] / 2;
                    SetSingleBiasByChannel(channel, setBias);
                    Log.LogInfo("SN:" + SnDict[seat] + "\tCHANNEL:" + channel + "\tSETBIAS:" + setBias);
                }
                Thread.Sleep(1000);
                for (int channel = 0; channel < 4; channel++)
                {
                    int setBias = CalBias[seat * 100 + channel];
                    SetSingleBiasByChannel(channel, setBias);
                    Log.LogInfo("SN:" + SnDict[seat] + "\tCHANNEL:" + channel + "\tSETBIAS:" + setBias);
                }
            }
        }
        protected override void SaveCurrentInfo()
        {
            Log.LogInfo("Save Target Bias ...");

            Log.LogInfo("Save Target Bias ...");
            BoardCurrents current = new BoardCurrents {BoardName = DataMember.BoardName};
            foreach (var seat in Seats)
            {
                SeatInit curs = new SeatInit()
                {
                    Sn = SnDict[seat],
                    Position = seat,
                    CurrentDac0 = CalBias[seat * 100 + 0],
                    CurrentDac1 = CalBias[seat * 100 + 1],
                    CurrentDac2 = CalBias[seat * 100 + 2],
                    CurrentDac3 = CalBias[seat * 100 + 3],
                    Icc0 = InitIccSet[seat]
                };
                current.SlotInit.Add(curs);
            }
            TempInit.SaveCalData(current);
        }

        private void InitParam(IReadOnlyDictionary<string, string> param)
        {
            try
            {

                DataMember.FloorNum = int.Parse(param["Floor"]);
                DataMember.BoardNum = int.Parse(param["Locate"]);
                DataMember.BoardName = param["Name"];
                SeatsCount = int.Parse(param["SEATS_COUNT"]);

                DataMember.OvenTempCheck = bool.Parse(param["OVEN_TEMP_CHECK"]);
                var biasRande = param["SET_BIAS_RANGE"].Split(',');
                DataMember.SetBiasRangeMin = int.Parse(biasRande[0]);
                DataMember.SetBiasRangeMax = int.Parse(biasRande[1]);


                var iccRange = param["CONNECT_CHECK_ICC_RANGE"].Split(',');
                DataMember.ConnectIccRangeMin = int.Parse(iccRange[0]);
                DataMember.ConnectIccRangeMax = int.Parse(iccRange[1]);
                DataMember.CheckTemperature = double.Parse(param["CHECK_TEMPERATURE"]);
                DataMember.TargetOvenTemperature = double.Parse(param["TARGET_OVEN_TEMPERATURE"]);

                DataMember.OvenTimeout = int.Parse(param["OVEN_TIMEOUT"]);
                
                DataMember.TargetCurrent = int.Parse(param["TARGET_CURRENT"]);
                DataMember.MesStep = param["MES_STEP"];

                DataMember.ReadCurrentk = double.Parse(param["READ_CURRENTK"]);
                DataMember.ReadCurrentb = double.Parse(param["READ_CURRENTB"]);
                DataMember.ReadIteck = double.Parse(param["READ_ITECK"]);
                DataMember.ReadItecb = double.Parse(param["READ_ITECB"]);

                DataMember.TargetTecTemperature = double.Parse(param["TARGET_TEC_TEMPERATURE"]);
                DataMember.SetTemperatureb = double.Parse(param["SET_TEMPERATUREB"]);
                DataMember.SetTemperaturek = double.Parse(param["SET_TEMPERATUREK"]);

                DataMember.ConnectCheckBias = int.Parse(param["CONNECT_CHECK_BIAS"]);

                var calBias = param["CAL_BIAS_ARRAY"].Split(',');
                foreach (var val in calBias)
                {
                    DataMember.CalBiasPoint.Add(int.Parse(val));
                }

                var tecRange = param["CONNECT_CHECK_TEC_RANGE"].Split(',');
                DataMember.ConnectTecTempRangeMin = int.Parse(tecRange[0]);
                DataMember.ConnectTecTempRangeMax = int.Parse(tecRange[1]);

                DataMember.DeltaIccb = double.Parse(param["DELTA_ICCB"]);
                DataMember.DeltaIcck = double.Parse(param["DELTA_ICCK"]);
                DataMember.DacMa = double.Parse(param["DAC_MA"]);
                DataMember.CocType = param["COC_TYPE"];
                DataMember.OvenPort = param["OvenPort"];
                DataMember.CtrlPort = param["ControlPort"];
                DataMember.IsMesCheck = bool.Parse(param["MES_CHECK"]);
                DataMember.IsHold = bool.Parse(param["HOLD_FLAG"]);

            }
            catch (Exception ex)
            {
                Log.LogError("load param unsuccess: " + ex.Message);
                Log.LogError(ex.StackTrace);
            }
            Log.LogInfo(param["Name"]+":load param success");
        }

        static Cfp8Tosa()
        {
            var configReader = ConfigReader.GetInstance(@"BiFiles\CFP8CocPnTable.xml");
            Bh1PnTable = new Dictionary<string, List<string>>();
            Bh1PnTable = new Dictionary<string, List<string>>();
            configReader.GetItem("BH1", out Bh1PnTable);
            configReader.GetItem("BH2", out Bh2PnTable);
           
        }

        public Cfp8Tosa(Dictionary<string, string> param, ILog logger)
        {

            Log = new DriverLog(logger, "CFP8", param["Name"]);
            TempInit = new InitTemp(GetTempFile());
            InitParam(param);
           
            if (OvenControl == null)
                OvenControl = new OvenControl(new Comm(DataMember.OvenPort, 9600));
            CtrlCom = new ProtocolCfp8(DataMember.BoardNum, new Comm(DataMember.CtrlPort), Log);
            
        }
        public Cfp8Tosa(BaseProtocol protocolControl, BaseOvenControl ovenControl, Dictionary<string, string> param, ILog logger)
        {
            Log = new DriverLog(logger, "CFP8", param["Name"]);
            InitParam(param);
            CtrlCom = protocolControl;
            OvenControl = ovenControl;
        }
        public override string GetVersion()
        {
            return "CFP8: V2.00";
        }
         
        public override string GetCocTypeBySn(string sn,string[] cocInfo)
        {
            var bh2Count = 0;
            List< Dictionary<string, string>> typeInfo;
            var state = GetCocInfo(sn, cocInfo, out typeInfo);
            if (state)
            {
                foreach (var info in typeInfo)
                {
                    switch (info["type"])
                    {
                        case "BH1":
                            break;
                        case "BH2":
                            bh2Count++;
                            break;
                    }
                }
                return bh2Count > 0 ? "BH2" : "BH1";
            }
            else
                return "";

        }
  
        public override void CatchException(int seat)
        {
            int floorNum = DataMember.FloorNum;
            lock (FloorLock[floorNum])
            {
                int seatIndex = seat - 1;
                CtrlCom.SelectProduct(seatIndex);
                foreach (var bias in new[] { 53, 0 })
                {
                    for (var channel = 0; channel < 4; channel++)
                    {
                        if (SetSingleBiasByChannel(channel, bias))
                            Log.LogInfo("SN:" + SnDict[seat] + "\tCHANNEL:" + channel + "\tSETBIAS:" + bias);
                    }
                    Thread.Sleep(1000);
                }
                RemoveSeat(seat);
                CtrlCom.EnableVoltage(Seats, true);
            }
        }

      
        public override bool IsCocCheck()
        {
            return true;
        }
    
        public override string GetCocTypeByPlan()
        {
            return DataMember.CocType;
        }

        private bool GetCocInfo(string sn, string[] srcInfo, out List<Dictionary<string, string>> cocInfo)
        {
            bool state;

            cocInfo = new List<Dictionary<string, string>>();
            var count = srcInfo.Length / 4;

            for (var i = 0; i < count; i++)
            {
                var cocSn = srcInfo[i * 4];
                var cocPn = srcInfo[i * 4 + 1];

                Dictionary<string, string> info = new Dictionary<string, string>();
                //4 channels
                //ch1
                if (Bh1PnTable.ContainsKey("ch1") && Bh1PnTable["ch1"].Contains(cocPn))
                {
                    info["channel"] = "1";
                    info["type"] = "BH1";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);
                    
                }
                else if (Bh2PnTable.ContainsKey("ch1") && Bh2PnTable["ch1"].Contains(cocPn))
                {
                    info["channel"] = "1";
                    info["type"] = "BH2";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);
                }
                //ch2
                if (Bh1PnTable.ContainsKey("ch2") && Bh1PnTable["ch2"].Contains(cocPn))
                {
                    info["channel"] = "2";
                    info["type"] = "BH1";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);


                }
                else if (Bh2PnTable.ContainsKey("ch2") && Bh2PnTable["ch2"].Contains(cocPn))
                {
                    info["channel"] = "2";
                    info["type"] = "BH2";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);

                }
                //ch3
                if (Bh1PnTable.ContainsKey("ch3") && Bh1PnTable["ch3"].Contains(cocPn))
                {
                    info["channel"] = "3";
                    info["type"] = "BH1";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);


                }
                else if (Bh2PnTable.ContainsKey("ch3") && Bh2PnTable["ch3"].Contains(cocPn))
                {
                    info["channel"] = "3";
                    info["type"] = "BH2";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);

                }
                if (Bh1PnTable.ContainsKey("ch4") && Bh1PnTable["ch4"].Contains(cocPn))
                {
                    info["channel"] = "4";
                    info["type"] = "BH1";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);

                }
                else if (Bh2PnTable.ContainsKey("ch4") && Bh2PnTable["ch4"].Contains(cocPn))
                {
                    info["channel"] = "4";
                    info["type"] = "BH2";
                    info["cocSn"] = cocSn;
                    info["cocPN"] = cocPn;
                    cocInfo.Add(info);

                }
              
            }

            if (cocInfo.Count != 4)
            {
                Log.LogError("Sn = " + sn + "  get coc info from camstar :empty");
                state = false;
            }
            else
                state = true;
            return state;
        }
        
    }
}
