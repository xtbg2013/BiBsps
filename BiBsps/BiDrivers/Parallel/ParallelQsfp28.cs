using System;
using System.Collections.Generic;
using log4net;
using BiBsps.BiLog;
using BiBsps.BiInterface;

namespace BiBsps.BiDrivers.Parallel
{
    internal class ParallelQsfp28 : ParallelBaseDriver
    {
        public ParallelQsfp28(Dictionary<string, string> param, IParallelProtocol parallelProtocol, ILog logger)
        {
            ParallelProtocol = parallelProtocol;
            Log = new DriverLog(logger, "ParallelQsfp28g", param["Name"]);
            InitParam(param);
        }

        protected sealed override void InitParam(Dictionary<string, string> param)
        {
            try
            {
                DataMember.BoardName = param["Name"];
                DataMember.FloorNum = int.Parse(param["Floor"]);
                DataMember.BoardNum = int.Parse(param["Locate"]);
                DataMember.SeatsCount = int.Parse(param["SEATS_COUNT"]);
                SeatsCount = DataMember.SeatsCount;
                DataMember.SetBiasStep = int.Parse(param["SET_BIAS_STEP"]);
                DataMember.TargetBias = int.Parse(param["TARGET_BIAS"]);

                var biasRande = param["SET_BIAS_RANGE"].Split(',');
                DataMember.SetBiasRangeMin = int.Parse(biasRande[0]);
                DataMember.SetBiasRangeMax = int.Parse(biasRande[1]);
                DataMember.MesStep = param["MES_STEP"];
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

        public override string GetVersion()
        {
            return "V1.00";
        }
    }
}
