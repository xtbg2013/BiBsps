using System;
using System.Collections.Generic;
using System.Globalization;
using SfinxLoader;
using Syntune.Utils;
using Syntune.Core;
using Syntune.Core.Internal;

namespace BiBsps.BiDrivers.Iqm
{
    public class SwedenDb
    {
        private static bool _enabled = false;

        public static IP2Writer P2W = new P2Writer();
        public static ITicketProvider Tp = new TicketProvider();
        public static ParameterCollection BaseCollection = new ParameterCollection();

        public static void SimpleTest(string batchProject, string batchNumber)
        {
            #region test parameters
            var globalParams = new [] {
                "ID:rack.furnace",
                "ID:slot.furnace",
                "ID:DUTposition.furnace",
                "Time:load.furnace",
                "Time:unload.furnace",
                "TimeISO8601:load.furnace",
                "TimeISO8601:unload.furnace",
            };

            var measureParams = new [] {
                "X_PA2",
                "X_PA1",
                "X_PreSOA",
                "TERM",
            };

            var fcVol = new double[3,4] {
                { 1,2,3,4 },
                { 5,6,7,8 },
                { 9,10,11,12 },
            };

            var checkResult = new [] {
                true, true, true
            };
            #endregion

            InitalizeCore();

            var sfinxData = new SfinxData("99998888", 1, 1, @"D:\SM.net\F5500_Raw_Data", "62", batchProject, batchNumber, globalParams, measureParams);

            var dt = DateTime.Now;
            sfinxData.GlobalParam["Time:load.furnace"] = dt.ToString(CultureInfo.InvariantCulture);
            sfinxData.GlobalParam["TimeISO8601:load.furnace"] = dt.ToString("yyyy-MM-ddTHH:mm:sszzz");

            sfinxData.Load(fcVol, 0, dt, checkResult[0]);

            dt = DateTime.Now;
            sfinxData.Load(fcVol, 0, dt, checkResult[0]);

            dt = DateTime.Now;
            sfinxData.Load(fcVol, 0, dt, checkResult[0]);

            dt = DateTime.Now;
            sfinxData.GlobalParam["Time:unload.furnace"] = dt.ToString();
            sfinxData.GlobalParam["TimeISO8601:unload.furnace"] = dt.ToString("yyyy-MM-ddTHH:mm:sszzz");

            sfinxData.Save();
        }

        public static void InitalizeCore()
        {
            if (_enabled) return;

            BootStrapper.Boot();
            _enabled = true;
        }

        public static void SaveP2File(string path, string wafer, string chipId, ParameterCollection pc)
        {
            P2W.RootPath = path + @"\FurnaceData";
            P2W.DevicePath = @"device data\" + wafer + @"\" + chipId;
            P2W.Save2P2File(pc);
        }

        public static void SaveTicket(string path, string stationNumber, string batchProject, string batchNumber, string wafer, string chipId, string desc, DateTime starttime, uint runNo, ExecutionStatus status = ExecutionStatus.Passed)
        {
            /* ticket's path is determined by configuration of SM.net */
            //Tp = new TicketProvider();

            Tp.BatchProject = batchProject;                 /* 326 for Dual IQM, 8000 to be determined */
            Tp.BatchNumber = batchNumber;
            Tp.BatchName = batchProject + "-" + batchNumber + " Furnace Batch";
            Tp.Database = "S9";
            Tp.UnitId = desc;                         /* Rack_Floor_Board */
            Tp.IntegrationLevel = "WBDCM";
            Tp.RecipeStep = "";
            Tp.RunNumber = runNo;
            Tp.WaferSerial = wafer;                   /* Temporary W6503 */
            Tp.LaserSerial = Tp.WaferSerial + "-" + chipId;
            Tp.LaserPath = $@"FurnaceData\device data\{wafer}\{chipId}\";
            Tp.StationName = "Burnin";
            Tp.StationNumber = int.Parse(stationNumber);
            Tp.StepName = "COC.TestAndBurnIn.BurnIn1.BurnIn";
            Tp.Task = "Furnace";
            Tp.StartTime = starttime;
            Tp.StopTime = DateTime.Now;
            Tp.ExecutionStatus = status;
            Tp.Parameters = BaseCollection;

            Tp.EnqueueTicket(true);
        }
    }
    public class SfinxData
    {
        private const int TotalTestTime = 3;

        private int _mTime = 0;

        public string mP2FilePath;
        public string mStationNo;
        public string mBatchProject;
        public string mBatchNumber;
        public string mWaferId;
        public string mChipId;
        public string mDesc;
        public uint mRunNo;

        public string[] mGlobalParamName;
        public Dictionary<string, string> GlobalParam = new Dictionary<string, string>();
        public string[] mMeasureParamName;
        public Dictionary<string, string[]> MeasureParam = new Dictionary<string, string[]>();
        public DateTime[] MeasuerTime = new DateTime[TotalTestTime];

        private static string SfinxParamName(string param)
        {
            return $"V:{param}_medium_current.Furnace";
        }

        private string ParamName(string sfinxParam)
        {
            var name = sfinxParam.Split(':')[1];
            return name.Substring(0, name.Length - "_medium_current.Furnace".Length);
        }

        private static readonly DateTime StartDate = new DateTime(2017,12,3);

        public static string BatchNumber()
        {
            var today = DateTime.Now;
            return today.Subtract(StartDate).Days.ToString("#0000");
        }

        public SfinxData(string sn, int floor, int number, string p2FilePath, string stationNo, string batchProject, string batchNumber,string[] globalParams, string[] measureParams)
        {
            MeasuerTime[0] = DateTime.Now;

            mP2FilePath = p2FilePath;
            mStationNo = stationNo;
            mBatchProject = batchProject;
            mBatchNumber = batchNumber;
            mGlobalParamName = globalParams;
            mMeasureParamName = measureParams;

            foreach (var t in mGlobalParamName)
            {
                GlobalParam[t] = "";
            }

            foreach (var t in mMeasureParamName)
            {
                MeasureParam[t] = new string[TotalTestTime];
            }

            if (sn.Length == "XXXXYYYY".Length)/* IQMGenII temporary SN */
            {
                mWaferId = "W" + sn.Substring(0, 4);
                mChipId = sn.Substring(4, 4);
            }
            else/*WXXXX_YYYY*//* IQMGenI SN */
            {
                mWaferId = "W" + sn.Substring(1, 4);
                mChipId = sn.Substring(6, 4);
            }
            mDesc = $"0_{floor}_{number}";
        }

        public void CleanData()
        {
            if (_mTime <= 0) return;

            _mTime = 0;

            for (var i = 0; i < TotalTestTime; i++)
            {
                foreach (var t in mMeasureParamName)
                {
                    MeasureParam[t][i] = 0.ToString("#0.00");
                }

                MeasuerTime[i] = DateTime.Now;
            }
        }

        public void Load(double[,] values, int seatIdx, DateTime startTime, bool status)
        {
            for (var i = 0; i < mMeasureParamName.Length; i++)
            {
                MeasureParam[mMeasureParamName[i]][_mTime] = values[seatIdx, i].ToString("#0.00");
            }

            MeasuerTime[_mTime] = startTime;
            _mTime++;
        }

        public void Save(ExecutionStatus status = ExecutionStatus.Passed)
        {
            var pc = new ParameterCollection();

            foreach (var t in mGlobalParamName)
            {
                pc.Add(t.ToLaserParameter(GlobalParam[t]));
            }

            foreach (var t in mMeasureParamName)
            {
                pc.Add(SfinxParamName(t).ToLaserParameter(MeasureParam[t]));
            }

            pc.Add("Time.Furnace".ToLaserParameter(MeasuerTime));

            SwedenDb.SaveP2File(mP2FilePath, mWaferId, mChipId, pc);
            SwedenDb.SaveTicket(mP2FilePath, mStationNo, mBatchProject, mBatchNumber, mWaferId, mChipId, mDesc, MeasuerTime[0], mRunNo, status);
        }
    }
}
