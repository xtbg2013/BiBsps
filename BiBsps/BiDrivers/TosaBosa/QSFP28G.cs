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
    internal class Qsfp28G:BaseDriver
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
                DataMember.ReadIteck = double.Parse(param["READ_ITECK"]);
                DataMember.ReadItecb = double.Parse(param["READ_ITECB"]);
                DataMember.SetTemperatureb = double.Parse(param["SET_TEMPERATUREB"]);
                DataMember.SetTemperaturek = double.Parse(param["SET_TEMPERATUREK"]);
                DataMember.TargetTecTemperature = double.Parse(param["TARGET_TEC_TEMPERATURE"]);
                DataMember.DeltaIcck = double.Parse(param["DELTA_ICCK"]);
                DataMember.DeltaIccb = double.Parse(param["DELTA_ICCB"]);

                DataMember.ConnectCheckBias = int.Parse(param["CONNECT_CHECK_BIAS"]);
                DataMember.TargetCurrent = double.Parse(param["TARGET_CURRENT"]);
                DataMember.DacMa = double.Parse(param["DAC_MA"]);
                DataMember.OvenTempCheck = bool.Parse(param["OVEN_TEMP_CHECK"]);

                var biasRande = param["SET_BIAS_RANGE"].Split(',');
                DataMember.SetBiasRangeMin = int.Parse(biasRande[0]); 
                DataMember.SetBiasRangeMax = int.Parse(biasRande[1]);
                var iccRange = param["CONNECT_CHECK_ICC_RANGE"].Split(',');
                DataMember.ConnectIccRangeMin = int.Parse(iccRange[0]);
                DataMember.ConnectIccRangeMax = int.Parse(iccRange[1]);


                var calBias = param["CAL_BIAS_ARRAY"].Split(',');
                foreach (var val in calBias)
                {
                    DataMember.CalBiasPoint.Add(int.Parse(val));
                }

                var tecRange = param["CONNECT_CHECK_TEC_RANGE"].Split(',');
                DataMember.ConnectTecTempRangeMin = int.Parse(tecRange[0]);
                DataMember.ConnectTecTempRangeMax = int.Parse(tecRange[1]);

                DataMember.CheckTemperature = double.Parse(param["CHCEK_TEMPERATURE"]);
                DataMember.TargetOvenTemperature = double.Parse(param["TARGET_OVEN_TEMPERATURE"]);
                DataMember.MesStep = param["MES_STEP"];
                DataMember.OvenTimeout = int.Parse(param["OVEN_TIMEOUT"]);
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
            Log.LogInfo("load param success");

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
                SetSyncBiasByChannel(channelIndex, startingDac / 2);
                Thread.Sleep(2000);
                SetSyncBiasByChannel(channelIndex, startingDac);
                Thread.Sleep(2000);
                var curIcc = ReadIcc();
                SetSyncBias(startingDac / 2);
                Thread.Sleep(2000);
                SetSyncBias(0);
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
        protected override void SetCalBias( )
        {
            Log.LogInfo("Set Target Bias ...");
            foreach (var seat in Seats)
            {
                var seatIndex = seat - 1;
                CtrlCom.SelectProduct(seatIndex);
                for (var channel = 0; channel < 4; channel++)
                {
                    var setBias = CalBias[seat * 100 + channel] / 2;
                    SetSingleBiasByChannel(channel, setBias);
                    Log.LogInfo("SN:" + SnDict[seat] + "\tCHANNEL:" + channel + "\tSETBIAS:" + setBias);
                }
                Thread.Sleep(1000);
                for (var channel = 0; channel < 4; channel++)
                {
                    var setBias = CalBias[seat * 100 + channel];
                    SetSingleBiasByChannel(channel, setBias);
                    Log.LogInfo("SN:" + SnDict[seat] + "\tCHANNEL:" + channel + "\tSETBIAS:" + setBias);
                }
            }
        }
        protected override void SaveCurrentInfo()
        {
            Log.LogInfo("Save Target Bias ...");
            var current = new BoardCurrents {BoardName = DataMember.BoardName};
            foreach (var seat in Seats)
            {
                var curs = new SeatInit
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
        protected string GetTempFile()
        {
            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configurations\TEMP_TOSA28G.xml");
        }
        public Qsfp28G(Dictionary<string, string> param, ILog logger)
        {
            Log = new DriverLog(logger, "QSFP28G", param["Name"]);
            TempInit = new InitTemp(GetTempFile());
            InitParam(param);
            if (OvenControl == null)
                OvenControl = new OvenControl(new Comm(DataMember.OvenPort, 9600));
            CtrlCom = new ProtocolQsfp28G(DataMember.BoardNum, new Comm(DataMember.CtrlPort), Log);       
        }
        public override string GetVersion()
        {
            return "QSFP Version:V2.00";
        }
        
        public override void CatchException(int seat)
        {
            var floorNum = DataMember.FloorNum;
            lock (FloorLock[floorNum])
            {
                var seatIndex = seat - 1;
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
                CtrlCom.EnableVoltage(Seats,true);
         
            }
        }
       

    }
}
