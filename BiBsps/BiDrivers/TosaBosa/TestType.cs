using System;
using System.Collections.Generic;
using log4net;
using BiBsps.BiLog;

namespace BiBsps.BiDrivers.TosaBosa
{
    internal class TestType : BaseDriver
    {
        protected override void CalculateTargetBias()
        {
            throw new NotImplementedException();
        }

        protected override void SetCalBias()
        {
            throw new NotImplementedException();
        }

        protected override void SaveCurrentInfo()
        {
            throw new NotImplementedException();
        }

        protected string GetTempFile()
        {
            return "";
        }

        public TestType(Dictionary<string, string> param, ILog logger)
        {
            Log = new DriverLog(logger, "QSFP28G", param["Name"]);
            SeatsCount = 26;
            TempInit = new InitTemp(GetTempFile());
            DataMember.BoardName = param["Name"];
            DataMember.FloorNum = int.Parse(param["Floor"]);
            DataMember.BoardNum = int.Parse(param["Locate"]);
            Log.LogInfo("Construct Driver TestType success");
        }

        public override string GetVersion()
        {
            return "V1.00";
        }

        public override Dictionary<int, string> GetSnSet()
        {
            //var sn = new Dictionary<int, string>
            //{
            //    [1] = "1",
            //    [2] = "2",
            //    [3] = "3"
            //};

            return null;
            //return sn; 
        }

        protected override Dictionary<int, bool> PreCheckConnections()
        {
            var ret = new Dictionary<int, bool>();
            for (var i = 1; i <= 16; i++)
                ret[i] = true;
            return ret;
        }

        public override bool EnableBoard()
        {
            return true;
        }

        public override bool DisableBoard()
        {
            return true;
        }

        public override Dictionary<int, List<KeyValuePair<string, string>>> ReadDataSet(string type)
        {
            var boardName = DataMember.BoardName;
            var floorNum = DataMember.FloorNum;
            var boardNum = DataMember.BoardNum;

            Log.LogInfo(DataMember.BoardName + ":read---------------------");
            var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
            const double icc = 150;
            foreach (var seat in Seats)
            {
                var record = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("PCName", Environment.MachineName),
                    new KeyValuePair<string, string>("Floor", floorNum.ToString()),
                    new KeyValuePair<string, string>("BoardName", boardName),
                    new KeyValuePair<string, string>("floor", DataMember.FloorNum.ToString()),
                    new KeyValuePair<string, string>("board", DataMember.BoardNum.ToString()),
                    new KeyValuePair<string, string>("Seat", seat.ToString()),
                    new KeyValuePair<string, string>("ICC", icc.ToString("F2"))
                };
                ret[seat] = record;
            }

            return ret;
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
        }

        public override bool IsCocCheck()
        {
            return true;
        }

        public override string GetCocTypeBySn(string sn, string[] cocInfo)
        {
            return "BH1";
        }

        public override string GetCocTypeByPlan()
        {
            return "BH1";
        }
    }
}
