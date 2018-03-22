using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using log4net;
using NationalInstruments.VisaNS;
using Syntune.Core;
using BiInterface;
using BiBsps.BiLog;

namespace BiBsps.BiDrivers.Iqm
{
    public class IqmGenI : IBoard
    {
        protected readonly DriverLog Log;
        protected Dictionary<int, string> SnDict;
        protected string ProductType;

        static IqmGenI()
        {
        }

        public IqmGenI(Dictionary<string, string> param, ILog uiLog)
        {
            ProductType = "IQMGenI";
            Log = new DriverLog(uiLog, ProductType, "");
            Log.LogInfo("IqmGenI start");

            InitParam(param);

            Log = new DriverLog(uiLog, ProductType, _name);
            Seats = new List<int>();
            SnDict = new Dictionary<int, string>();
        }
        public string GetVersion()
        {
            return "IQMGenI: V1.00";
        }
        #region configurations
        private int _floor;
        private int _number;
        private string _name;

        private bool _IsMesEnabled;
        public string _ExpectedMesStep;
        private bool _IsAutoHoldEnabled;

        public string ConnectPositive = ConfigReader.GetItem("ConnectPositive");
        public string ActiveChannel1 = ConfigReader.GetItem("ActiveChannel1");
        public string ActiveChannel2 = ConfigReader.GetItem("ActiveChannel2");
        public string ACH1 = ConfigReader.GetItem("ActiveDFBChan1T4");
        public string ACH2 = ConfigReader.GetItem("ActiveDFBChan5T8");
        public string ACH3 = ConfigReader.GetItem("ActiveDFBChan9T12");
        public string ACH4 = ConfigReader.GetItem("ActiveDFBChan13T16");
        public string ACH5 = ConfigReader.GetItem("ActiveDFBChan17T20");
        public string ACH6 = ConfigReader.GetItem("ActiveDFBChan21T24");
        public string ACH7 = ConfigReader.GetItem("ActiveDFBChan25T28");
        public string ACH8 = ConfigReader.GetItem("ActiveDFBChan29T32");
        public string ACH9 = ConfigReader.GetItem("ActiveDFBChan33T36");
        public string ACH10 = ConfigReader.GetItem("ActiveDFBChan37T40");
        public string ACH11 = ConfigReader.GetItem("ActiveDFBChan41T44");
        public string ACH12 = ConfigReader.GetItem("ActiveDFBChan45");
        public string ACH13 = ConfigReader.GetItem("ActiveDFBChan46T49");
        public string ACH14 = ConfigReader.GetItem("ActiveDFBChan50T53");
        public string ACH15 = ConfigReader.GetItem("ActiveDFBChan54");
        public string PChConnPara1T15Low = ConfigReader.GetItem("FirPChPara1T15Low");
        public string DFB32Addr = ConfigReader.GetItem("DFB32Addr");
        public string DFB13Addr = ConfigReader.GetItem("DFB13Addr");
        public string SwitchU19 = ConfigReader.GetItem("SwitchU19");
        public string PChConnPara1T15High = ConfigReader.GetItem("FirPChPara1T15High");
        public string SPChConnPara1T15Low = ConfigReader.GetItem("SecPChPara1T15Low");
        public string SPChConnPara1T15High = ConfigReader.GetItem("SecPChPara1T15High");
        public string TPChConnPara1T15Low = ConfigReader.GetItem("ThrPChPara1T15Low");
        public string TPChConnPara1T15High = ConfigReader.GetItem("ThrPChPara1T15High");
        public string ParaNameList1T15 = ConfigReader.GetItem("ParaNameList1T15");
        public string ReadCurK = ConfigReader.GetItem("ReadCurK");
        public string ReadCurB = ConfigReader.GetItem("ReadCurB");

        protected static Keithley2200 kethley;
        protected static string kethleytype;
        protected static object mKeithleyLock = new object();
        #endregion
        #region interfaces
        public void InitParam(Dictionary<string, string> param)
        {
            _floor = int.Parse(param["Floor"]);
            _number = int.Parse(param["Locate"]);
            _name = param["Name"];
            //SeatsCount = int.Parse(param["SEATS_COUNT"]);
            SeatsCount = int.Parse(ConfigReader.GetItem("SeatsCount"));

            _IsMesEnabled = bool.Parse(ConfigReader.GetItem("MesCheck"));
            _ExpectedMesStep = ConfigReader.GetItem("MesStep");
            _IsAutoHoldEnabled = bool.Parse(ConfigReader.GetItem("MesAutoHold"));

            #region keithley initialize
            if (kethley == null)
            {
                kethley = new Keithley2200();
                kethleytype = ConfigReader.GetItem("KeithleylayNew");
                string kpath = ConfigReader.GetItem("Keithleylay");
                try
                {
#if !DEBUG
                    kethley.mbSession = new SafeSession((MessageBasedSession)ResourceManager.GetLocalManager().Open(kpath));
#else
                    kethley.mbSession = new SafeSession((MessageBasedSession)ResourceManager.GetLocalManager().Open(kpath));
#endif
                }
                catch (Exception e)
                {
                    Log.LogWarn("Keithley connection Failed\r\n" + e.ToString());
                }
            }
            #endregion
            #region Sfinx initialize
            sfinxData = new SfinxData[SeatsCount];
            try
            {
                SwedenDb.InitalizeCore();
            }
            catch (Exception e)
            {
                Log.LogWarn("SwedenDb.InitalizeCore Failed\r\n" + e.ToString());
            }
            #endregion

            Log.LogInfo("load param success");
        }
        public virtual Dictionary<int, string> GetSnSet()
        {
            return null;
        }
        public virtual void AddSeat(int seat, string sn)
        {
            if (seat <= SeatsCount)
                if (Seats.Contains(seat))
                    Log.LogInfo("seat" + seat + " exists");
                else
                {
                    Seats.Add(seat);
                    SnDict[seat] = sn;
                    SfinxDataCreate(seat, sn);
                }
            else
                Log.LogWarn("seat:" + seat + " is Invalid.");
        }
        public virtual void RemoveSeat(int seat)
        {
            if (seat <= SeatsCount)
                if (!Seats.Contains(seat))
                    Log.LogInfo("seat" + seat + " does not exist.");
                else
                {
                    Seats.Remove(seat);
                    SnDict.Remove(seat);
                    sfinxData[seat - 1] = null;
                }
            else
                Log.LogWarn("seat" + seat + " is Invalid.");
        }
        private Dictionary<int, bool> PreBurnIn_CheckConnections()
        {
            SfinxDataReinit();

            lock (mKeithleyLock)
            {
                DfbPowerDown();
                //this.U19Setting("NO");
                var results = this.MainBoardFunction("Frist_Connect_Result" + _floor + ".txt", "Frist_Active_Result" + _floor + ".txt", 1);
                SfinxDataSave(results, false);

                return results;
            }
        }
        private Dictionary<int, bool> PostBurnIn_CheckConnections()
        {
            this.Log.LogInfo(this._name + " Restest Data Start, plese do not remove the DUT...");
            Dictionary<int, bool> results = this.MainBoardFunction("Second_Connect_Result" + this._floor + ".txt", "Second_Active_Result" + this._floor + ".txt", 2);
            this.Log.LogInfo(this._name + " Restest Data Finished!");
            SfinxDataSave(results, true);

            return results;
        }
        public virtual Dictionary<int, bool> CheckConnections(bool postBurnIn = false)
        {
            if (!postBurnIn)
            {
                return PreBurnIn_CheckConnections();
            }
            else
            {
                return PostBurnIn_CheckConnections();
            }
        }
        public virtual bool EnableBoard()
        {
            string pathactive = "Frist_Active_Result.txt";
            lock (mKeithleyLock)
            {
                DfbPowerOn(Pathset(pathactive));
                this.U19Setting("NO");
                return true;
            }
        }
        public virtual bool DisableBoard()
        {
            lock (mKeithleyLock)
            {
                this.Log.LogInfo(this._name + " Disable Board Start...");
                DfbPowerDown();
                KeithleyPowerOffSafe();
                this.SetU19Low();
                this.Log.LogInfo(this._name + " Disable Board Finish...");

                return true;
            }
        }
        public virtual Dictionary<int, List<KeyValuePair<string, string>>> ReadDataSet(string type)
        {
            var ret = new Dictionary<int, List<KeyValuePair<string, string>>>();
            string[] ParaNameList = ParaNameList1T15.Split(',');
            int[,] maindac;

            this.Log.LogInfo(this._name + " Read Data...");
            lock (mKeithleyLock)
            {
                maindac = this.ReadMainDac();
            }
            this.Log.LogInfo("SUCCESS!");

            foreach (var seat in this.Seats)
            {
                var entry = new List<KeyValuePair<string, string>>();
                int seatIndex = seat - 1;

                for (int paramIdx = 0; paramIdx < ParaNameList.Length; paramIdx++)
                {
                    entry.Add(new KeyValuePair<string, string>(ParaNameList[paramIdx], ConvertToVol(maindac[seatIndex, paramIdx]).ToString("#0.000")));
                }

                ret[seat] = entry;
            }

            return ret;
        }
        public int SeatsCount { get; set; }
        protected List<int> Seats { get; set; }
        public virtual bool SetUpTemperature(double target)
        {
            return true;
        }
        public virtual bool TearDownTemperature()
        {
            return true;
        }
        public void CatchException(int seat)
        {
            lock (mKeithleyLock)
            {
                int seatIndex = seat - 1;
                this.SelectProduct(seatIndex);
                this.RemoveSeat(seat);
            }
        }
        public virtual bool IsCocCheck()
        {
            return false;
        }
        public virtual double GetTargetOvenTemperature()
        {
            return double.NaN;
        }
        public virtual string GetCocTypeBySn(string sn, string[] cocInfo)
        {
            throw new NotImplementedException();
        }
        public virtual string GetCocTypeByPlan()
        {
            throw new NotImplementedException();
        }
        public virtual string GetMesStepName()
        {
            return _ExpectedMesStep;
        }
        public virtual bool IsMesCheck()
        {
            return _IsMesEnabled;
        }
        public virtual bool IsHold()
        {
            return _IsAutoHoldEnabled;
        }
        #endregion

        SfinxData[] sfinxData;
        string[] GlobalParameterKeys = {
            "ID:rack.furnace",
            "ID:slot.furnace",
            "ID:DUTposition.furnace",
            "Time:load.furnace",
            "Time:unload.furnace",
            "TimeISO8601:load.furnace",
            "TimeISO8601:unload.furnace",
        };
        string DFBBoardAddressOnFloor(string rawAddr)
        {
            int addr = Convert.ToInt32(rawAddr, 16);
            addr = addr - 2 * (this._number - 1);
            return addr.ToString("x2");
        }
        private byte[] QueryProduct(byte[] cmd, int delay)
        {
            return OsaUtility.Query(this._floor, this._number, cmd, delay);
        }
        private byte[] QueryProduct(byte[] cmd, int delay, string number)
        {
            return OsaUtility.Query(this._floor, Convert.ToInt32(Convert.ToByte(number, 16)), cmd, delay);
        }
        private byte[] QueryProductU19(byte[] cmd, int delay)
        {
            return OsaUtility.Query(01, 01, cmd, delay);
        }
        private byte[] QueryTec(byte[] cmd, int delay)
        {
            return OsaUtility.Query(this._floor, this._number + 4, cmd, delay);
        }

        private double ConvertToVol(int dac)
        {
            double ReadPostiveVolk = double.Parse(ConfigReader.GetItem(_name, "ReadPostiveVolK"));
            double ReadPostiveVolb = double.Parse(ConfigReader.GetItem(_name, "ReadPostiveVolB"));
            double ReadNegativeVolk = double.Parse(ConfigReader.GetItem(_name, "ReadNegativeVolK"));
            double ReadNegativeVolb = double.Parse(ConfigReader.GetItem(_name, "ReadNegativeVolB"));

            if (dac < 4096)
                return (Convert.ToDouble(dac) * ReadPostiveVolk + ReadPostiveVolb);
            else
                return (Convert.ToDouble(dac) * ReadNegativeVolk + ReadNegativeVolb);
        }

        private void CheckMainBoard()
        {
            int delay = 2000;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x02, 0xA0, 0x00 };
                byte[] response = this.QueryProduct(cmd, delay);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("CheckMainBoard Exception");
        }

        private void SetMainBoardVal()
        {
            int delay = 2000;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x03, 0x80, 0x10 };
                byte[] response = this.QueryProduct(cmd, delay);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("SetMainBoardVal Exception");
        }

        private void SetU19High()
        {
            int delay = 200;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x05, 0x00, 0x00 };
                byte[] response = this.QueryProductU19(cmd, delay);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("SetU19High Exception");
        }

        private void SetU19Low()
        {
            int delay = 200;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x06, 0x00, 0x00 };
                byte[] response = this.QueryProductU19(cmd, delay);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("SetU19Low Exception");
        }
        private void KeithleyPowerOnSafe()
        {
            KeithleySetPower("0", "0");
            kethley.PowerOn();
        }
        private void KeithleyPowerOffSafe()
        {
            KeithleySetPower("0", "0");
            kethley.PowerOff();
            Thread.Sleep(1000);
        }
        private void KeithleySetPower(string value1, string value2)
        {
            if (kethleytype.ToUpper() == "TRUE")
            {
                kethley.OutputVolSingleSafe(double.Parse(value1));
            }
            else
            {
                kethley.OutputVolSetSafe(double.Parse(value1), 1);
                kethley.OutputVolSetSafe(double.Parse(value2), 2);
            }
        }
        private void U19Setting(string setup)
        {
            //DialogResult result = MessageBox.Show("System will switch the U19, set the voltage to 0! ", "Switch U19!", MessageBoxButtons.OK, MessageBoxIcon.Question);

            //if (result.ToString()=="OK")
            {
                KeithleyPowerOffSafe();

                if (setup == "YES")
                {
                    System.Threading.Thread.Sleep(500);
                    this.SetU19High();
                }
                else
                {
                    this.SetU19Low();
                }

                KeithleyPowerOnSafe();
                KeithleySetPower(ActiveChannel1, ActiveChannel2);
            }
            //MessageBox.Show("Switch the U19 finished, Reset the voltage! ", "Reset Voltage!", MessageBoxButtons.OK);
        }

        private int[,] ReadMainDac()
        {
            int delay = 30000;
            int espectLength = 160;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x04, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd, delay);
                if ((response.Length == espectLength) || (response.Length > espectLength))
                {

                    int[,] ret = new int[SeatsCount, 16];
                    for (int i = 0; i < SeatsCount; i++)
                    {
                        for (int k = 0; k < 16; k++)
                        {
                            int high = Convert.ToInt32(response[k * 2 + i * 32]) * 256;
                            int low = Convert.ToInt32(response[k * 2 + i * 32 + 1]);

                            ret[i, k] = high + low;
                        }
                    }
                    return ret;
                }
                retry++;
            } while (retry < retryOut);
            throw new Exception("Read MUX ADC Exception");
        }

        /*
          * 1.initialzation Main board base on postive voltage
          * 2.read the 135 values and record to log and txt file
          * 3.set negative voltage
          * 
          * 4.initialzation the FCB board
          * 5.set the 45 channel data base on user configure
          * --Add 9 channels for user's new request
          * 6.read the 45 data and mapping to 9 SN and every sn has 5 data
        */
        private Dictionary<int, bool> MainBoardFunction(string pathcheck, string pathactive, int times)
        {
            bool[] checkmain = new bool[9];
            bool seatflag = true;
            DateTime stattime = DateTime.Now;
            string path = this.Pathset(pathcheck);
            var ret = new Dictionary<int, bool>();
            lock (mKeithleyLock)
            {
                //this.U19Setting("YES");
                if (times == 1)
                {
                    KeithleyPowerOffSafe();
                    this.SetU19High();
                    MessageBox.Show("Now the voltage is 0, Please Load the Cassettes! ", "Load Cassettes!", MessageBoxButtons.OK);

                    KeithleyPowerOnSafe();
                    KeithleySetPower(ActiveChannel1, ActiveChannel2);
                    DfbPowerOn(Pathset(pathactive));
                }
                else
                {
                    this.U19Setting("YES");
                    DfbPowerOn(Pathset(pathactive));
                }
                this.Log.LogInfo("Check Main Board Connect...");
                this.CheckMainBoard();
                this.SetMainBoardVal();

                this.Log.LogInfo("Read Main ADC, please waiting one minute...");
                var maindac = this.ReadMainDac();
                checkmain = this.DealWithData(maindac, path, "P", times, stattime);
                this.Log.LogInfo("The conection for Postive check finished!");
                foreach (var seat in this.Seats)
                {
                    int seatIndex = seat - 1;
                    ret[seat] = checkmain[seatIndex];
                    if (!checkmain[seatIndex])
                        seatflag = false;
                }

                //For Negtive check function
                stattime = DateTime.Now;
                if (seatflag)
                {
                    this.U19Setting("NO");

                    this.Log.LogInfo("Read Main ADC for Negtive, please waiting one minute...");
                    maindac = this.ReadMainDac();
                    checkmain = this.DealWithData(maindac, path, "N", times, stattime);
                    this.Log.LogInfo("The conection for Negtive check finished!");
                    foreach (var seat in this.Seats)
                    {
                        int seatIndex = seat - 1;
                        ret[seat] = checkmain[seatIndex];
                        if (!checkmain[seatIndex])
                            seatflag = false;
                    }
                }

                KeithleyPowerOffSafe();
                SetU19Low();                  //to protect U19
                DfbPowerDown();

                return ret;
            }
        }

        private void CheckFCBBoard(string num)
        {
            int delay = 2000;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x00, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd, delay, num);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set FCB Board Mode Exception");
        }

        private int[,] ReadDFBDac()
        {
            int delay = 2000;
            int espectLength = 68;
            int[,] ret = new int[SeatsCount, 6];
            int[] rettmp = new int[64];
            //For DFB 3f
            var cmd = new byte[] { 0x00, 0x40, 0x00, 0x00 };
            byte[] response = this.QueryProduct(cmd, delay, DFBBoardAddressOnFloor("3f"));
            if ((response.Length == espectLength) || (response.Length > espectLength))
            {
                for (int k = 0; k < 32; k++)
                {
                    int high = Convert.ToInt32(response[k * 2]) * 256;
                    int low = Convert.ToInt32(response[k * 2 + 1]);
                    rettmp[k] = high + low;
                }
            }

            //For DFB 3e

            cmd = new byte[] { 0x00, 0x40, 0x00, 0x00 };
            response = this.QueryProduct(cmd, delay, DFBBoardAddressOnFloor("3e"));
            if ((response.Length == espectLength) || (response.Length > espectLength))
            {
                for (int k = 0; k < 32; k++)
                {
                    int high = Convert.ToInt32(response[k * 2]) * 256;
                    int low = Convert.ToInt32(response[k * 2 + 1]);
                    rettmp[k + 32] = high + low;
                }
            }

            if (rettmp.Length == 64)
            {
                //Add the first fifth
                for (int i = 0; i < SeatsCount; i++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        ret[i, k] = rettmp[k + i * 5];
                    }
                }
                //Add the sixth for every seat
                for (int i = 0; i < SeatsCount; i++)
                {
                    for (int k = 45; k < 54; k++)
                    {
                        ret[i, 5] = rettmp[k];
                    }
                }
            }

            return ret;
        }

        /* Jiangbo: FCB in this file is DFB actually */
        private void SetDfbCurrentAll(int adc, string DFBAddr)
        {
            byte[] adcBytes = BitConverter.GetBytes(adc << 4);

            int delay = 1000;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x00, 0x10, adcBytes[1], adcBytes[0] };
                //var cmd = new byte[] { 0x01, 0x10, 0x10, 0x00 };
                byte[] response = this.QueryProduct(cmd, delay, DFBBoardAddressOnFloor(DFBAddr));
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);

            throw new Exception("SetDfbCurrentAll Exception " + adc.ToString());
        }
        private void SetFCBBoardVal(string chanadd, string chanadc, string DFBAddr)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            int delay = 1000;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                byte[] datatemp = new byte[3];
                datatemp[0] = Convert.ToByte(chanadd, 16);
                int value = Convert.ToInt32(chanadc.Trim());
                string hexOutput = String.Format("{0:X}", value) + '0';
                if (hexOutput.Length == 3)
                    hexOutput = '0' + hexOutput;
                if (hexOutput.Length == 2)
                    hexOutput = '0' + '0' + hexOutput;
                string hex1 = hexOutput.Substring(0, hexOutput.Length - 2);
                string hex2 = hexOutput.Substring(hexOutput.Length - 2, 2);

                byte[] arr = new byte[] { System.Text.Encoding.ASCII.GetBytes(hex1)[0], System.Text.Encoding.ASCII.GetBytes(hex2)[0] };
                datatemp[1] = arr[0];
                datatemp[2] = arr[1];
                var cmd = new byte[] { 0x01, datatemp[0], Convert.ToByte(hex1, 16), Convert.ToByte(hex2, 16) };
                //var cmd = new byte[] { 0x01, 0x10, 0x10, 0x00 };
                byte[] response = this.QueryProduct(cmd, delay, DFBBoardAddressOnFloor(DFBAddr));
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("Set Board Mode Exception");
        }
        private void CheckFCBConnSet(string path)
        {
            string[] tACH1 = ACH1.Split(',');
            string[] tACH2 = ACH2.Split(',');
            string[] tACH3 = ACH3.Split(',');
            string[] tACH4 = ACH4.Split(',');
            string[] tACH5 = ACH5.Split(',');
            string[] tACH6 = ACH6.Split(',');
            string[] tACH7 = ACH7.Split(',');
            string[] tACH8 = ACH8.Split(',');
            string[] tACH9 = ACH9.Split(',');
            string[] tACH10 = ACH10.Split(',');
            string[] tACH11 = ACH11.Split(',');
            string[] tACH13 = ACH13.Split(',');
            string[] tACH14 = ACH14.Split(',');
            string[] chanadc = new string[] { tACH1[0].ToString(), tACH1[1].ToString(), tACH1[2].ToString(), tACH1[3].ToString(),
                                              tACH2[0].ToString(), tACH2[1].ToString(), tACH2[2].ToString(), tACH2[3].ToString(),
                                              tACH3[0].ToString(), tACH3[1].ToString(), tACH3[2].ToString(), tACH3[3].ToString(),
                                              tACH4[0].ToString(), tACH4[1].ToString(), tACH4[2].ToString(), tACH4[3].ToString(),
                                              tACH5[0].ToString(), tACH5[1].ToString(), tACH5[2].ToString(), tACH5[3].ToString(),
                                              tACH6[0].ToString(), tACH6[1].ToString(), tACH6[2].ToString(), tACH6[3].ToString(),
                                              tACH7[0].ToString(), tACH7[1].ToString(), tACH7[2].ToString(), tACH7[3].ToString(),
                                              tACH8[0].ToString(), tACH8[1].ToString(), tACH8[2].ToString(), tACH8[3].ToString(),
                                              tACH9[0].ToString(), tACH9[1].ToString(), tACH9[2].ToString(), tACH9[3].ToString(),
                                              tACH10[0].ToString(), tACH10[1].ToString(), tACH10[2].ToString(), tACH10[3].ToString(),
                                              tACH11[0].ToString(), tACH11[1].ToString(), tACH11[2].ToString(), tACH11[3].ToString(), ACH12.ToString(),
                                              tACH13[0].ToString(), tACH13[1].ToString(), tACH13[2].ToString(), tACH13[3].ToString(),
                                              tACH14[0].ToString(), tACH14[1].ToString(), tACH14[2].ToString(), tACH14[3].ToString(),ACH15.ToString()};
            string[] chanadd = new string[] { "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa","ab","ac","ad","ae","af",
                                              "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba","bb","bc","bd","be","bf",
                                              "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa","ab","ac","ad","ae","af",
                                              "b0", "b1", "b2", "b3", "b4", "b5" };
            this.CheckFCBBoard(DFB32Addr);
            this.CheckFCBBoard(DFB13Addr);
            for (int i = 0; i < chanadc.Length; i++)
            {
                if (i < 32)
                {
                    try
                    {
                        this.SetFCBBoardVal(chanadd[i], chanadc[i], DFB32Addr);
                    }
                    catch
                    { }
                }
                else
                {
                    try
                    {
                        this.SetFCBBoardVal(chanadd[i], chanadc[i], DFB13Addr);
                    }
                    catch
                    { }
                }
            }
            //var fcbdata = this.ReadMainDac();
            //this.DealWithData(fcbdata, path,"F");
        }

        private void DFBOff(string DFBAddr)
        {
            int delay = 2000;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x11, 0x00, 0x00 };
                byte[] response = this.QueryProduct(cmd, delay, DFBBoardAddressOnFloor(DFBAddr));
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("DFBOff Exception");
        }
        private void SelectProduct(int position)
        {
            int delay = 200;
            int espectLength = 4;
            int retryOut = 1;
            int retry = 0;
            do
            {
                var cmd = new byte[] { 0x01, 0x0f, 0x00, Convert.ToByte(position) };
                byte[] response = this.QueryProduct(cmd, delay);
                if (response.Length == espectLength)
                    return;
                retry++;
            } while (retry < retryOut);
            throw new Exception("SelectProduct Exception");
        }

        private bool IsDfbDeployed = bool.Parse(ConfigReader.GetItem("IsDfbDeployed"));
        private void DfbPowerDown()
        {
            if (IsDfbDeployed)
            {
                this.Log.LogInfo("The system is shutdown the DBF board");
                DFBOff(DFB32Addr);
                DFBOff(DFB13Addr);
            }
        }

        private void DfbPowerOn(string path)
        {
            if (IsDfbDeployed)
            {
                this.Log.LogInfo("the DBF board output working");

                string globalAdc;
                try
                {
                    globalAdc = ConfigReader.GetItem("ActiveDFBChanAll");
                }
                catch (Exception)
                {
                    globalAdc = "NA";
                }

                if (!globalAdc.Equals("NA"))
                {
                    this.Log.LogInfo(String.Format("The system is setup the DBF board with current value {0}", globalAdc));
                    this.SetDfbCurrentAll(Int32.Parse(globalAdc), DFBBoardAddressOnFloor("3f"));
                    this.SetDfbCurrentAll(Int32.Parse(globalAdc), DFBBoardAddressOnFloor("3e"));
                }
                else
                {
                    this.Log.LogInfo(String.Format("The system is setup the DBF board, please wait one minute..."));
                    CheckFCBConnSet("NoUsePath");
                }

                this.Log.LogInfo("DFB board setting Finished!");
            }
        }
        private string[][] GetParaSpec(int timecut)
        {
            string[][] PChSpec = new string[3][];
            PChSpec[2] = ParaNameList1T15.Split(',');
            if (timecut == 1)
            {
                PChSpec[0] = PChConnPara1T15Low.Split(',');
                PChSpec[1] = PChConnPara1T15High.Split(',');
                for (int i = 0; i < 15; i++)
                    PChSpec[2][i] = "Pre_" + PChSpec[2][i];
            }
            if (timecut == 2)
            {
                PChSpec[0] = SPChConnPara1T15Low.Split(',');
                PChSpec[1] = SPChConnPara1T15High.Split(',');
                for (int i = 0; i < 15; i++)
                    PChSpec[2][i] = "Mid_" + PChSpec[2][i];
            }
            if (timecut == 3)
            {
                PChSpec[0] = TPChConnPara1T15Low.Split(',');
                PChSpec[1] = TPChConnPara1T15High.Split(',');
                for (int i = 0; i < 15; i++)
                    PChSpec[2][i] = "Pos_" + PChSpec[2][i];
            }

            return PChSpec;
        }

        private bool[] DealWithData(int[,] maindac, string path, string type, int timecut, DateTime ststime)
        {
            bool[] chresult = new bool[9];
            //Dictionary<string, string> paradatas = new Dictionary<string, string>();
            List<KeyValuePair<string, string>> paradatas = new List<KeyValuePair<string, string>>();
            //string [] PChLow = PChConnPara1T15Low.Split(',');
            //string[] PChHigh = PChConnPara1T15High.Split(',');
            string[][] checkspecs = new string[3][];
            checkspecs = this.GetParaSpec(timecut);
            //string [] ParaList = ParaNameList1T15.Split(',');
            this.Log.LogInfo("The ADC as below ...");
            string headtitle = "      ";
            for (int i = 0; i < 15; i++)
            {
                headtitle = headtitle + checkspecs[2][i] + "       ";
            }
            //headtitle = "\r\n" + headtitle + "AP1,AP2,CP2,BP1,BP2,BS2,BS1,MonA,AS2,MonB,AS1,CP1,MonC,GND,GND";
            headtitle = "\r\n" + headtitle + "\r\n";
            WriteText(path, headtitle);
            double[,] fcVol = new double[Seats.Count, 15];
            for (int i = 0; i < Seats.Count; i++)
            {
                string sn = SnDict[Seats[i]];
                string[] daclist = new string[15];
                chresult[Seats[i] - 1] = true;
                string showlog = sn;
                daclist[0] = sn;
                for (int k = 0; k < 15; k++)
                {
                    //----------Mark the real calculate from DAC to Voltage
                    fcVol[i, k] = this.ConvertToVol(maindac[Seats[i] - 1, k]);
                    daclist[i] = fcVol[i, k].ToString();
                    if (Math.Round(fcVol[i, k], 2) == 0)
                    {
                        showlog = showlog + " " + "0.00" + ",    ";
                        paradatas.Add(new KeyValuePair<string, string>(checkspecs[2][k], "0.00"));
                    }
                    else
                    {
                        showlog = showlog + " " + Math.Round(fcVol[i, k], 2).ToString() + ",    ";
                        paradatas.Add(new KeyValuePair<string, string>(checkspecs[2][k], Math.Round(fcVol[i, k], 2).ToString()));
                    }
                    if (type == "P")
                    {
                        if (fcVol[i, k] < float.Parse(checkspecs[0][k]))
                        {
                            this.Log.LogInfo("The SN: " + sn + " Parameter: " + checkspecs[2][k] + " is Lower than SPEC: " + checkspecs[0][k]);
                            chresult[Seats[i] - 1] = false;
                        }
                        else if (fcVol[i, k] > float.Parse(checkspecs[1][k]))
                        {
                            this.Log.LogInfo("The SN: " + sn + " Parameter: " + checkspecs[2][k] + " is Higher than SPEC: " + checkspecs[1][k]);
                            chresult[Seats[i] - 1] = false;
                        }
                    }
                }
                showlog = showlog.Remove(showlog.Length - 4, 4) + "\r\n";
                this.Log.LogInfo(showlog);
                WriteText(path, showlog);

                if ((timecut == 1 && type == "P") || (timecut == 1 && type == "N") || (timecut == 2 && type == "P"))
                {
                    try
                    {
                        sfinxData[Seats[i] - 1].Load(fcVol, i, ststime, chresult[Seats[i] - 1]);
                    }
                    catch (Exception e)
                    {
                        Log.LogInfo("Sfinx load failed, " + e.ToString());
                    }
                }
            }
            return chresult;
        }
        public void WriteText(string path, string data)
        {
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(data);
            sw.Flush();
            sw.Close();
            fs.Close();
        }
        private string Pathset(string filename)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string path = Environment.CurrentDirectory + "\\IQM_TEST_DATA\\" + date + "\\" + filename;
            if (!File.Exists(path))
            {
                if (!Directory.Exists(Environment.CurrentDirectory + "\\IQM_TEST_DATA\\" + date + "\\"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "\\IQM_TEST_DATA\\" + date + "\\");
            }
            return path;
        }

        #region SfinxData Operations

        private void SfinxDataCreate(int seat, string sn)
        {
            sfinxData[seat - 1] = new SfinxData(sn, _floor, _number,
                ConfigReader.GetItem("SfinxP2FilePath"),
                ConfigReader.GetItem("StationNumber"),
                "0325", SfinxData.BatchNumber(),
                GlobalParameterKeys,
                (ParaNameList1T15).Split(','))
            {
                GlobalParam =
                {
                    ["ID:rack.furnace"] = "IQMGenIOven",
                    ["ID:slot.furnace"] = _name,
                    ["ID:DUTposition.furnace"] = seat.ToString()
                }
            };
        }

        private void SfinxDataReinit()
        {
            var dt = DateTime.Now;
            for (var seat = 0; seat < SeatsCount; seat++)
            {
                if (sfinxData[seat] == null) continue;

                sfinxData[seat].GlobalParam["Time:load.furnace"] = dt.ToString(CultureInfo.InvariantCulture);
                sfinxData[seat].GlobalParam["TimeISO8601:load.furnace"] = dt.ToString("yyyy-MM-ddTHH:mm:sszzz");
                sfinxData[seat].CleanData();
                sfinxData[seat].mRunNo++;//Now, only mRunNo during software lifecycle is traced
            }
        }

        private void SfinxDataSave(Dictionary<int, bool> result, bool postBurnIn)
        {
            foreach (var seat in this.Seats)
            {
                DateTime dt = DateTime.Now;

                if (postBurnIn || (!postBurnIn && !result[seat]))
                {
                    if (sfinxData[seat - 1] != null)
                    {
                        sfinxData[seat - 1].GlobalParam["Time:unload.furnace"] = dt.ToString();
                        sfinxData[seat - 1].GlobalParam["TimeISO8601:unload.furnace"] = dt.ToString("yyyy-MM-ddTHH:mm:sszzz");
                        try
                        {
                            sfinxData[seat - 1].Save(result[seat] ? ExecutionStatus.Passed : ExecutionStatus.Failed);
                            //sfinxData[seat - 1] = null;
                            Log.LogInfo("Sfinx[" + seat.ToString() + "] save");
                        }
                        catch (Exception e)
                        {
                            Log.LogWarn("Sfinx[" + seat.ToString() + "] save failed: " + e.ToString());
                        }
                    }
                }
            }
        }
        #endregion
    }
}
