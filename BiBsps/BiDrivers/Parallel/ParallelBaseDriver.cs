using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BiBsps.BiGlobalFiies;
using BiBsps.BiInterface;
using BiBsps.BiLog;
using BiInterface;

namespace BiBsps.BiDrivers.Parallel
{
    internal abstract class ParallelBaseDriver: BaseBoard
    {
        protected IParallelProtocol ParallelProtocol;
        protected DriverLog Log;
        protected new VbmsPlanData DataMember;
        protected List<int> Seats;
        protected Dictionary<int, string> SnDict;

        protected ParallelBaseDriver()
        {
            DataMember = new VbmsPlanData();
            SnDict = new Dictionary<int, string>();
        }


        #region IBoard Implementation

        public override bool EnableBoard()
        {
            Log.LogInfo("Enbale board ...");
            foreach (var slot in SnDict.Keys)
            {
                string msg;
                ParallelProtocol.SelectDut(DataMember.FloorNum, DataMember.BoardNum, slot, out msg);
                ParallelProtocol.SendPassword(out msg);
                ParallelProtocol.WriteParam(CmdType.WriteBurninMode, out msg);
                EnableAllChannel(false);
                EnableAllChannel(true);
                SetSyncBias(DataMember.TargetBias);
            }
            return true;
        }

        public override bool DisableBoard()
        {
            foreach (var seat in SnDict.Keys)
            {
                string msg;
                var res = ParallelProtocol.SelectDut(DataMember.FloorNum, DataMember.BoardNum, seat, out msg);
                if (!res) continue;
                res = ParallelProtocol.SendPassword(out msg);
                if (!res) continue;
                StepTuneBias(0, DataMember.TargetBias, DataMember.SetBiasStep, SetSyncBias);
                res = ParallelProtocol.WriteParam(CmdType.WriteBurninMode, out msg);
                EnableAllChannel(false);
                res = ParallelProtocol.EnableVoltage(seat, false, out msg);
            }
            return true;
        }

        public override Dictionary<int, string> GetSnSet()
        {
            Log.LogInfo("Read sn...");
            for (var seat = 1; seat <= SeatsCount; seat++)
            {
                Log.LogInfo($"Read slot = {seat} sn");
                string msg;
                byte[] data;
                var res = ParallelProtocol.SelectDut(DataMember.FloorNum, DataMember.BoardNum, seat, out msg);
                if (!res) continue;
                res = ParallelProtocol.EnableVoltage(seat, true, out msg);
                if (!res) continue;
                Thread.Sleep(200);
                res = ParallelProtocol.ReadParam(CmdType.ReadSn, out data, out msg);
                if (!res) continue;
                var sn = Encoding.ASCII.GetString(data, 0, 7);
                if (IsNumAndEnCh(sn))
                {
                    SnDict[seat] = sn;
                    Log.LogInfo("Read sn success");
                }
                else
                {
                    Log.LogError("Read sn success,but sn is invalid");
                }
            }
            return SnDict;
        }

        public override void AddSeat(int seat, string sn)
        {
            if (seat <= SeatsCount)
                if (SnDict.Keys.Contains(seat))
                    Log.LogWarn("seat" + seat + " exists");
                else
                {
                    SnDict[seat] = sn;
                }
            else
                Log.LogError("seat:" + seat + " is Invalid.");
        }
        public override void RemoveSeat(int seat)
        {
            if (seat <= SeatsCount)
                if (!SnDict.Keys.Contains(seat))
                    Log.LogInfo("seat" + seat + " does not exist.");
                else
                {
                    SnDict.Remove(seat);
                }
            else
                Log.LogWarn("seat" + seat + " is Invalid.");
        }

        public override Dictionary<int, List<KeyValuePair<string, string>>> ReadDataSet(string type)
        {
            var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
            foreach (var seat in SnDict.Keys)
            {
                var entry = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("pcName", Environment.MachineName),
                    new KeyValuePair<string, string>("board", DataMember.BoardNum.ToString()),
                    new KeyValuePair<string, string>("floor", DataMember.FloorNum.ToString()),
                    new KeyValuePair<string, string>("board", DataMember.BoardNum.ToString()),
                    new KeyValuePair<string, string>("boardName", DataMember.BoardName),
                    new KeyValuePair<string, string>("seat", seat.ToString())
                };
                string msg;
                ParallelProtocol.SelectDut(DataMember.FloorNum, DataMember.BoardNum, seat, out msg);
                ParallelProtocol.SendPassword(out msg);
                byte[] data;
                var biastValue = 0;

                if (ParallelProtocol.ReadParam(CmdType.ReadBias0, out data, out msg))
                {
                    entry.Add(new KeyValuePair<string, string>("bias0", CalBias(data, biastValue).ToString("F2")));
                }
                if (ParallelProtocol.ReadParam(CmdType.ReadBias1, out data, out msg))
                {
                    entry.Add(new KeyValuePair<string, string>("bias1", CalBias(data, biastValue).ToString("F2")));
                }
                if (ParallelProtocol.ReadParam(CmdType.ReadBias2, out data, out msg))
                {
                    entry.Add(new KeyValuePair<string, string>("bias2", CalBias(data, biastValue).ToString("F2")));
                }
                if (ParallelProtocol.ReadParam(CmdType.ReadBias3, out data, out msg))
                {
                    entry.Add(new KeyValuePair<string, string>("bias3", CalBias(data, biastValue).ToString("F2")));
                }
                if (ParallelProtocol.ReadParam(CmdType.ReadTemperature, out data, out msg))
                {
                    entry.Add(new KeyValuePair<string, string>("temperature", CalTemp(data).ToString("F2")));
                }
                ret[seat] = entry;
            }
            return ret;
        }

        public override void CatchException(int seat)
        {
            string msg;
            var res = ParallelProtocol.SelectDut(DataMember.FloorNum, DataMember.BoardNum, seat, out msg);
            if (!res)
            {
                Log.LogError(msg);
                return;
            }
            if (!ParallelProtocol.SendPassword(out msg))
            {
                Log.LogError(msg);
                return;
            }
            StepTuneBias(0, DataMember.TargetBias, DataMember.SetBiasStep, SetSyncBias);
        }

        #endregion


        #region Protected Methods

        protected void EnableAllChannel(bool enable)
        {
            string msg;
            if (enable)
            {
                ParallelProtocol.WriteParam(CmdType.WriteEnableAllChan, out msg);
            }
            else
            {
                ParallelProtocol.WriteParam(CmdType.WriteDisableAllChan, out msg);
            }
        }

        protected void SetSyncBias(int value)
        {
            string msg;
            var data = new byte[2];
            data[1] = (byte)(value * 1000 / 2 / 256);
            data[0] = (byte)(value * 1000 / 2 % 256);

            var res = ParallelProtocol.WriteParam(CmdType.WriteChanBias0, out msg, data);
            if (!res)
            {
                Log.LogError($"Set bias0 error:{msg}");
            }
            res = ParallelProtocol.WriteParam(CmdType.WriteChanBias1, out msg, data);
            if (!res)
            {
                Log.LogError($"Set bias1 error:{msg}");
            }
            res = ParallelProtocol.WriteParam(CmdType.WriteChanBias2, out msg, data);
            if (!res)
            {
                Log.LogError($"Set bias2 error:{msg}");
            }
            res = ParallelProtocol.WriteParam(CmdType.WriteChanBias3, out msg, data);
            if (!res)
            {
                Log.LogError($"Set bias3 error:{msg}");
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

        protected bool IsNumAndEnCh(string input)
        {
            const string pattern = @"^[A-Za-z0-9]+$";
            var regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        protected double CalBias(byte[] data, double nrpp)
        {
            double rawAdc = data[0];
            return ((rawAdc + 124) / (15725 * (1.145845 - 0.00463 * nrpp)) * 1000);
        }

        protected double CalTemp(byte[] data)
        {
            double rawAdc = data[1] * 256 + data[0];
            return (rawAdc / 256.0);
        }

        protected override Dictionary<int, bool> PreCheckConnections()
        {
            Log.LogInfo("Check connections...");
            var ret = new Dictionary<int, bool>();
            foreach (var slot in SnDict.Keys)
            {
                ret[slot] = true;
            }
            return ret;
        }

        #endregion


        protected abstract void InitParam(Dictionary<string, string> param);
    }
}
