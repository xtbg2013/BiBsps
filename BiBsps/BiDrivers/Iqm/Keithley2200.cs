using System;
using System.Collections.Generic;
using System.IO;
using NationalInstruments.VisaNS;
using System.Threading;

namespace BiBsps.BiDrivers.Iqm
{
    public class SafeSession
    {
        MessageBasedSession mSession = null;
        public SafeSession(MessageBasedSession s)
        {
            mSession = s;
        }
        public void Write(string cmd)
        {
            mSession.Write("SYSTem:REMote");
            mSession.Write(cmd);
        }
        public string Query(string cmd)
        {
            mSession.Write("SYSTem:REMote");
            return mSession.Query(cmd);
        }
    }
    public class Keithley2200
    {
        public MemoryStream screenImageMS;
        public SafeSession mbSession = null;
        //com打开和关闭设备连接
        public double MaxPower;
        private string address;
        public string Address
        {
            get { return  address; }
            set { address = value; }    
        }
        private string slotnumber;
        public string SlotNumber
        {
            get { return slotnumber; }
            set { slotnumber = value; }
        }
        
        public void Init()
        {
            this.Clear();
            
        }
        public bool MinSet()
        {
            string cmd = "SYST:COMM:GPIB:ADDR ";
            mbSession.Write(cmd);
            return true;
        }
        public bool MaxSet()
        {
            string cmd=":INP" + slotnumber +  ":ATT " + "MAX";
            mbSession.Write(cmd);

            return true;
        }
        
        public string OutputVolGet(int channel = -1)
        {
            string outputPower;
            string cmd;
            if (channel > 0)
            {
                cmd = "MEASure:VOLTage? CH" + channel;
            }
            else
            {
                cmd = "MEASure:VOLTage?";
            }
            outputPower = mbSession.Query(cmd);
            return outputPower;
        }
        private List<double> SplitValue(double origin, double target, double step)
        {
            double STEP = step;
            List<double> values = new List<double>();
            bool isUp = (target > origin) ? true : false;

            double start = origin;
            if (isUp)
            {
                while (start < target)
                {
                    values.Add(start);
                    start += STEP;
                }
            }
            else
            {
                while (start > target)
                {
                    values.Add(start);
                    start -= STEP;
                }
            }

            values.Add(target);

            return values;
        }
        const int EosProtectDelay = 200;
        public bool StepSet(Func<string, bool> SetFunc, double from, double to, double step)
        {
            List<double> values = SplitValue(from, to, step);
            foreach (double v in values)
            {
                SetFunc(v.ToString("#0.00"));
                Thread.Sleep(EosProtectDelay);
            }

            return true;
        }

        public bool StepSet(Func<string, int, bool> SetFunc, int chnl, double from, double to, double step)
        {
            List<double> values = SplitValue(from, to, step);
            foreach (double v in values)
            {
                SetFunc(v.ToString("#0.00"), chnl);
                Thread.Sleep(EosProtectDelay);
            }

            return true;
        }
        public double GetCurrentVoltage(int chnl)
        {
            double curVal = 0;
            try
            {
                curVal = Double.Parse(OutputVolGet(chnl));
            }
            catch (Exception)
            {
                curVal = 0;
            }

            return curVal;
        }

        private void CheckVoltage(int chnl, double value)
        {
            int MaxTry = 5;
            int CurTry = 0;
            double curValue;

            while (Math.Abs((curValue = GetCurrentVoltage(chnl)) - value) > 0.1)
            {
                Thread.Sleep(500);

                CurTry++;
                if (CurTry == MaxTry)
                {
                    throw new Exception(String.Format("KeithleyCheckVoltage Fail : expected {0} but {1} achieved", value, curValue));
                }
            }
        }
        public bool OutputVolSetSafe(double value, int channel)
        {
            StepSet(OutputVolSet, channel, GetCurrentVoltage(channel), value, 2);
            CheckVoltage(channel, value);
            return true;
        }
        public bool OutputVolSingleSafe(double value)
        {
            StepSet(OutputVolSingle, GetCurrentVoltage(1), value, 2);
            CheckVoltage(-1, value);
            return true;
        }
        public bool OutputVolSet(string voltagval, int channel)
        {
            string cmd;
            cmd = "APPLy CH" + channel+", "+ voltagval;
            mbSession.Write(cmd);
            return true;
        }
        public bool OutputVolSingle(string voltagval)
        {
            string cmd;
            cmd = "VOLTAGE " + voltagval;
            mbSession.Write(cmd);
            return true;
        }
        public bool Clear()
        {
            string cmd= "OUTP:PROT:CLE";
            mbSession.Write(cmd);
            return true;
        }
        public bool PowerOn()
        {
            string cmd = "OUTPUT ON";
            mbSession.Write(cmd);
            return true;
        }
        public bool PowerOff()
        {
            string cmd = "OUTPUT OFF";
            mbSession.Write(cmd);
            return true;
        }
    }
}
